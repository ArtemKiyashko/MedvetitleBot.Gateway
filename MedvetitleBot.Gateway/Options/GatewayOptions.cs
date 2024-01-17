using System;
namespace MedvetitleBot.Gateway.Options
{
	public class GatewayOptions
	{
        public string TableServiceConnection { get; set; } = "UseDevelopmentStorage=true";
        public string ChatsTableName { get; set; } = "Chats";
        public string DefaultPartitionKey { get; set; } = "primary";
    }
}

