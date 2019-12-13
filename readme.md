# Getting started
## Kubernetes
### �w��
minikube
kubectl �R�O��u��
[�ѦҺ���](https://kubernetes.io/docs/tasks/tools/install-minikube/ "Title")
[�J���ѦҺ���](https://medium.com/@C.W.Hu/kubernetes-implement-ingress-deployment-tutorial-7431c5f96c3e"Title")
https://medium.com/better-programming/k8s-tips-using-a-serviceaccount-801c433d0023
### �t�m
�bdeployment��Ƨ��H�t�κ޲z����������PowerShell
�ϥ�hyperV�Ұ�minikube�A���O:
minikube start --vm-driver=hyperv --cpus 2 --memory 4096
(�Xvm-driver=virtualbox)

kubectl apply -f db/
kubectl apply -f es/
kubectl apply -f service/

minikube ip


[k8s Image building script �ѦҺ���](https://github.com/chrislusf/seaweedfs/wiki/Deployment-to-Kubernetes-and-Minikube "Title")

### ���O
�Ұ�
minikube start --vm-driver=hyperv

�R��
minikube delete

����
minikube stop

����context
kubectl config use-context minikube

�}�Ҿާ@���O
minikube dashboard

�d�ݶ��s
kubectl cluster-info

[�ѦҺ���](https://kubernetes.io/docs/setup/learning-environment/minikube/#minikube-features "Title")
[�ѦҺ���](https://k8smeetup.github.io/docs/tutorials/stateless-application/hello-minikube "Title")
[�ѦҺ���](https://peihsinsu.gitbooks.io/docker-note-book/content/the-mini-env-of-k8s---minikube.html "Title")

## EF ��Ʈw Migrations
board-game-member-db
1. �b�Ұ�docker-compose��A�s�W��Ʈwlocalhost,1487�A�HSA�b���n�J
2. ���۷s�W��ƮwBoardGameMember
3. �s�W�n�JUser ID=AuthWebService Password=auth$WebService �w�]��ƮwBoardGameMember ���j�����K�X�L��
4. ��ƮwBoardGameMember�]�mAuthWebService�v���A�Ѧ� �ݭn��Ʈw�v��
5. �bVS�_�l�M�׳]��AuthWebService�A�M��޲z���D���x�w�]�M�׳]��MemberRepository
6. �bMemberRepository��DesignDbContextFactory���w�s�u�r��Server=localhost,1487;Initial Catalog=BoardGameMember;Persist Security Info=True;User ID=AuthWebService;Password=auth$WebService;TrustServerCertificate=False;
7. �b�M��޲z���D���x��J Update-Database InitialCreate -Context MemberContext


### �M��
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
https://dotblogs.com.tw/abc12207/2018/03/24/entity-framework-core-migrations-at-separate-class-library-project

### �ݭn��Ʈw�v��
�R��
���ܥ��󵲺c�y�z
��s
�إ߸�ƪ�
�Ѧ�
���J
���

### ���O
�ҥ�
Enable-migrations

[�ܧ�W��] ����Ʈw�ܧ󪺩R�W�A�ۭq
[��Ʈw] �ϥΪ�DbContext�A�ۭq�A���d�ҬOMemberContext
[��X���|] ���ε{���M�פU��XMigrations���۹���|�A�Ҧp Out/Migrations

�[�J�ܧ�
Add-Migration [�ܧ�W��] -Context [��Ʈw]
Add-Migration [�ܧ�W��] -Context [��Ʈw] -OutputDir [��X���|]

��s�ܧ�
Update-Database [�ܧ�W��] -Context [��Ʈw]
Update-Database -Context [��Ʈw]
-Verbose

�����ܧ�
Remove-Migration -Context [��Ʈw]

# Swagger
package Swashbuckle.AspNetCore