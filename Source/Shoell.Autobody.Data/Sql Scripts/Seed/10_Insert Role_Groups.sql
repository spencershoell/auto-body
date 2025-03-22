DELETE FROM [Identity].[RoleGroups]
WHERE [GroupId] = '720362b3-097b-4bb8-9321-37b899de6f4e';

INSERT INTO [Identity].[RoleGroups] ([RoleId], [GroupId])
SELECT
	 [Id]
	,'720362b3-097b-4bb8-9321-37b899de6f4e'
FROM [Identity].[Roles]
