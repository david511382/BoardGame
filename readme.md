# 專案介紹
## 專案功能
	線上桌遊平台，提供玩家登入，線上開局遊玩。

## 架構說明
	專案架構: 微服務架構
	前端: Angular
	Gateway: Ocelot
	後端: .Net Core
	後端Api業務: Auth(用戶語認證授權)、Lobby(大廳遊戲房間)、Game(遊戲邏輯)
	資料庫: MSSQL資料庫，Game(遊戲)、Member(用戶)
	Redis:儲存遊戲房間跟遊戲狀態的3主3從叢集Redis，還有附加哨兵模式
	日誌: 使用NLog發送至ELK(Elasticsearch、Kibana)。
![avatar](系統架構圖.png)

## 檔案結構
	src┬ApiGateways-OcelotApiGateway
	   ├Services┬Game-GameWebService
	   │		 ├AuthWebService
	   │		 └LobbyWebService
	   ├WebUI-BoardGameAngular
	   ├Respositories
	   └Common

# Getting started
## Docker Compose
完整全部執行: make up  

elk 需要設定 vm.max_map_count=262144  
Windows with Docker Desktop WSL 2 backend  
```
wsl -d docker-desktop
sysctl -w vm.max_map_count=262144
```
[參考資料](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html#_set_vm_max_map_count_to_at_least_262144)  

## 資料庫初始化
使用 EF 資料庫 Migrations  
在VS起始專案設為 BoardGameWebService

在套件管理器主控台輸入
啟用
```
 Enable-migrations
```
更新
```
 Update-Database -Context MemberContext
 Update-Database -Context GameContext
```

### 套件
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
參考資料:https://dotblogs.com.tw/abc12207/2018/03/24/entity-framework-core-migrations-at-separate-class-library-project

### 需要資料庫權限
1. 刪除
2. 改變任何結構描述
3. 更新
4. 建立資料表
5. 參考
6. 插入
7. 選取

### 指令
[變更名稱] 為資料庫變更的命名，自訂
[資料庫] 使用的DbContext，自訂，此範例是MemberContext
[輸出路徑] 應用程式專案下輸出Migrations的相對路徑，例如 Out/Migrations

加入變更
Add-Migration [變更名稱] -Context [資料庫]
Add-Migration [變更名稱] -Context [資料庫] -OutputDir [輸出路徑]

更新變更
Update-Database [變更名稱] -Context [資料庫]
Update-Database -Context [資料庫]
-Verbose

移除變更
Remove-Migration -Context [資料庫]

# Swagger
nuget package Swashbuckle.AspNetCore