﻿using BlazorEventsTodo.Todo;
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
        private readonly WebApplicationFactory<Server.Startup> _factory;

        public TodoApiTests()
        {
            _factory = new WebApplicationFactory<Server.Startup>();
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
    }
}
