# Swagger
package Swashbuckle.AspNetCore

# EF
## Migrations
### �� Class Library �ϥ� Migrations

�M��
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
https://dotblogs.com.tw/abc12207/2018/03/24/entity-framework-core-migrations-at-separate-class-library-project

### �ݭn��Ʈw�v��
�R��
��s
���J
���
���ܥ��󵲺c�y�z
�إ߸�ƪ�
�Ѧ�
�s��

### ���O
Enable-migrations

Add-Migration InitialCreate -Context MemberContext

Update-Database -Context MemberContext
-Verbose
Remove-Migration -Context MemberContext