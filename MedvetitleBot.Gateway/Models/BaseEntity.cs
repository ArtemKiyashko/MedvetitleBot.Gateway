using System;
using Azure;
using Azure.Data.Tables;

namespace MedvetitleBot.Gateway.Models
{
    public class BaseEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}

