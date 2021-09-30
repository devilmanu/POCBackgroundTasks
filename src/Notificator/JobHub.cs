using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Notificator
{
    public class JobHub : Hub<JobHubContract> 
    {
        public async Task SendProgress(JobMessage jobMessage) 
        {
            await Clients.All.SendProgress(jobMessage);
        }
    }


    public interface JobHubContract
    {
        Task SendProgress(JobMessage jobMessage);
    }
}
