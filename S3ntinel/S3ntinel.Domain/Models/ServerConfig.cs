namespace S3ntinel.Domain.Models
{
 
    //model for server monitoring configurations
    public class ServerConfig
    {
        public string Name { get; set; }
        public string HostName { get; set; }
        public string Type { get; set; }
        public List<string> Services { get; set; }
        public string UserName { get; set; }
        public string PrivateKeyPath { get; set; }
        public string PassPhrase { get; set; }


    }
}
