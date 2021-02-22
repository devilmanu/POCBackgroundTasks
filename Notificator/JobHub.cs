using Contracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notificator
{
    public class JobHub : Hub
    {
        public async Task MessageWasReceive(JobMessage jobMessage)
        {
            await Clients.All.SendAsync("sendprogress", jobMessage);
        }
    }

    public interface JobHubContract
    {
        Task Receive(JobMessage jobMessage);
    }
}
