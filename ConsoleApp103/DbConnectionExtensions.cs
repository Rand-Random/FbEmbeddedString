using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace ConsoleApp103;

public static class DbConnectionExtensions
{
    public static void CheckConnection(this IDbConnection dbCon)
    {
        if (dbCon == null)
            throw new Exception("## Database connection cannot be established ##");

        if (string.IsNullOrWhiteSpace(dbCon.ConnectionString))
            throw new Exception("## Database connection cannot be established ##");

        if (dbCon.State == ConnectionState.Broken)
            dbCon.Close();

        if (dbCon.State == ConnectionState.Closed)
            dbCon.Open();
        else if (dbCon.State == ConnectionState.Broken)
            throw new Exception("## Database connection is in an invalid state ##");
    }
    public static async Task CheckConnectionAsync(this FbConnection fbCon, CancellationToken cancellationToken)
    {
        if (fbCon == null)
            throw new Exception("## Database connection cannot be established ##");

        if (string.IsNullOrWhiteSpace(fbCon.ConnectionString))
            throw new Exception("## Database connection cannot be established ##");

        if (fbCon.State == ConnectionState.Broken)
            await fbCon.CloseAsync();

        if (fbCon.State == ConnectionState.Closed)
            await fbCon.OpenAsync(cancellationToken);
        else if (fbCon.State == ConnectionState.Broken)
            throw new Exception("## Database connection is in an invalid state ##");
    }
    public static void AddParameterWithValue(this IDbCommand command, string parameterName, object parameterValue)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = parameterValue;
        command.Parameters.Add(parameter);
    }
}