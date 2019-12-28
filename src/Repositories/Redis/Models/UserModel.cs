namespace RedisRepository.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public int? RoomID { get; set; }

        public UserModel()
        {
            RoomID = null;
        }
    }
}
