USE [Migrately]
GO
/****** Object:  StoredProcedure [dbo].[UserPhone2FA_Update]    Script Date: 2/7/2023 4:22:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Author: Huan Le
-- Create date: 02/04/2023
-- Description: Proc to Update user with phone# & 2FA parameters
-- Code Reviewer: Josh Haynes

CREATE proc [dbo].[UserPhone2FA_Update]
		   	@Email nvarchar(255)
           ,@FirstName nvarchar(100)
           ,@LastName nvarchar(100)
           ,@Mi nvarchar(2)
           ,@AvatarUrl varchar(255)
           ,@Password varchar(100)
		   ,@PhoneNumber nvarchar(50)
		   ,@IsActive bit
		   ,@Is2FAEnabled bit
		   ,@UserId int
		   ,@Id int 

AS
/* TEST CODE

Declare @Id int = 5
		,@UserId int = 228

	Declare @Email nvarchar(255) = 'sample@gmail.com'
           ,@FirstName nvarchar(100) = 'Treeman'
           ,@LastName nvarchar(100) = 'Jorgan'
           ,@Mi nvarchar(2) = 'S'
           ,@AvatarUrl varchar(255)= 'anImages.com'
           ,@Password varchar(100) = 'Password1!'
			,@PhoneNumber nvarchar(50) = '6265007222'
		   ,@IsActive bit = 1
		   ,@Is2FAEnabled bit = 1
		   
		   
		  

	Execute dbo.UserPhone2FA_Update
			@Email 
           ,@FirstName 
           ,@LastName 
           ,@Mi 
           ,@AvatarUrl 
           ,@Password
			,@PhoneNumber
			,@IsActive
			,@Is2FAEnabled
			,@UserId
			,@Id
			

	Select *
	FROM dbo.Users as u
	--where email = 'samples92@email.com'
	inner join dbo.User2FA as ub
	on u.Id = ub.UserId

*/

BEGIN

Declare @dateNow datetime2 = getutcdate()

UPDATE dbo.Users
		SET [FirstName] = @FirstName 
           ,[LastName] = @LastName 
           ,[Mi] = @Mi 
           ,AvatarUrl = @AvatarUrl 
           ,[Password] = @Password
		   ,[DateModified] = @dateNow

	WHERE Id =  @UserId


UPDATE dbo.User2FA
		SET	[PhoneNumber] = @PhoneNumber
		   ,[IsActive] = @IsActive
		   ,[Is2FAEnabled] = @Is2FAEnabled
		   
		WHERE UserId = @UserId

		

END
GO
