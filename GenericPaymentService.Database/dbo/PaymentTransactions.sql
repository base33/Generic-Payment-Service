CREATE TABLE [dbo].[PaymentTransactions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionId] NVARCHAR(50) NOT NULL, 
    [Amount] MONEY NOT NULL, 
    [PaymentRequest] NVARCHAR(MAX) NOT NULL, 
    [ProductType] NVARCHAR(MAX) NOT NULL, 
    [Paid] BIT NOT NULL, 
    [Notified] BIT NOT NULL 
)
