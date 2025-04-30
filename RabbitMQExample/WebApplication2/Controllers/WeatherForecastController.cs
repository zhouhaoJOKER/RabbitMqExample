using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication2.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly Simulator _simulator;
    private readonly UserSessionContext _userSessionContext;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache, Simulator simulator, UserSessionContext userSessionContext)
    {
        _logger = logger;
        this._memoryCache = memoryCache;
        _simulator = simulator;
        _userSessionContext = userSessionContext;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        // ���ַ�ʽע�Ỻ�棬����������ڹ���֮ǰ�����ʣ������»�����Ĺ���ʱ�䣬�������ʱ�䵽�ˣ����������Ƴ����һ�ȡ���ص�
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(10))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(20))
            .RegisterPostEvictionCallback(CacheItemRemovedCallback, state: null);

        _memoryCache.Set("myKey", "myValue", cacheEntryOptions);

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }


    private static void CacheItemRemovedCallback(object key, object value, EvictionReason reason, object state)
    {
        // �����ﴦ������Ƴ����߼�
        if (reason == EvictionReason.Expired)
        {
            Console.WriteLine($"Cache item '{key}' was removed because it expired.");
        }
        else
        {
            Console.WriteLine($"Cache item '{key}' was removed because of {reason}.");
        }
    }



    [HttpGet(Name = "GetWeatherForecastTown")]
    public IEnumerable<WeatherForecast> GetWeatherForecastTown()
    {
        string val = _memoryCache.Get<string>("myKey");

        Console.WriteLine($"Cache item val>{val}");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet]
    public string Health()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    [HttpGet]
    public string GetTest()
    {
        string startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Thread.Sleep(5000);
        return startTime + "---" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    [HttpGet]
    public IActionResult TestCacheExpiration()
    {
        var key = "testKey";
        var value = Guid.NewGuid().ToString();

        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(5))
            .SetPriority(CacheItemPriority.Normal)
            .RegisterPostEvictionCallback((k, v, r, s) =>
            {
                Console.WriteLine($"Callback! Key: {k}, Value: {v}, Reason: {r}");
                // ����ʹ����־���
                _logger.LogInformation("Cache expired: {Key} ({Reason})", k, r);
            });

        _memoryCache.Set(key, value, options);

        return Ok(new
        {
            Key = key,
            Value = value,
            ExpiresAt = DateTime.Now.AddSeconds(5),
            Message = "Cache set. Check console/logs after 5+ seconds"
        });
    }

    [HttpGet]
    public IActionResult Login([FromQuery, Required] string loginToken)
    {
        _simulator.InitToken(loginToken);

        _userSessionContext.AddUserSimulator(loginToken, _simulator);

        return Ok(loginToken);
    }

    [HttpGet]
    public IActionResult GetToken([FromQuery, Required] string loginToken)
    {
        var s = _userSessionContext.GetUserSimulator(loginToken);
        return Ok(s.GetToken());
    }
}
