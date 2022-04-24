using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using FirebirdSql.Data.FirebirdClient;

namespace ConsoleApp103
{

    public class Importer
    {

        private readonly string _conString;

        public Importer(string dbPath, bool embedded = false)
        {
            var conStringBuilder = embedded ? GetEmbeddedConnectionString(dbPath) : GetConnectionStringBase(dbPath);
            _conString = conStringBuilder.ToString();
        }
        private FbConnectionStringBuilder GetEmbeddedConnectionString(string dbPath)
        {
            //go up to solution dir
            var fbClientDll = Path.GetFullPath("..\\..\\..\\..\\Fb4xEmbedded\\fbclient.dll");
            return new FbConnectionStringBuilder
            {
                Database = dbPath,
                ClientLibrary = fbClientDll,
                ServerType = FbServerType.Embedded,
                UserID = "sysdba",
                Password = "masterkey",
            };
        }
        private FbConnectionStringBuilder GetConnectionStringBase(string firebirdPath)
        {
            var stringBuilder = new FbConnectionStringBuilder
            {
                DataSource = "localhost",
                Database = firebirdPath,
                Port = 3052,
                UserID = "sysdba",
                Password = "masterkey",
            };

            return stringBuilder;
        }

        public async Task ReadAsync(string query, CancellationToken cancellationToken)
        {
            FbConnection dbConnection = new(_conString);
            await using var _ = dbConnection.ConfigureAwait(false);
            await dbConnection.CheckConnectionAsync(cancellationToken).ConfigureAwait(false);

            var sw = Stopwatch.StartNew();
            var intCounter = 0;

            try
            {
                DbCommand dbCommand = dbConnection.CreateCommand();
                await using var __ = dbCommand.ConfigureAwait(false);
                dbCommand.CommandText = query;

                dbCommand.AddParameterWithValue("perBegin", new DateTime(2015, 01, 01));
                dbCommand.AddParameterWithValue("perEnd", new DateTime(2025, 12, 31, 23, 59, 59));
                var dbReader = await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                await using var ___ = dbReader.ConfigureAwait(false);
                while (await dbReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    intCounter++;
                }
            }
            finally
            {
                sw.Stop();
                Console.WriteLine("ReadAsync");
                Console.WriteLine($"{query} | Results: {intCounter}");
                Console.WriteLine(sw.ElapsedMilliseconds.ToString("N2"));
                Console.WriteLine();
            }
        }
        public void Read(string query)
        {
            using FbConnection dbConnection = new(_conString);
            dbConnection.CheckConnection();

            var sw = Stopwatch.StartNew();
            var intCounter = 0;

            try
            {
                using DbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = query;

                dbCommand.AddParameterWithValue("perBegin", new DateTime(2015, 01, 01));
                dbCommand.AddParameterWithValue("perEnd", new DateTime(2025, 12, 31, 23, 59, 59));
                using var dbReader = dbCommand.ExecuteReader();
                while (dbReader.Read())
                {
                    intCounter++;
                }
            }
            finally
            {
                sw.Stop();
                Console.WriteLine("Read");
                Console.WriteLine($"{query} | Results: {intCounter}");
                Console.WriteLine(sw.ElapsedMilliseconds.ToString("N2"));
                Console.WriteLine();
            }
        }
    }
}
