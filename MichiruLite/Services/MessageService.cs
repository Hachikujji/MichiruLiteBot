using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MichiruLite.Services
{
    class MessageService
    {
		public MessageService() { }
		public async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
		{
			// If the message was not in the cache, downloading it will result in getting a copy of `after`.
			var message = await before.GetOrDownloadAsync();
			Console.WriteLine($"{message} -> {after}");
		}

		public async Task MessageRecieved(SocketMessage message)
		{
			Console.WriteLine($"{message.Author}: {message.Content}");
		}
	}
}
