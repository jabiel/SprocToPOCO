using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SprocToPOCO.Logic
{
    public class SqlMetaProvider
    {
        string connStr;
        public SqlMetaProvider(string connectionString)
        {
            connStr = connectionString;
        }

        /// <summary>
        /// Wykonuje procke, query i zwraca liste kolumn z result setu
        /// </summary>
        /// <param name="sprocNameAndParams"></param>
        /// <returns></returns>
        public List<SprocColumn> GetResultsetColumnsFromStoredProc(string sprocNameAndParams)
        {
            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = conn.CreateCommand())
                {                    
                    conn.Open();
                    List<SprocColumn> columns = new List<SprocColumn>();

                    cmd.CommandText = sprocNameAndParams;
                    cmd.CommandType = System.Data.CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();

                    if (reader.GetSchemaTable() != null)
                    {
                        foreach (DataRow c in reader.GetSchemaTable().Rows)
                        {
                            columns.Add(new SprocColumn()
                            {
                                Name = c["ColumnName"].ToString(),
                                Type = c["DataType"].ToString(),
                                Length = (int)c["ColumnSize"],
                                IsNullable = (bool)c["AllowDBNull"],
                            });
                        }
                    }
                    return columns;   
                }
            }
        }


        /// <summary>
        /// Pobiera parametry procedury
        /// </summary>
        /// <param name="sprocName"></param>
        /// <returns></returns>
        public List<SprocParam> GetParamsFromStoredProc(string sprocName)
        {
            const string queryProcParamsSQL = "SELECT PARAMETER_MODE, PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = '{0}' ORDER BY ORDINAL_POSITION";

            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    List<SprocParam> pars = new List<SprocParam>();

                    cmd.CommandText = string.Format(queryProcParamsSQL, sprocName);
                    cmd.CommandType = System.Data.CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        pars.Add(new SprocParam() {
                            Name = (string)reader["PARAMETER_NAME"],
                            Datatype = (string)reader["DATA_TYPE"],
                            OutParam = (string)reader["PARAMETER_NAME"] == "OUT",
                            MaxLen = reader["CHARACTER_MAXIMUM_LENGTH"] as int? ?? default(int)
                        });
                    }
                    return pars;
                }
            }                      
        }


        public List<SprocParam> GetColumnsFromTableName(string tableName)
        {
            const string queryTableColumnsSQL = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, columnproperty(object_id('BTC'),COLUMN_NAME,'IsIdentity') IsIdentity"
                                                +" FROM INFORMATION_SCHEMA.COLUMNS"
                                                +" WHERE TABLE_NAME = 'BTC' ORDER BY ORDINAL_POSITION";

            using (var conn = new SqlConnection(connStr))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    List<SprocParam> pars = new List<SprocParam>();

                    cmd.CommandText = string.Format(queryTableColumnsSQL, tableName);
                    cmd.CommandType = System.Data.CommandType.Text;
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        pars.Add(new SprocParam()
                        {
                            Name = (string)reader["PARAMETER_NAME"],
                            Datatype = (string)reader["DATA_TYPE"],
                            OutParam = (string)reader["PARAMETER_NAME"] == "OUT",
                            MaxLen = reader["CHARACTER_MAXIMUM_LENGTH"] as int? ?? default(int)
                        });
                    }
                    return pars;
                }
            }
        }

    }
}
