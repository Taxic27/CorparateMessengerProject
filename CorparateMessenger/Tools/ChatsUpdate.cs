using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorparateMessenger.Tools
{
    public class UserUpdatedMessage
    {
    }

    public class GroupUpdatedMessage
    {
    }

    public class UpdateLastMessage
    {
        public Guid ChatId { get; }

        public UpdateLastMessage(Guid chatId)
        {
            ChatId = chatId;
        }
    }
}
