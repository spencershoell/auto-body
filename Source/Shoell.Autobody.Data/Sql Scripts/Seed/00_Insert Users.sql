DECLARE @username nvarchar(max) = 'spencer@shoell.com
';
DECLARE @externalId nvarchar(max) = 'd2ad2949-abb8-45b5-a072-1cf87d79d86a';

INSERT INTO [Identity].[Users]
        ([Id],[ExternalId],[DateCreated],[DateModified],[DateArchived],[DateDeleted],[CreatedById],[ModifiedById],[ArchivedById],[DeletedById],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash],[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount])
    VALUES
        ('00000000-0000-0000-0000-000000000000','',GETDATE(),GETDATE(),NULL,NULL,'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',NULL,NULL,'administrator@app.local',UPPER('administrator@app.local'),'administrator@app.local',UPPER('administrator@app.local'),0,'',NEWID(),NEWID(),'',0,0,NULL,0,0),
        ('b95befc9-72ab-4a2e-a896-29c125dc31b9',@externalId,GETDATE(),GETDATE(),NULL,NULL,'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',NULL,NULL,@username,UPPER(@username),@username,UPPER(@username),0,'',NEWID(),NEWID(),'',0,0,NULL,0,0);