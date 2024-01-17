using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using TG = Telegram.Bot.Types;
using MedvetitleBot.Gateway.Interfaces;
using MedvetitleBot.Gateway.Extensions;
using Telegram.Bot.Types.Enums;
using VM = MedvetitleBot.Gateway.ViewModels;
using AutoMapper;

namespace MedvetitleBot.Gateway
{
    public class Gateway
    {
        private readonly ILogger<Gateway> _logger;
        private readonly IStorageRepository _storageRepository;
        private readonly IMapper _mapper;

        public Gateway(
            ILogger<Gateway> logger,
            IStorageRepository storageRepository,
            IMapper mapper)
        {
            _logger = logger;
            _storageRepository = storageRepository;
            _mapper = mapper;
        }

        [FunctionName("Gateway")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] TG.Update tgUpdate,
            ILogger log)
        {
            string messageString = tgUpdate.ToJson();
            log.LogInformation($"Update received: {messageString}");

            switch (tgUpdate.Type)
            {
                case UpdateType.Message:
                    if (tgUpdate.Message.MigrateFromChatId.HasValue)
                        await _storageRepository.DeleteChat(tgUpdate.Message.MigrateFromChatId.Value);
                    await _storageRepository.UpsertChat(_mapper.Map<VM.Chat>(tgUpdate.Message.Chat));
                    break;
                case UpdateType.EditedMessage:
                    await _storageRepository.UpsertChat(_mapper.Map<VM.Chat>(tgUpdate.EditedMessage.Chat));
                    break;
                case UpdateType.MyChatMember:
                    if (tgUpdate.MyChatMember.NewChatMember.Status == ChatMemberStatus.Kicked)
                        await _storageRepository.DeleteChat(tgUpdate.MyChatMember.Chat.Id);
                    break;
                default:
                    _logger.LogWarning($"Unknown update type: {tgUpdate.Type}");
                    return new OkResult();
            }
            return new OkResult();
        }
    }
}

