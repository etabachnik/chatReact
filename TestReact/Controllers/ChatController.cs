using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace TestReact.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        IUserManagement _userMgr;
        IRoomManager _roomMgr;
        IMessageStore _messageStore;
        public ChatController(IUserManagement userMgr, IRoomManager roomMgr, IMessageStore messageStore)
        {
            _userMgr = userMgr;
            _roomMgr = roomMgr;
            _messageStore = messageStore;
        }
        public LoginResult Login(string user, string password)
        {
            return _userMgr.Login(user, password);
        }
        public void Logout(string user)
        {
            _userMgr.Logout(user);
        }

        public IEnumerable<string> GetRooms()
        {
            return _roomMgr.GetAllRooms();
        }

        public IEnumerable<string> RoomUsers(string roomId)
        {
            return _roomMgr.GetRoomMembers(roomId);
        }
        public void Join(string roomId, string userId)
        {
            _roomMgr.AddToRoom(roomId, userId);
        }
        public void Leave(string roomId, string userId)
        {
            _roomMgr.LeaveRoom(roomId, userId);
        }

        public void SendMessage(Message message)
        {
            _messageStore.Submit(message);
        }

        public IEnumerable<Message> RecieveMessage(string userId)
        {
            return _messageStore.GetPending(userId);
        }

    }


}
