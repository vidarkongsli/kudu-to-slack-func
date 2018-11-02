using System;
using Newtonsoft.Json;

namespace v2
{
    public class SlackMessage
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "icon_emoji")]
        public string IconEmoji { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }
        public static SlackMessage From(KuduDeployResult result)
        {
            var s = new SlackMessage
            {
                Username = (result.IsSuccess ? "Published: " : "Failed: ")
                           + (result.SiteName ?? "unknown"),
                IconEmoji = result.IsSuccess ? ":shipit:" : ":warning:",
                Text = $@"Initiated by: {result.Author ?? "unknown"} at {result.EndTime:R}
<https://{result.HostName}|{result.SiteName}> Id: {result.Id}{Environment.NewLine}```{result.Message}```",
                Channel = "channel"
            };
            return s;
        }
        public SlackMessage ToChannel(string channel)
        {
            Channel = channel;
            return this;
        }
    }

}