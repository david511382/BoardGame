namespace DAL.Structs
{
    public class UserModel
    {
        public UserInfoModel UserInfo { get; set; }
        public int? GameRoomID { get; set; }

        public UserModel()
        {
            GameRoomID = null;
        }

        public UserModel(UserModel data)
        {
            UserInfo = data.UserInfo;
            GameRoomID = data.GameRoomID;
        }
    }
}
