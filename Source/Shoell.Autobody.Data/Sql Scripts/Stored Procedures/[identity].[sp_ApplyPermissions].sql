CREATE PROCEDURE [Identity].[sp_ApplyPermissions]
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION
	DELETE FROM [Identity].[User_Roles];

	INSERT INTO [Identity].[User_Roles]
		([UserId]
		,[RoleId])
	SELECT DISTINCT  
		 [User_Groups].[UserId]
		,[Role_Groups].[RoleId]
	FROM [Identity].[User_Groups]
		INNER JOIN [Identity].[Role_Groups]
			ON [User_Groups].[GroupId] = [Role_Groups].[GroupId]

	COMMIT TRANSACTION
END
GO

EXEC [Identity].[sp_ApplyPermissions];