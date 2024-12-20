
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Data.SqlClient;



namespace NLP
{
    class DAL : IDisposable
    {

        #region Private fields
        // IDisposable related variables/members
        private bool disposed = false;

        private string dbconnectionstring;


        private SqlConnection sqlDbConnection = null;
        #endregion

        #region Properties
        public string DBConnectionString
        {
            get { return dbconnectionstring; }
            set
            {
                dbconnectionstring = value;
                sqlDbConnection.ConnectionString = dbconnectionstring;
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="SqlConnection"/> object.
        /// </summary>
        /// <value>The <see cref="SqlConnection"/> object.</value>
        protected virtual SqlConnection Connection
        {
            get
            {
                return sqlDbConnection;
            }
        }

        #endregion

        #region Ctors
        public DAL()
        {
            sqlDbConnection = new SqlConnection();

        }

        public DAL(string dbConnectionString) // dbConnectionString in Decrypted format
        {
            sqlDbConnection = new SqlConnection();
            this.DBConnectionString = dbConnectionString;
        }

        #endregion

        #region Private Methods
        protected virtual bool OpenConnection()
        {
            try
            {
                if (sqlDbConnection == null)
                {
                    sqlDbConnection = new SqlConnection();
                    sqlDbConnection.ConnectionString = dbconnectionstring;
                }
                if (sqlDbConnection.State != ConnectionState.Open)
                {
                    sqlDbConnection.Open();
                }
            }
            catch
            {
                throw;
            }
            return true;
        }

        protected virtual bool CloseConnection()
        {
            try
            {
                if (sqlDbConnection != null)
                {
                    if (sqlDbConnection.State != ConnectionState.Closed)
                    {
                        sqlDbConnection.Close();
                    }

                }
            }
            catch
            {
                throw;
            }
            return true;

        }

        protected virtual bool DisposeConnection()
        {
            try
            {
                if (sqlDbConnection != null)
                {
                    //Calling Dispose also calls SqlConnection.Close.

                    sqlDbConnection.Dispose();
                }
            }
            catch
            {
                throw;
            }
            return true;
        }

        #endregion

        #region Public methods
        public virtual void ExecuteNonQuery(string sqlProcName, IEnumerable<SqlParameter> sqlProcParams, bool sqlConnKeepalive)
        {
            string datacommand = "";
            SqlCommand cmdNonQuery = null;

            datacommand = sqlProcName;

            try
            {
                OpenConnection();
                cmdNonQuery = sqlDbConnection.CreateCommand();

                cmdNonQuery.CommandText = datacommand;
                cmdNonQuery.CommandType = CommandType.StoredProcedure;

                if (sqlProcParams != null)
                {
                    foreach (SqlParameter param in sqlProcParams)
                    {
                        cmdNonQuery.Parameters.Add(param);
                    }
                }

                cmdNonQuery.ExecuteNonQuery();

                // If client deos not want to keep connection alive, close connection
                if (sqlConnKeepalive == false)
                    CloseConnection();

            }
            catch
            {
                // In case of an exception, close the connection
                CloseConnection();
                throw;
            }
            finally
            {
                if (cmdNonQuery != null)
                    cmdNonQuery.Dispose();


            }


        }



        public virtual object ExecuteScalar(string sqlProcName, IEnumerable<SqlParameter> sqlProcParams, bool sqlConnKeepalive)
        {
            return ExecuteScalar(sqlProcName, sqlProcParams, sqlConnKeepalive, CommandType.StoredProcedure);
        }

        public virtual object ExecuteScalar(string sqlProcName, IEnumerable<SqlParameter> sqlProcParams, bool sqlConnKeepalive, CommandType commandType)
        {

            string datacommand = "";
            SqlCommand cmdScalarReturnQuery = null;
            object retScalarValue = null;

            datacommand = sqlProcName;


            try
            {

                OpenConnection();
                cmdScalarReturnQuery = sqlDbConnection.CreateCommand();

                cmdScalarReturnQuery.CommandText = datacommand;
                cmdScalarReturnQuery.CommandType = commandType;

                if (sqlProcParams != null)
                {
                    foreach (SqlParameter param in sqlProcParams)
                    {
                        cmdScalarReturnQuery.Parameters.Add(param);
                    }
                }

                retScalarValue = cmdScalarReturnQuery.ExecuteScalar();

                // If client deos not want to keep connection alive, close connection
                if (sqlConnKeepalive == false)
                    CloseConnection();

                return retScalarValue;

            }
            catch
            {
                // In case of an exception, close the connection
                CloseConnection();
                throw;
            }
            finally
            {
                if (cmdScalarReturnQuery != null)
                    cmdScalarReturnQuery.Dispose();


            }


        }
        public virtual void ExecuteNonQueryInsert(string sql, bool sqlConnKeepalive)
        {

            OpenConnection();
            
            
            
            SqlCommand cmdQuery = sqlDbConnection.CreateCommand();
            cmdQuery.CommandText = sql;
            int result = cmdQuery.ExecuteNonQuery();

            if (sqlConnKeepalive == false)
                CloseConnection();

        }

        public virtual DataSet ExecuteSQL(string sql, bool sqlConnKeepalive)
        {
            string datacommand = "";
            SqlCommand cmdReaderQuery = null;
            SqlDataReader retSqlReader = null;
            DataSet dsSqlReaderDataSet = new DataSet();
            DataTable dtSqlReaderDataSetTable = new DataTable();
            DataReaderAdapter daDataReaderToDataSet = new DataReaderAdapter();

            datacommand = sql;

            try
            {

                OpenConnection();
                cmdReaderQuery = sqlDbConnection.CreateCommand();
                cmdReaderQuery.CommandText = datacommand;
                cmdReaderQuery.CommandType = CommandType.Text;
                cmdReaderQuery.CommandTimeout = 360;                    //kept timeing out on iris


                daDataReaderToDataSet.SelectCommand = cmdReaderQuery;
                daDataReaderToDataSet.Fill(dsSqlReaderDataSet);

                // If client deos not want to keep connection alive, close connection
                if (sqlConnKeepalive == false)
                    CloseConnection();

                return dsSqlReaderDataSet;

            }
            catch
            {
                // In case of an exception, close the connection
                CloseConnection();
                throw;
            }
            finally
            {
                if (cmdReaderQuery != null)
                    cmdReaderQuery.Dispose();
            }
        }

        public virtual DataSet ExecuteSQLInsert(string sql, bool sqlConnKeepalive)
        {
            string datacommand = "";
            SqlCommand cmdReaderQuery = null;
            SqlDataReader retSqlReader = null;
            DataSet dsSqlReaderDataSet = new DataSet();
            DataTable dtSqlReaderDataSetTable = new DataTable();
            DataReaderAdapter daDataReaderToDataSet = new DataReaderAdapter();

            datacommand = sql;

            try
            {

                OpenConnection();
                cmdReaderQuery = sqlDbConnection.CreateCommand();
                cmdReaderQuery.CommandText = datacommand;
                cmdReaderQuery.CommandType = CommandType.Text;
                cmdReaderQuery.CommandTimeout = 360;                    //kept timeing out on iris


                daDataReaderToDataSet.SelectCommand = cmdReaderQuery;
                daDataReaderToDataSet.Fill(dsSqlReaderDataSet);

                // If client deos not want to keep connection alive, close connection
                if (sqlConnKeepalive == false)
                    CloseConnection();

                return dsSqlReaderDataSet;

            }
            catch
            {
                // In case of an exception, close the connection
                CloseConnection();
                throw;
            }
            finally
            {
                if (cmdReaderQuery != null)
                    cmdReaderQuery.Dispose();
            }
        }
        public virtual XmlReader ExecuteXmlReader(string sqlProcName, IEnumerable<SqlParameter> sqlProcParams, bool sqlConnKeepalive)
        {

            string datacommand = "";
            SqlCommand cmdXmlReaderQuery = null;
            XmlReader retXmlReader = null;

            datacommand = sqlProcName;


            try
            {

                OpenConnection();
                cmdXmlReaderQuery = sqlDbConnection.CreateCommand();

                cmdXmlReaderQuery.CommandText = datacommand;
                cmdXmlReaderQuery.CommandType = CommandType.StoredProcedure;

                if (sqlProcParams != null)
                {
                    foreach (SqlParameter param in sqlProcParams)
                    {
                        cmdXmlReaderQuery.Parameters.Add(param);
                    }
                }

                retXmlReader = cmdXmlReaderQuery.ExecuteXmlReader();


                // If client deos not want to keep connection alive, close connection
                if (sqlConnKeepalive == false)
                    CloseConnection();

                return retXmlReader;

            }
            catch
            {
                // In case of an exception, close the connection
                CloseConnection();
                throw;
            }
            finally
            {
                if (cmdXmlReaderQuery != null)
                    cmdXmlReaderQuery.Dispose();


            }


        }

        public virtual DataSet ExecuteQuery(string sqlProcName, IEnumerable<SqlParameter> sqlProcParams, bool sqlConnKeepalive, CommandType commandType)
        {

            string datacommand = "";
            SqlCommand cmdReaderQuery = null;
            SqlDataReader retSqlReader = null;
            DataSet dsSqlReaderDataSet = new DataSet();
            DataTable dtSqlReaderDataSetTable = new DataTable();
            DataReaderAdapter daDataReaderToDataSet = new DataReaderAdapter();

            datacommand = sqlProcName;


            try
            {

                OpenConnection();
                cmdReaderQuery = sqlDbConnection.CreateCommand();

                cmdReaderQuery.CommandText = datacommand;
                cmdReaderQuery.CommandType = commandType;

                if (sqlProcParams != null)
                {
                    foreach (SqlParameter param in sqlProcParams)
                    {
                        cmdReaderQuery.Parameters.Add(param);
                    }
                }


                //retSqlReader = cmdReaderQuery.ExecuteReader();
                daDataReaderToDataSet.SelectCommand = cmdReaderQuery;
                daDataReaderToDataSet.Fill(dsSqlReaderDataSet);

                //daDataReaderToDataSet.FillFromReader(dtSqlReaderDataSetTable, retSqlReader);
                //dsSqlReaderDataSet.Tables.Add(dtSqlReaderDataSetTable);




                // If client deos not want to keep connection alive, close connection
                if (sqlConnKeepalive == false)
                    CloseConnection();

                return dsSqlReaderDataSet;

            }
            catch
            {
                // In case of an exception, close the connection
                CloseConnection();
                throw;
            }
            finally
            {
                if (cmdReaderQuery != null)
                    cmdReaderQuery.Dispose();


            }


        }

        public virtual DataSet ExecuteQuery(string sqlProcName, IEnumerable<SqlParameter> sqlProcParams, bool sqlConnKeepalive)
        {
            return ExecuteQuery(sqlProcName, sqlProcParams, sqlConnKeepalive, CommandType.StoredProcedure);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                Debug.WriteLine(string.Format("Started Disposing an instance of {0} having HashCode {1}", GetType().FullName, this.GetHashCode()));

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    //component.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                DisposeConnection();

                // Note disposing has been done.
                disposed = true;
                Debug.WriteLine(string.Format("Finished Disposing an instance of {0} having HashCode {1}", GetType().FullName, this.GetHashCode()));
            }
        }



        #endregion

        #region DataReaderAdapter class
        /// <summary>
        /// Custom Adapter built mainly to create disconnected dataset
        /// from connected datareader object
        /// </summary>
        public class DataReaderAdapter : DbDataAdapter
        {
            public int FillFromReader(DataTable dataTable, IDataReader dataReader)
            {
                return this.Fill(dataTable, dataReader);
            }
        }
        #endregion

    }
}
