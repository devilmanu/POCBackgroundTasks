using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JobManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly JobService _service;
        public JobsController(ILogger<JobsController> logger, JobService service)
        {
            _logger = logger;
            _service = service;
        }

        public JobService Service { get; }

        [HttpPost]
        public IActionResult Post(JobServiceDto dto, CancellationToken cancellationToken)
        {
            _service.Create(dto, cancellationToken);
            return Accepted("",dto);
        }

        [HttpGet]
        public IActionResult Get(CancellationToken cancellationToken)
        {
            return Ok(_service.GetJobs(cancellationToken));
        }
    }
}
