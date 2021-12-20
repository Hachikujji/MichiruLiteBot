using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MichiruLite.Services
{
    internal class AuthorizationService
    {
        private const string tokenPath = "D://discordToken.txt";

        public AuthorizationService(DiscordSocketClient client)
        {
            try
            {
                client.LoginAsync(TokenType.Bot, File.ReadAllLines(tokenPath).First()).GetAwaiter().GetResult();
                client.StartAsync().GetAwaiter().GetResult();
                client.Ready += () =>
                {
                    return Task.CompletedTask;
                };
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Token file not founded [{tokenPath}]: {ex}");
            }
        }
    }
}