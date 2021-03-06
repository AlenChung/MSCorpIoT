-- Copyright (c) Microsoft. All rights reserved.
-- Licensed under the MIT license. See LICENSE file in the project root for full license information.

CREATE TABLE [Incidents] (
	[IncidentId]		int				NOT NULL, -- 
	[DepartmentType]	int				NOT NULL, -- Service - ambo police or fire
	[IncidentCategory]	int				NOT NULL, -- Incident category,Eg Alert, Animal, Arrest, Car, Fire, Officer Required, Stranger
	[RegionId]			int				NOT NULL, -- Specifies the region the incident is in, used for RLS
	[CallNumber]		varchar(255)	NOT NULL,
	[Phone]				varchar(255)	NOT NULL,
	[Title]				varchar(255)	NOT NULL,
	[ReportedBy]		varchar(255)	NOT NULL,
	[ReportedAt]		datetime2		NOT NULL,
	[Address1]			varchar(255)	NOT NULL,
	[Address2]			varchar(255)	NOT NULL,
	[Address3]			varchar(255)	NOT NULL,
	[Address4]			varchar(255)	NOT NULL,
	[Description]		varchar(MAX)	NOT NULL,
	[Latitude]			float			NOT NULL,
	[Longitude]			float			NOT NULL
 CONSTRAINT [PK_Incidents_IncidentId] PRIMARY KEY CLUSTERED (
	[IncidentId] ASC
 )
) 
GO
