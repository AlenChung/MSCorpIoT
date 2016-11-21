--------------------------------------------------------------
-- SETUP FOR RLS Demo
--------------------------------------------------------------

CREATE SCHEMA Security;
GO

CREATE FUNCTION [Security].fn_incident_predicate(@regionId AS int)
    RETURNS TABLE
WITH SCHEMABINDING
AS
    RETURN SELECT 1 AS fn_incident_predicate_result
	WHERE 
	(	
		--If the user is in the supervisor role they can see all.
		IS_MEMBER ( 'Supervisor' ) = 1 
	) OR EXISTS 
	(	
		--If the user has a mapping to the UserRegion then they can see based on the incident.
		SELECT		1 
		FROM		dbo.[UserRegion] UR
		INNER JOIN	dbo.[User] U 
		ON			UR.UserId = U.UserId
		WHERE		U.UserName = USER_NAME() 
		AND			UR.RegionId = @regionId 
	)
GO


CREATE SECURITY POLICY policy_incident
ADD FILTER PREDICATE [Security].fn_incident_predicate(RegionId) 
ON dbo.Incidents
WITH (STATE = ON);
GO
