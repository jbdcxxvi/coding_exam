using Renci.SshNet;
using S3ntinel.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;
using S3ntinel.Domain.Models;


//NOTE: `return` are for notifying if there are any alerts that needs checking.
namespace S3ntinel.Infrastructure
{
    public class LinuxServerChecker : IServerMonitor
    {
        private readonly ILogger<LinuxServerChecker> _logger;

        public LinuxServerChecker(ILogger<LinuxServerChecker> logger)
        {
            _logger = logger;
        }

        //check disk space
        public bool CheckDiskSpace(ServerConfig serverConfig)
        {
            bool checkDisks = false;
            string server = serverConfig.HostName;

            try
            {

                //instantiate ssh configurations
                string userName = serverConfig.UserName;
                string privateKeyPath = serverConfig.PrivateKeyPath;
                string passPhrase = serverConfig.PassPhrase;

                //load the private key
                var keyFile = new PrivateKeyFile(privateKeyPath, passPhrase);

                //create connnection using the private key
                var connectionInfo = new ConnectionInfo(server, userName, new PrivateKeyAuthenticationMethod(userName, keyFile));

                //establish connection and check disk spaces
                using (var sshClient = new SshClient(connectionInfo))
                {
                    sshClient.Connect();
                    var diskCommand = sshClient.CreateCommand("df -h /");
                    var result = diskCommand.Execute();
                    sshClient.Disconnect();

                    var lines = parseString(result, '\n');

                    //iterate thru the results to parse the drives and percentages
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var columns = parseString(lines[i], ' ');
                        if (columns.Length >= 5)
                        {
                            string drive = columns[0];
                            double percentRemaining = Convert.ToDouble(columns[4].Remove(columns[4].Length - 1));

                            if (percentRemaining > 20.00)
                            {
                                _logger.LogInformation($"Drive {drive} on {server} has {percentRemaining:0.00}% space available.");
                            }
                            else 
                            {
                                _logger.LogCritical($"[***ALERT***] Drive {drive} on {server} has {percentRemaining:0.00}% space available.");
                                checkDisks = true;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"[***ERROR***] Error checking disk space on Linux server {server}.");
                checkDisks = true;
            }

            return checkDisks;
        }

        private string[] parseString(string result, char delimiter) {

            return result.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
        }

        //check services
        public bool CheckServices(ServerConfig serverConfig)
        {
            bool checkServices = false;
            //instantiate ssh configurations
            var server = serverConfig.HostName;
            var services = serverConfig.Services;


            //iterate thru the services
            foreach (var serviceName in services)
            {
                try
                {
                    //instantiate additional ssh configurations
                    string userName = serverConfig.UserName;
                    string privateKeyPath = serverConfig.PrivateKeyPath;
                    string passPhrase = serverConfig.PassPhrase;

                    //load the private key
                    var keyFile = new PrivateKeyFile(privateKeyPath, passPhrase);

                    //create connnection using the private key
                    var connectionInfo = new ConnectionInfo(server, userName, new PrivateKeyAuthenticationMethod(userName, keyFile));

                    //establish connection
                    using (var sshClient = new SshClient(connectionInfo))
                    {
                        sshClient.Connect();
                        var serviceCommand = sshClient.CreateCommand($"systemctl status {serviceName}");
                        var result = serviceCommand.Execute();
                        sshClient.Disconnect();

                        if (result.Contains("Active: active (running)"))
                        {
                            _logger.LogInformation($"Service {serviceName} on {server} is running.");
                        }   
                        else 
                        {
                            _logger.LogCritical($"[***ALERT***] Service {serviceName} on {server} is not running.");
                            checkServices = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, $"[***ERROR***] Error checking service {serviceName} on {server}.");
                    checkServices = true;
                }
            }

            return checkServices;
        }

        //ping server
        public bool PingServer(string server)
        {
            try
            {
                //instatiate Ping
                Ping ping = new Ping();
                PingReply reply = ping.Send(server);

                //check if the server is pinged successfully.
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
