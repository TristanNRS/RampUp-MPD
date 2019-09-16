using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DbAccess
{
    public class Db
    {
        //public bool validateInput(string input, Dictionary<string, string> metadata)
        //{
        //    if(metadata["isRequired"].Equals("Y") && input.Length > 0)
        //    {
        //        string pattern;
        //        switch (metadata["dataType"])
        //        {
        //            case "int":
        //                pattern = "^\\d+$";
        //                break;
        //            case "float":
        //                pattern = "[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)";
        //                break;
        //            case "email":
        //                pattern = ".+";
        //                break;
        //            case "phoneNumber":
        //                pattern = "^\\d{3}-\\d{4}$";
        //                break;
        //            case "date":
        //                pattern = "((0?[13578]|10|12)(-|\\/)((0[0-9])|([12])([0-9]?)|(3[01]?))(-|\\/)((\\d{4})|(\\d{2}))|(0?[2469]|11)(-|\\/)((0[0-9])|([12])([0-9]?)|(3[0]?))(-|\\/)((\\d{4}|\\d{2})))";
        //                break;
        //            case "time":
        //                pattern = ".+";
        //                break;
        //            case "string":
        //                pattern = ".+";
        //                break;
        //            default:
        //                return false;
        //        }
        //        var match = Regex.Match(input, pattern);
        //        if (match.Success)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    return false;
        //}

        public string mapDbTypeToInputType(string dataType, string colName)
        {
            string stringType = "string";
            string intType = "int";
            string floatType = "float";

            if (colName.ToLower().Contains("email")){
                return "email";
            }
            else if(colName.ToLower().Contains("phone") || colName.ToLower().Contains("mobile"))
            {
                return "phoneNumber";
            }
            switch (dataType.ToLower())
            {
                case "char":
                    return stringType;
                case "varchar":
                    return stringType;
                case "nvarchar":
                    return stringType;
                case "nchar":
                    return stringType;
                case "text":
                    return stringType;

                case "bit":
                    return intType;
                case "tinyint":
                    return intType;
                case "smallint":
                    return intType;
                case "int":
                    return intType;
                case "bigint":
                    return intType;
                case "decimal":
                    return intType;

                case "numeric":
                    return floatType;
                case "float":
                    return floatType;
                case "real":
                    return floatType;

                case "date":
                    return "date";
                case "datetime":
                    return "date";
                case "time":
                    return "time";
            }

            return null;
        }

        public string getSqlSelect(List<string> cols, string tableName)
        {
            string sql = "SELECT ";
            int count = 1;
            cols.ForEach((string colName) =>
            {
                if (count < cols.Count)
                    sql += $"[{colName}], ";
                else if (count == cols.Count)
                    sql += $"[{colName}] FROM [dbo].[{tableName}]";
                count += 1;
            });
            return sql;
        }

        public string getSqlInsert(List<string> cols, List<string> values, string tableName)
        {
            string sql = $"INSERT INTO [dbo].[{tableName}] (";
            int count = 1;
            cols.ForEach((string colName) =>
            {
                if (count < cols.Count)
                    sql += $"[{colName}], ";
                else if (count == cols.Count)
                    sql += $"[{colName}]) ";
                count += 1;
            });
            sql += "VALUES (";
            count = 1;
            values.ForEach((string value) =>
            {
                if (count < values.Count)
                    sql += $"'{value}', ";
                else if (count == values.Count)
                    sql += $"'{value}') ";
                count += 1;
            });
            return sql;
        }

        public string getSqlUpdate(List<string> cols, List<string> newValues,string pkName ,string pkValue, string tableName)
        {
            string sql = $"UPDATE [dbo].[{tableName}] SET ";
            int count = 1;
            cols.ForEach((string colName) =>
            {
                if (count < cols.Count)
                    sql += $"[{colName}]= '{newValues.ElementAt(count - 1)}', ";
                else if (count == cols.Count)
                    sql += $"[{colName}]= '{newValues.ElementAt(count - 1)}' ";
                count += 1;
            });
            sql += $"WHERE [{pkName}]={pkValue}";
            
            return sql;
        }

        public List<string> getListOfTables()
        {
            List<string> tableNames = new List<string>();
            try
            {
                SqlConnection cnn = this.getConnection();
                cnn.Open();
                string sql = "SELECT table_name FROM information_schema.tables WHERE table_schema ='dbo';";
                SqlCommand command = this.getCommand(sql, cnn);
                SqlDataReader dataReader = this.getDataReader(command);

                while (dataReader.Read())
                {
                    // add names of table to dropdown menu
                    if (!dataReader.GetValue(0).ToString().Equals("Users"))
                        tableNames.Add(dataReader.GetValue(0).ToString());
                }

                // close off connections
                this.closeOff(cnn, command, dataReader);

                return tableNames;
            }
            catch (Exception err)
            {
                Console.Write(err);
                return null;
            }
        }

        public int getNumTables()
        {
            try
            {
                SqlConnection cnn = this.getConnection();
                cnn.Open();

                string sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                SqlCommand command = this.getCommand(sql, cnn);
                SqlDataReader dataReader = this.getDataReader(command);

                int tableCount = 0;
                while (dataReader.Read())
                {
                    tableCount += (int)dataReader.GetValue(0);
                }

                // one less table since Users table is not shown 
                tableCount -= 1;

                // close off connections
                this.closeOff(cnn, command, dataReader);

                return tableCount;
            } catch (Exception err)
            {
                Console.Write(err);
                return -1;
            }
        }

        public Dictionary<string, Dictionary<string, string>> getTableMetadata(string table, SqlConnection cnn = null)
        {
            // returns dictionary with following format :
            /*
             * 
             * {
             *      columnName: {
             *          dataType: <string>,
             *          isRequired: <string>
             *      },
             *      columnName: {
             *          dataType: <string>,
             *          isRequired: <string>
             *      }
             * }
             * 
             **/
            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                if (cnn == null)
                {
                    cnn = this.getConnection();
                    cnn.Open();
                }


                string sql = $"select COLUMN_NAME, DATA_TYPE, IS_NULLABLE from information_schema.columns where TABLE_NAME = N'{table}'";
                SqlCommand command = this.getCommand(sql, cnn);
                SqlDataReader dataReader = this.getDataReader(command);
                int count = 0;
                while (dataReader.Read())
                {
                    Dictionary<string, string> props = new Dictionary<string, string>();
                    props.Add("dataType", dataReader["DATA_TYPE"].ToString());

                    string isRequired = dataReader["IS_NULLABLE"].ToString().Equals("YES") ? "N" : "Y";
                    props.Add("isRequired", isRequired);

                    data.Add(dataReader["COLUMN_NAME"].ToString(), props );
                    count += 1;
                }

                // close off connections
                if (cnn == null)
                    this.closeOff(cnn, command, dataReader);
                else
                    this.closeOff(null, command, dataReader);

                return data;
            }
            catch (Exception err)
            {
                Console.Write(err);
                return null;
            }
        }

        public List<string> getAllColumnNames(string table = null, SqlConnection cnn = null, Dictionary<string, Dictionary<string, string>> data = null)
        {
            if (data == null)
                data = this.getTableMetadata(table, cnn);
            return new List<string>(data.Keys);
        }

        public List<string> getColumnNamesWithoutPk(string table=null, SqlConnection cnn = null, Dictionary<string, Dictionary<string, string>> data=null)
        {
            if(data == null)
                data = this.getTableMetadata(table, cnn);
            return new List<string>(data.Keys.Skip(1));
        }

        public SqlConnection getConnection()
        {
            string connectionString = @"Data Source=DESKTOP-HN7F6PM\SQLEXPRESS1;Initial Catalog=Test;Integrated Security=True";
            return new SqlConnection(connectionString);
        }

        public SqlCommand getCommand(string sql, SqlConnection conn)
        {
            return new SqlCommand(sql, conn);

        }

        public SqlDataReader getDataReader(SqlCommand cmd)
        {
            return cmd.ExecuteReader();
        }

        public void closeOff(SqlConnection conn=null, SqlCommand cmd=null, SqlDataReader reader=null)
        {
            if(reader != null)
                reader.Close();
            if(cmd != null)
                cmd.Dispose();
            if(conn != null)
                conn.Close();
        }

    }
}
