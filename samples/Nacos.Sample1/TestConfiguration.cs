using Lycoris.Base.Extensions;
using Lycoris.Nacos.Extensions;

namespace Nacos.Sample1
{
    public class TestConfiguration : NacosConfiguration<TestConfiguration>
    {
        /// <summary>
        /// 
        /// </summary>
        public TestConfiguration()
        {
            this.DataId = "Post";
            this.Group = "Galaxy.Configuration";
            this.NacosListener = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> AnonymousAvatar { get; set; } = new List<string>();

        /// <summary>
        /// 帖子气泡图
        /// </summary>
        public Dictionary<int, string> PostBubble { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// 帖子创建社区热度阈值
        /// </summary>
        public int PostHotCreateCommunity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Received(TestConfiguration? config)
        {
            Console.WriteLine("Post.Galaxy.Configuration" + config.ToJson());

            this.AnonymousAvatar = config?.AnonymousAvatar ?? new List<string>();
            this.PostBubble = config?.PostBubble ?? new Dictionary<int, string>();
            this.PostHotCreateCommunity = config?.PostHotCreateCommunity ?? new int();
        }
    }
}
