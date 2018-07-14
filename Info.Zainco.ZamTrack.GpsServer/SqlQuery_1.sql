/****** Object:  StoredProcedure [dbo].[CreateGpsDataTrack]    Script Date: 05/04/2011 00:33:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateGpsDataTrack]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreateGpsDataTrack]
GO
/****** Object:  StoredProcedure [dbo].[DeleteAllDataPointsForDevice]    Script Date: 05/04/2011 00:33:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAllDataPointsForDevice]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAllDataPointsForDevice]
GO
/****** Object:  StoredProcedure [dbo].[GetAllDevices]    Script Date: 05/04/2011 00:33:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllDevices]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllDevices]
GO
/****** Object:  StoredProcedure [dbo].[GetGpsPointsForDevice]    Script Date: 05/04/2011 00:33:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetGpsPointsForDevice]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetGpsPointsForDevice]
GO
/****** Object:  Table [dbo].[Tracking]    Script Date: 05/04/2011 00:33:53 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tracking]') AND type in (N'U'))
DROP TABLE [dbo].[Tracking]
GO
/****** Object:  Table [dbo].[Tracking]    Script Date: 05/04/2011 00:33:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tracking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Command] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[DeviceId] [varchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[Direction] [float] NOT NULL,
	[EventId] [int] NOT NULL,
	[GpsTimeStamp] [datetime] NOT NULL,
	[IsValidGps] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[VehicleSpeed] [decimal](18, 0) NOT NULL,
 CONSTRAINT [PK_Tracking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Tracking]') AND name = N'IX_Tracking')
CREATE NONCLUSTERED INDEX [IX_Tracking] ON [dbo].[Tracking] 
(
	[DeviceId] ASC,
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
SET IDENTITY_INSERT [dbo].[Tracking] ON
INSERT [dbo].[Tracking] ([Id], [Latitude], [Longitude], [Command], [DeviceId], [Direction], [EventId], [GpsTimeStamp], [IsValidGps], [Status], [VehicleSpeed]) VALUES (146, 51.355815, -0.1863, N'CFG:Z31', N'800704', 230, 531, CAST(0x00009EB9014E3698 AS DateTime), 1, 47000000, CAST(0 AS Decimal(18, 0)))
INSERT [dbo].[Tracking] ([Id], [Latitude], [Longitude], [Command], [DeviceId], [Direction], [EventId], [GpsTimeStamp], [IsValidGps], [Status], [VehicleSpeed]) VALUES (147, 51.355815, -0.1863, N'CFG:Z31', N'800704', 230, 531, CAST(0x00009EB9014E3698 AS DateTime), 1, 47000000, CAST(0 AS Decimal(18, 0)))
INSERT [dbo].[Tracking] ([Id], [Latitude], [Longitude], [Command], [DeviceId], [Direction], [EventId], [GpsTimeStamp], [IsValidGps], [Status], [VehicleSpeed]) VALUES (148, 51.355815, -0.1863, N'CFG:Z31', N'800704', 230, 531, CAST(0x00009EB9014E3698 AS DateTime), 1, 47000000, CAST(0 AS Decimal(18, 0)))
INSERT [dbo].[Tracking] ([Id], [Latitude], [Longitude], [Command], [DeviceId], [Direction], [EventId], [GpsTimeStamp], [IsValidGps], [Status], [VehicleSpeed]) VALUES (149, 51.355815, -0.1863, N'CFG:Z31', N'800704', 230, 531, CAST(0x00009EB9014E3698 AS DateTime), 1, 47000000, CAST(0 AS Decimal(18, 0)))
INSERT [dbo].[Tracking] ([Id], [Latitude], [Longitude], [Command], [DeviceId], [Direction], [EventId], [GpsTimeStamp], [IsValidGps], [Status], [VehicleSpeed]) VALUES (150, 51.355815, -0.1863, N'CFG:Z31', N'800704', 230, 531, CAST(0x00009EB9014E3698 AS DateTime), 1, 47000000, CAST(0 AS Decimal(18, 0)))
SET IDENTITY_INSERT [dbo].[Tracking] OFF
/****** Object:  StoredProcedure [dbo].[GetGpsPointsForDevice]    Script Date: 05/04/2011 00:33:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetGpsPointsForDevice]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetGpsPointsForDevice](@DeviceId varchar(50))

AS
SET NOCOUNT ON
SELECT        Id, Latitude, Longitude, Command, DeviceId, Direction, EventId, GpsTimeStamp, IsValidGps, Status, VehicleSpeed
FROM            Tracking
WHERE        (DeviceId = @DeviceId)
	RETURN
' 
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllDevices]    Script Date: 05/04/2011 00:33:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllDevices]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetAllDevices]

AS
	SET NOCOUNT ON
	SELECT DeviceId, VehicleSpeed,GpsTimeStamp,Latitude,Longitude,Direction,IsValidGps,EventId FROM Tracking
	RETURN
' 
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteAllDataPointsForDevice]    Script Date: 05/04/2011 00:33:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAllDataPointsForDevice]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeleteAllDataPointsForDevice](@DeviceId varchar(50))
AS
	SET NOCOUNT ON
	DELETE FROM TRACKING WHERE DEVICEID=@DEVICEID
	RETURN
' 
END
GO
/****** Object:  StoredProcedure [dbo].[CreateGpsDataTrack]    Script Date: 05/04/2011 00:33:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateGpsDataTrack]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[CreateGpsDataTrack]
(
	@Latitude float,@Longitude float,@Command varchar(50),@DeviceId varchar(50),
	@Direction float,@EventId int,@GpsTimeStamp DateTime,
	@IsValidGps bit, @Status int,@VehicleSpeed decimal
)
AS
	SET NOCOUNT ON
	INSERT INTO  Tracking VALUES(@Latitude,@Longitude,
								@Command,@DeviceId,@Direction,
								@EventId,@GpsTimeStamp,@IsValidGps,
								@Status,@VehicleSpeed)
	
' 
END
GO
