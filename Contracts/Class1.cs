using System;

namespace Contracts
{
    public interface IJobMessage
    {
        string JobName { get; set; }
        string JobId { get; set; }
        string Progress { get; set; } // demo
        JobStatus Status { get; set; }
    }

    public enum JobStatus
    {
        InProgress,
        Initialized,
        Finished,
        Queued
    }

    public class JobMessage : IJobMessage
    {
        public string JobName { get; set; }

        public string JobId { get; set; }
        public string Progress { get; set; } // demo
        public JobStatus Status { get; set; }
    }
}
