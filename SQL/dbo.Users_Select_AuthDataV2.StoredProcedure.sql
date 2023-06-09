USE [Migrately]
GO
/****** Object:  StoredProcedure [dbo].[Users_Select_AuthDataV2]    Script Date: 12/27/2022 10:47:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: Pereyda, Brian
-- Create date: 12/19/2022
-- Description: Checks Users Login Authorization
-- Code Reviewer: Juan Pereida

-- MODIFIED BY:
-- MODIFIED DATE:
-- Code Reviewer:
-- Note:
-- =============================================


CREATE proc [dbo].[Users_Select_AuthDataV2]
										@Email nvarchar(255)


as


/*

	Declare @Email nvarchar(255) = 'brianp@dispostable.com'


	Execute [dbo].[Users_Select_AuthDataV2]
										@Email

*/


BEGIN


		Declare @Id int
		Declare @statusId int

		SELECT	@Id = [Id]
				,@statusId = StatusId
		FROM dbo.Users
		WHERE Email = @Email

		SELECT [Id] as UserId
			  ,[Email]
			  ,[Password]
			  ,StatusId
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

		  IF @statusId = 5
		  THROW 51000, 'Account has been removed', 1;


END
GO
