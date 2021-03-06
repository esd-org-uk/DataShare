
/****** Object:  Table dbo.DS_Theme    Script Date: 03/05/2014 17:03:48 ******/
SET IDENTITY_INSERT dbo.DS_Theme ON;
IF NOT EXISTS(SELECT * FROM  dbo.DS_Theme WHERE Id=1)
INSERT dbo.DS_Theme (Id, Title, CssClass) VALUES (1, N'Default', N'green');
SET IDENTITY_INSERT dbo.DS_Theme OFF;


/****** Object:  Table dbo.EdmMetadata    Script Date: 03/05/2014 17:03:48 ******/
SET IDENTITY_INSERT dbo.EdmMetadata ON;
IF NOT EXISTS(SELECT * FROM  dbo.EdmMetadata WHERE Id=1)
INSERT dbo.EdmMetadata (Id, ModelHash) VALUES (1, N'02A6F0BB9922633768E81BFEBD80833DC38EEB71D93072878A5E6C9FA3B376E1');
SET IDENTITY_INSERT dbo.EdmMetadata OFF;


/****** Object:  Table dbo.aspnet_Applications    Script Date: 03/05/2014 17:03:48 ******/
IF NOT EXISTS(SELECT * FROM  dbo.aspnet_Applications WHERE ApplicationId=N'49239412-13bf-4892-bb6e-bccb04dd7ba7')
INSERT dbo.aspnet_Applications (ApplicationName, LoweredApplicationName, ApplicationId, Description) VALUES (N'/', N'/', N'49239412-13bf-4892-bb6e-bccb04dd7ba7', NULL);

/****** Object:  Table dbo.aspnet_SchemaVersions    Script Date: 03/05/2014 17:03:55 ******/
IF NOT EXISTS(SELECT * FROM  dbo.aspnet_SchemaVersions WHERE Feature=N'common' AND CompatibleSchemaVersion=N'1')
INSERT dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) VALUES (N'common', N'1', 1);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_SchemaVersions WHERE Feature=N'health monitoring' AND CompatibleSchemaVersion=N'1')
INSERT dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) VALUES (N'health monitoring', N'1', 1);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_SchemaVersions WHERE Feature=N'membership' AND CompatibleSchemaVersion=N'1')
INSERT dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) VALUES (N'membership', N'1', 1);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_SchemaVersions WHERE Feature=N'personalization' AND CompatibleSchemaVersion=N'1')
INSERT dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) VALUES (N'personalization', N'1', 1);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_SchemaVersions WHERE Feature=N'profile' AND CompatibleSchemaVersion=N'1')
INSERT dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) VALUES (N'profile', N'1', 1);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_SchemaVersions WHERE Feature=N'role manager' AND CompatibleSchemaVersion=N'1')
INSERT dbo.aspnet_SchemaVersions (Feature, CompatibleSchemaVersion, IsCurrentVersion) VALUES (N'role manager', N'1', 1);

/****** Object:  Table dbo.aspnet_Users    Script Date: 03/05/2014 17:03:55 ******/
IF NOT EXISTS(SELECT * FROM  dbo.aspnet_Users WHERE UserId=N'b48eef16-19b9-4884-8d58-36de0bf33e3c')
INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, MobileAlias, IsAnonymous, LastActivityDate) VALUES (N'49239412-13bf-4892-bb6e-bccb04dd7ba7', N'b48eef16-19b9-4884-8d58-36de0bf33e3c', N'admin', N'admin', NULL, 0, CAST(0x0000A04500890617 AS DateTime));

/****** Object:  Table dbo.aspnet_Roles    Script Date: 03/05/2014 17:03:55 ******/
IF NOT EXISTS(SELECT * FROM  dbo.aspnet_Roles WHERE RoleId=N'326c5885-92fc-44ba-846f-7ab733088295')
INSERT dbo.aspnet_Roles (ApplicationId, RoleId, RoleName, LoweredRoleName, Description) VALUES (N'49239412-13bf-4892-bb6e-bccb04dd7ba7', N'326c5885-92fc-44ba-846f-7ab733088295', N'SchemaCreator', N'schemacreator', NULL);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_Roles WHERE RoleId=N'81ff7e3f-dd66-47cb-a2e4-237513a6e0a6')
INSERT dbo.aspnet_Roles (ApplicationId, RoleId, RoleName, LoweredRoleName, Description) VALUES (N'49239412-13bf-4892-bb6e-bccb04dd7ba7', N'81ff7e3f-dd66-47cb-a2e4-237513a6e0a6', N'SchemaEditor', N'schemaeditor', NULL);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_Roles WHERE RoleId=N'1b696889-61d0-483c-8e40-6f6da6d84bd7')
INSERT dbo.aspnet_Roles (ApplicationId, RoleId, RoleName, LoweredRoleName, Description) VALUES (N'49239412-13bf-4892-bb6e-bccb04dd7ba7', N'1b696889-61d0-483c-8e40-6f6da6d84bd7', N'SuperAdministrator', N'superadministrator', NULL);

IF NOT EXISTS(SELECT * FROM  dbo.aspnet_Roles WHERE RoleId=N'e2d2b2a2-898d-476a-8b88-517a1a823e39')
INSERT dbo.aspnet_Roles (ApplicationId, RoleId, RoleName, LoweredRoleName, Description) VALUES (N'49239412-13bf-4892-bb6e-bccb04dd7ba7', N'e2d2b2a2-898d-476a-8b88-517a1a823e39', N'Uploader', N'uploader', NULL);

/****** Object:  Table dbo.aspnet_UsersInRoles    Script Date: 03/05/2014 17:03:55 ******/
IF NOT EXISTS(SELECT * FROM  dbo.aspnet_UsersInRoles WHERE UserId=N'b48eef16-19b9-4884-8d58-36de0bf33e3c' AND RoleId = N'1b696889-61d0-483c-8e40-6f6da6d84bd7')
INSERT dbo.aspnet_UsersInRoles (UserId, RoleId) VALUES (N'b48eef16-19b9-4884-8d58-36de0bf33e3c', N'1b696889-61d0-483c-8e40-6f6da6d84bd7');

/****** Object:  Table dbo.aspnet_Membership    Script Date: 03/05/2014 17:03:55 ******/
IF NOT EXISTS(SELECT * FROM  dbo.aspnet_Membership WHERE UserId=N'b48eef16-19b9-4884-8d58-36de0bf33e3c')
INSERT dbo.aspnet_Membership (ApplicationId, UserId, Password, PasswordFormat, PasswordSalt, MobilePIN, Email, LoweredEmail, PasswordQuestion, PasswordAnswer, IsApproved, IsLockedOut, CreateDate, LastLoginDate, LastPasswordChangedDate, LastLockoutDate, FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart, Comment) VALUES (N'49239412-13bf-4892-bb6e-bccb04dd7ba7', N'b48eef16-19b9-4884-8d58-36de0bf33e3c', N'54OSfzH+b1W26qHyQg5MrsQWiZM=', 1, N'YuAzhj/r9eVy9H1bdp62EQ==', NULL, N'web.manager@redbridge.gov.uk', N'web.manager@redbridge.gov.uk', NULL, NULL, 1, 0, CAST(0x00009EB70092C124 AS DateTime), CAST(0x0000A04500890617 AS DateTime), CAST(0x00009EE900F80C80 AS DateTime), CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), 0, CAST(0xFFFF2FB300000000 AS DateTime), NULL);


