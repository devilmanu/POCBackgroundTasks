using Contracts;
using MassTransit;
using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Utils;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notificator
{
    class CliConsumer :
    IConsumer<JobMessage>
    {
        public CliConsumer() { }

        public async Task Consume(ConsumeContext<JobMessage> context)
        {
            IReadOnlyList<IHubProtocol> protocols = new IHubProtocol[] { new JsonHubProtocol() };
            await context.Publish<All<JobHub>>(new
            {
                Messages = protocols.ToProtocolDictionary("sendprogress", new object[] { context.Message })
            });
        }
    }
}
