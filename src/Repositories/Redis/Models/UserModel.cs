namespace RedisRepository.Models
{
    public class UserModel
    {
        public UserInfoModel UserInfo { get; set; }
        public int? RoomID { get; set; }

        public UserModel()
        {
            RoomID = null;
        }
    }
}
