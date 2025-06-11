using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorparateMessenger.Tools
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public ChatRoleType Role { get; set; }
        public string Username { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSenderInfoVisible { get; set; } = true;
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public byte[]? FileSize { get; set; }
    }
}
