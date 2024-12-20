 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace NLP
{
    public class DBHandle
    {
        public DBHandle()
        {
        }

        public static Int32 GetDBInt(SqlDataReader objReader, Int32 iField)
        {
            int iResult = 0;

            if (objReader.FieldCount > iField)
            {
                iResult = objReader.IsDBNull(iField) ? 0 : objReader.GetInt32(iField);
            }

            return iResult;
        }

        public static bool GetDBBoolFromInt(SqlDataReader objReader, Int32 iField)
        {
            if (objReader.IsDBNull(iField))
            {
                return false;
            }
            else
            {
                int i = objReader.GetInt32(iField);
                return (i == 0) ? false : true;
            }
        }

        public static bool GetDBBool(SqlDataReader objReader, Int32 iField)
        {
            return objReader.IsDBNull(iField) ? false : objReader.GetBoolean(iField);
        }

        public static string GetDBString(SqlDataReader objReader, Int32 iField)
        {
            string sResult = string.Empty;

            if (objReader.FieldCount > iField)
            {
                sResult = objReader.IsDBNull(iField) ? string.Empty : objReader.GetString(iField);
            }

            return sResult;
        }

        public static DateTime GetDBDateTime(SqlDataReader objReader, Int32 iField)
        {
            DateTime dtResult = new DateTime();

            if (objReader.FieldCount > iField)
            {
                if (!objReader.IsDBNull(iField))
                {
                    dtResult = objReader.GetDateTime(iField);
                }
            }

            return dtResult;
        }

        public static string GetConnectionString()
        {
            return GetConnectionString("DataSource");
        }


        public static string GetConnectionString(string sSource)
        {
            string sResult = string.Empty;

            try
            {
                if (!DecryptionHandler.Decryption((string)new AppSettingsReader().GetValue(sSource, typeof(string)), out sResult))
                {
                    sResult = string.Empty;
                }
            }
            catch (System.Exception objError)
            {
            }

            return sResult;
        }

    }
}

