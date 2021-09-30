using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using Contracts;
using Hangfire;
using MassTransit;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Storage.Monitoring;

namespace JobManager.Services
{
    public class JobService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public readonly IBackgroundJobClient _backgroundJobClient;

        public JobService(IPublishEndpoint publishEndpoint, IBackgroundJobClient backgroundJobClient)
        {
            _publishEndpoint = publishEndpoint;
            _backgroundJobClient = backgroundJobClient;
        }

        public IEnumerable<CompletedJobsDto> GetJobs(CancellationToken token)
        {
            return JobStorage.Current.GetMonitoringApi().SucceededJobs(0, 100).Select(o => new CompletedJobsDto { 
                InSucceededState = o.Value.InSucceededState,
                JobId = o.Key,
                Result = o.Value.Result,
                SucceededAt = o.Value.SucceededAt,
                TotalDuration = o.Value.TotalDuration
            });
        }


        public JobServiceDto Create(JobServiceDto dto, CancellationToken token) 
        {
            _backgroundJobClient.Enqueue(() => RunJob(dto));
            // Get available threads  
            ThreadPool.GetAvailableThreads(out int workers, out int ports);
            Console.WriteLine($"Availalbe worker threads: {workers} ");
            Console.WriteLine($"Available completion port threads: {ports}");
            return dto;
        }

        public async Task RunJob(JobServiceDto dto)
        {
            var message = new JobMessage();
            message.JobName = dto.JobName;
            message.JobId = dto.JobId;
            message.Progress = "0%";
            message.Status = JobStatus.Initialized;
            await _publishEndpoint.Publish(message);

            await NOCli(message);

            message.Status = JobStatus.Finished;
            message.Progress = "100%";
            await _publishEndpoint.Publish(message);
        }

        public Task OpenCli(JobMessage message)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.EnableRaisingEvents = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            { Console.WriteLine(e.Data); });
            p.OutputDataReceived += new DataReceivedEventHandler(async (sender, e) =>
            {
                message.Progress = e.Data;
                message.Status = JobStatus.InProgress;
                await _publishEndpoint.Publish(message);
            });
            p.StartInfo.FileName = "C:\\Users\\devil\\source\\repos\\DemoBackgroundTasks\\JobService\\bin\\Debug\\netcoreapp3.1\\JobService.exe";
            p.StartInfo.Arguments = $"{message.JobName} { message.JobId}";
            p.Start();

            // To avoid deadlocks, use an asynchronous read operation on at least one of the streams.  
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.CancelOutputRead();
            return Task.CompletedTask;
        }

        public async Task NOCli(JobMessage message)
        {
            for (int i = 0; i < 1000; i++)
            {
                message.Progress = $"{(int)Math.Round((double)(100 * i) / 1000)}%";
                message.Status = JobStatus.InProgress;
                await _publishEndpoint.Publish(message);
            };
        }
    }

    public class CompletedJobsDto
    {
        public string JobId { get; set; }
        public object Result { get; set; }
        public long? TotalDuration { get; set; }
        public DateTime? SucceededAt { get; set; }
        public bool InSucceededState { get; set; }
    }

}
