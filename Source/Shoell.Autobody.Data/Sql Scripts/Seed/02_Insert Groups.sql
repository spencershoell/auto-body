INSERT INTO [Identity].[Groups]
           ([Id],[Name],[Description],[DateCreated],[DateModified],[DateArchived],[DateDeleted],[CreatedById],[ModifiedById],[ArchivedById],[DeletedById])
     VALUES
           ('720362b3-097b-4bb8-9321-37b899de6f4e','Admins','Adminsitrator Users',GETDATE(),GETDATE(),NULL,NULL,'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',NULL,NULL);