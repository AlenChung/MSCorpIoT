--------------------------------------------------------------
-- SETUP User and User groups and roles.
--------------------------------------------------------------

CREATE USER JohnC WITHOUT LOGIN;
CREATE USER EvanD WITHOUT LOGIN; 
CREATE USER BenM WITHOUT LOGIN; 

GRANT SELECT ON dbo.[Incidents] to EvanD
GRANT SELECT ON dbo.[Incidents] to JohnC
GRANT SELECT ON dbo.[Incidents] to BenM

CREATE ROLE Supervisor;
EXEC sp_addrolemember 'Supervisor', 'EvanD';


CREATE TABLE [User] (
	[UserId]			INT				NOT NULL, 
	[UserName]			sysname			NOT NULL -- name of the user
 CONSTRAINT [PK_User_UserId] PRIMARY KEY CLUSTERED ([UserId] ASC)
) 
GO

CREATE TABLE [UserRegion] (
	[UserId]			INT				NOT NULL, 
	[RegionId]			INT				NOT NULL

 CONSTRAINT [PK_UserRegion] PRIMARY KEY CLUSTERED ([UserId] ASC, [RegionId] ASC),
 CONSTRAINT [FK_User] FOREIGN KEY (UserID) REFERENCES [User](UserId)
) 
GO

INSERT INTO [User] VALUES 
(1, 'JohnC'),
(2, 'EvanD'),
(3, 'BenM')

INSERT INTO [UserRegion] VALUES 
(1, 98074),
(1, 98075),
(3, 98053),
(3, 98052)
GO 