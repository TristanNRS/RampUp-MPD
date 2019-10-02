using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Configuration;


// TODO: FIX REGEX for updating dates to include acceptance of times
// TODO: FIX REGEX for validations


namespace DbAccess
{
    public class Db
    {
        public string validateInput(string input, string colName, Dictionary<string, string> metadata)
        {
            /**
             * Validates a given input based on DB type of input and the column name associated with input
             * */
            if(input.Length > 0)
            {
                if (input.Contains("'") || input.Contains("`") || input.Contains("\""))
                    return "No single quotes, double quotes or back ticks allowed";
                string type = this.mapDbTypeToInputType(metadata["dataType"], colName);
                string pattern;
                switch (type)
                {
                    case "int":
                        pattern = "^\\d+$";
                        break;
                    case "float":
                        pattern = "[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)";
                        break;
                    case "email":
                        pattern = ".+";
                        break;
                    case "phoneNumber":
                        pattern = "^\\d{3}-\\d{4}$";
                        break;
                    case "date":
                        pattern = ".+";
                        break;
                    case "time":
                        pattern = ".+";
                        break;
                    case "string":
                        pattern = ".+";
                        break;
                    default:
                        return "Error: no type found";
                }
                var match = Regex.Match(input, pattern);
                if (match.Success)
                {
                    return "Success";
                } else 
                    return $"Expected {type} ";
            } else
            {
                if (metadata["isRequired"].Equals("Y"))
                    return "Field is required";
            }
            return "Success";
        }

        public string mapDbTypeToInputType(string dataType, string colName)
        {
            /**
             * Maps DB type to an input type which may be easily compared using regex for validation
             * 
             * */
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
            // Returns a SQL SELECT statement including all the cols specified in the List<string> from the relevant table
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
            // Returns a SQL INSERT statement to insert all the values specified into the specified cols into the relevant table
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

        public string getSqlUpdate(List<string> cols, List<string> newValues, Dictionary<string, string> primaryKeys, string tableName)
        {
            // Returns a SQL UPDATE statement updating all the cols specified with the newValues based on the primaryKeys passed
            string sql = $"UPDATE [dbo].[{tableName}] SET ";
            int count = 1;
            cols.ForEach((string colName) =>
            {
                string newValue = newValues.ElementAt(count - 1);
                if (count < cols.Count)
                    sql += newValue != string.Empty ? $"[{colName}]= '{newValue}', " : $"[{colName}]= NULL,";
                else if (count == cols.Count)
                    sql += newValue != string.Empty ? $"[{colName}]= '{newValue}' " : $"[{colName}]= NULL ";
                count += 1;
            });

            sql += "WHERE ";

            count = 1;
            List<string> keys = primaryKeys.Keys.ToList();
            keys.ForEach((string key) =>
            {
                if (count < keys.Count)
                    sql += $"{key} = {primaryKeys[key]} AND ";
                else
                    sql += $"{key} = {primaryKeys[key]}";
                count += 1;
            });

            
            return sql;
        }

        public string getSqlDelete(Dictionary<string, string> primaryKeys, string tableName)
        {
            // Returns a SQL DELETE statement to delete all values where the primary key = some value 
            // primaryKeys follows the format : <primaryKeyName> : <primaryKeyValue>

            string sql = $"DELETE FROM [dbo].[{tableName}] WHERE ";
            int count = 1;
            List<string> keys = primaryKeys.Keys.ToList();
            keys.ForEach((string key) =>
            {
                if (count < keys.Count)
                    sql += $"{key} = {primaryKeys[key]} AND ";
                else 
                    sql += $"{key} = {primaryKeys[key]}";
                count += 1;
            });

            return sql;
        }

        public string getSqlSearch(string searchText, List<string> cols, string tableName)
        {
            // Returns a SQL SELECT statement that returns a subset of the specified table based on any of the cols passed being equal or similiar to the searchText

            string sql = $"SELECT * FROM [dbo].[{tableName}] WHERE ";
            int count = 1;
            cols.ForEach((string colName) =>
            {
                if (count < cols.Count)
                    sql += $"{colName} LIKE '%{searchText}%' OR ";
                else if (count == cols.Count)
                    sql += $"[{colName}] LIKE '%{searchText}%'";
                count += 1;
            });
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

        public List<string> getPrimaryKeys(string table)
        {
            try
            {
                SqlConnection cnn = this.getConnection();
                cnn.Open();

                List<string> primaryKeys = new List<string>();
                string sqlPk = $@"
                        SELECT K.COLUMN_NAME
                        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS C
                        JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS K ON C.TABLE_NAME = K.TABLE_NAME
                            AND C.CONSTRAINT_CATALOG = K.CONSTRAINT_CATALOG
                            AND C.CONSTRAINT_SCHEMA = K.CONSTRAINT_SCHEMA
                            AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                        WHERE C.CONSTRAINT_TYPE = 'PRIMARY KEY'
                            AND K.TABLE_NAME = '{table}';
                    ";
                SqlCommand commandPk = this.getCommand(sqlPk, cnn);
                SqlDataReader dataReaderPk = this.getDataReader(commandPk);

                while (dataReaderPk.Read())
                {
                    primaryKeys.Add(dataReaderPk["COLUMN_NAME"].ToString());
                }

                // close off connections
                this.closeOff(cnn, commandPk, dataReaderPk);

                if(primaryKeys.Count > 0)
                    return primaryKeys;
                return null;
            }
            catch (Exception err)
            {
                Console.Write(err.Message);
                return null; 
            }

        }

        public List<string> getForeignKeys(string table)
        {
            try
            {
                SqlConnection cnn = this.getConnection();
                cnn.Open();

                List<string> foreignKeys = new List<string>();
                string sqlFk = $@"
                        SELECT K.COLUMN_NAME
                        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS C
                        JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS K ON C.TABLE_NAME = K.TABLE_NAME
                            AND C.CONSTRAINT_CATALOG = K.CONSTRAINT_CATALOG
                            AND C.CONSTRAINT_SCHEMA = K.CONSTRAINT_SCHEMA
                            AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                        WHERE C.CONSTRAINT_TYPE = 'FOREIGN KEY'
                            AND K.TABLE_NAME = '{table}';
                    ";
                SqlCommand commandFk = this.getCommand(sqlFk, cnn);
                SqlDataReader dataReaderFk = this.getDataReader(commandFk);

                while (dataReaderFk.Read())
                {
                    foreignKeys.Add(dataReaderFk["COLUMN_NAME"].ToString());
                }

                // close off connections
                this.closeOff(cnn, commandFk, dataReaderFk);

                if (foreignKeys.Count > 0)
                    return foreignKeys;
                return null;
            }
            catch(Exception err)
            {
                Console.Write(err.Message);
                return null;
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
             *          isRequired: <string: Y|N>,
             *          isPrimaryKey: <string: Y|N>,
             *          isForeignKey: <string: Y|N>
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

                List<string> primaryKeys = this.getPrimaryKeys(table);
                List<string> foreignKeys = this.getForeignKeys(table);

                int count = 0;
                while (dataReader.Read())
                {
                    string colName = dataReader["COLUMN_NAME"].ToString();
                    // dataType
                    Dictionary<string, string> props = new Dictionary<string, string>();
                    props.Add("dataType", dataReader["DATA_TYPE"].ToString());

                    // isRequired
                    string isRequired = dataReader["IS_NULLABLE"].ToString().Equals("YES") ? "N" : "Y";
                    props.Add("isRequired", isRequired);

                    // isPrimaryKey
                    if (primaryKeys != null && primaryKeys.Contains(colName))
                        props.Add("isPrimaryKey", "Y");
                    else
                        props.Add("isPrimaryKey", "N");

                    // isForeignKey
                    if (foreignKeys != null && foreignKeys.Contains(colName))
                        props.Add("isForeignKey", "Y");
                    else
                        props.Add("isForeignKey", "N");

                    data.Add(colName, props );
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

        protected string formatColName(string colName)
        {
            // Formats a DB column name to a more readable format
            // EG. staff_id becomes Staff Id
            // EG. department becomes Department

            string[] arr;
            string formattedValue;
            if (colName.Contains("_"))
            {
                arr = colName.Split('_');

                for(int i = 0; i < arr.Length; i += 1)
                {
                    arr[i] = arr[i].Insert(0,arr[i].ElementAt(0).ToString().ToUpper());
                    arr[i] = arr[i].Remove(1, 1);
                }

                formattedValue = string.Join(" ", arr);
            } else
            {
                formattedValue = colName.Insert(0, colName.ElementAt(0).ToString().ToUpper());
                formattedValue = formattedValue.Remove(1, 1);
            }

            return formattedValue;
        }

        public List<string> getFormattedColNames(List<string> colNames)
        {
            List<string> formattedNames = new List<string>();
            colNames.ForEach((colName) =>
            {
                formattedNames.Add(formatColName(colName));
            });

            return formattedNames;
        }

        public List<string> getAllColumnNames(string table = null, SqlConnection cnn = null, Dictionary<string, Dictionary<string, string>> data = null)
        {
            if (data == null)
                data = this.getTableMetadata(table, cnn);

            return new List<string>(data.Keys);
        }

        public List<string> getEditableInsertableColumnNames(string table=null, SqlConnection cnn = null, Dictionary<string, Dictionary<string, string>> data=null)
        {
            if(data == null)
                data = this.getTableMetadata(table, cnn);

            List<string> colNames = new List<string>();
            foreach(var kvp in data)
            {
                Dictionary<string, string> props = kvp.Value;
                if (props["isPrimaryKey"].Equals("N") || (props["isPrimaryKey"].Equals("Y") && props["isForeignKey"].Equals("Y")))
                    colNames.Add(kvp.Key);
            }
            
            return colNames.Count > 0 ? colNames : null;
        }

        public SqlConnection getConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
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
