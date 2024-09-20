using S3ntinel.Domain.Interfaces;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using Microsoft.Extensions.Logging;
using System.Management;
using S3ntinel.Domain.Models;

//NOTE: `return` are for notifying if there are any alerts that needs checking.
namespace S3ntinel.Infrastructure
{
    public class WindowsServerChecker : IServerMonitor
    {
        private readonly ILogger<WindowsServerChecker> _logger;

        public WindowsServerChecker(ILogger<WindowsServerChecker> logger)
        {
            _logger = logger;
        }

        //check disk space
        //used WMI under System.Management for server
        public bool CheckDiskSpace(ServerConfig serverConfig)
        {
            bool checkDisks = false;
            string server = serverConfig.HostName;

            try
            {
                //windows authentication configuration
                ConnectionOptions options = new ConnectionOptions
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    Authentication = AuthenticationLevel.PacketPrivacy
                };

                //establish connection and query drives
                ManagementScope scope = new($@"\\{server}\root\cimv2", options);
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DriveType = 3");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                //iterate thru the drives.
                foreach (ManagementObject disk in searcher.Get())
                {
                    string driveName = disk["Name"].ToString();
                    ulong totalSize = (ulong)disk["Size"];
                    ulong freeSpace = (ulong)disk["FreeSpace"];
                    double availableSpace = (double)freeSpace / totalSize * 100;

                    if (availableSpace > 20.00)
                    {
                        _logger.LogInformation($"Drive {driveName} on {server} has {availableSpace:0.00}% space available.");
                        
                    }
                    else
                    {
                        _logger.LogCritical($"[***ALERT***] Drive {driveName} on {server} has {availableSpace:0.00}% space available.");
                        checkDisks = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"[***ERROR***] Error checking disk space on {server}");
                checkDisks = true;
            }

            return checkDisks;

        }

        //check services
        public bool CheckServices(ServerConfig serverConfig)
        {
            bool checkServives = false;
            var server = serverConfig.HostName;
            var services = serverConfig.Services;

            foreach (string service in services)
            {
                try
                {
                    //initialize service controller
                    ServiceController serviceController = new ServiceController(service, server);

                    //check if service is running
                    if (serviceController.Status == ServiceControllerStatus.Running)
                    {
                        _logger.LogInformation($"Service {service} on {server} is running.");
                    }
                    else 
                    {
                        _logger.LogCritical($"[***ALERT***] Service {service} on {server} is not running.");
                        checkServives = true;
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, $"[***ERROR***] Error checking service {service} on {server}");
                    checkServives = true;
                }
            }

            return checkServives;

            
        }

        //ping server
        public bool PingServer(string server)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(server);

                if (reply.Status == IPStatus.Success)
                {
                    _logger.LogInformation($"Ping to {server} successful.");
                }
                else
                {
                    _logger.LogCritical($"[***ALERT***] Ping to {server} failed.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"[***ERROR***] Error pinging {server}");
                return false;
            }
        }
    }
}
