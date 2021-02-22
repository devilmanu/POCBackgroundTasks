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

        public IEnumerable<SucceededJobDto> GetJobs(CancellationToken token)
        {
            return JobStorage.Current.GetMonitoringApi().SucceededJobs(0, 100).Select(o => o.Value);
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
            message.Status = "Initialized";
            await _publishEndpoint.Publish(message);

            await OpenCli(message);

            message.Status = "Finnished";
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
                message.Status = "working";
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
    }

}
