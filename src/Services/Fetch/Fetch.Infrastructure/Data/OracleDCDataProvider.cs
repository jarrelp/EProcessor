using System.Data;
using Devart.Data.Oracle;
using Ecmanage.eProcessor.Services.Fetch.Fetch.Application.Common.Interfaces;

namespace Ecmanage.eProcessor.Services.Fetch.Fetch.Infrastructure.Data;

public class OracleDCDataProvider : IOracleDCDataProvider
{
    private readonly OracleConnection _dbConnection;

    public OracleDCDataProvider(string connectionString)
    {
        _dbConnection = CreateConnection(connectionString);
    }

    private OracleConnection CreateConnection(string newConnectionString)
    {
        OracleConnection retVal = new(newConnectionString)
        {
            Unicode = true
        };
        retVal.Open();


        OracleGlobalization loSessionInfo = retVal.GetSessionInfo();
        loSessionInfo.Language = "AMERICAN";
        loSessionInfo.Territory = "AMERICA";
        loSessionInfo.Currency = "$";
        loSessionInfo.ISOCurrency = "AMERICA";
        loSessionInfo.NumericCharacters = ".,";

        loSessionInfo.DateFormat = "DD-MON-RR";
        loSessionInfo.DateLanguage = "AMERICAN";


        loSessionInfo.TimeStampFormat = "DD-MON-RR HH.MI.SSXFF AM";

        loSessionInfo.TimeStampTZFormat = "DD-MON-RR HH.MI.SSXFF AM TZR";
        loSessionInfo.DualCurrency = "$";


        loSessionInfo.NCharConversionException = false;
        retVal.SetSessionInfo(loSessionInfo);

        return retVal;
    }

    public async Task<List<string>> GetColumnNamesAsync()
    {
        List<string> columnNames = new List<string>();

        using (_dbConnection)
        {
            await _dbConnection.OpenAsync();

            string sql = $@"
            SELECT column_name
            FROM all_tab_columns
            WHERE table_name = 'GNT_EMAILQUEUE'";

            using (var command = new OracleCommand(sql, _dbConnection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string columnName = reader.GetString(0);
                        columnNames.Add(columnName);
                    }
                }
            }


        }

        return columnNames;
    }























    public void Close()
    {
        _dbConnection.Close();
    }

    public OracleTransaction BeginTransaction()
    {
        return _dbConnection.BeginTransaction();
    }

    static void AddParams(OracleCommand cmd, OracleCommand qry)
    {
        if (qry.Parameters != null)
        {
            foreach (OracleParameter param in qry.Parameters)
            {
                OracleParameter sqlParam = new OracleParameter
                {
                    Direction = param.Direction,
                    DbType = param.DbType
                };

                if (sqlParam.Direction == ParameterDirection.Output || sqlParam.Direction == ParameterDirection.InputOutput)
                {
                    sqlParam.Size = param.Size;
                }



                if (param.DbType == DbType.Object)
                {
                    sqlParam.OracleDbType = param.OracleDbType;
                }

                sqlParam.ParameterName = param.ParameterName;
                sqlParam.Value = param.Value;
                cmd.Parameters.Add(sqlParam);
            }
        }
    }

    protected string LogVerbose(string method, OracleCommand qry, long duration = -1)
    {
        string lsDuration = "";
        if (duration > -1)
        {
            lsDuration = " Duration (ms): " + duration + (duration > 1000 ? " ***" : "");
        }
        string lsParams = "";
        if (qry.Parameters.Count > 0)
        {
            lsParams += " Params: ";
            foreach (OracleParameter param in qry.Parameters)
            {
                lsParams += "[" + param.ParameterName + "," + param.Value + "] ";
            }
        }
        return method + " " + lsDuration + " " + qry.CommandText + lsParams;
    }

    public IDataReader GetReader(OracleCommand qry)
    {
        System.Diagnostics.Stopwatch? loSW = null;
        loSW = new System.Diagnostics.Stopwatch();
        loSW.Start();

        IDataReader rdr;

        using (OracleCommand cmd = new OracleCommand(qry.CommandText))
        {

            cmd.CommandType = qry.CommandType;

            AddParams(cmd, qry);
            cmd.Connection = _dbConnection;

            try
            {
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (OracleException ex)
            {
                throw new Exception("", ex);
            }
            finally
            {
                loSW.Stop();
            }
        }

        return rdr;
    }

    public OracleDataSet GetDataSet(OracleCommand qry)
    {
        System.Diagnostics.Stopwatch? loSW = null;
        loSW = new System.Diagnostics.Stopwatch();
        loSW.Start();

        OracleDataSet ds = new OracleDataSet();
        OracleCommand cmd = new OracleCommand(qry.CommandText)
        {

            CommandType = qry.CommandType
        };
        OracleDataAdapter da = new OracleDataAdapter(cmd);
        using (_dbConnection)
        {
            cmd.Connection = _dbConnection;
            AddParams(cmd, qry);
            try
            {
                da.Fill(ds);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("ORA-04068") || e.Message.Contains("ORA-04061") || e.Message.Contains("ORA-04065"))
                {
                    da.Fill(ds);
                }
            }

            cmd.Dispose();
            da.Dispose();

            loSW.Stop();

            return ds;
        }
    }

    public object ExecuteScalar(OracleCommand poQuery)
    {
        return ExecuteScalar(poQuery, null);
    }

    public object ExecuteScalar(OracleCommand poQuery, OracleTransaction? poTransaction)
    {
        System.Diagnostics.Stopwatch? loSW = null;
        loSW = new System.Diagnostics.Stopwatch();
        loSW.Start();

        using (_dbConnection)
        {
            object? loResult = null;
            poQuery.Connection = _dbConnection;
            if (poTransaction != null)
            {
                poQuery.Transaction = poTransaction;
            }
            loResult = poQuery.ExecuteScalar();







            foreach (OracleParameter param in poQuery.Parameters)
            {
                if (param.Direction == ParameterDirection.Output)
                {
                    loResult = param.Value;
                    break;
                }
            }
            loSW.Stop();

            return loResult;
        }
    }

    public int ExecuteQuery(OracleCommand poQuery)
    {
        return ExecuteQuery(poQuery, null);
    }

    public int ExecuteQuery(OracleCommand poQuery, OracleTransaction? poTransaction)
    {
        System.Diagnostics.Stopwatch? loSW = null;
        loSW = new System.Diagnostics.Stopwatch();
        loSW.Start();

        using (_dbConnection)
        {
            if (poTransaction != null)
            {
                poQuery.Transaction = poTransaction;
            }

            poQuery.Connection = _dbConnection;



            int result = -1;
            try
            {
                result = poQuery.ExecuteNonQuery();
            }
            catch (Exception)
            {

                loSW.Stop();

                throw;
            }
            loSW.Stop();

            return result;
        }
    }
}