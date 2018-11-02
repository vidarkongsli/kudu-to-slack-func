using System;

namespace v2 {
        public class KuduDeployResult
    {
        public string Id { get; set; }
        public KuduStatus Status { get; set; }
        public string StatusText { get; set; }
        public string AuthorEmail { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public string Deployer { get; set; }
        public DateTimeOffset ReceivedTime { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastSuccessEndTime { get; set; }
        public bool Complete { get; set; }
        public string SiteName { get; set; }
        public string HostName { get; set; }
        public bool IsSuccess => Status == KuduStatus.Success;

        public enum KuduStatus
        {
            Failed,
            Success
        }
    }

}