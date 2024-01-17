using AutoMapper;
using MedvetitleBot.Gateway.Models;
using VM = MedvetitleBot.Gateway.ViewModels;
using TG = Telegram.Bot.Types;
namespace MedvetitleBot.Gateway.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ChatEntity, VM.Chat>().ReverseMap();
            CreateMap<VM.Chat, TG.Chat>().ReverseMap();
        }
    }
}

