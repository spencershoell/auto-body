INSERT INTO [Identity].[Roles]
        ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
    VALUES
        (NEWID(),'Group.Create',UPPER('Group.Create'),NEWID()),
        (NEWID(),'Group.Read',UPPER('Group.Read'),NEWID()),
        (NEWID(),'Group.Update',UPPER('Group.Update'),NEWID()),
        (NEWID(),'Group.Delete',UPPER('Group.Delete'),NEWID()),
        (NEWID(),'Group.Recycle',UPPER('Group.Recycle'),NEWID()),
        (NEWID(),'Group.Recover',UPPER('Group.Recover'),NEWID()),
        (NEWID(),'Group.Purge',UPPER('Group.Purge'),NEWID()),
        (NEWID(),'Group.Archive',UPPER('Group.Archive'),NEWID()),
        (NEWID(),'Group.Restore',UPPER('Group.Restore'),NEWID()),

        (NEWID(),'Role.Read',UPPER('Role.Read'),NEWID()),

        (NEWID(),'User.Create',UPPER('User.Create'),NEWID()),
        (NEWID(),'User.Read',UPPER('User.Read'),NEWID()),
        (NEWID(),'User.Update',UPPER('User.Update'),NEWID()),
        (NEWID(),'User.Delete',UPPER('User.Delete'),NEWID()),
        (NEWID(),'User.Recycle',UPPER('User.Recycle'),NEWID()),
        (NEWID(),'User.Recover',UPPER('User.Recover'),NEWID()),
        (NEWID(),'User.Purge',UPPER('User.Purge'),NEWID()),
        (NEWID(),'User.Archive',UPPER('User.Archive'),NEWID()),
        (NEWID(),'User.Restore',UPPER('User.Restore'),NEWID());
