USE [Migrately]
GO
/****** Object:  StoredProcedure [dbo].[Users2FA_SelectAll]    Script Date: 2/7/2023 4:22:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[Users2FA_SelectAll]


as

/*

Execute dbo.Users2FA_SelectAll

*/


BEGIN

	SELECT u.Id
		,Email
		,FirstName
		,LastName
		,Mi
		,AvatarUrl
		,Password
		,StatusId
		,ub.Id
		,ub.PhoneNumber
		,ub.IsActive
		,ub.Is2FAEnabled
		--,ub.DateCreated
		,DateModified


	FROM dbo.Users as u
	inner join dbo.User2FA as ub
	on u.Id = ub.Id

END
GO
