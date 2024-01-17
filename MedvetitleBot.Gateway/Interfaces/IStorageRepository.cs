using System.Threading.Tasks;
using MedvetitleBot.Gateway.ViewModels;

namespace MedvetitleBot.Gateway.Interfaces
{
	public interface IStorageRepository
	{
		public Task<Chat> GetChatById(long chatId);
		public Task<long> UpsertChat(Chat chat);
		public Task DeleteChat(long chatId);
	}
}

