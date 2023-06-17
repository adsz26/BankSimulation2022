-- táblák adatainak lekérdezése
SELECT * FROM Bank
SELECT * FROM Customer
SELECT * From CustomerTransaction
SELECT * FROM BankTransaction

-- Server name (régi 2022): (LocalDB)\MSSQLLocalDB
-- Server name (új 2023): banksqldatabasedemo.database.windows.net
------------------------------------------------------------------------
-- innentõl kijelölve a script végéig legenerál mindent futtatás után --
------------------------------------------------------------------------

-- adatbázis létrehozása, ha még nincs hozzáadva
GO
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BankDB')
BEGIN
    CREATE DATABASE BankDB
END
GO
    USE BankDB
    DROP TABLE IF EXISTS [Customer]
    DROP TABLE IF EXISTS [Bank]
    DROP TABLE IF EXISTS [CustomerTransaction]
    DROP TABLE IF EXISTS [BankTransaction]
GO

-- adatbázis táblák létrehozása
GO
CREATE TABLE [dbo].[Bank] (
    [BankID]							INT				IDENTITY (1, 1) NOT NULL,
    [BankName]							NVARCHAR (50)	NOT NULL,
    [BankPassword]						NVARCHAR (50)	NOT NULL,
    [BankAccountMoneyAccountNumber]		NVARCHAR (50)	NOT NULL,
    [BankAccountMoneyCurrency]			NVARCHAR (50)	DEFAULT 'HUF' NOT NULL,
    [BankAccountMoneyBalance]			FLOAT    (53)	DEFAULT 0 NOT NULL,
    [CentralBankMoneyAccountNumber]		NVARCHAR (50)	DEFAULT '00000000-00000000' NOT NULL,
    [CentralBankMoneyCurrency]			NVARCHAR (50)	DEFAULT 'HUF' NOT NULL,
    [CentralBankMoneyBalance]			FLOAT    (53)	DEFAULT 0 NOT NULL,
    [NumberOfCustomers]					INT				DEFAULT 0 NOT NULL,
    PRIMARY KEY CLUSTERED ([BankID] ASC)
);

CREATE TABLE [dbo].[Customer] (
    [CustomerID]						INT				IDENTITY (1, 1) NOT NULL,
    [CustomerName]						NVARCHAR (50)	NOT NULL,
    [CustomerPassword]					NVARCHAR (50)	NOT NULL,
    [CustomerAccountNumber]				NVARCHAR (50)	NOT NULL,
    [CustomerAccountNumberCurrency]		NVARCHAR (50)	DEFAULT 'HUF' NOT NULL,
    [CustomerAccountNumberBalance]		FLOAT    (53)	DEFAULT 0 NOT NULL,
    [CustomerCreditAccount]				NVARCHAR (50)	DEFAULT '00000000-00000000' NOT NULL,
    [CustomerCreditAccountCurrency]		NVARCHAR (50)	DEFAULT 'HUF' NOT NULL,
    [CustomerCreditAccountBalance]		FLOAT    (53)	DEFAULT 0 NOT NULL,
    [BankID]							INT				NOT NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC),
    CONSTRAINT [BankFK] FOREIGN KEY ([BankID]) REFERENCES [dbo].[Bank] ([BankID])
);

CREATE TABLE [dbo].[CustomerTransaction] (
    [CustomerTransactionID]				INT				IDENTITY (1, 1) NOT NULL,
    [CustomerID]						INT				NOT NULL,
    [Date]								DATETIME		NOT NULL DEFAULT GETDATE(),
    [CustomerTransactionType]			NVARCHAR (50)	NOT NULL,
    [CustomerTransactionAmount]			FLOAT    (53)	NOT NULL,
    [CustomerTransactionCurrency]		NVARCHAR (50)	NOT NULL DEFAULT 'HUF',
    [BeneficiaryName]					NVARCHAR (50)	NULL,
    [BeneficiaryAccountNumber]			NVARCHAR (50)	NULL,
    [LoanFromBank]						INT				NOT NULL DEFAULT 0
    PRIMARY KEY CLUSTERED ([CustomerTransactionID] ASC)
);

CREATE TABLE [dbo].[BankTransaction] (
    [BankTransactionID]					INT				IDENTITY (1, 1) NOT NULL,
    [BankID]							INT				NOT NULL,
    [Date]								DATETIME		NOT NULL DEFAULT GETDATE(),
    [BankTransactionType]				NVARCHAR (50)	NOT NULL,
    [BankTransactionAmount]				FLOAT    (53)	NOT NULL,
    [BankTransactionCurrency]			NVARCHAR (50)	NOT NULL DEFAULT 'HUF',
    PRIMARY KEY CLUSTERED ([BankTransactionID] ASC)
);

-- trigger parancsok

GO -- Bank ügyfél számának növelése
CREATE TRIGGER BankNumberOfCustomersInsert ON Customer AFTER INSERT
AS BEGIN SET NOCOUNT ON
    UPDATE Bank
    SET NumberOfCustomers = NumberOfCustomers + 1
    WHERE BankID = (select BankID from inserted)
END


GO -- Bank ügyfél számának csökkentése
CREATE TRIGGER BankNumberOfCustomersDelete ON Customer AFTER DELETE
AS BEGIN SET NOCOUNT ON
    UPDATE Bank
    SET NumberOfCustomers = NumberOfCustomers - 1
    WHERE BankID = (select BankID from deleted)
END

GO -- Bank számlapénzének összegzése az ügyfeleinek számlaszám egyenlegei alapján
CREATE TRIGGER BankAccountMoneyBalanceUpdateAfterNewCustomerInsert ON Customer AFTER INSERT
AS BEGIN SET NOCOUNT ON
    UPDATE Bank SET BankAccountMoneyBalance += i.CustomerAccountNumberBalance
    FROM Bank b join inserted i on b.BankID = i.BankID
END

-- insert parancsok

GO
-- Bankok hozzáadása az adatbázishoz
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank1', 'Bank1', '11111111-11111111', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank2', 'Bank2', '22222222-22222222', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank3', 'Bank3', '33333333-33333333', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank4', 'Bank4', '44444444-44444444', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank5', 'Bank5', '55555555-55555555', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank6', 'Bank6', '66666666-66666666', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank7', 'Bank7', '77777777-77777777', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank8', 'Bank8', '88888888-88888888', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)
INSERT INTO dbo.Bank (BankName, BankPassword, BankAccountMoneyAccountNumber, BankAccountMoneyCurrency, BankAccountMoneyBalance, CentralBankMoneyAccountNumber, CentralBankMoneyCurrency, CentralBankMoneyBalance, NumberOfCustomers) VALUES ('Bank9', 'Bank9', '99999999-99999999', 'HUF', 0, '00000000-00000000', 'HUF', 500000, 0)

GO
-- Ügyfelek hozzáadása az adatbázishoz
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel1', 'Ugyfel1', '94102442-56539178', 'HUF', 600000, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel2', 'Ugyfel2', '40436037-19150436', 'HUF', 700000, 2)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel3', 'Ugyfel3', '29405101-72249152', 'HUF', 771269, 9)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel4', 'Ugyfel4', '11659534-44679772', 'HUF', 834795, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel5', 'Ugyfel5', '91935864-28857171', 'HUF', 269648, 4)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel6', 'Ugyfel6', '70047728-61741995', 'HUF', 319846, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel7', 'Ugyfel7', '37120926-60140099', 'HUF', 578592, 2)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel8', 'Ugyfel8', '56544058-68179700', 'HUF', 297865, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel9', 'Ugyfel9', '73361960-94956068', 'HUF', 502384, 3)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel10', 'Ugyfel10', '82131907-51304912', 'HUF', 536849, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel11', 'Ugyfel11', '28046195-21393670', 'HUF', 907725, 5)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel12', 'Ugyfel12', '94828154-21919288', 'HUF', 859691, 7)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel13', 'Ugyfel13', '65255585-50535285', 'HUF', 534190, 4)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel14', 'Ugyfel14', '46372422-94675909', 'HUF', 799741, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel15', 'Ugyfel15', '47967459-50957186', 'HUF', 894072, 2)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel16', 'Ugyfel16', '99753327-76096361', 'HUF', 515467, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel17', 'Ugyfel17', '62682322-10766007', 'HUF', 260681, 5)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel18', 'Ugyfel18', '67862441-72148172', 'HUF', 532665, 8)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel19', 'Ugyfel19', '64747245-78701677', 'HUF', 285091, 3)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel20', 'Ugyfel20', '76524499-00777364', 'HUF', 433742, 5)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel21', 'Ugyfel21', '01061058-89039949', 'HUF', 633616, 7)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel22', 'Ugyfel22', '56200316-99047997', 'HUF', 772744, 3)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel23', 'Ugyfel23', '45372482-09108226', 'HUF', 646730, 4)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel24', 'Ugyfel24', '34187596-20824245', 'HUF', 969281, 5)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel25', 'Ugyfel25', '78227089-11035809', 'HUF', 480412, 6)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel26', 'Ugyfel26', '24206381-89674738', 'HUF', 272308, 3)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel27', 'Ugyfel27', '15826616-88991910', 'HUF', 335646, 4)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel28', 'Ugyfel28', '36140931-91275190', 'HUF', 317020, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel29', 'Ugyfel29', '10958724-51327613', 'HUF', 173533, 9)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel30', 'Ugyfel30', '79135428-69921097', 'HUF', 761160, 4)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel31', 'Ugyfel31', '73453853-13100321', 'HUF', 181184, 7)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel32', 'Ugyfel32', '18176131-67517225', 'HUF', 168032, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel33', 'Ugyfel33', '97008523-72982758', 'HUF', 924623, 4)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel34', 'Ugyfel34', '44066100-95480340', 'HUF', 725746, 9)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel35', 'Ugyfel35', '14296570-01015928', 'HUF', 769350, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel36', 'Ugyfel36', '40764234-37248215', 'HUF', 401737, 2)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel37', 'Ugyfel37', '69478228-20143883', 'HUF', 816686, 3)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel38', 'Ugyfel38', '06845661-74209568', 'HUF', 655105, 9)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel39', 'Ugyfel39', '45622706-93034402', 'HUF', 797023, 5)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel40', 'Ugyfel40', '21725709-62106846', 'HUF', 243143, 5)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel41', 'Ugyfel41', '20792676-32083669', 'HUF', 504778, 7)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel42', 'Ugyfel42', '48061978-42991701', 'HUF', 510918, 6)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel43', 'Ugyfel43', '03323216-46862643', 'HUF', 494367, 7)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel44', 'Ugyfel44', '54630372-36838827', 'HUF', 917212, 7)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel45', 'Ugyfel45', '78852348-02801172', 'HUF', 708602, 2)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel46', 'Ugyfel46', '60106614-36581863', 'HUF', 108157, 8)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel47', 'Ugyfel47', '43171202-74322510', 'HUF', 403171, 7)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel48', 'Ugyfel48', '52649932-84141807', 'HUF', 154554, 6)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel49', 'Ugyfel49', '14964673-32196190', 'HUF', 700973, 1)
INSERT INTO dbo.Customer (CustomerName, CustomerPassword, CustomerAccountNumber, CustomerAccountNumberCurrency, CustomerAccountNumberBalance, BankID) VALUES ('Ugyfel50', 'Ugyfel50', '12218735-26164106', 'HUF', 296309, 9)