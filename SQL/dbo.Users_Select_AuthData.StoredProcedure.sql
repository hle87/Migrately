USE [Migrately]
GO
/****** Object:  StoredProcedure [dbo].[Users_Select_AuthData]    Script Date: 11/21/2022 11:03:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Author: Jonathan S.
-- Create date: 11/17/2022
-- Description: Select a password of a user by using email to find them.
-- Code Reviewer:

CREATE proc [dbo].[Users_Select_AuthData]
			@Email nvarchar(255)

/*
Declare @Email nvarchar(255) = 'sample@domain.com'

Execute [dbo].[Users_Select_AuthData] @Email
*/

AS

BEGIN

Declare @Id int

SELECT @Id = [Id]
FROM dbo.Users
WHERE Email = @Email

SELECT [Id]
	  ,[Email]
      ,[Password]
	  ,Roles =(
			SELECT r.Name as name
					
			FROM dbo.Roles as r 
			inner join dbo.UserRoles as ur
			on r.Id = ur.RoleId
			WHERE ur.UserId = @Id
			For JSON AUTO
			)
  FROM [dbo].[Users]

  WHERE Email = @Email

END


GO
