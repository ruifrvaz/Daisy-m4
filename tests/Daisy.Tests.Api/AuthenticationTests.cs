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
    /// Integration tests for the Authentication API endpoints.
    /// </summary>
    public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthenticationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetToken_WithValidCredentials_ReturnsToken()
        {
            var request = new
            {
                username = "admin",
                password = "admin"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);

            Assert.True(tokenResponse.TryGetProperty("access_token", out var token));
            Assert.False(string.IsNullOrEmpty(token.GetString()));
            Assert.True(tokenResponse.TryGetProperty("token_type", out var tokenType));
            Assert.Equal("Bearer", tokenType.GetString());
        }

        [Fact]
        public async Task GetToken_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var request = new
            {
                username = "invalid",
                password = "invalid"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AccessProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/odata/Impulses");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AccessProtectedEndpoint_WithValidToken_ReturnsOk()
        {
            var token = await GetAuthToken();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync("/odata/Impulses");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
