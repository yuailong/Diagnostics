// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks.Tests
{
    public class HealthCheckMiddlewareTests
    {
        [Fact]
        public async Task IgnoresRequestThatDoesNotMatchPath()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks();
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/frob");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [Fact]
        public async Task ReturnsEmptyHealthyRequestIfNoHealthChecksRegistered()
        {
            var expectedJson = JsonConvert.SerializeObject(new
            {
                status = "Healthy",
                results = new { }
            }, Formatting.None);

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks();
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/health");

            var result = await resp.Content.ReadAsStringAsync();
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task ReturnsResultsFromHealthChecks()
        {
            var expectedJson = JsonConvert.SerializeObject(new
            {
                status = "Unhealthy",
                results = new
                {
                    Foo = new
                    {
                        status = "Healthy",
                        description = "Good to go!",
                        data = new { }
                    },
                    Bar = new
                    {
                        status = "Degraded",
                        description = "Feeling a bit off.",
                        data = new { someUsefulAttribute = 42 }
                    },
                    Baz = new
                    {
                        status = "Unhealthy",
                        description = "Not feeling good at all",
                        data = new { }
                    },
                    Boz = new
                    {
                        status = "Unhealthy",
                        description = string.Empty,
                        data = new { }
                    },
                },
            }, Formatting.None);

            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                        .AddCheck("Foo", () => Task.FromResult(HealthCheckResult.Healthy("Good to go!")))
                        .AddCheck("Bar", () => Task.FromResult(HealthCheckResult.Degraded("Feeling a bit off.", new Dictionary<string, object>()
                        {
                            { "someUsefulAttribute", 42 }
                        })))
                        .AddCheck("Baz", () => Task.FromResult(HealthCheckResult.Unhealthy("Not feeling good at all", new Exception("Bad times"))))
                        .AddCheck("Boz", () => Task.FromResult(HealthCheckResult.Unhealthy(new Exception("Very bad times"))));
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/health");

            var result = await resp.Content.ReadAsStringAsync();
            Assert.Equal(expectedJson, result);
        }

        [Fact]
        public async Task StatusCodeIs200IfNoChecks()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks();
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task StatusCodeIs200IfAllChecksHealthy()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                        .AddCheck("Foo", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")))
                        .AddCheck("Bar", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")))
                        .AddCheck("Baz", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")));
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task StatusCodeIs200IfCheckIsDegraded()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                        .AddCheck("Foo", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")))
                        .AddCheck("Bar", () => Task.FromResult(HealthCheckResult.Degraded("Not so great.")))
                        .AddCheck("Baz", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")));
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task StatusCodeIs503IfCheckIsUnhealthy()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                        .AddCheck("Foo", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")))
                        .AddCheck("Bar", () => Task.FromResult(HealthCheckResult.Unhealthy("Pretty bad.")))
                        .AddCheck("Baz", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")));
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.ServiceUnavailable, resp.StatusCode);
        }

        [Fact]
        public async Task StatusCodeIs500IfCheckIsFailed()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHealthChecks("/health");
                })
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                        .AddCheck("Foo", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")))
                        .AddCheck("Bar", () => Task.FromResult(new HealthCheckResult(HealthCheckStatus.Failed, null, null, null)))
                        .AddCheck("Baz", () => Task.FromResult(HealthCheckResult.Healthy("A-ok!")));
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();

            var resp = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
        }
    }
}
