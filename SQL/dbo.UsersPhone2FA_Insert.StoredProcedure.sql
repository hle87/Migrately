USE [Migrately]
GO
/****** Object:  StoredProcedure [dbo].[UsersPhone2FA_Insert]    Script Date: 2/7/2023 4:22:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Author: Huan Le
-- Create date: 02/02/2023
-- Description: Proc to insert user with phone# & 2FA request return id
-- Code Reviewer: Josh Haynes

CREATE proc [dbo].[UsersPhone2FA_Insert]
			@Email nvarchar(255)
           ,@FirstName nvarchar(100)
           ,@LastName nvarchar(100)
           ,@Mi nvarchar(2)
           ,@AvatarUrl varchar(255)
           ,@Password varchar(100)
		   ,@PhoneNumber nvarchar(50)
		   ,@IsActive bit
		   ,@Is2FAEnabled bit
		   ,@Id int OUTPUT



/* Test Code

Declare 
			@Email nvarchar(255) = 'samples92@email.com'
           ,@FirstName nvarchar(100) = 'John'
           ,@LastName nvarchar(100) = 'Smith'
           ,@Mi nvarchar(2) = 'S'
           ,@AvatarUrl varchar(255)= 'anImages.com'
           ,@Password varchar(100) = 'password22'
		   ,@PhoneNumber nvarchar(50) = '6266367120'
		   ,@IsActive bit = 'True'
		   ,@Is2FAEnabled bit = 'True'
		   ,@Id int 

Execute dbo.UsersPhone2FA_Insert
			@Email 
           ,@FirstName 
           ,@LastName 
           ,@Mi 
           ,@AvatarUrl 
           ,@Password
		   ,@PhoneNumber
		   ,@IsActive
		   ,@Is2FAEnabled
		   ,@Id OUTPUT 

Select * From dbo.User2FA
		WHERE userId = @Id

Select * FROM dbo.Users 
		WHERE Id = @Id


*/ 

AS 

BEGIN

	INSERT INTO dbo.Users
		([Email]
		,[FirstName]
		,[LastName]
		,[Mi]
		,[AvatarUrl]
		,[Password])
	VALUES
		(@Email 
		,@FirstName 
		,@LastName 
		,@Mi 
		,@AvatarUrl 
		,@Password) 

	SET @Id = SCOPE_IDENTITY()

	--Declare @UserId int = 0

	INSERT INTO dbo.User2FA
				([PhoneNumber]
			   ,[IsActive]
			   ,[Is2FAEnabled]
			   ,[UserId]	)

		VALUES
				(@PhoneNumber 
			   ,@IsActive 
			   ,@Is2FAEnabled
			   ,@Id)



END
GO
