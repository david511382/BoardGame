using DAL;
using DAL.Interfaces;
using DAL.Structs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class UserInfoDAL : IUserInfoDAL
    {
        private MemberContext _ctx;

        public UserInfoDAL(MemberContext ctx)
        {
            _ctx = ctx;
        }

        ~UserInfoDAL()
        {
            _ctx.Dispose();
        }

        public async Task<UserInfo> Query(int id)
        {
            return await _ctx.UserInfos
                .AsNoTracking()
                .SingleAsync(s => s.ID == id);
        }

        public async Task<UserInfo> QueryByUsernameAndPassword(string username, string password)
        {
            return await _ctx.UserInfos
                .AsNoTracking()
                .Where(s => s.Username.Equals(username) &&
                s.Password.Equals(password))
                .SingleAsync();
        }

        public async Task<UserInfo> Add(UserInfo userInfo)
        {
            _ctx.UserInfos.Add(userInfo);
            await _ctx.SaveChangesAsync();

            return userInfo;
        }

        public async Task Update(UserInfo userInfo)
        {
            UserInfo currentItem = await _ctx.UserInfos
                .SingleAsync(i => i.ID == userInfo.ID);

            currentItem.Name = userInfo.Name;
            currentItem.Username = userInfo.Username;
            currentItem.Password = userInfo.Password;
            currentItem.RegisterTime = userInfo.RegisterTime;

            await _ctx.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            _ctx.Remove(new UserInfo { ID = id });

            await _ctx.SaveChangesAsync();
        }
    }
}
