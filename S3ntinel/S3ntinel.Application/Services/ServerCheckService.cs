using Newtonsoft.Json;
using S3ntinel.Domain.Interfaces;
using S3ntinel.Domain.Models;
using S3ntinel.Application.Interfaces;
using S3ntinel.Infrastructure;
using Microsoft.Extensions.Logging;

namespace S3ntinel.Application.Services
{
    public class ServerCheckService : IServerCheckService
    {
        private readonly IServerMonitor _windowsChecker;
        private readonly IServerMonitor _linuxChecker;

        private List<ServerConfig> _serverConfigs;
        private readonly ILogger<ServerCheckService> _logger;

        public ServerCheckService(WindowsServerChecker windowsChecker, LinuxServerChecker linuxChecker, ILogger<ServerCheckService> logger)
        {
            _windowsChecker = windowsChecker;
            _linuxChecker = linuxChecker;

            LoadConfigurations();
            _logger = logger;
        }

        //get configurations for monitoring_configs.json
        private void LoadConfigurations()
        {
            string cfgDir = "C:\\MonitoringConfigs\\";                  //to be changed to more dynamic connection string
            string cfgFile = "monitoring_configs.json";
            string jsonFilePath = Path.Combine(cfgDir, cfgFile);
            string json = File.ReadAllText(jsonFilePath);
            _serverConfigs = JsonConvert.DeserializeObject<List<ServerConfig>>(json);
        }

        //main monitoring method
        public bool MonitorServers()
        {
            //instatiations
            var toCheck = false;

            IServerMonitor checker;

            //iterate thru the configs
            foreach (var serverConfig in _serverConfigs) {

                _logger.LogInformation($"Checking server: {serverConfig.Name}");

                //determine what checker to use
                //used interface to simplify the call of methods
                //tbdev: to add other server checkers as well.
                //case "Solaris":
                //    checker = _linuxChecker;
                //    break;
                switch (serverConfig.Type) {
                    
                    //assign a windows checker
                    case "Windows":
                        checker = _windowsChecker;
                        break;

                    //assign a linux checker
                    case "Linux":
                        checker = _linuxChecker;
                        break;
                    
                    default:
                        checker = _windowsChecker;  //to add a default checker
                        break;
                }

                //check ping, disk space, and services depending on type
                var isServerPinged = checker.PingServer(serverConfig.HostName);

                //check whether to continue checking disk space and services
                if (isServerPinged)
                {
                    //run disk space check
                    var isSpaceAdequate = checker.CheckDiskSpace(serverConfig);

                    //run services check
                    var areServicesRunning = checker.CheckServices(serverConfig);

                    //check if there alerts for disk space and service check
                    if (isSpaceAdequate || areServicesRunning)
                    {
                        toCheck = true;
                    }
                }
                else 
                {
                    _logger.LogCritical($"[***ALERT***] {serverConfig.Name} is down. Please check!");
                    toCheck = true;
                }
                
            }

            return toCheck;
        }
    }
}
