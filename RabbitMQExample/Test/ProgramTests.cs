using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2;
using Xunit;

namespace MyWebApi.Tests;

public class WeatherForecastTests : IClassFixture<WebApplicationFactory<WebApplication2.Program>>
{
    private readonly WebApplicationFactory<WebApplication2.Program> _factory;

    public WeatherForecastTests(WebApplicationFactory<WebApplication2.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsSuccess()
    {
        // 创建 HttpClient
        var client = _factory.CreateClient();

        // 调用 API
        var response = await client.GetAsync("/weatherforecast");

        // 断言
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public void WeatherService_IsInjectedCorrectly()
    {
        // 直接从 DI 容器获取服务
        var weatherService = _factory.Services.GetRequiredService<UserSessionContext>();

        // 断言服务可用
        Assert.NotNull(weatherService);
    }
}