using S3ntinel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3ntinel.Domain.Interfaces
{
    //template for windows/linux server checkers
    //doubles as abstraction to the actual class
    public interface IServerMonitor
    {
        bool PingServer(string server);
        bool CheckDiskSpace(ServerConfig serverConfig);
        bool CheckServices(ServerConfig serverConfig);
    }
}
