using Exiled.API.Interfaces;
using System.ComponentModel;
using System.Net;

namespace Rcon.Configs
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Rcon password")]
        public string RconPassword { get; set; } = "password";

        [Description("Rcon port, specify a free port for TCP")]
        public int RconPort { get; set; } = 27020;

        [Description("Server Ip")]
        public IPAddress IPAddress { get; set; } = IPAddress.Any;
    }
}
