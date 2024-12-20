
using System.Data;
using System.Data.SqlClient;
namespace NLP
{
    public static class DbHelper
    {
        private static string getExceptionString(string methodName, string dbName, string command,
           CommandType commandType, string[] tableNames, string message, params SqlParameter[] param)
        {
            string returnString = string.Empty;

            try
            {
                returnString = string.Format(
                    "Error in Method: {0}, Database: {1}, Command: {2}, CommandType: {3} \r\n",
                    methodName, dbName, command, commandType.ToString());

                if (tableNames != null)
                {
                    foreach (string s in tableNames)
                    {
                        returnString = returnString + "Table: " + s + "\r\n";
                    }
                }

                if (param != null)
                {
                    foreach (SqlParameter sp in param)
                    {
                        returnString = returnString + "Parameter: " + sp.ParameterName
                            + ", Direction: " + System.Enum.GetName(typeof(ParameterDirection), sp.Direction)
                            + ", Value: " + sp.Value.ToString() + "\r\n";
                    }
                }
            }
            catch
            {
            }

            returnString = returnString + message;

            return returnString;
        }

        public static SqlDataReader GetDataReader(string sproc, params SqlParameter[] param)
        {
            return DbHelper.ExecuteReaderCommand("", sproc, CommandType.StoredProcedure, param);
        }

        public static DataTable GetDataTable(string sproc, params SqlParameter[] param)
        {
            return DbHelper.ExecuteGetDataTable("", sproc, param);
        }
        public static SqlDataReader GetDataReader(string dbName, string sproc, params SqlParameter[] param)
        {
            return DbHelper.ExecuteReaderCommand(dbName, sproc, CommandType.StoredProcedure, param);
        }

        public static DataTable GetDataTable(string dbName, string sproc, params SqlParameter[] param)
        {
            return DbHelper.ExecuteGetDataTable(dbName, sproc, param);
        }
        //public static SqlDataReader ExecuteReaderCommand(string command, CommandType commandType, params SqlParameter[] param)
        //{
        //    return ExecuteReaderCommand("", command, commandType, param);
        //}
        public static SqlDataReader ExecuteReaderCommand(string dbName, string command, CommandType commandType, params SqlParameter[] param)
        {
            string connectionString = "";
            try
            {
                if (string.IsNullOrEmpty(dbName))
                    connectionString = DBHandle.GetConnectionString();
                else
                    connectionString = DBHandle.GetConnectionString(dbName);
                var conn = new SqlConnection(connectionString);

                return ExecuteReaderCommand(conn, command, commandType, param);
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteReaderCommand", dbName, command, commandType, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static SqlDataReader ExecuteReaderCommand(SqlConnection conn, string command, CommandType commandType, params SqlParameter[] param)
        {
            try
            {
                SqlCommand cmd = PrepareCommand(command, conn, commandType, param);
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);// this input parameter will help close connection when datareader closed           
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteReaderCommand", conn.ConnectionString, command, commandType, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }

        /// <summary>
        /// This will work for a stored procedure returning only one datatable
        /// </summary>
        /// <param name="dbName">database name from config file to connect and execute this sp</param>
        /// <param name="command">stored procedure name</param>
        /// <param name="param"></param>
        /// <returns></returns>
        //public static DataTable ExecuteGetDataTable(string command, params SqlParameter[] param)
        //{
        //    return ExecuteGetDataTable("", command, param);
        //}
        public static DataTable ExecuteGetDataTable(string dbName, string command, params SqlParameter[] param)
        {
            string connectionString = "";
            try
            {
                if (string.IsNullOrEmpty(dbName))
                    connectionString = DBHandle.GetConnectionString();
                else
                    connectionString = DBHandle.GetConnectionString(dbName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    return ExecuteGetDataTable(conn, command, param);
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteGetDataTable", dbName, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static DataTable ExecuteGetDataTable(SqlConnection conn, string command, params SqlParameter[] param)
        {
            try
            {
                using (SqlCommand cmd = PrepareCommand(command, conn, CommandType.StoredProcedure, param))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteGetDataTable", conn.ConnectionString, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static object ExecuteScalar(string command, CommandType commandType, params SqlParameter[] param)
        {
            return ExecuteScalar("", command, commandType, param);
        }
        public static object ExecuteScalarWithNoDataChange(string dbName, string command, CommandType commandType, params SqlParameter[] param)
        {
            string connectionString = "";
            try
            {
                if (string.IsNullOrEmpty(dbName))
                    connectionString = DBHandle.GetConnectionString();
                else
                    connectionString = DBHandle.GetConnectionString(dbName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    return ExecuteScalarWithNoDataChange(conn, command, commandType, param);
                }

            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteScalar", connectionString, command, commandType, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static object ExecuteScalar(string dbName, string command, CommandType commandType, params SqlParameter[] param)
        {
            string connectionString = "";
            try
            {
                if (string.IsNullOrEmpty(dbName))
                    connectionString = DBHandle.GetConnectionString();
                else
                    connectionString = DBHandle.GetConnectionString(dbName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    return ExecuteScalar(conn, command, commandType, param);
                }

            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteScalar", connectionString, command, commandType, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static object ExecuteScalar(SqlConnection conn, string command, CommandType commandType, params SqlParameter[] param)
        {
            try
            {
                SqlCommand cmd = PrepareCommand(command, conn, commandType, param);
                conn.Open();
                object returnObject = cmd.ExecuteScalar();
                return returnObject;
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteScalar", conn.ConnectionString, command, commandType, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static object ExecuteScalarWithNoDataChange(SqlConnection conn, string command, CommandType commandType, params SqlParameter[] param)
        {
            try
            {
                SqlCommand cmd = PrepareCommandWithNoDataChange(command, conn, commandType, param);
                conn.Open();
                object returnObject = cmd.ExecuteScalar();
                return returnObject;
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteScalar", conn.ConnectionString, command, commandType, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static void ExecuteSproc(string command, params SqlParameter[] param)
        {
            ExecuteSproc("", command, param);
        }
        /// <summary>
        /// Run the stored procedure without updating blank input parameter by DBNull.Value
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        public static void ExecuteSprocWithNoDataChange(string command, params SqlParameter[] param)
        {
            ExecuteSprocWithNoDataChange("", command, param);
        }
        /// <summary>
        /// Run the stored procedure without updating blank input parameter by DBNull.Value
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="command"></param>
        /// <param name="param"></param>
        public static void ExecuteSprocWithNoDataChange(string dbName, string command, params SqlParameter[] param)
        {
            string connectionString = "";
            if (string.IsNullOrEmpty(dbName))
                connectionString = DBHandle.GetConnectionString();
            else
                connectionString = DBHandle.GetConnectionString(dbName);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    ExecuteSprocWithNoDataChange(conn, command, param);
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteSproc", connectionString, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static void ExecuteSproc(string dbName, string command, params SqlParameter[] param)
        {
            string connectionString = "";
            if (string.IsNullOrEmpty(dbName))
                connectionString = DBHandle.GetConnectionString();
            else
                connectionString = DBHandle.GetConnectionString(dbName);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    ExecuteSproc(conn, command, param);
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteSproc", connectionString, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }

        public static void ExecuteSproc(SqlConnection conn, string command, params SqlParameter[] param)
        {
            try
            {
                SqlCommand cmd = PrepareCommand(command, conn, CommandType.StoredProcedure, param);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteSproc", conn.ConnectionString, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        public static void ExecuteSprocWithNoDataChange(SqlConnection conn, string command, params SqlParameter[] param)
        {
            try
            {
                SqlCommand cmd = PrepareCommandWithNoDataChange(command, conn, CommandType.StoredProcedure, param);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteSproc", conn.ConnectionString, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        /// <summary>
        /// Run the stored procedure without updating blank input parameter by DBNull.Value
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="command"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable ExecuteGetDataTableWithNoDataChange(string dbName, string command, params SqlParameter[] param)
        {
            string connectionString = "";
            try
            {
                if (string.IsNullOrEmpty(dbName))
                    connectionString = DBHandle.GetConnectionString();
                else
                    connectionString = DBHandle.GetConnectionString(dbName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    return ExecuteGetDataTableWithNoDataChange(conn, command, param);
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteGetDataTable", dbName, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }
        /// <summary>
        /// Run the stored procedure without updating blank input parameter by DBNull.Value
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="command"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable ExecuteGetDataTableWithNoDataChange(SqlConnection conn, string command, params SqlParameter[] param)
        {
            try
            {
                using (SqlCommand cmd = PrepareCommandWithNoDataChange(command, conn, CommandType.StoredProcedure, param))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteSproc", conn.ConnectionString, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
        }

        public static int ExecuteSprocBatchUsingAdapter(string command, DataTable source, int batchSize, params SqlParameter[] param)
        {
            return ExecuteSprocBatchUsingAdapter("", command, source, batchSize, param);
        }
        public static int ExecuteSprocBatchUsingAdapter(string dbName, string command, DataTable source, int batchSize, params SqlParameter[] param)
        {
            int updated = 0;
            try
            {
                string connectionString = "";
                if (string.IsNullOrEmpty(dbName))
                    connectionString = DBHandle.GetConnectionString();
                else
                    connectionString = DBHandle.GetConnectionString(dbName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = PrepareCommandWithNoDataChange(command, conn, CommandType.StoredProcedure, param);
                    cmd.UpdatedRowSource = UpdateRowSource.None;
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter);
                        adapter.UpdateBatchSize = batchSize;
                        adapter.UpdateCommand = cmd;
                        updated = adapter.Update(source);
                    }
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteSproc", dbName, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
            return updated;
        }

        public static int ExecuteSprocBatch(string command, DataTable source, int startIdx, params SqlParameter[] param)
        {
            return ExecuteSprocBatch("", command, source, startIdx, param);
        }
        public static int ExecuteSprocBatch(string dbName, string command, DataTable source, int startIdx, params SqlParameter[] param)
        {
            int updated = 0;
            try
            {
                string connectionString = "";
                if (string.IsNullOrEmpty(dbName))
                    connectionString = DBHandle.GetConnectionString();
                else
                    connectionString = DBHandle.GetConnectionString(dbName);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = PrepareCommandWithNoDataChange(command, conn, CommandType.StoredProcedure, param);
                    //cmd.UpdatedRowSource = UpdateRowSource.None;
                    conn.Open();
                    int j;
                    for (int i = 0; i < source.Rows.Count; i++)
                    {
                        for (j = startIdx; j < param.Count(); j++)
                        {
                            if (param[j].SqlDbType == SqlDbType.Int)
                            {
                                param[j].Value = Convert.ToInt32(source.Rows[i][param[j].SourceColumn]);
                            }
                            else if (param[j].SqlDbType == SqlDbType.DateTime)
                            {
                                if (source.Rows[i][param[j].SourceColumn] != DBNull.Value)
                                    param[j].Value = Convert.ToDateTime(source.Rows[i][param[j].SourceColumn]);
                                else
                                    param[j].Value = DBNull.Value;
                            }
                            else
                            {
                                if (source.Rows[i][param[j].SourceColumn] == DBNull.Value)
                                    param[j].Value = DBNull.Value;
                                else
                                    param[j].Value = source.Rows[i][param[j].SourceColumn].ToString();
                            }

                        }
                        if (cmd.ExecuteNonQuery() > 0)
                            updated++;
                    }
                }
            }
            catch (Exception ex)
            {
                string s = getExceptionString("ExecuteSproc", dbName, command, CommandType.StoredProcedure, null, ex.Message, param);
                throw new Exception(s, ex);
            }
            return updated;
        }
        public static void WriteBulk(string dbName, string destinationTable, DataTable source)
        {
            WriteBulk(dbName, destinationTable, source, false);
        }
        public static void WriteBulk(string dbName, string destinationTable, DataTable source, bool tableLock)
        {
            string connectionString = "";
            if (string.IsNullOrEmpty(dbName))
                connectionString = DBHandle.GetConnectionString();
            else
                connectionString = DBHandle.GetConnectionString(dbName);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // enable triggers

                SqlBulkCopy bulkCopy;
                if (tableLock)
                {
                    bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);
                }
                else
                {
                    bulkCopy = new SqlBulkCopy(connection);
                }

                //ctx.BulkInsert(list, operation =>
                //{
                //    operation.DataTableBatchSize = 100000;
                //    operation.BatchSize = 10000;

                //});

                // set the destination table name

                bulkCopy.DestinationTableName = destinationTable;

                connection.Open();

                // write the data in destination table

                bulkCopy.WriteToServer(source);

                connection.Close();
            }
        }
        public static void WriteBulk(string destinationTable, DataTable source)
        {
            WriteBulk(destinationTable, source, false);
        }
        public static void WriteBulk(string destinationTable, DataTable source, bool tableLock)
        {
            WriteBulk("", destinationTable, source, tableLock);
        }
        public static SqlParameterCollection GetParams(string dbName, string command)
        {
            try
            {
                string connectionString;
                if (string.IsNullOrEmpty(dbName))
                {
                    connectionString = DBHandle.GetConnectionString();
                }
                else
                {
                    connectionString = DBHandle.GetConnectionString(dbName);
                }
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(command, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlCommandBuilder.DeriveParameters(cmd);
                    return cmd.Parameters;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Error while discovering parameters with connection string: ", dbName, ", sql statement/ stored procedure name:", command), ex);
            }
        }
        public static SqlParameterCollection GetParams(string command)
        {
            return GetParams("", command);
        }
        /// <summary>
        /// string[3]: string[0] parameter name, string[1] parameter type, string[2] parameter direction
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static List<string[]> GetInputParams(string command)
        {
            List<string[]> param = new List<string[]>();
            SqlParameterCollection collection = DbHelper.GetParams(command);
            foreach (SqlParameter p in collection)
            {
                if (p.Direction == ParameterDirection.Input)
                {
                    param.Add(new string[] { p.ParameterName, p.SqlDbType.ToString() });
                }
            }
            return param;
        }
        /// <summary>
        /// string[3]: string[0] parameter name, string[1] parameter type, string[2] parameter direction
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static List<string[]> GetInputParams(string dbName, string command)
        {
            List<string[]> param = new List<string[]>();
            SqlParameterCollection collection = DbHelper.GetParams(dbName, command);
            foreach (SqlParameter p in collection)
            {
                if (p.Direction == ParameterDirection.Input)
                {
                    param.Add(new string[] { p.ParameterName, p.SqlDbType.ToString(), p.Direction.ToString() });
                }
            }
            return param;
        }

        private static SqlCommand PrepareCommand(string command, SqlConnection conn, CommandType commandType, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand(command, conn);
            cmd.CommandType = commandType;
            cmd.CommandTimeout = 0;
            if (param != null && param.Any())
            {
                foreach (SqlParameter Param in param)
                {
                    if (Param.Value == null)
                        Param.Value = DBNull.Value;
                    cmd.Parameters.Add(Param);
                }
            }
            return cmd;
        }
        private static SqlCommand PrepareCommandWithNoDataChange(string command, SqlConnection conn, CommandType commandType, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand(command, conn);
            cmd.CommandType = commandType;
            cmd.CommandTimeout = 0;
            if (param != null && param.Any())
            {
                foreach (SqlParameter Param in param)
                {
                    cmd.Parameters.Add(Param);
                }
            }
            return cmd;
        }
        public static SqlParameter GetParameter(string paramName, ParameterDirection paramDirection, DbType dbType, object value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.Direction = paramDirection;
            param.DbType = dbType;
            param.Value = value;
            return param;
        }

        public static SqlParameter GetParameter(string paramName, ParameterDirection paramDirection, DbType dbType)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.Direction = paramDirection;
            param.DbType = dbType;
            return param;
        }
        public static SqlParameter GetParameter(string paramName, ParameterDirection paramDirection, SqlDbType dbType, object value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.Direction = paramDirection;
            param.SqlDbType = dbType;
            param.Value = value;
            return param;
        }
        /// <summary>
        /// This is use when there is table-valued type in stored procedure
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramDirection"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <param name="typeName">The type name of the specified table-valued parameter</param>
        /// <returns></returns>
        public static SqlParameter GetParameter(string paramName, ParameterDirection paramDirection, SqlDbType dbType, string typeName, object value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.Direction = paramDirection;
            param.SqlDbType = dbType;
            param.Value = value;
            param.TypeName = typeName;
            return param;
        }

        public static SqlParameter GetParameter(string paramName, ParameterDirection paramDirection, SqlDbType dbType, int? parameterSize, object value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.Direction = paramDirection;
            param.SqlDbType = dbType;
            param.Size = parameterSize == null ? param.Size : parameterSize.Value;
            param.Value = value;
            return param;
        }
        public static SqlParameter GetParameter(string paramName, DbType dbType, string sourceColumn, int? parameterSize)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.SourceColumn = sourceColumn;
            param.DbType = dbType;
            param.Size = parameterSize == null ? param.Size : parameterSize.Value;
            return param;
        }
        public static SqlParameter GetParameter(string paramName, SqlDbType dbType, string sourceColumn, int? parameterSize)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            param.SourceColumn = sourceColumn;
            param.SqlDbType = dbType;
            param.Size = parameterSize == null ? param.Size : parameterSize.Value;
            return param;
        }
        public static DataTable CreateTableType(IEnumerable<int> Ids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            foreach (int id in Ids)
            {
                DataRow dr = dt.NewRow();
                dr[0] = id;
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static DataTable CreateTableType(IEnumerable<string> items)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Item", typeof(string));
            foreach (string item in items)
            {
                DataRow dr = dt.NewRow();
                dr[0] = item;
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #region extension methods
        public static R Single<R>(this SqlDataReader reader, Func<SqlDataReader, R> selector)
        {
            R result = default(R);
            if (reader.Read())
                result = selector(reader);
            if (reader.Read())// more than 1 result return
            {
                // throw exception
                throw new DataException("Multiple rows returned from query");
            }
            return result;
        }
        public static IEnumerable<R> Multiple<R>(this SqlDataReader reader, Func<SqlDataReader, R> selector)
        {
            List<R> list = new List<R>();
            while (reader.Read())
            {
                R result = default(R);
                result = selector(reader);
                list.Add(result);
            }
            return list;
        }

        #endregion extension methods
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable ConvertTo<T>(IEnumerable<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable ConvertTo<T>(IEnumerable<T> list, string[] columnNames)
        {
            DataTable table = CreateTable<T>(columnNames);
            Type entityType = typeof(T);
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                {
                    if (columnNames.Contains(prop.Name))
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(entityType);

            foreach (System.ComponentModel.PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
                prop.PropertyType) ?? prop.PropertyType);
            }

            return table;
        }
        public static DataTable CreateTable<T>(string[] columnNames)
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            System.ComponentModel.PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(entityType);

            foreach (System.ComponentModel.PropertyDescriptor prop in properties)
            {
                if (columnNames.Contains(prop.Name))
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
                    prop.PropertyType) ?? prop.PropertyType);
                }
            }

            return table;
        }
    }
}

