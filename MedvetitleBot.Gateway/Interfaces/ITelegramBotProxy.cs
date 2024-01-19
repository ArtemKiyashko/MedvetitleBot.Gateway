using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MedvetitleBot.Gateway.Interfaces
{
	public interface ITelegramBotProxy
	{
		void SetCommandHandler(string command, Action<Update> action);
		void SetBotKickedHandler(Action<Update> action);
		Task<Message> SendTextMessageAsync(ChatId chatId, string text);
		void OnSendForbidden(Action<ChatId> action);
		Task ProcessUpdateAsync(Update tgUpdate);
	}
}

