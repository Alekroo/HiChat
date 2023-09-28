using ChatServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServer // Note: actual namespace depends on the project name.
{
    class Program
    {

        static TcpListener _listener;

        static List<Client> _users;


        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();

            

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);
                BroadcastConnection();
            }

            /*Broadcast connections to everyone on the server*/
            

        }


        static void BroadcastConnection()
        {
            foreach (var user in _users)
            {
                foreach(var usr in _users) 
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.clientSocket.Client.Send(broadcastPacket.getPacketBytes());  
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach(var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.clientSocket.Client.Send(msgPacket.getPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.clientSocket.Client.Send(broadcastPacket.getPacketBytes());
            }

            BroadcastMessage($"{disconnectedUser.Username} disconnected!");
        }


    }



}