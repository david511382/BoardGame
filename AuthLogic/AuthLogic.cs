using AuthLogic.Domain.Interfaces;
using AuthLogic.Domain.Models;
using MemberRepository;
using System;
using System.Threading.Tasks;

namespace AuthLogic
{
    public class AuthLogic : IAuth
    {
        private UserInfoDAL _db;

        public AuthLogic(string dbConnectStr)
        {
            _db = new UserInfoDAL(dbConnectStr);
        }

        public async Task<bool> RegisterPlayer(UserInfo info)
        {
            try
            {
                await _db.Add(new MemberRepository.Models.UserInfo
                {
                    Name = info.Name,
                    Username = info.Username,
                    Password = info.Password
                });

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<UserInfoWithID> LoginPlayer(UserIdentity identity)
        {
            try
            {
                MemberRepository.Models.UserInfo dbInfo = await _db.QueryByUsernameAndPassword(identity.Username, identity.Password);

                UserInfoWithID result = new UserInfoWithID
                {
                    Id = dbInfo.ID,
                    Name = dbInfo.Name,
                    Username = dbInfo.Username,
                    Password = dbInfo.Password
                };

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UpdatePlayerInfo(int id, UserInfo info)
        {
            try
            {
                await _db.Update(new MemberRepository.Models.UserInfo
                {
                    ID = id,
                    Name = info.Name,
                    Username = info.Username,
                    Password = info.Password
                });

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
