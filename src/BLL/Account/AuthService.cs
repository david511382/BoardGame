﻿using BLL.Interfaces;
using DAL.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace BLL
{
    public class AuthService : IAuthService
    {
        private readonly IUserInfoDAL _db;

        public AuthService(IUserInfoDAL db)
        {
            _db = db;
        }

        public async Task RegisterPlayer(UserInfo info)
        {
            string password = encodePassword(info.Password);
            await _db.Add(new DAL.Structs.UserInfo
            {
                Name = info.Name,
                Username = info.Username,
                Password = password,
                RegisterTime = DateTime.Now
            });
        }

        public async Task<UserInfoWithID> LoginPlayer(UserIdentity identity)
        {
            try
            {
                string password = encodePassword(identity.Password);
                DAL.Structs.UserInfo dbInfo = await _db.QueryByUsernameAndPassword(identity.Username, password);

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
                string password;
                if (info.Password == null)
                {
                    DAL.Structs.UserInfo curInfo = await _db.Query(id);
                    password = curInfo.Password;
                }
                else
                    password = encodePassword(info.Password);

                await _db.Update(new DAL.Structs.UserInfo
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
