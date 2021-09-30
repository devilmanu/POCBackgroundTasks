using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace CrossCutting.ServiceDiscovery
{
public class ServiceDiscoveryHostedService : IHostedService
    {
        private readonly IConsulClient _client;
        private readonly ServiceConfig _config;
        private readonly IConfiguration _configuration;
        private string _registrationId;

        public ServiceDiscoveryHostedService(IConsulClient client, ServiceConfig config, IConfiguration configuration)
        {
            _client = client;
            _config = config;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _registrationId = $"{_config.ServiceName}-{Guid.NewGuid()}";
            var uri = _configuration.GetServiceUri(_config.ServiceName);
            Console.WriteLine($"{_config.ServiceName}-{uri}");
            var registration = new AgentServiceRegistration
            {
                ID = _registrationId,
                Name = _config.ServiceName,
                Address = uri.Host,
                Port = uri.Port
            };

            try
            {
                await _client.Agent.ServiceDeregister(registration.ID, cancellationToken);
                await _client.Agent.ServiceRegister(registration, cancellationToken);
                Console.WriteLine($" SIIIIIIII {_config.ServiceName}-{uri}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($" NOOOOOOOO {_config.ServiceName}-{uri}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.Agent.ServiceDeregister(_registrationId, cancellationToken);
        }
    }
}