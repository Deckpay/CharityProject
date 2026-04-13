-- ============================================================================
-- ADATBÁZIS LÉTREHOZÁS
-- ============================================================================
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'Ertekmento_db')
BEGIN
    ALTER DATABASE Ertekmento_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Ertekmento_db;
END
GO

CREATE DATABASE Ertekmento_db;
GO

USE Ertekmento_db;
GO

-- ============================================================================
-- 1. COUNTY (Magyarország vármegyéi)
-- ============================================================================
CREATE TABLE Counties (
    CountyId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    CountyName NVARCHAR(100) NOT NULL UNIQUE,
);
GO

-- ============================================================================
-- 2. PRODUCT CATEGORY (Termék kategóriák)
-- ============================================================================
CREATE TABLE ProductCategory (
    ProductCategoryId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    ProductCategoryName NVARCHAR(100) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- ============================================================================
-- 3. USERS (Felhasználók)
-- ============================================================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    UserRole INT NOT NULL, -- 1: Admin, 2: Sender, 3: Requester, stb.
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    UserStatus INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
);
GO

-- ============================================================================
-- 4. PRODUCT (Termékek)
-- ============================================================================
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    SenderId INT NOT NULL,
    ProductCategoryId INT NOT NULL,
    CountyId INT NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    ProductDescription NVARCHAR(MAX),
    ImagePath NVARCHAR(500),
    ProductStatus INT NOT NULL, -- Pl: 1: Elérhető, 2: Folyamatban, 3: Átadva
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_Product_SenderId FOREIGN KEY (SenderId) REFERENCES Users(UserId),
    CONSTRAINT FK_Product_CategoryId FOREIGN KEY (ProductCategoryId) REFERENCES ProductCategory(ProductCategoryId),
    CONSTRAINT FK_Product_CountyId FOREIGN KEY (CountyId) REFERENCES Counties(CountyId)
);
GO

-- ============================================================================
-- 5. PRODUCT REQUEST (Igénylések)
-- ============================================================================
CREATE TABLE ProductRequests (
    ProductRequestId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    ProductId INT NOT NULL,
    RequesterId INT NOT NULL,
    RequestStatus INT NOT NULL, -- Pl: 1: Függőben, 2: Elfogadva, 3: Elutasítva
    RequestedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ProcessedAt DATETIME2,
    
    CONSTRAINT FK_ProductRequests_ProductId FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE,
    CONSTRAINT FK_ProductRequests_RequesterId FOREIGN KEY (RequesterId) REFERENCES Users(UserId)
);
GO

-- ============================================================================
-- 6. REQUESTER LIMIT RULE
-- ============================================================================
CREATE TABLE RequesterLimitRule (
    RequesterLimitRuleId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    RequesterLimitRuleCategoryId INT NOT NULL,
    PeriodType NVARCHAR(50) NOT NULL, -- 'Monthly', 'Quarterly', 'Yearly'
    MaxQuantity INT NOT NULL,
    RequesterLimitRuleDescription NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_RequesterLimitRule_CategoryId FOREIGN KEY (RequesterLimitRuleCategoryId) REFERENCES ProductCategory(ProductCategoryId),
    CONSTRAINT CK_RequesterLimitRule_PeriodType CHECK (PeriodType IN ('Weekly', 'Monthly', 'Quarterly', 'Semiannual')),
    CONSTRAINT CK_RequesterLimitRule_MaxQuantity CHECK (MaxQuantity > 0)
);
GO

-- ============================================================================
-- 7. REQUESTER LIMIT USAGE
-- ============================================================================
CREATE TABLE RequesterLimitUsage (
    RequesterLimitUsageId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    RequesterId INT NOT NULL,
    RuleId INT NOT NULL,
    PeriodStart DATETIME2 NOT NULL,
    PeriodEnd DATETIME2 NOT NULL,
    UsedQuantity INT NOT NULL DEFAULT 0,
    LastResetAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_RequesterLimitUsage_RequesterId FOREIGN KEY (RequesterId) REFERENCES Users(UserId),
    CONSTRAINT FK_RequesterLimitUsage_RuleId FOREIGN KEY (RuleId) REFERENCES RequesterLimitRule(RequesterLimitRuleId),
    CONSTRAINT UC_RequesterLimitUsage UNIQUE (RequesterId, RuleId, PeriodStart) -- Nem létezhet két olyan sor a táblában, ahol ez a három adat (Felhasználó, Szabály, Időszak kezdete) egyszerre megegyezik
);
GO

-- ============================================================================
-- 8. CHAT
-- ============================================================================
CREATE TABLE Chat (
    ChatId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    ProductRequestId INT NOT NULL UNIQUE,
    SenderId INT NOT NULL,
    RequesterId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_Chat_ProductRequestId FOREIGN KEY (ProductRequestId) REFERENCES ProductRequests(ProductRequestId) ON DELETE CASCADE,
    CONSTRAINT FK_Chat_SenderId FOREIGN KEY (SenderId) REFERENCES Users(UserId),
    CONSTRAINT FK_Chat_RequesterId FOREIGN KEY (RequesterId) REFERENCES Users(UserId)
);
GO

-- ============================================================================
-- 9. CHAT MESSAGE
-- ============================================================================
CREATE TABLE ChatMessage (
    ChatMessageId INT IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    ChatId INT NOT NULL,
    SenderId INT NOT NULL,
    ChatMessage NVARCHAR(MAX) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    SentAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReadAt DATETIME2,
    
    CONSTRAINT FK_ChatMessage_ChatId FOREIGN KEY (ChatId) REFERENCES Chat(ChatId) ON DELETE CASCADE,
    CONSTRAINT FK_ChatMessage_SenderId FOREIGN KEY (SenderId) REFERENCES Users(UserId)
);
GO

-- ============================================================================
-- INDEXEK A GYORSABB KERESÉSHEZ
-- ============================================================================

-- Termékeknél a leggyakoribb lekérdezések: "Kategória szerint", "Gazda szerint", "Helyszín szerint"
CREATE INDEX IX_Products_CategoryId ON Products(ProductCategoryId);
CREATE INDEX IX_Products_SenderId ON Products(SenderId);
CREATE INDEX IX_Products_CountyId ON Products(CountyId);

-- Igényléseknél: "Ki igényelte?" és "Melyik terméket?"
-- (A termékek státuszát is sűrűn fogod nézni)
CREATE INDEX IX_ProductRequests_RequesterId ON ProductRequests(RequesterId);
CREATE INDEX IX_ProductRequests_ProductId ON ProductRequests(ProductId);

-- Chat üzeneteknél: "Melyik beszélgetéshez tartozik?" (Nagyon fontos a gyors üzenetváltáshoz)
CREATE INDEX IX_ChatMessage_ChatId ON ChatMessage(ChatId);
GO

-- ============================================================================
-- SAMPLE DATA - FELTÖLTÉS
-- ============================================================================

INSERT INTO Counties (CountyName) VALUES
('Bács-Kiskun'), ('Baranya'), ('Békés'), 
('Borsod-Abaúj-Zemplén'), ('Budapest'), ('Csongrád-Csanád'), ('Fejér'), 
('Győr-Moson-Sopron'), ('Hajdú-Bihar'), ('Heves'), 
('Jász-Nagykun-Szolnok'), ('Komárom-Esztergom'), ('Nógrád'), 
('Pest'), ('Somogy'), ('Szabolcs-Szatmár-Bereg'), 
('Tolna'), ('Vas'), ('Veszprém'), ('Zala');
GO

INSERT INTO ProductCategory (ProductCategoryName) VALUES
('Élelmiszer'), ('Ruházkodás'), ('Bútor'), ('Elektronika'), 
('Könyv & Oktatás'), ('Gyógyszer & Egészség'), ('Játék & Hobbi'), ('Egyéb');
GO

INSERT INTO Users (Email,PasswordHash,UserName,UserRole,FirstName,LastName)
VALUES
('userSender@teszt.com', '$2a$11$VLFdUfGRb8knvF3/6ZmExu6FisY1bchLLZGeBMdSY41IModGHdelq', 'Felado', 1, 'Feladó', 'Felhasználó'),
('userRequester@teszt.com', '$2a$11$BVS3cjGK0hmhG8ZxggmceO5yjleAxXJ/.szVzlzK5qAKH.7jReH62', 'Igenylo', 2, 'Igénylő', 'Felhasználó'),
('useradmin@teszt.com', '$2a$11$KbfNOaTwYw3nXBBoW/LGNu0Mgk788abljOV0kXlI.TUx7CKRnhbNK', 'Admin', 3, 'Admin', 'Felhasználó');
GO

INSERT INTO Products (SenderId,ProductCategoryId,CountyId,ProductName,ProductDescription,ImagePath,ProductStatus)
VALUES
(1, 3, 5, 'Asztal', 'Családi asztal, 4 férőhelyes', '/images/products/asztal.jpg', 1),
(1, 4, 2, 'Mosósép', 'Régi de működő mosógép', '/images/products/mosogep.jpg', 1),
(1, 7, 9, 'Gyerekjátékok', 'Több gyerekjáték egyben', '/images/products/gyerekjatekok.jpg', 1),
(1, 7, 9, 'Pelenkák', 'Több pelenka nagy csomagban', '/images/products/pelenka.jpg', 1),
(1, 4, 10, 'Szekrény', 'Kissebb 2 ajtós szekrény', '/images/products/szekreny.jpg', 1),
(1, 2, 7, 'Kabát', 'Meleg télikabát', '/images/products/kabat.jpg', 1);
GO


-- =====================================================
-- LEKÉRDEZÉS - ÖSSZES TÁBLA ELLENŐRZÉSE
-- =====================================================
SELECT * FROM Counties;
SELECT * FROM ProductCategory;
SELECT * FROM Users;
SELECT * FROM Products;
SELECT * FROM ProductRequests;
SELECT * FROM RequesterLimitRule;
SELECT * FROM RequesterLimitUsage;
SELECT * FROM Chat;
SELECT * FROM ChatMessage;