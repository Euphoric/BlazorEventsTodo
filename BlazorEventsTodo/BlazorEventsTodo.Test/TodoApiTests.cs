using BlazorEventsTodo.Todo;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BlazorEventsTodo
{
    public class TodoApiTests
    {
        private readonly TestServerFactory _factory;

        public TodoApiTests()
        {
            _factory = new TestServerFactory();
        }

        [Fact]
        public async Task Gets_empty_todos()
        {
            var client = _factory.CreateClient();

            var response = await client.GetFromJsonAsync<List<TodoItem>>("/api/todo");

            Assert.Empty(response);
        }

        [Fact]
        public async Task Posts_new_todo()
        {
            var client = _factory.CreateClient();

            var newTodo = new CreateTodo() { Title = "New todo" };
            var postResponse = await client.PostAsJsonAsync("/api/todo", newTodo);
            postResponse.EnsureSuccessStatusCode();
            var newTodoId = await postResponse.Content.ReadFromJsonAsync<Guid>();

            var response = await client.GetFromJsonAsync<List<TodoItem>>("/api/todo");

            var createdTodo = Assert.Single(response);
            Assert.Equal(newTodoId, createdTodo.Id);
            Assert.Equal("New todo", createdTodo.Title);
        }

        [Fact]
        public async Task Toggles_todo_complete()
        {
            var client = _factory.CreateClient();

            Guid newTodoId;
            {
                var newTodo = new CreateTodo() { Title = "New todo" };
                var postResponse = await client.PostAsJsonAsync("/api/todo", newTodo);
                postResponse.EnsureSuccessStatusCode();
                newTodoId = await postResponse.Content.ReadFromJsonAsync<Guid>();
            }

            {
                var response = await client.GetFromJsonAsync<List<TodoItem>>("/api/todo");
                var createdTodo = Assert.Single(response);
                Assert.False(createdTodo.IsFinished);
            }

            {
                var postResponse = await client.PostAsJsonAsync($"/api/todo/{newTodoId}/finish", "");
                postResponse.EnsureSuccessStatusCode();
            }

            {
                var response = await client.GetFromJsonAsync<List<TodoItem>>("/api/todo");
                var createdTodo = Assert.Single(response);
                Assert.True(createdTodo.IsFinished);
            }

            {
                var postResponse = await client.PostAsJsonAsync($"/api/todo/{newTodoId}/start", "");
                postResponse.EnsureSuccessStatusCode();
            }

            {
                var response = await client.GetFromJsonAsync<List<TodoItem>>("/api/todo");
                var createdTodo = Assert.Single(response);
                Assert.False(createdTodo.IsFinished);
            }
        }

        [Fact]
        public async Task Deletes_todo()
        {
            var client = _factory.CreateClient();

            Guid newTodoId;
            {
                var newTodo = new CreateTodo() { Title = "New todo" };
                var postResponse = await client.PostAsJsonAsync("/api/todo", newTodo);
                postResponse.EnsureSuccessStatusCode();
                newTodoId = await postResponse.Content.ReadFromJsonAsync<Guid>();
            }

            {
                var postResponse = await client.DeleteAsync($"/api/todo/{newTodoId}");
                postResponse.EnsureSuccessStatusCode();
            }

            {
                var response = await client.GetFromJsonAsync<List<TodoItem>>("/api/todo");
                Assert.Empty(response);
            }
        }

        [Fact]
        public async Task Cannot_start_or_finish_deleted_item()
        {
            var client = _factory.CreateClient();

            Guid newTodoId;
            {
                var newTodo = new CreateTodo() { Title = "New todo" };
                var postResponse = await client.PostAsJsonAsync("/api/todo", newTodo);
                postResponse.EnsureSuccessStatusCode();
                newTodoId = await postResponse.Content.ReadFromJsonAsync<Guid>();
            }

            {
                var postResponse = await client.DeleteAsync($"/api/todo/{newTodoId}");
                postResponse.EnsureSuccessStatusCode();
            }

            {
                var postResponse = await client.PostAsJsonAsync($"/api/todo/{newTodoId}/finish", "");
                Assert.Equal(System.Net.HttpStatusCode.BadRequest, postResponse.StatusCode);
            }

            {
                var postResponse = await client.PostAsJsonAsync($"/api/todo/{newTodoId}/start", "");
                Assert.Equal(System.Net.HttpStatusCode.BadRequest, postResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Retrieves_empty_history()
        {
            var client = _factory.CreateClient();

            var history = await client.GetFromJsonAsync<List<TodoHistoryItem>>("/api/todo/history");

            Assert.Empty(history);
        }

        [Fact]
        public async Task Retrieves_history_of_all_actions()
        {
            var client = _factory.CreateClient();

            var firstTodo = new CreateTodo() { Title = "First todo" };
            var firstPostResponse = await client.PostAsJsonAsync("/api/todo", firstTodo);
            Guid firstTodoId = await firstPostResponse.Content.ReadFromJsonAsync<Guid>();

            await client.PostAsJsonAsync($"/api/todo/{firstTodoId}/finish", "");

            var secondTodo = new CreateTodo() { Title = "Second todo" };
            var secondPostResponse = await client.PostAsJsonAsync("/api/todo", secondTodo);
            Guid secondTodoId = await secondPostResponse.Content.ReadFromJsonAsync<Guid>();

            await client.PostAsJsonAsync($"/api/todo/{firstTodoId}/start", "");

            await client.PostAsJsonAsync($"/api/todo/{secondTodoId}/finish", "");

            await client.DeleteAsync($"/api/todo/{firstTodoId}");

            var history = await client.GetFromJsonAsync<List<TodoHistoryItem>>("/api/todo/history");

            DeepAssert.Equal(new List<TodoHistoryItem>{
                new TodoHistoryItem("Item created: First todo"),
                new TodoHistoryItem("Item finished: First todo"),
                new TodoHistoryItem("Item created: Second todo"),
                new TodoHistoryItem("Item restarted: First todo"),
                new TodoHistoryItem("Item finished: Second todo"),
                new TodoHistoryItem("Item deleted: First todo"),
            }, history);
        }
    }
}

