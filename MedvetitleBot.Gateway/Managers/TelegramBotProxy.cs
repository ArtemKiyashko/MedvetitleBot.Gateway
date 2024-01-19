using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedvetitleBot.Gateway.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MedvetitleBot.Gateway.Managers
{
	public class TelegramBotProxy : ITelegramBotProxy
	{
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ILogger<TelegramBotProxy> _logger;
        private Action<Update> _botKickedHandler;
        private IDictionary<string, Action<Update>> _commandHandlers = new Dictionary<string, Action<Update>>();
        private Action<ChatId> _onSendForbidden;

        public TelegramBotProxy(
            ITelegramBotClient telegramBotClient,
            ILogger<TelegramBotProxy> logger)
		{
            _telegramBotClient = telegramBotClient;
            _logger = logger;
        }

        public void OnSendForbidden(Action<ChatId> action) => _onSendForbidden = action;

        public Task ProcessUpdateAsync(Update tgUpdate)
        {
            
        }

        public async Task<Message> SendTextMessageAsync(ChatId chatId, string text)
        {
            try
            {
                return await _telegramBotClient.SendTextMessageAsync(chatId, text);
            }
            catch(ApiRequestException ex) when (ex.ErrorCode == 403)
            {
                if (_onSendForbidden is not null)
                    HandleSendForbidden(chatId, text, ex);
                else throw;
            }

            return default;
        }

        public void SetBotKickedHandler(Action<Update> action) => _botKickedHandler = action;

        public void SetCommandHandler(string command, Action<Update> action) =>
            _commandHandlers[command] = action;

        private async Task<string> GetCommandText(Message message, MessageEntity botCommandEntity)
        {
            var botUser = await _telegramBotClient.GetMeAsync();
            var fullCommand = message.Text.Substring(botCommandEntity.Offset, botCommandEntity.Length);

            return fullCommand.Replace($"@{botUser.Username}", string.Empty).ToLower().Trim();
        }

        private void HandleSendForbidden(ChatId chatId, string text, ApiRequestException ex)
        {
            try
            {
                _onSendForbidden(chatId);
                _logger.LogWarning(ex, $"Cannot send message \'{text}\' to the chat id: {chatId}");
            }
            catch (Exception exf)
            {
                _logger.LogError(exf, $"Cannot handle send forbidden to the chat id: {chatId}");
            }
        }
    }
}

