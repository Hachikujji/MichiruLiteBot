using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MichiruLite.Services
{
    class CommandHandler
    {
        public CommandHandler(DiscordSocketClient client)
        {
            client.MessageReceived += send;
        }
        private async Task send(SocketMessage msg)
        {
            Console.WriteLine("asd");
        }
    }
}
