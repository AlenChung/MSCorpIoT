Import-Module C:\Dev\MSCorp.FirstResponse\util\MSCorp.DocumentDb.Commands\MSCorp.DocumentDb.Commands\bin\Debug\MSCorp.DocumentDb.Commands.dll
Add-DocumentDbSeedData 
https://leef-fr-docdb.documents.azure.com:443/
C0nPH61pkq7IR6H6lpp7NUbj9p/Dn2Lz749FBDGK2EdcRbkOqQ1XSHnRAcnPqnLUbZaG7sj+Hf2Hk4fp+2Xq3Q==
C:\Dev\MSCorp.FirstResponse\deployment\DocumentDb\TicketData.json


SELECT * 
FROM x
WHERE ST_WITHIN(x.Location, 
{ "type": "Polygon",  "coordinates": [[
[-122.022430, 47.584540], 
[-122.022217, 47.580847],
[-122.014425, 47.577210], 
[-122.003700, 47.577954],
[-122.003914, 47.584410],
[-122.022430, 47.584540]
]]})


[-122.022430, 47.584540], 
[-122.022217, 47.580847],
[-122.014425, 47.577210], 
[-122.003700, 47.577954],
[-122.003914, 47.584410],
[-122.022430, 47.584540]