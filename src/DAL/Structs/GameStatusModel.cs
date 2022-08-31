namespace DAL.Structs
{
    public class GameStatusModel
    {
        public GameStatusModel()
        {
        }

        public GameStatusModel(GameStatusModel data)
        {
            Room = data.Room;
            DataJson = data.DataJson;
        }

        public RoomModel Room { get; set; }
        public string DataJson { get; set; }
    }
}
