namespace TestReact
{

    public interface IRoomManager

    {

        public void AddRoom(string roomiD);
        public bool AddToRoom(string roomiD, string userId);
        public void LeaveRoom(string roomiD, string userId);
        public void LeaveAll(string userId);
        public IEnumerable<string>  GetRoomMembers(string roomiD);
        public bool IsMember(string roomId, string userId);
        public IEnumerable<string> GetAllRooms();

    }
    public class RoomManager : IRoomManager
    {
        private Dictionary<string, List<string>> _rooms;
        public RoomManager()

        {
            _rooms = new Dictionary<string, List<string>>(); 
        }
        public void AddRoom(string roomID)

        {
           lock(_rooms)
        {
           if(!_rooms.ContainsKey(roomID))
            {
             _rooms[roomID] = new List<string>();
            }
         }

        }
        public bool Exists(string roomId)
        {

            return _rooms.ContainsKey(roomId);
        }

        public bool AddToRoom(string roomId, string userId)
        {
            if(!_rooms.TryGetValue(roomId, out List<string> users))
                return false;

            lock(_rooms[roomId]) {
                _rooms[roomId].Add(userId);
                return true;
            }
        }

        public void LeaveRoom(string roomId, string userId)
        {
            if(!_rooms.TryGetValue(roomId, out List<string> users))
                return;
            lock(_rooms[roomId]) {
                _rooms[roomId].Remove(userId);
                return;
            }
        }
        public void LeaveAll(string userId)
        {
            foreach(KeyValuePair<string, List<string>> room in _rooms)
            {
                lock (room.Value)
                    room.Value.Remove(userId);
            }
        }

        public IEnumerable<string>  GetRoomMembers(string roomiD)
        {

          if(!_rooms.TryGetValue(roomiD, out List<string> users))
            return new List<string>();
            return new List<string>(users);
        }

        public bool IsMember(string roomId, string userId)
        {

            if(!_rooms.TryGetValue(roomId, out List<string> users))
              return false;

            lock(users) {
                return users.Contains(userId);
            }

        }

        public IEnumerable<string> GetAllRooms()
        {
            return _rooms.Keys;
        }
    }
}
