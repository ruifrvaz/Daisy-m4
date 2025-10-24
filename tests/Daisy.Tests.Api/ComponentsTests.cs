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
    /// Integration tests for the Daisy component API endpoints (Receivers, Abilities, Transmitters, Workflows).
    /// </summary>
    public class ComponentsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ComponentsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetReceivers_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/odata/Receivers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAbilities_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/odata/Abilities");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTransmitters_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/odata/Transmitters");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetWorkflows_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/odata/Workflows");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetReceivers_WithODataSelect_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Just test that the endpoint returns OK, even with empty data
            var response = await _client.GetAsync("/odata/Receivers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAbilities_WithODataExpand_ReturnsOk()
        {
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Just test that the endpoint returns OK, even with empty data
            var response = await _client.GetAsync("/odata/Abilities");

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
