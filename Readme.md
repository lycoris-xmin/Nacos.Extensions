### Nacos SDK 相关使用扩展及部分封装请求

## 使用Naocs远端配置AppSetting.json

### 配置注册
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddNacosAppSettings(opt => 
{
    // 如果是阿里云购买的服务需要填写秘钥（自己搭建的nacos服务集群，不填写该部分）
    opt.AccessKey = "accessKey";
    opt.SecretKey = "secretKey";

    // 如果是自己搭建的则需要填写帐号密码 (阿里云购买的MES引擎，不填写该部分)
    opt.UserName = "nacos username";
    opt.Password = "nacos password";

    opt.Server = "nacos  service ipaddress";
    opt.Namespace = "namespace";

    // 连接超时时间，默认：15秒
    opt.DefaultTimeOut = 15000;
    // 监听轮询时间，默认：1秒
    opt.ListenInterval = 1000;

    // 添加配置
    opt.AddConfigListener("logsetting", "group");
    opt.AddConfigListener("redis", "group");
    opt.AddConfigListener("sql", "group");
    opt.AddConfigListener("es", "group");
    opt.AddConfigListener("otlp", "group");
})
```

### 读取配置

#### nacos上配置信息
```json
{
    "Redis": {
        //redis服务器地址
        "Host": "host",
        //redis服务器端口
        "Port": 6379,
        //redis服务器密码
        "Password": "",
        //redis服务器 数据库配置
        "UseDatabase": 0,
        "Poolsize": 50,
        "SSL": false,
        "ConnectTimeout": 15000,
        "SyncTimeout ": 5000,
        "ConnectRetry": 5,
        "Nodes":["1","2","3"]
    }
}
```

#### 读取方法
```csharp
// 读取redis配置
var host = NacosAppSettings.GetConfig("Redis:Host");
// 读不到的时候，取默认值
var host = NacosAppSettings.GetConfig("Redis:Host"，"默认值");
// 
var host = NacosAppSettings.TryGetConfig("Redis:Host");
// 读取实体或列表方法
var nodes = NacosAppSettings.TryGetSection<List<string>>("Redis:Nodes")
```

## 使用Naocs注册发现

```csharp
// 注册中心
services.AddNacosRegisterCenter(opt =>
{
    opt.Server = AppSettings.Nacos.ServerAddresses;
    opt.Namespace = AppSettings.Nacos.Namespace;
    opt.ClusterName = AppSettings.Nacos.ClusterName;
    opt.GroupName = AppSettings.Application.GroupName;
    opt.ServiceName = AppSettings.Application.ServiceName;

    // 如果是自己搭建的nacos服务，需要使用帐号密码连接
    opt.UserName = AppSettings.Nacos.UserName;
    opt.Password = AppSettings.Nacos.Password;

    // 如果是阿里云购买的服务，需要使用ak,sk进行连接
    opt.AccessKey = AppSettings.Nacos.AccessKey;
    opt.SecretKey = AppSettings.Nacos.SecretKey;

    // 如果需要自己指定当前实例服务的请求地址，请输入指定ip，如果不需要指定，程序会默认获取当前机器IP作为实例连接地址
    if (!AppSettings.Application.ServerIP.IsNullOrEmpty())
        opt.Ip = AppSettings.Application.ServerIP;

    // 服务访问端口
    opt.Port = AppSettings.Application.HttpPort;

    // 添加实例头部自定义信息
    opt.AddMeta("ServiceGuid", AppSettings.Application.ServiceGuid);
});
```

## 使用Nacos配置中心

### 创建Naocs配置实体

**配置实体需要继承 `NacosConfiguration` 类，并做相应配置,配置建议注册为 `Singleton` 单例服务**

```csharp
public class AuthenticationConfiguration : NacosConfiguration<AuthenticationConfiguration>
{
    public string Default { get; set; }

    public List<string> Other { get; set; }

    //.....其他配置,只要属性的类型与naocs上的配置json对应即可

    public AuthenticationConfiguration()
    {
        // nacos配置上对应的DataId
        DataId = "Authentication";
        // nacos配置上对应的group
        Group = "Galaxy.Configuration";
        // 是否启用配置监听(如果启动，配置变化时，将会触发回调)
        // 默认配置不会启用监听
        NacosListener = true;
    }

    // 以下两种监听方法，任选其一重写即可，两个都重写的话，回调将只会执行 Listener 方法;

    // 重写监听方法
    public override void Listener(ILogger? logger,string configuration)
    {
        // do something
    }

    // 重写监听方法
    public override void Received(AuthenticationConfiguration? configuration)
    {
        // do something
    }
}
```

### 注册配置中心
```csharp
// 配置中心
services.AddNacosNamingConfiguration(opt =>
{
    opt.Server = AppSettings.Nacos.ServerAddresses;
    // 可以与Naocs远端配置不同一个命名空间
    opt.Namespace = AppSettings.Nacos.Namespace;

     // 如果是自己搭建的nacos服务，需要使用帐号密码连接
    opt.UserName = AppSettings.Nacos.UserName;
    opt.Password = AppSettings.Nacos.Password;

    // 如果是阿里云购买的服务，需要使用ak,sk进行连接
    opt.AccessKey = AppSettings.Nacos.AccessKey;
    opt.SecretKey = AppSettings.Nacos.SecretKey;

    // 添加Naocs对应配置实体
    opt.AddNacosConfiguration<AuthenticationConfiguration>();
});
```


## Nacos请求封装

### 注册
```csharp
// 默认注册方式
services.AddNacosHttpClient();
// 自定义部分配置注册方法
services.AddNacosHttpClient(opt => 
{
    // 启用请求日志记录
    opt.EnableLogger = true;
    // 是否记录所有头部信息，默认：否
    opt.AllowAllHeaderFilter = true;
    // 日志请求头过滤集合
    opt.HeaderFilter = [];
    // 日志Cookie过滤集合
    opt.CookieFilter = [];
});
```

### 使用方式
```csharp
public class Demo
{
    private readonly INacosHttpClient _nacos;

    public Demo(INacosHttpClient nacos)
    {
        _naocs = nacos;
    }

    // 此处为作为示例方便展示，故不要纠结res重复声明问题
    public async Task DemoTestAsync()
    {
        // 所有方法均有同步、异步，方法入参相同，此处仅以异步方法作为示例
        // 基础用法
        // get
        var res = await _nacos.HttpGetAsync("group", "dataid", "querying");
        var res = await _nacos.HttpGetAsync("group", "dataid", "/api/controller/action", "querying");

        // post
        var res = await _nacos.HttpPostAsync("group", "dataid", "jsonbody");
        var res = await _nacos.HttpPostAsync("group", "dataid", "/api/controller/action", "jsonbody");

        // put
        var res = await _nacos.HttpPutAsync("group", "dataid", "jsonbody");
        var res = await _nacos.HttpPutAsync("group", "dataid", "/api/controller/action", "jsonbody");

        // delete
        var res = await _nacos.HttpDeleteAsync("group", "dataid", "jsonbody");
        var res = await _nacos.HttpDeleteAsync("group", "dataid", "/api/controller/action", "jsonbody");

        // 更复杂的请求
        var res = await _nacos.HttpRequestAsync("group", "dataid", opt =>
        {
            opt.HttpMethod = HttpMethod.Post;
            opt.AddHeader("Header", "value");
            opt.AddCookies("Cookie", "value");

            // get请求使用
            opt.AddQuerying("querying");

            // 其他请求使用
            opt.AddJsonBody("jsonbody");

            // 表单
            opt.AddFormData("key", "value");
            opt.AddFormFile("key", "filePath", "fileName");

            // 日志唯一标识，可不设置
            opt.TraceId = "";
        });

        // 多请求并发执行
        var requests = 
        [
            new NacosMultipleHttpRequest()
            {
                GroupName = "",
                ServiceName = "",
                Option = opt => 
                {
                    opt.HttpMethod = HttpMethod.Post;
                    opt.AddHeader("Header", "value");
                    opt.AddCookies("Cookie", "value");

                    // get请求使用
                    opt.AddQuerying("querying");

                    // 其他请求使用
                    opt.AddJsonBody("jsonbody");

                    // 表单
                    opt.AddFormData("key", "value");
                    opt.AddFormFile("key", "filePath", "fileName");

                    // 日志唯一标识，可不设置
                    opt.TraceId = "";
                }
            },
            new NacosMultipleHttpRequest()
            {
                GroupName = "",
                ServiceName = "",
                Option = opt => 
                {
                    opt.HttpMethod = HttpMethod.Post;
                    opt.AddHeader("Header", "value");
                    opt.AddCookies("Cookie", "value");

                    // get请求使用
                    opt.AddQuerying("querying");

                    // 其他请求使用
                    opt.AddJsonBody("jsonbody");

                    // 表单
                    opt.AddFormData("key", "value");
                    opt.AddFormFile("key", "filePath", "fileName");

                    // 日志唯一标识，可不设置
                    opt.TraceId = "";
                }
            }
        ]

        var resArray = await _nacos.MultipleHttpRequestAsync(requests);
    }
}
```

