// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using ConsoleApp103;
using FirebirdSql.Data.FirebirdClient;

var dbPath = @"C:\foo\bar.fdb";
var stringQuery = @"select CAST(ABS(t1.DB + t1.CR) as varchar(8190)), CAST(t1.AdjustId as varchar(8190)) 
from TabAdjustAccount as t1 
JOIN TabAdjust as t2 on t1.AdjustId = t2.AdjustId 
where t1.AccType = 1 and t2.EffectiveDate >= @perBegin and t2.EffectiveDate <= @perEnd;";
var nonStringQuery = @"select ABS(t1.DB + t1.CR), t1.AdjustId 
from TabAdjustAccount as t1 
JOIN TabAdjust as t2 on t1.AdjustId = t2.AdjustId 
where t1.AccType = 1 and t2.EffectiveDate >= @perBegin and t2.EffectiveDate <= @perEnd;";

Console.WriteLine("Importer as non-embedded:");
var importer = new Importer(dbPath);

importer.Read(stringQuery);
//importer.Read(nonStringQuery);

//await importer.ReadAsync(stringQuery, CancellationToken.None);
//await importer.ReadAsync(nonStringQuery, CancellationToken.None);

FbConnection.ClearAllPools();

Console.WriteLine("Importer as embedded:");
var importerEmbedded = new Importer(dbPath, true);

importerEmbedded.Read(stringQuery);
//importerEmbedded.Read(nonStringQuery);

//await importerEmbedded.ReadAsync(stringQuery, CancellationToken.None);
//await importerEmbedded.ReadAsync(nonStringQuery, CancellationToken.None);

Console.ReadKey();