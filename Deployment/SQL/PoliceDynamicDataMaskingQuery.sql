--EvanD is in teh supervisor role, so can see unmasked incidents.
EXECUTE ('SELECT TOP 10 Title, ReportedBy, Phone, Description FROM dbo.Incidents') AS USER = 'EvanD'

--JohnC is not a supervisor so cannot see the full unmasked details.
EXECUTE ('SELECT TOP 10 Title, ReportedBy, Phone, Description FROM dbo.Incidents') AS USER = 'JohnC' 