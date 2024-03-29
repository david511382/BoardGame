﻿using DAL.Structs;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserInfoDAL
    {
        Task<UserInfo> Query(int id);
        Task<UserInfo> QueryByUsernameAndPassword(string username, string password);
        Task<UserInfo> Add(UserInfo userInfo);
        Task Update(UserInfo userInfo);
        Task Delete(int id);
    }
}
