using Exiled.API.Features;
using Rcon.Configs;
using RconApi.API.Features;
using RconAuth;
using RconAuth.Enums;
using System;
using System.Net;

namespace Rcon
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "Руслан0308c";
        public override string Name => "Rcon";
        public override Version Version => new(1, 0, 1);
        public override Version RequiredExiledVersion => new(8, 14, 0);

        public static Plugin Singleton;

        private RconAuthServer _rconAuthServer;

        public override void OnEnabled()
        {
            Singleton = this;

            _rconAuthServer = new RconAuthServer(Config.RconPort, Config.IPAddress, Config.RconPassword);

            _rconAuthServer.ServerStarting += () =>
            {
                Log.Info("Сервер запускается...");
            };

            _rconAuthServer.ServerStarted += () =>
            {
                Log.Info($"Сервер запустился на порту {_rconAuthServer.Port}");
            };

            _rconAuthServer.ClientConnected += (ClientApi<PacketTypeRequest> clientApi) =>
            {
                IPEndPoint ep = clientApi.Client.Client.RemoteEndPoint as IPEndPoint;
                Log.Info($"Клиент с ip: {ep.Address} подключился!");
            };

            _rconAuthServer.ClientDisconnecting += (ClientApi<PacketTypeRequest> clientApi) =>
            {
                IPEndPoint ep = clientApi.Client.Client.RemoteEndPoint as IPEndPoint;
                Log.Info($"Клиент с ip: {ep.Address} отключился!");
            };

            _rconAuthServer.Authenticated += (ClientApi<PacketTypeRequest> clientApi) =>
            {
                IPEndPoint ep = clientApi.Client.Client.RemoteEndPoint as IPEndPoint;
                Log.Info($"Клиент с ip: {ep.Address} авторизовался!");
            };

            _rconAuthServer.NotAuthenticated += (ClientApi<PacketTypeRequest> clientApi) =>
            {
                IPEndPoint ep = clientApi.Client.Client.RemoteEndPoint as IPEndPoint;
                Log.Error($"Клиент с ip: {ep.Address} не авторизовался!");
            };

            _rconAuthServer.Command += (ClientApi<PacketTypeRequest> clientApi) =>
            {
                IPEndPoint ep = clientApi.Client.Client.RemoteEndPoint as IPEndPoint;

                string response = Server.ExecuteCommand(clientApi.ClientData.Payload);
                RconAuthServer.SendResponse(clientApi.BinaryWriter, clientApi.ClientData.MessageId, PacketTypeResponse.ResponseValue, response);

                Log.Info($"Клиент с ip: {ep.Address} ввел команду: {clientApi.ClientData.Payload}");
            };

            _rconAuthServer.StartServer();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;

            _rconAuthServer.StopServer();

            base.OnDisabled();
        }
    }
}
