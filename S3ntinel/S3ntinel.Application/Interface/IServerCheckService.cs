using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3ntinel.Application.Interfaces
{

    //template for ServerCheckService class
    //doubles as abstraction to the actual class
    public interface IServerCheckService
    {
        bool MonitorServers();
    }
}
