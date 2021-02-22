using System;

namespace Contracts
{
    public interface IJobMessage
    {
        string JobName { get; set; }
        int JobId { get; set; }
        string Progress { get; set; } // demo
        string Status { get; set; }
    }

    public class JobMessage : IJobMessage
    {
        public string JobName { get; set; }

        public int JobId { get; set; }
        public string Progress { get; set; } // demo
        public string Status { get; set; }
    }
}
