using AuthWebService.Models;
using Convert;
using MemberRepository;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthWebService.Sevices
{
    class AuthService : IAuthService
    {
        private UserInfoDAL _db;

        public AuthService(string dbConnectStr)
        {
            _db = new UserInfoDAL(dbConnectStr);
        }

        public async Task<bool> RegisterPlayer(UserInfo info)
        {
            try
            {
                string password = encodePassword(info.Password);
                await _db.Add(new MemberRepository.Models.UserInfo
                {
                    Name = info.Name,
                    Username = info.Username,
                    Password = password
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<UserInfoWithID> LoginPlayer(UserIdentity identity)
        {
            try
            {
                string password = encodePassword(identity.Password);
                MemberRepository.Models.UserInfo dbInfo = await _db.QueryByUsernameAndPassword(identity.Username, password);

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
                string password = encodePassword(info.Password);
                await _db.Update(new MemberRepository.Models.UserInfo
                {
                    ID = id,
                    Name = info.Name,
                    Username = info.Username,
                    Password = password
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string encodePassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] sha256Bytes = sha256.ComputeHash(bytes);
                return sha256Bytes.BytesToHex();
            }
        }
    }
}
