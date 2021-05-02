using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tipalti.AspNetCore.ExceptionMiddleware.Models;
using Tipalti.AspNetCore.ExceptionMiddleware.Tests.Exceptions;
using Tipalti.AspNetCore.ExceptionMiddleware.Tests.Utils;
using Xunit;

namespace Tipalti.AspNetCore.ExceptionMiddleware.Tests.Integration
{
    public class MiddlewareTests
    {
        [Fact]
        public async Task NoMiddleware_NoThrow_ShouldReturnCorrectResponse()
        {
            const string GIVEN_RESPONSE = "hello";

            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.Map("/", context =>
                                {
                                    return context.Response.WriteAsync(GIVEN_RESPONSE);
                                });
                            });
                        });
                })
                .StartAsync();

            var response = await host.GetTestClient().GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().Be(GIVEN_RESPONSE);
        }

        [Fact]
        public async Task NoMiddleware_WithThrow_ShouldThrow()
        {
            const string GIVEN_ERROR_CODE = "123";

            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseEnvironment("Production")
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.Map("/", context =>
                                {
                                    throw new TestException(GIVEN_ERROR_CODE);
                                });
                            });
                        });
                })
                .StartAsync();

            await Assert.ThrowsAsync<TestException>(() => host.GetTestClient().GetAsync("/"));
        }

        [Fact]
        public async Task WithMiddleware_WithThrow_ShouldSerializeResponse()
        {
            const string GIVEN_ERROR_CODE = "123";

            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseEnvironment("Production")
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseExceptionMiddleware(cfg =>
                            {
                                cfg.JsonSerializerOptions.ConfigureForTest();

                                cfg.AddHandler<TestException>(exc => new ExceptionResult
                                {
                                    ErrorCode = exc.ErrorCode,
                                    StatusCode = HttpStatusCode.InternalServerError
                                });
                            });

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.Map("/", context =>
                                {
                                    throw new TestException(GIVEN_ERROR_CODE);
                                });
                            });
                        });
                })
                .StartAsync();

            var response = await host.GetTestClient().GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var responseObj = await response.Content.ReadAsObjectAsync<ExceptionMiddlewareResponse>();

            responseObj.ErrorCode.Should().Be(GIVEN_ERROR_CODE);
        }

        [Fact]
        public async Task WithMiddleware_WithThrow_WithoutRegisteredHandler_ShouldThrow()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseEnvironment("Production")
                        .ConfigureServices(services =>
                        {
                            services.AddRouting();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseExceptionMiddleware(cfg =>
                            {
                                cfg.JsonSerializerOptions.ConfigureForTest();

                                cfg.AddHandler<TestException>(exc => new ExceptionResult
                                {
                                    ErrorCode = exc.ErrorCode,
                                    StatusCode = HttpStatusCode.InternalServerError
                                });
                            });

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.Map("/", context =>
                                {
                                    throw new ArgumentNullException();
                                });
                            });
                        });
                })
                .StartAsync();

            var response = await host.GetTestClient().GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
