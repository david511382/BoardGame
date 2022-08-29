namespace DAL.Models
{
    public class UserModel
    {
        public UserInfoModel UserInfo { get; set; }
        public int? GameRoomID { get; set; }

        public UserModel()
        {
            GameRoomID = null;
        }
    }
}
