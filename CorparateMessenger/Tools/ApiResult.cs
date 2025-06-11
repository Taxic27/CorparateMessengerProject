using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CorparateMessenger.Tools
{
    public class ApiResult<T>
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public List<string>? Errors { get; set; }

        [JsonPropertyName("errorsAsString")]
        public string? ErrorsAsString { get; set; }
    }
}
