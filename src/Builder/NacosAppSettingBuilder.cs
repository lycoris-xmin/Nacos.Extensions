using Lycoris.Nacos.Extensions.Options;
using Nacos.Microsoft.Extensions.Configuration;

namespace Lycoris.Nacos.Extensions.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public class NacosAppSettingBuilder : NacosBaseOption
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataId">Configuration ID</param>
        /// <param name="Group">Configuration group</param>
        /// <param name="Optional">Determines if the Nacos Server is optional</param>
        public void AddConfigListener(string DataId, string Group, bool Optional = false)
        {
            this.Listeners ??= new List<ConfigListener>();
            this.Listeners.Add(new ConfigListener()
            {
                DataId = DataId,
                Group = Group,
                Optional = Optional
            });
        }

        internal List<ConfigListener> Listeners { get; set; } = new List<ConfigListener>();
    }
}
