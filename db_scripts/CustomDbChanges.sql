SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Commission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[TransactionId] [int] NOT NULL,
	[CreatedOnUtc] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOnUtc] [datetime2](7) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[TransactionTypeId] [int] NOT NULL,
	[TransactionNote] [varchar](500) NULL,
	[TransactionAmount] [decimal](18, 2) NOT NULL,
	[UpdateBalance] [decimal](18, 2) NOT NULL,
	[StatusId] [int] NOT NULL,
	[OrderId] [int] NULL,
	[UpdatedOnUtc] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
alter table [Order] alter column BillingAddressId int
GO
alter table [Customer] add [GoalId] [int] NULL,
	[TimelineId] [int] NULL,
	[ExperienceId] [int] NULL,
	[RiskToleranceId] [int] NULL,
	[InvestmentApproachId] [int] NULL,
	[Verified] [bit] NULL,
	[EmailAlert] [bit] NULL,
	[TextAlert] [bit] NULL,
	[IdentityVerificationId] [int] NULL,
	[DontInvestAmount] [bit] NULL,
	[CommissionToHouse] [decimal](18, 0) NULL,
	[InvestedAmount] [decimal](18, 2) NULL,
	[CurrentBalance] [decimal](18, 2) NULL
GO
insert into PermissionRecord (Name, SystemName, Category) values
('Admin area. Manage Transaction','ManageTransaction','Customers'),
('Admin area. Delete Transaction','DeleteTransaction','Customers') 
GO