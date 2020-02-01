# �M�פ���
## �[�c����
	�M�׬[�c: �L�A�Ȭ[�c
	�e��: Angular
	Gateway: Ocelot
	���: .Net Core
	���Api�~��: Auth(�Τ�y�{�ұ��v)�BLobby(�j�U�C���ж�)�BGame(�C���޿�)
	��Ʈw: MSSQL��Ʈw�AGame(�C��)�BMember(�Τ�)
	Redis:�x�s�C���ж���C�����A��3�D3�q�O��Redis�A�٦����[��L�Ҧ�
	Log: �ϥ�NLog�o�e��ELK(Elasticsearch�BKibana)�C
![avatar](�t�ά[�c��.png)

## �ɮ׵��c
	src�sApiGateways-OcelotApiGateway
	   �uServices�sGame-GameWebService
	   �x		�uAuthWebService
	   �x		�|LobbyWebService
	   �uWebUI-BoardGameAngular
	   �uRespositories
	   �|Common

# Getting started
## Docker Compose
����������� docker-compose up --build -d
�������� docker-compose -f redis.yml -f elk.yml up --build -d

## EF ��Ʈw Migrations
board-game-member-db
1. �b�Ұ�docker-compose��A�s�W��Ʈwlocalhost,1487�A�HSA�b���n�J
2. ���۷s�W��ƮwMember
3. �s�W�n�JUser ID=AuthWebService Password=auth$WebService ����Member �w�]��ƮwMember ���j�����K�X�L��
4. ��ƮwMember�]�mAuthWebService�v���A�Ѧ� �ݭn��Ʈw�v��
5. �bVS�_�l�M�׳]��AuthWebService�A�M��޲z���D���x�w�]�M�׳]��MemberRepository
6. �b�M��޲z���D���x��J Update-Database -Context MemberContext

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