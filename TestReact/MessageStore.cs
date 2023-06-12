using System.Net.NetworkInformation;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace TestReact
{
    public interface IMessageStore
    {
       public bool Submit(Message msg);
       public IEnumerable<Message> GetPending(string userId);
       public void Ack(string receiverId,IEnumerable<string> msgIds);

    }


    public class MessageStore: IMessageStore
    {
        private ILogger<MessageStore> _logger;
        private IUserManagement _userMgmt;
        private ConcurrentDictionary<string, List<Message>> _pendingMessages;
        private IRoomManager _rooms;

        public MessageStore(ILogger<MessageStore> logger, IUserManagement userMgmt, IRoomManager rooms)
        {
            _logger = logger;
            _userMgmt = userMgmt;
            _pendingMessages = new ConcurrentDictionary<string, List<Message>>();
            _rooms = rooms;
        }

        public bool Submit(Message msg)
        {
            if (msg == null)
            {
                _logger.LogError("MESSAGE IS NULL");
                return false;
            }
            UserInfo sender = _userMgmt.GetUser(msg.Sender);
            if (sender == null)
            {
                _logger.LogError("NO SENDER");
                return false;
            }

            if (msg.TargetRoomId == null)
            {
                _logger.LogError("NO ROOM");
                return false;
            }

            msg.MessageId = System.Guid.NewGuid().ToString();
            UserInfo receiver = _userMgmt.GetUser(msg.Receiver);

            if (receiver == null)
            {
                _logger.LogError("NO RECIEVER");
                return false;
            }

            if(!_rooms.IsMember(msg.RoomId, msg.Receiver) || !_rooms.IsMember(msg.RoomId, msg.Sender))
                return false;

            if (!_pendingMessages.TryGetValue(receiver.Id, out List<Message> messages))
            {

                lock (receiver)
                {
                    messages = _pendingMessages[receiver.Id] = new List<Message>();
                }

            }
            lock (messages)
            {
                messages.Add(msg);
            }
            return true;
        }

        public void Ack(string receiverId, IEnumerable<string> msgIds)

        {

            if (!_pendingMessages.TryGetValue(receiverId, out List<Message> messages))
            {
                return;
            }
            lock (messages)
            {

                messages = messages.Where(m => !msgIds.Contains(m.MessageId)).ToList();

            }
        }

        public IEnumerable<Message> GetPending(string userId)
        {
            if (!_pendingMessages.TryGetValue(userId, out List<Message> messages))
            {
               return new List<Message>();
            }
             return new List<Message>(messages);

        }
    }
}
