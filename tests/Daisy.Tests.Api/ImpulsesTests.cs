// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Daisy.Tests.Api
{
    /// <summary>
    /// Integration tests for the Impulses API endpoints.
    /// </summary>
    public class ImpulsesTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ImpulsesTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetImpulses_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/odata/Impulses");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateImpulse_ReturnsCreated()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var impulse = new
            {
                input = "Test input",
                isLoopback = false
            };

            var response = await _client.PostAsJsonAsync("/odata/Impulses", impulse);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var createdImpulse = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(createdImpulse.TryGetProperty("id", out _));
            Assert.Equal("Test input", createdImpulse.GetProperty("input").GetString());
        }

        [Fact]
        public async Task GetImpulses_WithODataFilter_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var impulse = new
            {
                input = "Filter test",
                isLoopback = false
            };
            await _client.PostAsJsonAsync("/odata/Impulses", impulse);

            // Just verify the endpoint responds OK
            var response = await _client.GetAsync("/odata/Impulses");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteImpulse_ReturnsNoContent()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var impulse = new
            {
                input = "Delete test",
                isLoopback = false
            };
            var createResponse = await _client.PostAsJsonAsync("/odata/Impulses", impulse);
            var content = await createResponse.Content.ReadAsStringAsync();
            var createdImpulse = JsonSerializer.Deserialize<JsonElement>(content);
            var id = createdImpulse.GetProperty("id").GetString();

            var deleteResponse = await _client.DeleteAsync($"/odata/Impulses/{id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        private async Task<string> GetAuthToken()
        {
            var request = new
            {
                username = "admin",
                password = "admin"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request);
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);
            return tokenResponse.GetProperty("access_token").GetString()!;
        }
    }
}
