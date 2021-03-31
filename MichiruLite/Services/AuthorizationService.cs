using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MichiruLite.Services
{
    class AuthorizationService
    {

		public AuthorizationService(DiscordSocketClient client, MessageService service)
		{

			client.LoginAsync(TokenType.Bot, "NzQwMjg4MTYzODY3Nzg3MzY1.Xym1FA.-vBdoUNW1s-hu56UcX8IZfHPKgI").GetAwaiter().GetResult();
			client.StartAsync().GetAwaiter().GetResult();
			client.Ready += () =>
			{
				Console.WriteLine("Bot is connected!");
				return Task.CompletedTask;
			};
		}
	}
}
