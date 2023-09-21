using Lycoris.Nacos.Extensions;

namespace Nacos.Sample1
{
    public class TestConfiguration2 : NacosConfiguration<TestConfiguration2>
    {
        public TestConfiguration2()
        {
            this.DataId = "Authentication";
            this.Group = "Galaxy.Configuration";
            this.NacosListener = true;
        }

        /// <summary>
        /// 手机一键登录一天限制次数
        /// </summary>
        public int PhoneQuickLoginDayMaxCount { get; set; }

        public override void Listener(ILogger? logger, string? configuration)
        {
            logger?.LogInformation(configuration);

            base.Listener(logger, configuration);
        }
    }
}
