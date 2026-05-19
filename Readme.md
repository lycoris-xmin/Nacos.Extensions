# Lycoris.Nacos.Extensions

Nacos SDK 扩展封装库，简化 Nacos 配置中心、服务注册发现以及基于 Nacos 服务发现的 HTTP 请求调用。

## 安装

```bash
dotnet add package Lycoris.Nacos.Extensions
```

### 框架支持

- .NET 6.0 / 7.0 / 8.0

---

## 快速开始

### 1. Nacos 远端配置（替代 appsettings.json）

将 Nacos 上的 JSON 配置注入到 `IConfiguration`，像读取本地 appsettings.json 一样读取远端配置。

**注册**

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddNacosAppSettings(opt =>
{
    // 阿里云 MSE 引擎使用 AccessKey / SecretKey
    opt.AccessKey = "accessKey";
    opt.SecretKey = "secretKey";

    // 自建 Nacos 集群使用用户名 / 密码
    opt.UserName = "nacos username";
    opt.Password = "nacos password";

    opt.Server = new List<string> { "http://127.0.0.1:8848" };
    opt.Namespace = "namespace";

    opt.DefaultTimeOut = 15000;
    opt.ListenInterval = 1000;

    // 添加需要监听的配置
    opt.AddConfigListener("logsetting", "DEFAULT_GROUP");
    opt.AddConfigListener("redis", "DEFAULT_GROUP");
});
```

**读取配置**

```csharp
// 读取字符串
var host = NacosAppSettings.GetConfig("Redis:Host");
var host = NacosAppSettings.GetConfig("Redis:Host", "127.0.0.1"); // 带默认值
var host = NacosAppSettings.TryGetConfig("Redis:Host");           // 读取失败不抛异常

// 读取实体
var nodes = NacosAppSettings.GetSection<List<string>>("Redis:Nodes");
var config = NacosAppSettings.GetSection<RedisConfig>("Redis");
```

---

### 2. 服务注册与发现

**注册**

```csharp
builder.Services.AddNacosRegisterCenter(opt =>
{
    opt.Server = new List<string> { "http://127.0.0.1:8848" };
    opt.Namespace = "namespace";
    opt.GroupName = "DEFAULT_GROUP";
    opt.ServiceName = "my-service";
    opt.ClusterName = "DEFAULT";
    opt.Port = 5000;

    // 自建集群：用户名 / 密码
    opt.UserName = "nacos";
    opt.Password = "nacos";

    // 阿里云 MSE：AccessKey / SecretKey
    // opt.AccessKey = "ak";
    // opt.SecretKey = "sk";

    // 指定本机 IP（默认自动获取）
    opt.Ip = "192.168.1.100";

    // 自定义元数据
    opt.AddMeta("version", "1.0.0");
});
```

---

### 3. 配置中心（强类型配置 + 自动监听）

定义配置实体，继承 `NacosConfiguration<T>`，注册后自动拉取配置并监听变更。

**定义配置实体**

```csharp
public class AuthenticationConfiguration : NacosConfiguration<AuthenticationConfiguration>
{
    public string Default { get; set; }
    public List<string> Other { get; set; }

    public AuthenticationConfiguration()
    {
        DataId = "Authentication";
        Group = "Galaxy.Configuration";
        NacosListener = true; // 启用配置变更监听
    }

    // 方式一：收到原始 JSON 字符串
    public override void Listener(ILogger? logger, string? configuration)
    {
        logger?.LogInformation("配置变更: {config}", configuration);
        base.Listener(logger, configuration);
    }

    // 方式二：收到已反序列化的强类型对象
    public override void Received(AuthenticationConfiguration? config)
    {
        if (config == null) return;
        Default = config.Default;
        Other = config.Other ?? new List<string>();
    }
}
```

> **注意**：如果同时重写 `Listener` 和 `Received`，只会执行 `Listener`。推荐任选其一即可。

**注册配置中心**

```csharp
builder.Services.AddNacosNamingConfiguration(opt =>
{
    opt.Server = new List<string> { "http://127.0.0.1:8848" };
    opt.Namespace = "namespace";

    opt.UserName = "nacos";
    opt.Password = "nacos";

    // 注册配置实体
    opt.AddNacosConfiguration<AuthenticationConfiguration>();

    // 启用 NacosOptions<T> 用于手动推送 / 拉取配置
    opt.UseNacosOptions();
});
```

**读取配置**

```csharp
public class MyService
{
    private readonly AuthenticationConfiguration _config;
    private readonly INacosOptions<AuthenticationConfiguration> _options;

    public MyService(
        AuthenticationConfiguration config,
        INacosOptions<AuthenticationConfiguration> options)
    {
        _config = config;
        _options = options;
    }

    public async Task RefreshAsync()
    {
        // 推送到 Nacos
        await _options.PushAsync();
        // 从 Nacos 拉取
        await _options.PullAsync();
        // 删除 Nacos 上的配置
        await _options.RemoveAsync();
    }
}
```

**手动操作配置中心**

```csharp
public class ConfigManager
{
    private readonly INacosConfigurationService _svc;

    public ConfigManager(INacosConfigurationService svc) => _svc = svc;

    public async Task DemoAsync()
    {
        // 获取配置
        var json = await _svc.GetConfigurationAsync("dataId", "group");
        var obj  = await _svc.GetConfigurationAsync<MyConfig>("dataId", "group");

        // 发布配置
        await _svc.PublishConfigurationAsync("dataId", "group", "{\"key\":\"value\"}");
        await _svc.PublishConfigurationAsync("dataId", "group", new MyConfig());

        // 删除配置
        await _svc.RemoveConfigurationAsync("dataId", "group");
    }
}
```

---

### 4. HTTP 请求（基于 Nacos 服务发现）

通过 groupName + serviceName 自动从 Nacos 获取健康实例地址并发起 HTTP 请求，无需关心 IP 和端口。

**注册**

```csharp
// 默认注册
builder.Services.AddNacosHttpClient();

// 带日志配置
builder.Services.AddNacosHttpClient(opt =>
{
    opt.EnableLogger = true;
    opt.AllowAllHeaderFilter = true;
    opt.HeaderFilter = new List<string> { "Authorization", "X-Request-Id" };
    opt.CookieFilter = new List<string> { "session" };
});
```

**基础请求**

```csharp
public class DemoService
{
    private readonly INacosHttpClient _client;

    public DemoService(INacosHttpClient client) => _client = client;

    public async Task DemoAsync()
    {
        // GET
        var res = await _client.HttpGetAsync("DEFAULT_GROUP", "api-service");
        var res = await _client.HttpGetAsync("DEFAULT_GROUP", "api-service", "/api/users", "page=1&size=10");

        // POST
        var res = await _client.HttpPostAsync("DEFAULT_GROUP", "api-service", "{\"name\":\"test\"}");
        var res = await _client.HttpPostAsync("DEFAULT_GROUP", "api-service", "/api/users", "{\"name\":\"test\"}");

        // PUT
        var res = await _client.HttpPutAsync("DEFAULT_GROUP", "api-service", "{\"name\":\"test\"}");
        var res = await _client.HttpPutAsync("DEFAULT_GROUP", "api-service", "/api/users/1", "{\"name\":\"test\"}");

        // PATCH
        var res = await _client.HttpPatchAsync("DEFAULT_GROUP", "api-service", "{\"name\":\"test\"}");
        var res = await _client.HttpPatchAsync("DEFAULT_GROUP", "api-service", "/api/users/1", "{\"name\":\"test\"}");

        // DELETE
        var res = await _client.HttpDeleteAsync("DEFAULT_GROUP", "api-service");
        var res = await _client.HttpDeleteAsync("DEFAULT_GROUP", "api-service", "/api/users/1", "{\"id\":1}");

        // 响应处理
        if (res.Success)
            Console.WriteLine($"Status: {res.HttpStatusCode}, Body: {res.Content}");
        else
            Console.WriteLine($"Error: {res.Exception?.Message}");
    }
}
```

**自定义请求**

```csharp
public async Task CustomRequestAsync()
{
    var res = await _client.HttpRequestAsync("DEFAULT_GROUP", "api-service", opt =>
    {
        opt.HttpMethod = HttpMethod.Post;
        opt.Url = "/api/users";
        opt.Timeout = 5;               // 超时（秒），默认 3
        opt.TraceId = "trace-123";     // 链路追踪 ID

        // 请求头
        opt.AddHeader("Authorization", "Bearer token");
        opt.AddHeaders(("X-Tenant", "123"), ("X-Region", "cn"));

        // Cookie
        opt.AddCookie("session", "abc123");
        opt.AddCookie("/api", "token", "xyz");

        // JSON 请求体
        opt.AddJsonBody(new { name = "test" });
        opt.AddJsonBody("{\"name\":\"test\"}");

        // 表单
        opt.AddFormData("key", "value");

        // 上传文件
        opt.AddFormFile("file", "/path/to/file.png", "avatar.png");
    });
}
```

**并发批量请求**

```csharp
public async Task BatchRequestAsync()
{
    var requests = new NacosMultipleHttpRequest[]
    {
        new("DEFAULT_GROUP", "user-service", opt =>
        {
            opt.HttpMethod = HttpMethod.Get;
            opt.Url = "/api/users/1";
        }),
        new("DEFAULT_GROUP", "order-service", opt =>
        {
            opt.HttpMethod = HttpMethod.Post;
            opt.Url = "/api/orders";
            opt.AddJsonBody(new { productId = 1 });
        }),
        new("DEFAULT_GROUP", "pay-service", opt =>
        {
            opt.HttpMethod = HttpMethod.Post;
            opt.Url = "/api/pay";
            opt.AddJsonBody(new { orderId = "abc" });
        })
    };

    var results = await _client.MultipleHttpRequestAsync(requests);
    foreach (var r in results)
        Console.WriteLine($"Success: {r.Success}, Content: {r.Content}");
}
```

---

### 5. 链路追踪

`HttpRequest` / `HttpRequestAsync` 方法内置追踪支持，可集成 OpenTelemetry、SkyWalking 或自定义追踪系统。

#### 注册追踪

```csharp
// OpenTelemetry 兼容追踪（基于 System.Diagnostics.Activity，无需额外依赖）
builder.Services.AddNacosHttpClient(opt =>
{
    opt.UseOpenTelemetryTracing();
});

// SkyWalking 追踪（自动传播 sw8 请求头）
builder.Services.AddNacosHttpClient(opt =>
{
    opt.UseSkyWalkingTracing();

    // 可选：设置 SkyWalking trace 上下文来源
    NacosSkyWalkingTracing.TraceContextProvider = () =>
    {
        // 从 Header、AsyncLocal 或其他来源获取当前 sw8 上下文
        return Activity.Current?.GetBaggageItem("sw8");
    };
});

// 自定义追踪实现
builder.Services.AddNacosHttpClient(opt =>
{
    opt.UseTracing<MyCustomTracing>();
});
```

#### 实现自定义追踪

```csharp
public class MyCustomTracing : INacosTracing
{
    public INacosTracingSpan? StartSpan(string serviceName, string httpMethod, string? url)
    {
        // 创建 Span，返回 null 表示不追踪
        return new MySpan(serviceName, httpMethod, url);
    }
}

public class MySpan : INacosTracingSpan
{
    public void SetTag(string key, string? value)
    {
        // 设置标签（如 http.status_code、nacos.group 等）
    }

    public void SetError(Exception exception)
    {
        // 记录异常
    }

    public void PropagateHeaders(Action<string, string> addHeader)
    {
        // 将追踪上下文注入出站请求头
        addHeader("x-trace-id", TraceContext.Current.TraceId);
    }

    public void Dispose()
    {
        // 结束 Span
    }
}
```

#### 追踪数据说明

每个 HTTP 请求会自动记录以下标签：

| 标签 | 说明 |
|------|------|
| `nacos.service` | Nacos 服务名 |
| `nacos.group` | Nacos 分组 |
| `http.method` | HTTP 请求方法 |
| `http.url` | 请求路径 |
| `http.status_code` | HTTP 响应状态码 |
| `exception.type` | 异常类型（请求失败时） |
| `exception.message` | 异常消息（请求失败时） |

#### 使用 SkyWalkingHeaderBuilder

```csharp
// 手动构建 SkyWalking sw8 头
var sw8 = SkyWalkingHeaderBuilder.BuildSw8Header(
    traceId: "abc123",
    segmentId: "seg456",
    spanId: 1,
    serviceName: "my-service",
    serviceInstance: "my-service-instance",
    endpoint: "/api/users",
    peer: "target-service"
);
```

> **提示**：使用 OpenTelemetry 时，该库创建的 `Activity` 会被 OpenTelemetry SDK 自动采集并导出到配置的 Exporter（Jaeger、Zipkin、OTLP 等），无需额外配置。

---

### 6. 弹性策略（Polly — 重试 / 断路器 / 降级）

基于 [Polly](https://github.com/App-vNext/Polly) v8 实现的弹性策略管道，支持重试、断路器和降级。

#### 注册弹性策略

```csharp
builder.Services.AddNacosHttpClient(opt =>
{
    opt.UseResilience(resilience =>
    {
        // 重试策略
        resilience.EnableRetry = true;
        resilience.RetryCount = 3;                    // 最大重试 3 次
        resilience.RetryBaseDelaySeconds = 2;          // 基础延迟 2 秒
        resilience.RetryBackoffType = RetryBackoffType.Exponential; // 指数退避

        // 断路器策略
        resilience.EnableCircuitBreaker = true;
        resilience.CircuitBreakerFailureRatio = 0.5;   // 失败率超过 50% 触发熔断
        resilience.CircuitBreakerMinimumThroughput = 5; // 最少 5 次采样
        resilience.CircuitBreakerBreakDurationSeconds = 30; // 熔断 30 秒
        resilience.CircuitBreakerSamplingDurationSeconds = 60; // 60 秒采样窗口

        // 降级策略（断路器打开或重试耗尽后返回兜底响应）
        resilience.EnableFallback = true;
        resilience.FallbackAsync = async () => new NacosHttpResponse
        {
            Success = true,
            Content = "{\"degraded\":true}",
            HttpStatusCode = System.Net.HttpStatusCode.OK
        };
    });
});
```

#### 配置参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `EnableRetry` | `false` | 启用重试 |
| `RetryCount` | `3` | 最大重试次数 |
| `RetryBaseDelaySeconds` | `2` | 重试基础延迟（秒） |
| `RetryBackoffType` | `Exponential` | 退避策略：`Fixed` / `Linear` / `Exponential` |
| `EnableCircuitBreaker` | `false` | 启用断路器 |
| `CircuitBreakerFailureRatio` | `0.5` | 失败比例阈值 |
| `CircuitBreakerMinimumThroughput` | `5` | 最小采样数 |
| `CircuitBreakerBreakDurationSeconds` | `30` | 熔断持续时间（秒） |
| `CircuitBreakerSamplingDurationSeconds` | `60` | 采样窗口时间（秒） |
| `EnableFallback` | `false` | 启用降级 |
| `FallbackAsync` | `null` | 降级响应工厂 |

#### 策略执行顺序

```
请求 → 断路器 → 重试 → 降级（兜底）
```

- **断路器**最先判断：如果断路器打开，直接跳过重试执行降级
- **重试**在断路器关闭时生效：对瞬时故障重试指定次数
- **降级**在重试耗尽或断路器打开时执行：返回兜底响应

> **提示**：所有策略仅对 `HttpRequest` / `HttpRequestAsync` 方法生效。弹性策略只作用于 HTTP 调用本身，不影响 Nacos 服务发现过程。

---

## API 参考

### INacosHttpClient

所有方法均提供同步和异步版本，参数一致。

| 方法 | 说明 |
|------|------|
| `HttpGet(group, service)` | GET 请求 |
| `HttpGet(group, service, querying)` | GET 请求带查询参数 |
| `HttpGet(group, service, url, querying)` | GET 请求带路径和查询参数 |
| `HttpPost(group, service, body)` | POST 请求 |
| `HttpPost(group, service, url, body)` | POST 请求带路径 |
| `HttpPut(group, service, body)` | PUT 请求 |
| `HttpPut(group, service, url, body)` | PUT 请求带路径 |
| `HttpPatch(group, service, body)` | PATCH 请求 |
| `HttpPatch(group, service, url, body)` | PATCH 请求带路径 |
| `HttpDelete(group, service, body)` | DELETE 请求 |
| `HttpDelete(group, service, url, body)` | DELETE 请求带路径 |
| `HttpRequest(group, service, configure)` | 自定义请求配置 |
| `MultipleHttpRequestAsync(requests)` | 并发批量请求 |
| `SetTraceId(traceId)` | 设置全局 TraceId |

### NacosAppSettings

| 方法 | 说明 |
|------|------|
| `GetConfig(key)` | 读取配置项 |
| `GetConfig(key, defaultValue)` | 读取配置项（带默认值） |
| `TryGetConfig(key)` | 安全读取（异常返回默认值） |
| `GetConfig<T>(key)` | 读取并转换为强类型 |
| `TryGetConfig<T>(key)` | 安全读取强类型 |
| `GetSection(key)` | 读取配置节点 |
| `TryGetSection(key)` | 安全读取配置节点 |
| `GetSection<T>(key)` | 读取配置节点并绑定为强类型 |
| `TryGetSection<T>(key)` | 安全读取并绑定 |

### INacosConfigurationService

| 方法 | 说明 |
|------|------|
| `GetConfigurationAsync(dataId, group)` | 获取配置（字符串） |
| `GetConfigurationAsync<T>(dataId, group)` | 获取配置（强类型） |
| `PublishConfigurationAsync(dataId, group, value)` | 发布配置 |
| `PublishConfigurationAsync<T>(dataId, group, value)` | 发布强类型配置 |
| `RemoveConfigurationAsync(dataId, group)` | 删除配置 |
| `AddConfigListenerAsync(dataId, group, listener)` | 添加配置监听 |
| `RemoveConfigListenerAsync(dataId, group, listener)` | 移除配置监听 |

### INacosServerService

| 方法 | 说明 |
|------|------|
| `GetHealthyInstanceAsync(group, service)` | 获取健康实例地址 |

### 链路追踪 API

#### INacosTracing

| 方法 | 说明 |
|------|------|
| `StartSpan(serviceName, httpMethod, url)` | 创建追踪 Span，返回 null 表示不追踪 |

#### INacosTracingSpan

| 方法 | 说明 |
|------|------|
| `SetTag(key, value)` | 设置 Span 标签 |
| `SetError(exception)` | 记录异常 |
| `PropagateHeaders(addHeader)` | 将追踪上下文传播到出站请求头 |
| `Dispose()` | 释放 Span（请求完成时自动调用） |

#### NacosHttpClientBuilder 追踪方法

| 方法 | 说明 |
|------|------|
| `UseOpenTelemetryTracing()` | 启用基于 System.Diagnostics.Activity 的追踪（自动兼容 OpenTelemetry） |
| `UseSkyWalkingTracing()` | 启用 SkyWalking 追踪（自动传播 sw8 头） |
| `UseTracing<T>()` | 启用自定义追踪实现 |

#### SkyWalkingHeaderBuilder

| 方法 | 说明 |
|------|------|
| `BuildSw8Header(traceId, segmentId, spanId, serviceName, serviceInstance, endpoint, peer)` | 构建 SkyWalking sw8 请求头值 |

### 弹性策略 API

#### NacosResilienceOptions

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `EnableRetry` | `bool` | `false` | 启用重试策略 |
| `RetryCount` | `int` | `3` | 最大重试次数 |
| `RetryBaseDelaySeconds` | `int` | `2` | 重试基础延迟 |
| `RetryBackoffType` | `RetryBackoffType` | `Exponential` | 退避策略 |
| `EnableCircuitBreaker` | `bool` | `false` | 启用断路器 |
| `CircuitBreakerFailureRatio` | `double` | `0.5` | 失败比例阈值 |
| `CircuitBreakerMinimumThroughput` | `int` | `5` | 最小吞吐量 |
| `CircuitBreakerBreakDurationSeconds` | `int` | `30` | 熔断持续时间 |
| `CircuitBreakerSamplingDurationSeconds` | `int` | `60` | 采样窗口 |
| `EnableFallback` | `bool` | `false` | 启用降级 |
| `FallbackAsync` | `Func<Task<NacosHttpResponse>>?` | `null` | 降级响应工厂 |

#### NacosResiliencePipeline

| 方法 | 说明 |
|------|------|
| `Create(options)` | 根据配置创建弹性管道 |
| `ExecuteAsync(action, ct)` | 通过弹性管道执行请求 |

---

## 依赖

本项目依赖于以下 NuGet 包：

- [nacos-sdk-csharp](https://github.com/nacos-group/nacos-sdk-csharp) — Apache License 2.0
- [nacos-sdk-csharp.AspNetCore](https://github.com/nacos-group/nacos-sdk-csharp) — Apache License 2.0
- [nacos-sdk-csharp.Extensions.Configuration](https://github.com/nacos-group/nacos-sdk-csharp) — Apache License 2.0
- [Polly.Core](https://github.com/App-vNext/Polly) — BSD 3-Clause License

---

## 开源许可

### 本项目

MIT License

Copyright (c) 2024 Lycoris

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

### 第三方依赖许可声明

本项目使用了以下第三方开源软件，其版权归各自所有者所有：

**Nacos SDK for C#** (`nacos-sdk-csharp`, `nacos-sdk-csharp.AspNetCore`, `nacos-sdk-csharp.Extensions.Configuration`)

```
                                 Apache License
                           Version 2.0, January 2004
                        http://www.apache.org/licenses/

   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION
   ...
```

以上组件根据 **Apache License 2.0** 授权使用。完整许可证文本请参见：
- https://www.apache.org/licenses/LICENSE-2.0
- https://github.com/nacos-group/nacos-sdk-csharp

本项目的 MIT 许可不影响上述第三方组件各自的 Apache 2.0 许可条款。
