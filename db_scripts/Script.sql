IF NOT EXISTS (SELECT * FROM ActivityLogType WHERE SystemKeyword = 'TransactionLog')
BEGIN
   INSERT INTO ActivityLogType ([SystemKeyword],[Name],[Enabled]) VALUES
   ('TransactionLog','Transaction Log',1)
   
   PRINT 'SUCCESSFULLY INSERTED ON [ActivityLogType]'
END
GO

IF COL_LENGTH('ActivityLog','Viewed') IS NULL
BEGIN
	ALTER TABLE [ActivityLog] ADD [Viewed] BIT

	PRINT 'SUCCESSFULLY ALTERED TABLE [ActivityLog]'
END
GO

IF NOT EXISTS (SELECT * FROM ScheduleTask WHERE [Name] IN ('Invest Amount','Return Investment','Update LP Value','Orders Retention'))
BEGIN
	INSERT INTO ScheduleTask ([Name],[Type],[Seconds],[Enabled],[StopOnError]) VALUES
	--('Invest Amount','Nop.Services.Transactions.InvestAmountTask, Nop.Services',9999,0,0),
    ('Return Investment','Nop.Services.Transactions.SendProfitTask, Nop.Services',9999,0,0),
    ('Generate Return','Nop.Services.Transactions.GetReturnAmountTask, Nop.Services',9999,0,0),
	('Update LP Value', 'Nop.Services.Transactions.UpdateLPValueTask, Nop.Services', 3600, 0,0),
	('Orders Retention', 'Nop.Services.Transactions.OrdersRetentionTask, Nop.Services', 84600, 0,0)
	
    PRINT 'SUCCESSFULLY INSERTED ON [ScheduleTask]'
END
GO

IF NOT EXISTS (SELECT * FROM Setting WHERE [Name] IN (
	'transactionsettings.apipoolid',
	'transactionsettings.apisession',
	'transactionsettings.liquiditypoolvalue',
	'transactionsettings.ApiRootUrl',
	'transactionsettings.investmentdateend',
	'transactionsettings.investmentdatestart'
	))
BEGIN
    INSERT INTO Setting ([Name],[Value],[StoreId]) VALUES
    ('transactionsettings.apipoolid','10207147',0),
    ('transactionsettings.apisession','PE9e4bpAycX42jBFQWtc3199191',0),
    ('transactionsettings.investmentdateend','5',0),
    ('transactionsettings.investmentdatestart','1',0),
    ('transactionsettings.defaultcommissionpercentage','25',0),
	('transactionsettings.simulateddatetime','',0),
	('transactionsettings.liquiditypoolvalue','1000000',0),
	('transactionsettings.ApiRootUrl','https://interest-generator-api.azurewebsites.net',0)

    PRINT 'SUCCESSFULLY INSERTED ON [Setting]'
END
GO

IF OBJECT_ID('Commission') IS NULL
BEGIN
	CREATE TABLE [dbo].[Commission](
		[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
		[Amount] [decimal](18, 2) NOT NULL,
		[TransactionId] [int] NOT NULL,
		[CreatedOnUtc] [datetime2](7) NOT NULL)

	ALTER TABLE [dbo].[Commission]  WITH CHECK ADD FOREIGN KEY([TransactionId])
	REFERENCES [dbo].[Transaction] ([Id])

	ALTER TABLE [dbo].[Commission]  WITH CHECK ADD FOREIGN KEY([TransactionId])
	REFERENCES [dbo].[Transaction] ([Id])

    PRINT 'SUCCESSFULLY TABLE [Commission] CREATED'
END

IF OBJECT_ID('WithdrawalMethod') IS NULL
BEGIN
	CREATE TABLE WithdrawalMethod(
		Id INT PRIMARY KEY IDENTITY,
		TypeId INT NOT NULL,
		[Name] VARCHAR(500) NOT NULL,
		IsEnabled BIT NOT NULL,
		IsRequested BIT NOT NULL
		)

	PRINT 'SUCCESSFULLY TABLE [WithdrawalMethod] CREATED'
END
GO

IF OBJECT_ID('WithdrawalMethodField') IS NULL
BEGIN
	CREATE TABLE WithdrawalMethodField(
		Id INT PRIMARY KEY IDENTITY,
		WithdrawalMethodId INT FOREIGN KEY REFERENCES WithdrawalMethod(Id) ON DELETE CASCADE ON UPDATE CASCADE,
		FieldName VARCHAR(500) NOT NULL,
		IsEnabled BIT NOT NULL
		)

	PRINT 'SUCCESSFULLY TABLE [WithdrawalMethodField] CREATED'
END
GO

IF OBJECT_ID('CustomerWithdrawalMethod') IS NULL
BEGIN
	CREATE TABLE CustomerWithdrawalMethod(
		Id INT PRIMARY KEY IDENTITY,
		CustomerId INT,
		WithdrawalMethodFieldId INT FOREIGN KEY REFERENCES WithdrawalMethodField(Id) ON DELETE CASCADE ON UPDATE CASCADE,
		[Value] VARCHAR(MAX) NOT NULL
		)

	PRINT 'SUCCESSFULLY TABLE [CustomerWithdrawalMethods] CREATED'
END
GO

IF OBJECT_ID('Transaction') IS NULL
BEGIN
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

    ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD FOREIGN KEY([CustomerId])
    REFERENCES [dbo].[Customer] ([Id])
    ON UPDATE CASCADE
    ON DELETE CASCADE

	PRINT 'SUCCESSFULLY TABLE [Transaction] CREATED'
END

IF COL_LENGTH('Customer','InvestmentApproachId') IS NULL
BEGIN
    ALTER TABLE Customer ADD [InvestmentApproachId] [int] NULL,
	    [Verified] [bit] NULL,
	    [EmailAlert] [bit] NULL,
	    [TextAlert] [bit] NULL,
	    [IdentityVerificationId] [int] NULL,
	    [DontInvestAmount] [bit] NULL,
	    [CommissionToHouse] [decimal](18, 0) NULL,
	    [InvestedAmount] [decimal](18, 2) NULL,
	    [CurrentBalance] [decimal](18, 2) NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Transaction','WithdrawalMethodId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Transaction] ADD [WithdrawalMethodId] INT FOREIGN KEY REFERENCES [dbo].[WithdrawalMethod] ([Id])
    ON UPDATE CASCADE
    ON DELETE CASCADE

	PRINT 'SUCCESSFULLY ALTERED TABLE [Transaction]'
END
GO

IF COL_LENGTH('Customer','DefaultWithdrawalMethodId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [DefaultWithdrawalMethodId] INT NOT NULL DEFAULT((0))

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Customer','LastReturnDate') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD LastReturnDate DATETIME

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Customer','PaymentTypeId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [PaymentTypeId] INT NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF NOT EXISTS (SELECT * FROM Setting WHERE [Name] IN ('transactionSettings.ForecfullyGiveReturn'))
BEGIN
    INSERT INTO Setting ([Name],[Value],[StoreId]) VALUES
    ('transactionSettings.ForecfullyGiveReturn','False',0)

    PRINT 'SUCCESSFULLY INSERTED ON [Setting]'
END
GO

IF NOT EXISTS (SELECT * FROM Setting WHERE [Name] IN ('transactionSettings.ApiRootUrl'))
BEGIN
    INSERT INTO Setting ([Name],[Value],[StoreId]) VALUES
    ('transactionSettings.ApiRootUrl','https://interest-generator-api.azurewebsites.net',0)

    PRINT 'SUCCESSFULLY INSERTED ON [Setting]'
END
GO

IF NOT EXISTS (SELECT * FROM Setting WHERE [Name] IN ('transactionSettings.liquiditypoolvalue'))
BEGIN
    INSERT INTO Setting ([Name],[Value],[StoreId]) VALUES
    ('transactionSettings.liquiditypoolvalue','1000000',0)

    PRINT 'SUCCESSFULLY INSERTED ON [Setting]'
END
GO

IF NOT EXISTS (SELECT * FROM Setting WHERE [Name] IN ('transactionSettings.OrderRetentionDays'))
BEGIN
    INSERT INTO Setting ([Name],[Value],[StoreId]) VALUES
    ('transactionSettings.OrderRetentionDays','30',0)

    PRINT 'SUCCESSFULLY INSERTED ON [Setting]'
END
GO

IF COL_LENGTH('Customer','ReturnAmountPercentagePerday') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [ReturnAmountPercentagePerday] DECIMAL(18,2) NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Transaction','WithdrawalMethodId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Transaction] ADD [WithdrawalMethodId] INT FOREIGN KEY REFERENCES [dbo].[WithdrawalMethod] ([Id])
    ON UPDATE CASCADE
    ON DELETE CASCADE

	PRINT 'SUCCESSFULLY ALTERED TABLE [Transaction]'
END
GO

IF COL_LENGTH('Customer','DefaultWithdrawalMethodId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [DefaultWithdrawalMethodId] INT NOT NULL DEFAULT((0))

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Customer','LastReturnDate') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD LastReturnDate DATETIME

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Customer','PaymentTypeId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [PaymentTypeId] INT NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF NOT EXISTS (SELECT * FROM Setting WHERE [Name] IN ('transactionSettings.ForecfullyGiveReturn'))
BEGIN
    INSERT INTO Setting ([Name],[Value],[StoreId]) VALUES
    ('transactionSettings.ForecfullyGiveReturn','False',0)

    PRINT 'SUCCESSFULLY INSERTED ON [Setting]'
END
GO

IF NOT EXISTS (SELECT * FROM MessageTemplate WHERE [Name] = 'Customer.DeleteAccountRequest')
BEGIN
    INSERT INTO MessageTemplate ([Name],[BccEmailAddresses],[Subject],[EmailAccountId],[Body],[IsActive],[DelayBeforeSend],[DelayPeriodId],[AttachedDownloadId],[LimitedToStores]) VALUES
    ('Customer.DeleteAccountRequest',NULL,'%Store.Name%. Account delete request.',1,'<p>  <a href="%Store.URL%">%Store.Name%</a> Account delete request </p> ',1,NULL,0,0,0)

    PRINT 'SUCCESSFULLY ADDED'
END
GO
GO

IF COL_LENGTH('Transaction','ReturnPercentage') IS NULL
BEGIN
    ALTER TABLE [dbo].[Transaction] ADD [ReturnPercentage] DECIMAL(18,2) NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Transaction]'
END
GO

IF COL_LENGTH('Transaction','ReturnPercentage') IS NOT NULL
BEGIN
    ALTER TABLE [Transaction] DROP COLUMN [ReturnPercentage];

	PRINT 'SUCCESSFULLY ALTERED TABLE [Transaction]'
END
GO

IF OBJECT_ID('ReturnTransaction') IS NULL
BEGIN
    CREATE TABLE [ReturnTransaction] (
        [Id] INT PRIMARY KEY IDENTITY NOT NULL,
        [ReturnAmount] DECIMAL(18,2) NOT NULL,
        [ReturnPercentage] DECIMAL(18,2) NOT NULL,
        [ReturnDateOnUtc] DATETIME2 NOT NULL,
        [TransactionId] INT FOREIGN KEY REFERENCES [Transaction]([Id])
        )

	PRINT 'SUCCESSFULLY CREATED TABLE [ReturnTransaction]'
END
GO

IF EXISTS (select * from MessageTemplate where [Name]='Service.ContactUs')
BEGIN
UPDATE MessageTemplate SET [Body] = '<p>
Phone Number: %ContactUs.SenderPhoneNumber% <br />
Message: %ContactUs.Body%
</p>
' WHERE [Name] = 'Service.ContactUs'
END
GO

IF NOT EXISTS (SELECT * FROM PermissionRecord WHERE SystemName = 'DeleteTransaction')
BEGIN
   INSERT INTO PermissionRecord ([Name],[SystemName],[Category]) VALUES
   ('Admin area. Delete Transaction', 'DeleteTransaction', 'Customers')
   
   PRINT 'SUCCESSFULLY INSERTED ON [PermissionRecord]'
END
GO

IF NOT EXISTS (SELECT * FROM PermissionRecord WHERE SystemName = 'ManageRollBackOrder')
BEGIN
   INSERT INTO PermissionRecord ([Name],[SystemName],[Category]) VALUES
   ('Admin area. RollBack Order', 'ManageRollBackOrder', 'Orders')
   
   PRINT 'SUCCESSFULLY INSERTED ON [PermissionRecord]'
END
GO

IF NOT EXISTS (SELECT * FROM PermissionRecord WHERE SystemName = 'ManageTransaction')
BEGIN
   INSERT INTO PermissionRecord ([Name],[SystemName],[Category]) VALUES
   ('Admin area. Manage Transaction', 'ManageTransaction', 'Customers')
   
   PRINT 'SUCCESSFULLY INSERTED ON [PermissionRecord]'
END
GO

IF COL_LENGTH('Store','StoreEmailAddress') IS NULL
BEGIN
    ALTER TABLE [Store] ADD [StoreEmailAddress] varchar(500)

	PRINT 'SUCCESSFULLY ALTERED TABLE [Store]'
END
GO

IF NOT EXISTS (SELECT * FROM MessageTemplate WHERE [Name] IN ('Transaction.PendingDebitTransaction.CustomerNotification',
'Transaction.CompletedDebitTransaction.CustomerNotification',
'Transaction.DeclinedDebitTransaction.CustomerNotification',
'Transaction.RemovedDebitTransaction.CustomerNotification',
'Transaction.PendingDebitTransaction.AdminNotification',
'Transaction.CompletedDebitTransaction.AdminNotification',
'Transaction.DeclinedDebitTransaction.AdminNotification',
'Transaction.RemovedDebitTransaction.AdminNotification',
'Transaction.PendingCreditTransaction.CustomerNotification',
'Transaction.CompletedCreditTransaction.CustomerNotification',
'Transaction.DeclinedCreditTransaction.CustomerNotification',
'Transaction.RemovedCreditTransaction.CustomerNotification',
'Transaction.PendingCreditTransaction.AdminNotification',
'Transaction.CompletedCreditTransaction.AdminNotification',
'Transaction.DeclinedCreditTransaction.AdminNotification',
'Transaction.RemovedCreditTransaction.AdminNotification'))
BEGIN
	INSERT INTO MessageTemplate ([Name],[Subject],[EmailAccountId],[IsActive],[DelayPeriodId],[AttachedDownloadId],[LimitedToStores],[Body])
    select col,'%Store.Name% | Transaction Email', 1,1,0,0,0, '<p>  <a href="%Store.URL%">%Store.Name%</a> <br /><br />  Transaction Customer: %Customer.Email% <br />  Transaction Type: %Transaction.Type% <br />  Transaction Status: %Transaction.Status% <br />  Transaction Amount: %Transaction.Amount% <br />  Transaction Created: %Transaction.CreatedOn% <br />  </p>' from
    (values ('Transaction.PendingDebitTransaction.CustomerNotification'),
    ('Transaction.CompletedDebitTransaction.CustomerNotification'),
    ('Transaction.DeclinedDebitTransaction.CustomerNotification'),
    ('Transaction.RemovedDebitTransaction.CustomerNotification'),
    ('Transaction.PendingDebitTransaction.AdminNotification'),
    ('Transaction.CompletedDebitTransaction.AdminNotification'),
    ('Transaction.DeclinedDebitTransaction.AdminNotification'),
    ('Transaction.RemovedDebitTransaction.AdminNotification'),
    ('Transaction.PendingCreditTransaction.CustomerNotification'),
    ('Transaction.CompletedCreditTransaction.CustomerNotification'),
    ('Transaction.DeclinedCreditTransaction.CustomerNotification'),
    ('Transaction.RemovedCreditTransaction.CustomerNotification'),
    ('Transaction.PendingCreditTransaction.AdminNotification'),
    ('Transaction.CompletedCreditTransaction.AdminNotification'),
    ('Transaction.DeclinedCreditTransaction.AdminNotification'),
    ('Transaction.RemovedCreditTransaction.AdminNotification'))
    as tbl(col)


	
    PRINT 'SUCCESSFULLY INSERTED ON [MessageTemplate]'
END
GO

IF NOT EXISTS (SELECT * FROM MessageTemplate WHERE [Name] IN ('Transaction.ReturnGenerated.AdminNotification','Transaction.ReturnGenerated.CustomerNotification'))
BEGIN
	INSERT INTO MessageTemplate ([Name],[Subject],[EmailAccountId],[IsActive],[DelayPeriodId],[AttachedDownloadId],[LimitedToStores],[Body])
    select col,'%Store.Name% | Return Transaction Email', 1,1,0,0,0, '<p>  <a href="%Store.URL%">%Store.Name%</a> <br /><br />  Transaction Customer: %Customer.Email% <br />  Transaction Type: %Transaction.Type% <br />  Transaction Status: %Transaction.Status% <br />  Transaction Amount: %Transaction.Amount% <br />  Transaction Created: %Transaction.CreatedOn% <br />  </p>' from
    (values ('Transaction.ReturnGenerated.AdminNotification'),('Transaction.ReturnGenerated.CustomerNotification'))
    as tbl(col)
	
    PRINT 'SUCCESSFULLY INSERTED ON [MessageTemplate]'
END
GO

IF COL_LENGTH('Customer','RiskToleranceId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [RiskToleranceId] INT NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Customer','ExperienceId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [ExperienceId] INT NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Customer','TimelineId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [TimelineId] INT NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Customer','GoalId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [GoalId] INT NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Customer]'
END
GO

IF COL_LENGTH('Commission','Percentage') IS NULL
BEGIN
    ALTER TABLE [dbo].[Commission] ADD [Percentage] DECIMAL(18,2) NOT NULL

	PRINT 'SUCCESSFULLY ALTERED TABLE [Commission]'
END
GO
