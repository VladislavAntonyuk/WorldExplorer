IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[users].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [users].[__EFMigrationsHistory]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[users].[Users]') AND type in (N'U'))
DROP TABLE [users].[Users]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[users].[outbox_messages]') AND type in (N'U'))
DROP TABLE [users].[outbox_messages]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[travellers].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [travellers].[__EFMigrationsHistory]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[travellers].[Review]') AND type in (N'U'))
DROP TABLE [travellers].[Review]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[travellers].[Visits]') AND type in (N'U'))
DROP TABLE [travellers].[Visits]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[travellers].[Travellers]') AND type in (N'U'))
DROP TABLE [travellers].[Travellers]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[travellers].[Places]') AND type in (N'U'))
DROP TABLE [travellers].[Places]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[travellers].[inbox_messages]') AND type in (N'U'))
DROP TABLE [travellers].[inbox_messages]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[places].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [places].[__EFMigrationsHistory]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[places].[PlaceImages]') AND type in (N'U'))
DROP TABLE [places].[PlaceImages]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[places].[Places]') AND type in (N'U'))
DROP TABLE [places].[Places]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[places].[outbox_messages]') AND type in (N'U'))
DROP TABLE [places].[outbox_messages]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[places].[LocationInfoRequests]') AND type in (N'U'))
DROP TABLE [places].[LocationInfoRequests]
GO

