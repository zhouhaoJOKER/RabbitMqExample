using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using Utility;
using WebApplication2.Dto;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    private readonly ZteUserSimulatorService _zteUserSimulatorService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache, Simulator simulator, UserSessionContext userSessionContext, ZteUserSimulatorService zteUserSimulatorService)
    {
        _logger = logger;
        this._memoryCache = memoryCache;
        _simulator = simulator;
        _userSessionContext = userSessionContext;
        _zteUserSimulatorService = zteUserSimulatorService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        // 这种方式注册缓存，如果缓存项在过期之前被访问，则会更新缓存项的过期时间，如果过期时间到了，会主动被移除并且获取到回调
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
        // 在这里处理缓存项被移除的逻辑
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
                // 或者使用日志框架
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
        _zteUserSimulatorService.LoginZte(loginToken);
        return Ok(loginToken);
    }

    /// <summary>
    /// 开启设备
    /// </summary>
    /// <param name="name"></param>
    /// <param name="deviceType"></param>
    /// <param name="zteDevice"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task StartDev([FromQuery] int num, string loginToken)
    {
        var s = _userSessionContext.GetUserSimulator(loginToken);
        await s.Yuyue(num, loginToken);
    }

    [HttpGet]
    public IActionResult GetToken([FromQuery, Required] string loginToken)
    {
        var s = _userSessionContext.GetUserSimulator(loginToken);
        return Ok(s.GetToken());
    }

    [HttpPost]
    public async Task SendMessageToWeb([FromQuery] string token)
    {
        var ws = _userSessionContext.GetUserSocket(token);
        if (ws != null)
        {
            _logger.LogInformation($"通知客户端web用户绑定成功loginToken");
            WSMessage notifyMsg = new WSMessage();
            notifyMsg.submsg = "LoginZte";
            notifyMsg.msg = "Notify";
            await ws.SendAsync(notifyMsg.ToSocketMessage(), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }


    [HttpGet]
    public async Task DownLoad([FromQuery, Required] string loginToken)
    {
        HttpClient _client = new HttpClient();

        _client.BaseAddress = new Uri("https://csl.zte.com.cn");

        _client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ6eGNzbCIsInN1YiI6IkNTTDEwMDAwMDE2IiwiYXVkIjpbImZlbmdtYWkiXSwiZXhwIjoxNzQ5MTE5NjA3LCJuYmYiOjE3NDkwOTA4MDcsImlhdCI6MTc0OTA5MDgwN30.Ql0R9eDpo1zIvvmoXCf7I8ttblkbb7GQmiRmOvRoDTc");
        _client.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://csl.zte.com.cn");

        StringContent stringContent = new StringContent("{\"vmId\":\"47\",\"vmName\":\"ENS47\",\"vmModel\":\"SWITCH\",\"projectName\":\"2025-06-05 10:33:34-project\",\"host\":\"192.168.6.252\",\"port\":\"xxvgei-0/20/1/2\"}", encoding: Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("oprationvm/downloadPacketCaptureFile", stringContent);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
 
            System.IO.File.WriteAllBytes("C:\\Users\\Administrator\\Desktop\\2.pcap", content);
        } 

    }

}
