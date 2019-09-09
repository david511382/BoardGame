# Swagger
package Swashbuckle.AspNetCore

# EF
## Migrations
### 用 Class Library 使用 Migrations

套件
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
https://dotblogs.com.tw/abc12207/2018/03/24/entity-framework-core-migrations-at-separate-class-library-project

### 需要資料庫權限
刪除
更新
插入
選取
改變任何結構描述
建立資料表
參考
連接

### 指令
Enable-migrations

Add-Migration InitialCreate -Context MemberContext

Update-Database -Context MemberContext
-Verbose
Remove-Migration -Context MemberContext