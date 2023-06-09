USE [Migrately]
GO
/****** Object:  Table [dbo].[User2FA]    Script Date: 2/7/2023 4:22:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User2FA](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Is2FAEnabled] [bit] NOT NULL,
	[UserId] [int] NOT NULL,
	[DateCreated] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserBridge2FA_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[User2FA] ADD  CONSTRAINT [DF_UserBridge2FA_DateCreated]  DEFAULT (getutcdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[User2FA]  WITH CHECK ADD  CONSTRAINT [FK_UserBridge2FA_Users1] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[User2FA] CHECK CONSTRAINT [FK_UserBridge2FA_Users1]
GO
