using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif


namespace AaTools.DataLayer
{
    /// <summary>
    /// Class for all database calls.  This class is abstract and should not be instantiated.  It is used to provide common functionality for database calls.
    /// </summary>
    public abstract class AaDataAccessLayer
    {
        

        /// <summary>
        /// Return the correct connection string based on the connection name. First check the environment variable, then the web.config
        /// </summary>
        /// <param name="connectionName">Name of either the environment variable or the web config connection</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns>Connection string if found. If not found, empty string</returns>
        private static string DbConnStr(string connectionName, bool logDebug=false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("Getting {ConnectionName} Connection string", connectionName);
            try
            {
                if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable(connectionName)))
                    return Environment.GetEnvironmentVariable(connectionName);
                else
#if NET8_0_OR_GREATER

                {
                    var configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

                    if (configuration.GetConnectionString(connectionName) != null)
                    {
                        return configuration.GetConnectionString(connectionName);
                    }
                }

#else
               if (ConfigurationManager.ConnectionStrings[connectionName] != null)
                {
                    return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
                }
#endif
            }
            catch (Exception ex)
            {
                AaTools.AaSerilog.GetInstance().Error(ex, "Error getting {ConnectionName} Connection string", connectionName);
            }
            return string.Empty;
        }

        /// <summary>
        /// Constants structure to store misc constants of different data types
        /// </summary>
        private struct Constants
        {
            /// <summary>
            /// max byte length for varchar and nvarchar.  This is not exactly the number of characters.  Some foreign unicode characters take up to 4 bytes.
            /// </summary>
            public const int VARCHAR_MAX = 8000;
            /// <summary>
            /// max byte length for varchar and nvarchar.  This is not exactly the number of characters.  Some foreign unicode characters take up to 4 bytes.
            /// </summary>
            public const int NVARCHAR_MAX = 4000;
        }

        /// <summary>
        /// Returns the constructor method for database calls
        /// </summary>
        protected AaDataAccessLayer() { }

        #region "Async DB Calls"

        /// <summary>
        /// Return a database table asynchronously
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static async Task<DataTable> ExecuteDatatableAsync(string storedProcedureName, ParameterBuilder paramBuilder, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteDatatableAsync {storedProcedureName}", storedProcedureName);
            return await ExecuteDatatableAsync_Internal(storedProcedureName, paramBuilder, db, CommandTimeout, throwError).ConfigureAwait(false);
        }

        /// <summary>
        /// Internal procedure for async datatable call
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        private static async Task<DataTable> ExecuteDatatableAsync_Internal(string storedProcedureName, ParameterBuilder paramBuilder, string db, int CommandTimeout, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteDatatableAsync_Internal {storedProcedureName}", storedProcedureName);
            DataTable dt = new DataTable();
            try
            {
                using (var newConnection = new SqlConnection(DbConnStr(db)))
                {
                    using (var myCommand = new SqlCommand(storedProcedureName, newConnection))
                    {
                        SqlDataReader reader = null;

                        myCommand.CommandType = CommandType.StoredProcedure;
                        if (paramBuilder != null) { myCommand.Parameters.AddRange(paramBuilder.Params); }
                        if (CommandTimeout > 0) { myCommand.CommandTimeout = CommandTimeout; }

                        await newConnection.OpenAsync().ConfigureAwait(false);
                        reader = await myCommand.ExecuteReaderAsync().ConfigureAwait(false);

                        if (reader != null)
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteDatatableAsync_Internal {storedProcedureName}", storedProcedureName);
                if (throwError)
                {
                    throw;
                }
            }

            return dt;
        }
        #endregion

        #region Json

        /// <summary>
        /// Asynchronously call stored procedure that returns Json Object T
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="commandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns>Task type T</returns>
        public static async Task<T> ExecuteJsonObjectAsync<T>(string storedProcedureName, ParameterBuilder paramBuilder, string db, int commandTimeout = -1, bool throwError = false, bool logDebug = false) where T : class
        {

            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteJsonObjectAsync<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);
            T result = null;
            try
            {
               string json = await ExecuteJsonStringAsync(storedProcedureName, paramBuilder, db, commandTimeout).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    result = AaDataLayerHelper.DeserializeJSON<T>(json);
                }
            }
            catch (Exception ex)
            {
                AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteJsonObjectAsync<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);
                if (throwError)
                {
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Asynchronously call stored procedure that returns Json as String
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="commandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static async Task<string> ExecuteJsonStringAsync(string storedProcedureName, ParameterBuilder paramBuilder, string db, int commandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteJsonStringAsync {storedProcedureName}", storedProcedureName);

            string result = null;
            StringBuilder jsonResult = new StringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
                {
                    SqlCommand command = new SqlCommand(storedProcedureName, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    if (commandTimeout > 0) { command.CommandTimeout = commandTimeout; }

                    if (paramBuilder != null)
                    {
                        command.Parameters.AddRange(paramBuilder.Params);
                    }

                    // Microsofts method to return Json from SQL to Client
                    // https://docs.microsoft.com/en-us/sql/relational-databases/json/use-for-json-output-in-sql-server-and-in-client-apps-sql-server?view=sql-server-ver15#use-for-json-output-in-a-c-client-app

                    await connection.OpenAsync().ConfigureAwait(false);
                    var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                    if (!reader.HasRows)
                    {
                        jsonResult.Append("[]");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            jsonResult.Append(reader.GetValue(0).ToString());
                        }
                    }

                    result = jsonResult.ToString();
                }
            }
            catch (Exception ex)
            {
                AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteJsonStringAsync {storedProcedureName}", storedProcedureName);
                if (throwError)
                {
                    throw;
                }

            }

            return result;
        }


        /// <summary>
        /// Return Json Object when stored procedure uses output parameter
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="OutputParamName">Name of the output parameter with the Json</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static T ExecuteObjectJson<T>(string storedProcedureName, ParameterBuilder paramBuilder, string OutputParamName, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false) where T : class
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteObjectJson<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

            T returnValue = default(T);

            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (CommandTimeout > 0) { command.CommandTimeout = CommandTimeout; }
                if (paramBuilder != null)
                {
                    command.Parameters.AddRange(paramBuilder.Params);
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    if (command != null && command.Parameters != null && command.Parameters[OutputParamName] != null)
                    {
                        try
                        {
                           string jsonResult = (string)Convert.ChangeType(command.Parameters[OutputParamName].Value, typeof(string));
                            return AaDataLayerHelper.DeserializeJSON<T>(jsonResult);
                        }
                        catch (Exception ex)
                        {
                            AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteObjectJson<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);
                            if (throwError) { throw; }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteObjectJson<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);
                    if (throwError) { throw; }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Asynchronouslyl Return Json Object when stored procedure uses output parameter
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="storedProcedureName"></param>
        /// <param name="paramBuilder"></param>
        /// <param name="OutputParamName"></param>
        /// <param name="db"></param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static async Task<T> ExecuteObjectJsonAsync<T>(string storedProcedureName, ParameterBuilder paramBuilder, string OutputParamName, string db, bool logDebug = false) where T : class
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteObjectJsonAsync<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

            T returnValue = default(T);
            
            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                if (paramBuilder != null)
                {
                    command.Parameters.AddRange(paramBuilder.Params);
                }

                try
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    if (command != null && command.Parameters != null && command.Parameters[OutputParamName] != null)
                    {
                        try
                        {
                            string jsonResult = (string)Convert.ChangeType(command.Parameters[OutputParamName].Value, typeof(string));
                            returnValue = AaDataLayerHelper.DeserializeJSON<T>(jsonResult);
                        }
                        catch (Exception ex)
                        {
                            AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteObjectJsonAsync<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

                        }
                    }
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteObjectJsonAsync<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

                }

                return returnValue;
            }
        }

        #endregion


        #region Standard Sql Connections
        /// <summary>
        /// Call a stored procedure and return a DataTable
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static DataTable ExecuteDatatable(string storedProcedureName, ParameterBuilder paramBuilder, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteDatatable {storedProcedureName}", storedProcedureName);

            DataSet dataset = new DataSet();

            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (CommandTimeout > 0) { command.CommandTimeout = CommandTimeout; }

                //Add any params
                if (paramBuilder != null) { command.Parameters.AddRange(paramBuilder.Params); }

                try
                {
                    // open the connection (closed when 'using' goes out of scope)
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataset);
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteDatatable {storedProcedureName}", storedProcedureName);
                    if (throwError)
                    {
                        throw;
                    }
                }
            }

            //Pull the table data
            DataTable dt = null;
            if (dataset != null && dataset.Tables.Count > 0) { dt = dataset.Tables[0]; }

            return dt;
        }

        /// <summary>
        /// Call a stored procedure and return a DataSet
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string storedProcedureName, ParameterBuilder paramBuilder, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteDataset {storedProcedureName}", storedProcedureName);

            DataSet dataset = new DataSet();

            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (CommandTimeout > 0) { command.CommandTimeout = CommandTimeout; }

                //Add any params
                if (paramBuilder != null) { command.Parameters.AddRange(paramBuilder.Params); }

                try
                {
                    // open the connection (closed when 'using' goes out of scope)
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataset);
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteDataset {storedProcedureName}", storedProcedureName);

                    if (throwError)
                    {
                        throw;
                    }
                }
            }

            //Pull the table data
            return dataset;
        }

        /// <summary>
        /// Call a stored procedure that returns no data
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string storedProcedureName, ParameterBuilder paramBuilder, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteNonQuery {storedProcedureName}", storedProcedureName);
            int result = 0;

            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                if (CommandTimeout > 0) { command.CommandTimeout = CommandTimeout; }
                command.CommandType = CommandType.StoredProcedure;

                if (paramBuilder != null)
                {
                    command.Parameters.AddRange(paramBuilder.Params);
                }

                try
                {
                    // open the connection (closed when 'using' goes out of scope)
                    connection.Open();
                    result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteNonQuery {storedProcedureName}", storedProcedureName);

                    if (throwError)
                    {
                        throw;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Asynchronously call a stored procedure that returns no data
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        public static void ExecuteNonQueryAsync(string storedProcedureName, ParameterBuilder paramBuilder, string db, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteNonQueryAsync {storedProcedureName}", storedProcedureName);

            SqlConnectionStringBuilder connectionBuilder =
             new SqlConnectionStringBuilder(DbConnStr(db))
             {
                 ConnectTimeout = 4000
             };

            SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString);
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);

                command.CommandType = CommandType.StoredProcedure;

                if (paramBuilder != null)
                {
                    command.Parameters.AddRange(paramBuilder.Params);
                }

                try
                {
                    // open the connection (closed when 'using' goes out of scope)
                    connection.Open();

                    command.BeginExecuteNonQuery(new AsyncCallback(EndExecuteNonQueryAsync), command);
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteNonQueryAsync {storedProcedureName}", storedProcedureName);
                }
            }
        }

        /// <summary>
        /// End of the ExecuteNonQueryAsync
        /// </summary>
        /// <param name="result"></param>
        public static void EndExecuteNonQueryAsync(IAsyncResult result)
        {

            SqlCommand command = null;
            try
            {
                command = (SqlCommand)result.AsyncState;
                command.EndExecuteNonQuery(result);
            }
            catch (Exception ex)
            {
                AaTools.AaSerilog.GetInstance().Error(ex, "Error in EndExecuteNonQueryAsync {storedProcedureName}", command.CommandText);
            }
            finally
            {
                command.Connection.Close();
                command.Dispose();
            }
        }

        /// <summary>
        /// Execute a stored procedure that returns a scalar object T
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string storedProcedureName, ParameterBuilder paramBuilder, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteScalar<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

            T value = default(T);

            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (CommandTimeout > 0) { command.CommandTimeout = CommandTimeout; }

                if (paramBuilder != null)
                {
                    command.Parameters.AddRange(paramBuilder.Params);
                }

                try
                {
                    connection.Open();
                    value = (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteScalar<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);
                    if (throwError) { throw; }
                }
            }

            return value;
        }

        /// <summary>
        /// Execute a stored procedure with a return parameter of a simple object type
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="OutputParamName"></param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static T ExecuteWithReturnParam<T>(string storedProcedureName, ParameterBuilder paramBuilder, string OutputParamName, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteWithReturnParam<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

            T value = default(T);

            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (CommandTimeout > 0) { command.CommandTimeout = CommandTimeout; }
                if (paramBuilder != null)
                {
                    command.Parameters.AddRange(paramBuilder.Params);
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    if (command != null && command.Parameters != null && command.Parameters[OutputParamName] != null)
                    {
                        try
                        {
                            value = (T)Convert.ChangeType(command.Parameters[OutputParamName].Value, typeof(T));
                        }
                        catch (Exception ex)
                        {
                            AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteWithReturnParam<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

                            if (throwError) { throw; }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteWithReturnParam<T> {typeof(T)} {storedProcedureName}", typeof(T), storedProcedureName);

                    if (throwError) { throw; }

                }
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedrue to be called</param>
        /// <param name="paramBuilder">ParamBuilder object with parameters to be passed to the stored procedure</param>
        /// <param name="db">Name of the database connection string, will check environmental variables first and then config files</param>
        /// <param name="CommandTimeout">Optional number of seconds before stored procedure times out. Default if not provided or passed as -1 is 30 seconds</param>
        /// <param name="throwError">When this is not passed in or passed in as false, any error will be logged and ignored. If set to true then it will throw the error back to the calling code.</param>
        /// <param name="logDebug">When true, will log debug messages</param>
        /// <returns></returns>
        public static SqlParameterCollection ExecuteWithParamReturns(string storedProcedureName, ParameterBuilder paramBuilder, string db, int CommandTimeout = -1, bool throwError = false, bool logDebug = false)
        {
            if (logDebug) AaTools.AaSerilog.GetInstance().Debug("ExecuteWithParamReturns {storedProcedureName}", storedProcedureName);
            using (SqlConnection connection = new SqlConnection(DbConnStr(db)))
            {
                //setup the SQL Command Object
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (CommandTimeout > 0) { command.CommandTimeout = CommandTimeout; }

                //Add any params
                if (paramBuilder != null) { command.Parameters.AddRange(paramBuilder.Params); }

                // open the connection and pull the data (closed when 'using' goes out of scope)
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    AaTools.AaSerilog.GetInstance().Error(ex, "Error in ExecuteWithParamReturns {storedProcedureName}", storedProcedureName);
                    if (throwError) { throw; }

                }
                return command.Parameters;
            }
        }

        #endregion


    }
}