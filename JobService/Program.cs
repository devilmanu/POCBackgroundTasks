using System;
using System.Linq;
using System.Threading;

namespace JobService
{
    class Program
    {
        static void Main(string[] args)
        {
            var jobId = args.ElementAtOrDefault(1) ?? "pruebaid";
            var jobName = args.ElementAtOrDefault(0) ?? "pruebaname";

            var job = new Job(jobId, jobName);
            job.RunJob();
        }
    }

    public class Job
    {
        private string _jobId { get; set; }
        private string _jobName { get; set; }
        public Job(string jobId, string jobName)
        {
            _jobId = jobId;
            _jobName = jobName;

        }

        public void RunJob()
        {
            for (int i = 1; i <= 1000; i++)
            {
                Thread.Sleep(50);
                Console.WriteLine($"{(int)Math.Round((double)(100 * i) / 1000)}%");
            }
        }
    }
}
