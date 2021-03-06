﻿using RedisRepository.Models;
using System.Threading.Tasks;

namespace LobbyWebService.Services
{
    public interface IRedisService
    {
        Task<GameModel> Game(int ID);

        Task<RoomModel[]> ListRooms();
        Task<RoomModel> CreateRoom(UserInfoModel info, int gameID);
        Task<RoomModel> Room(int hostID);
        Task<RoomModel> AddRoomPlayer(int hostID, UserInfoModel info);

        Task<GameStatusModel> GameStatus(int hostID);

        Task<UserModel> User(int userID);
    }
}
