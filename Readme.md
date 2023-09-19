## Nacos SDK 相关使用扩展及部分封装请求

### Appsettings 使用Naocs远端配置读取

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
    // 注意当前仅支持一个配置
    opt.AddConfigListener("DataId", "group");
})
```

