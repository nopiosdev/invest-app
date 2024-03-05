INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.accountvalue', 'Account Value', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.accountvalue'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.netcontribution', 'NET Contribution', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.netcontribution'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.investedamount', 'Invested Amount', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.investedamount'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.netreturn', 'NET Return', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.netreturn'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.addfunds', 'Invest', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.addfunds'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.withdrawfunds', 'Withdraw', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.withdrawfunds'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'transaction.method', 'Method', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'transaction.method'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'transaction.amount', 'Amount', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'transaction.amount'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'transaction.status', 'Status', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'transaction.status'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'transaction.date', 'Date', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'transaction.date'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.invest', 'Invest', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.invest'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.withdraw', 'Withdraw', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.withdraw'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.deposit.amount', 'Invest Amount', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.deposit.amount'
);

-- Example for 'customer.recenttransactions'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.recenttransactions', 'Recent Transactions', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.recenttransactions'
);

-- Example for 'customer.dashboard'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.dashboard', 'Dashboard', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.dashboard'
);

-- Example for 'customer.analytics'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.analytics', 'Analytics', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.analytics'
);

-- Example for 'customer.transactions'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.transactions', 'Transactions', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.transactions'
);

-- Example for 'customer.invest'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.invest', 'Invest', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.invest'
);

-- Example for 'customer.withdraw'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.withdraw', 'Withdraw', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.withdraw'
);

-- Example for 'account.withdraw.amount'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.withdraw.amount', 'Withdrawal Amount', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.withdraw.amount'
);

-- Example for 'account.transactions'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.transactions', 'Transactions', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.transactions'
);

-- Example for 'account.deposits'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.deposits', 'Deposits', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.deposits'
);

-- Example for 'account.withdrawals'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.withdrawals', 'Withdrawals', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.withdrawals'
);

-- Example for 'account.dashboard'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.dashboard', 'Dashboard', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.dashboard'
);

-- Example for 'account.analytics'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.analytics', 'Analytics', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.analytics'
);

-- Example for 'account.withdraw.methods'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.withdraw.methods', 'Withdrawal Methods', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.withdraw.methods'
);

-- Example for 'account.analysis.chart'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.analysis.chart', 'Analysis Chart', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.analysis.chart'
);

-- Example for 'account.analysis.tradingreports'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.analysis.tradingreports', 'Trading Reports', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.analysis.tradingreports'
);

-- Example for 'account.statements'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.statements', 'Account Statements', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.statements'
);

-- Example for 'customer.invest.continue'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.invest.continue', 'Continue', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.invest.continue'
);

-- Example for 'customer.withdraw.continue'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.withdraw.continue', 'Continue', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.withdraw.continue'
);

-- Example for 'account.settings.title'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.settings.title', 'Account Management', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.settings.title'
);

-- Example for 'account.accountdetails'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.accountdetails', 'Account Details', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.accountdetails'
);

-- Example for 'account.address'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.address', 'Address', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.address'
);

-- Example for 'account.settings'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.settings', 'Settings', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.settings'
);

-- Example for 'account.disclosure'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.disclosure', 'Disclosure', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.disclosure'
);

-- Example for 'account.submitteddocuments'
INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.submitteddocuments', 'Submitted Documents', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.submitteddocuments'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.login.returntologin', 'Return To Login', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.login.returntologin'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'platform.company.name', 'Invest-i', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'platform.company.name'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Account.Login.Welcome', 'Wealth Management Platform Powered by AI', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Account.Login.Welcome'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Account.chatbot', 'Your AI Chat Bot (Comming Soon)', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Account.chatbot'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'chat.chatbot.tooltip', 'How can I help?', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'chat.chatbot.tooltip'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Chat.ChatBox.Text.Label', 'How can I help?', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Chat.ChatBox.Text.Label'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Dashboard.NumberOfPendingDeposits', 'Number Of Pending Deposits', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Dashboard.NumberOfPendingDeposits'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Dashboard.TotalAmountOfPendingDeposits', 'Total Amount of Pending Depoits', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Dashboard.TotalAmountOfPendingDeposits'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Dashboard.lpinfo', 'Liquidity Pool Information', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Dashboard.lpinfo'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Dashboard.liquitypooltotalvalue', 'Liquidity Pool Total Value', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Dashboard.liquitypooltotalvalue'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Dashboard.liquiditypoollimitvalue', 'Liquidity Pool Deposit Limit', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Dashboard.liquiditypoollimitvalue'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Dashboard.TotalAmountOfDeposits', 'Total Amount Of Deposits', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Dashboard.TotalAmountOfDeposits'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'ContactUs.PhoneNumber', 'Your phone number', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'ContactUs.PhoneNumber'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'ContactUs.PhoneNumber.Hint', 'Enter your phone number.', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'ContactUs.PhoneNumber.Hint'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Configuration.Stores.Fields.StoreEmailAddress', 'Store Email Address', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Configuration.Stores.Fields.StoreEmailAddress'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Configuration.Settings.GeneralCommon.LinkedinLink', 'LinkedIn Link', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Configuration.Settings.GeneralCommon.LinkedinLink'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Configuration.Settings.GeneralCommon.DiscordLink', 'Discord Link', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Configuration.Settings.GeneralCommon.DiscordLink'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Configuration.Settings.GeneralCommon.TelegramLink', 'Telegram Link', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Configuration.Settings.GeneralCommon.TelegramLink'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Orders.Fields.RollBackOrder', 'Roll Back Order', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Orders.Fields.RollBackOrder'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.withdraw.transaction.investedamountexceed', 'You are about to withdraw from your invested funds. Please note that the NET Return will be adjusted based on the remaining amount.', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.withdraw.transaction.investedamountexceed'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.withdraw.transaction.nowithdrawalmethodfound', 'Please select a Withdrawal Method', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.withdraw.transaction.nowithdrawalmethodfound'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.invest.returnamount', 'Interest Credited', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.invest.returnamount'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.feature.comingsoon', 'Feature Coming Soon', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.feature.comingsoon'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'activitylog.customer.invest.transaction.successfull', 'Your Deposit is successful', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'activitylog.customer.invest.transaction.successfull'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'activitylog.customer.withdraw.transaction.successfull', 'Your withdrawal is successful', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'activitylog.customer.withdraw.transaction.successfull'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'activitylog.customer.invest.transaction.pending', 'Your deposit is pending', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'activitylog.customer.invest.transaction.pending'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.theme', 'Theme', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.theme'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.currency', 'Currency', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.currency'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'account.preference', 'Account Preference', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'account.preference'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.invest.transaction.successfull', 'Your Deposit is successful', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.invest.transaction.successfull'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'customer.withdraw.transaction.successfull', 'Your Withdrawal is successful', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'customer.withdraw.transaction.successfull'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'pagetitle.dashboard', 'Dashboard', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'pagetitle.dashboard'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'pagetitle.transactions', 'Transactions', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'pagetitle.transactions'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'pagetitle.invest', 'Invest', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'pagetitle.invest'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'pagetitle.withdraw', 'Withdraw', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'pagetitle.withdraw'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'pagetitle.analytics', 'Analytics', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'pagetitle.analytics'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionSearchModel.Field.Month', 'Select Month', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionSearchModel.Field.Month'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionSearchModel.Field.Year', 'Select Year', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionSearchModel.Field.Year'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionModel.Fields.CustomerFullName', 'Full Name', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionModel.Fields.CustomerFullName'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionModel.Fields.CustomerEmail', 'Email Address', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionModel.Fields.CustomerEmail'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionModel.Fields.ReturnAmount', 'Return Amount', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionModel.Fields.ReturnAmount'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionModel.Fields.ReturnPercentage', 'Return %', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionModel.Fields.ReturnPercentage'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionModel.Fields.WithdrawalMethod', 'Withdrawal Method', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionModel.Fields.WithdrawalMethod'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionModel.Fields.Month', 'Month', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionModel.Fields.Month'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.ReturnTransactionModel.Fields.Year', 'Year', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.ReturnTransactionModel.Fields.Year'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'admin.customers.customercommission', 'Sales and Commissions', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'admin.customers.customercommission'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'admin.customers.customercommissionmodel.totalpaidcommission', 'Total Paid Commission', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'admin.customers.customercommissionmodel.totalpaidcommission'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'Admin.Customers.CustomerCommission.List.Customers', 'List of Customers', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'Admin.Customers.CustomerCommission.List.Customers'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'admin.returntransaction', 'Customer Returns', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'admin.returntransaction'
);

INSERT INTO [dbo].[LocaleStringResource] ([ResourceName], [ResourceValue], [LanguageId])
SELECT 'admin.customers.returntransactions', 'Customer Returns', 1
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[LocaleStringResource]
    WHERE [ResourceName] = 'admin.customers.returntransactions'
);

