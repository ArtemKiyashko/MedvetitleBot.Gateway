using System.Threading.Tasks;
using AutoMapper;
using Azure.Data.Tables;
using MedvetitleBot.Gateway.Interfaces;
using MedvetitleBot.Gateway.Models;
using MedvetitleBot.Gateway.Options;
using MedvetitleBot.Gateway.ViewModels;
using Microsoft.Extensions.Options;

namespace MedvetitleBot.Gateway.Repositories
{
	public class StorageRepository : IStorageRepository
	{
        private readonly TableServiceClient _tableServiceClient;
        private readonly IMapper _mapper;
        private readonly GatewayOptions _options;
        private readonly TableClient _chats;

        public StorageRepository(
            TableServiceClient tableServiceClient,
            IOptions<GatewayOptions> options,
            IMapper mapper)
		{
            _tableServiceClient = tableServiceClient;
            _mapper = mapper;
            _options = options.Value;

            _chats = _tableServiceClient.GetTableClient(_options.ChatsTableName);
            _chats.CreateIfNotExists();
        }

        public async Task DeleteChat(long chatId)
        {
            await _chats.DeleteEntityAsync(_options.DefaultPartitionKey, chatId.ToString());
        }

        public async Task<Chat> GetChatById(long chatId)
        {
            var modelResult = await _chats.GetEntityIfExistsAsync<ChatEntity>(_options.DefaultPartitionKey, chatId.ToString());

            if (!modelResult.HasValue)
                return default;

            var result = _mapper.Map<Chat>(modelResult.Value);
            return result;
        }

        public async Task<long> UpsertChat(Chat chat)
        {
            var model = _mapper.Map<ChatEntity>(chat);
            model.PartitionKey = _options.DefaultPartitionKey;
            model.RowKey = chat.Id.ToString();
            await _chats.UpsertEntityAsync(model);
            return model.Id;
        }
    }
}

