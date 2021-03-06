using System.Threading.Tasks;
using Xunit;

namespace BlazorEventsTodo
{
    public class BasicTests
        : IClassFixture<TestServerFactory>
    {
        private readonly TestServerFactory _factory;

        public BasicTests(TestServerFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Gets_weather_forecasts()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/weatherForecast");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact(Skip = "FIX")]
        public async Task Api_path_returns_NotFound()
        {
            var client = _factory.CreateClient();

            var x = await client.GetAsync("/api/xxx");
            Assert.False(x.IsSuccessStatusCode, "Got success for non-existent path.");

            var y = await client.GetAsync("/api/todo");
            Assert.True(y.IsSuccessStatusCode, "Got failure for existing path.");
        }
    }
}
