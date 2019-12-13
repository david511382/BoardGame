# Getting started
## Kubernetes
### 安裝
minikube
kubectl 命令行工具
[參考網站](https://kubernetes.io/docs/tasks/tools/install-minikube/ "Title")
[入門參考網站](https://medium.com/@C.W.Hu/kubernetes-implement-ingress-deployment-tutorial-7431c5f96c3e"Title")
https://medium.com/better-programming/k8s-tips-using-a-serviceaccount-801c433d0023
### 配置
在deployment資料夾以系統管理員身分執行PowerShell
使用hyperV啟動minikube，指令:
minikube start --vm-driver=hyperv --cpus 2 --memory 4096
(—vm-driver=virtualbox)

kubectl apply -f db/
kubectl apply -f es/
kubectl apply -f service/

minikube ip


[k8s Image building script 參考網站](https://github.com/chrislusf/seaweedfs/wiki/Deployment-to-Kubernetes-and-Minikube "Title")

### 指令
啟動
minikube start --vm-driver=hyperv

刪除
minikube delete

關閉
minikube stop

切換context
kubectl config use-context minikube

開啟操作面板
minikube dashboard

查看集群
kubectl cluster-info

[參考網站](https://kubernetes.io/docs/setup/learning-environment/minikube/#minikube-features "Title")
[參考網站](https://k8smeetup.github.io/docs/tutorials/stateless-application/hello-minikube "Title")
[參考網站](https://peihsinsu.gitbooks.io/docker-note-book/content/the-mini-env-of-k8s---minikube.html "Title")

## EF 資料庫 Migrations
board-game-member-db
1. 在啟動docker-compose後，連上資料庫localhost,1487，以SA帳號登入
2. 接著新增資料庫BoardGameMember
3. 新增登入User ID=AuthWebService Password=auth$WebService 預設資料庫BoardGameMember 不強制執行密碼過期
4. 資料庫BoardGameMember設置AuthWebService權限，參考 需要資料庫權限
5. 在VS起始專案設為AuthWebService，套件管理器主控台預設專案設為MemberRepository
6. 在MemberRepository中DesignDbContextFactory指定連線字串Server=localhost,1487;Initial Catalog=BoardGameMember;Persist Security Info=True;User ID=AuthWebService;Password=auth$WebService;TrustServerCertificate=False;
7. 在套件管理器主控台輸入 Update-Database InitialCreate -Context MemberContext


### 套件
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
https://dotblogs.com.tw/abc12207/2018/03/24/entity-framework-core-migrations-at-separate-class-library-project

### 需要資料庫權限
刪除
改變任何結構描述
更新
建立資料表
參考
插入
選取

### 指令
啟用
Enable-migrations

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
package Swashbuckle.AspNetCore