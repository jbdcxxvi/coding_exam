using Microsoft.Extensions.Logging;
using S3ntinel.Infrastructure;
using Moq;
using Xunit;
using S3ntinel.Domain.Models;

namespace S3ntinel.Test
{
    public class UnitTest1
    {

        #region Linux Server Checks
        [Theory]
        [InlineData("192.168.1.69")]
        public void LinuxServerPingSuccess(string server)
        {
            //arrange
            var mockLogger = new Mock<ILogger<LinuxServerChecker>>();

            //inject
            var checker = new LinuxServerChecker(mockLogger.Object);

            //act: return value should be true signifying ok
            Assert.True(checker.PingServer(server));

        }

        [Theory]
        [InlineData("192.168.1.5")]
        public void LinuxServerPingFail(string server)
        {
            //arrange
            var mockLogger = new Mock<ILogger<LinuxServerChecker>>();

            //inject
            var checker = new LinuxServerChecker(mockLogger.Object);

            //act: return value should be false signifying to check
            Assert.False(checker.PingServer(server));
        }

        [Fact]
        public void LinuxServerCheckServicesFail()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "Server_2",
                HostName = "192.168.1.69",
                Type = "Linux",
                Services = new List<string>() { "apache2", "sshd", "nginx" },
                UserName = "jbdcxxvi",
                PrivateKeyPath = "C:\\.ssh\\private_key.pem",
                PassPhrase = ""

            };
            var mockLogger = new Mock<ILogger<LinuxServerChecker>>();

            //inject
            var checker = new LinuxServerChecker(mockLogger.Object);

            //act: true signifies to check since a service has either not found or not running
            Assert.True(checker.CheckServices(serverConfig));

        }

        [Fact]
        public void LinuxServerCheckServicesSuccess()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "Server_2",
                HostName = "192.168.1.69",
                Type = "Linux",
                Services = new List<string>() { "sshd", "nginx" },
                UserName = "jbdcxxvi",
                PrivateKeyPath = "C:\\.ssh\\private_key.pem",
                PassPhrase = ""

            };
            var mockLogger = new Mock<ILogger<LinuxServerChecker>>();

            //inject
            var checker = new LinuxServerChecker(mockLogger.Object);

            //act: false signifying no problems with the services
            Assert.False(checker.CheckServices(serverConfig));

        }

        [Fact]
        public void LinuxServerCheckDiskSpaceSuccess()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "Server_2",
                HostName = "192.168.1.69",
                Type = "Linux",
                Services = new List<string>() { "sshd", "nginx" },
                UserName = "jbdcxxvi",
                PrivateKeyPath = "C:\\.ssh\\private_key.pem",
                PassPhrase = ""

            };
            var mockLogger = new Mock<ILogger<LinuxServerChecker>>();

            //inject
            var checker = new LinuxServerChecker(mockLogger.Object);

            //act: false signifying no problems with the disk space
            Assert.False(checker.CheckDiskSpace(serverConfig));
        }

        [Fact]
        public void LinuxServerCheckDiskSpaceFail()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "Server_2",
                HostName = "192.168.1.70",
                Type = "Linux",
                Services = new List<string>() { "sshd", "nginx" },
                UserName = "jbdcxxvi",
                PrivateKeyPath = "C:\\.ssh\\private_key.pem",
                PassPhrase = ""

            };
            var mockLogger = new Mock<ILogger<LinuxServerChecker>>();

            //inject
            var checker = new LinuxServerChecker(mockLogger.Object);

            //act: true signifying no problems with the disk space
            Assert.True(checker.CheckDiskSpace(serverConfig));
        }
        #endregion

        #region Windows Server Checks
        [Theory]
        [InlineData("192.168.1.65")]
        public void WindowsServerPingSuccess(string server)
        {
            //arrange
            var mockLogger = new Mock<ILogger<WindowsServerChecker>>();

            //inject
            var checker = new WindowsServerChecker(mockLogger.Object);

            //act: return value should be true signifying ok
            Assert.True(checker.PingServer(server));

        }

        [Theory]
        [InlineData("192.168.1.5")]
        public void WindowsServerPingFail(string server)
        {
            //arrange
            var mockLogger = new Mock<ILogger<WindowsServerChecker>>();

            //inject
            var checker = new WindowsServerChecker(mockLogger.Object);

            //act: return value should be false signifying to check
            Assert.False(checker.PingServer(server));
        }

        [Fact]
        public void WindowsServerCheckServicesFail()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "WinServer1",
                HostName = "192.168.1.65",
                Type = "Windows",
                Services = new List<string>() { "w3svc", "sshd", "Appinfo", "wuauserv" }

            };
            var mockLogger = new Mock<ILogger<WindowsServerChecker>>();

            //inject
            var checker = new WindowsServerChecker(mockLogger.Object);

            //act: true signifies to check since service either not found or not running 
            Assert.True(checker.CheckServices(serverConfig));

        }

        [Fact]
        public void WindowsServerCheckServicesSuccess()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "WinServer1",
                HostName = "192.168.1.65",
                Type = "Windows",
                Services = new List<string>() { "Appinfo" }

            };
            var mockLogger = new Mock<ILogger<WindowsServerChecker>>();

            //inject
            var checker = new WindowsServerChecker(mockLogger.Object);

            //act: false signifying no problem with the services
            Assert.False(checker.CheckServices(serverConfig));

        }

        [Fact]
        public void WindowsServerCheckDiskSpaceSuccess()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "WinServer1",
                HostName = "192.168.1.65",
                Type = "Windows",
                Services = new List<string>() { "Appinfo" }

            };
            var mockLogger = new Mock<ILogger<WindowsServerChecker>>();

            //inject
            var checker = new WindowsServerChecker(mockLogger.Object);

            //act: false signifying to check the disk spaces
            Assert.False(checker.CheckDiskSpace(serverConfig));
        }

        [Fact]
        public void WindowsServerCheckDiskSpaceFail()
        {
            //arrange
            var serverConfig = new ServerConfig()
            {
                Name = "WinServer1",
                HostName = "192.168.1.70",
                Type = "Windows",
                Services = new List<string>() { "Appinfo" }

            };
            var mockLogger = new Mock<ILogger<WindowsServerChecker>>();

            //inject
            var checker = new WindowsServerChecker(mockLogger.Object);

            //act: true signifying to check the disk spaces
            Assert.True(checker.CheckDiskSpace(serverConfig));
        }
        #endregion
    }
}