using Microsoft.AspNetCore.Mvc;
using S3ntinel.Application.Interfaces;

namespace S3ntinel.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerMonitorController : ControllerBase
    {
        //used interface to obscure the classes
        private readonly IServerCheckService _serverCheckService;

        public ServerMonitorController(IServerCheckService serverCheckService) { 
            _serverCheckService = serverCheckService;
        }

        
        [HttpGet(Name="MonitorServers")]
        public IActionResult MonitorServers() {

            var toCheck = _serverCheckService.MonitorServers();

            if (toCheck)
                return Ok("Server Check: Check for Issues in the Alerts folder!");
            else
                return Ok("Server Check: Completed! No Issues Found!");
        }
    }
}
