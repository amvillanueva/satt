
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Utils
{
    public class SqlConnector
    {
        public static DataSet getDataSet(String cnx, string spName, ArrayList alParametros)
        {
            DataSet ds = null;
            using (SqlConnection connection = new SqlConnection(cnx))
            {

                connection.Open();

                SqlCommand cmd = getCommand(connection, spName, alParametros);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

            }

            if (ds == null)
                throw new Exception("Error de consulta, la consulta no devolvió ningún resultado");

            return ds;
        }

        public static DataSet getDataSet(String cnx, string sql)
        {
            DataSet ds = null;
            using (SqlConnection connection = new SqlConnection(cnx))
            {
                connection.Open();

                SqlCommand cmd = getCommand(connection, sql);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            if (ds == null)
                throw new Exception("Error de consulta, la consulta no devolvió ningún resultado");

            return ds;
        }

        internal static DataSet getDataSet(object connectionString, string v, ArrayList alParameters)
        {
            throw new NotImplementedException();
        }

        public static DataTable getDataTable(String cnx, string spName, ArrayList alParametros)
        {
            DataTable dt = null;
            using (SqlConnection connection = new SqlConnection(cnx))
            {
                connection.Open();

                SqlCommand cmd = getCommand(connection, spName, alParametros);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

            }

            if (dt == null)
                throw new Exception("Error de consulta, la consulta no devolvió ningún resultado");
            return dt;
        }

        public static DataTable getDataTable(String cnx, string sql)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(cnx))
            {
                connection.Open();

                SqlCommand cmd = getCommand(connection, sql);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }

            if (dt == null)
                throw new Exception("Error de consulta, la consulta no devolvió ningún resultado");

            return dt;
        }

        public static object executeScalar(String cnx, string sql, ArrayList alParametros)
        {
            object obj = null;
            using (SqlConnection connection = new SqlConnection(cnx))
            {
                connection.Open();

                SqlCommand cmd = getCommand(connection, sql, alParametros);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                obj = cmd.ExecuteScalar();

            }

            if (obj == null)
                throw new Exception("Error de transacción, la ejecución no devolvió ningún resultado");

            return obj;
        }

        public static bool executeReader(String cnx, string sql, ArrayList alParametros, ref string mensajeRpa)
        {
            bool rpta;
            using (SqlConnection connection = new SqlConnection(cnx))
            {
                connection.Open();

                SqlCommand cmd = getCommand(connection, sql, alParametros);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;

                SqlDataReader obj = cmd.ExecuteReader();

                obj.Read();
                rpta = TSQL.CodigoRetorno(obj.GetString(0), ref mensajeRpa);
                obj.Close();

            }

            return rpta;
        }

        public static int executeNonQuery(String cnx, string spName, ArrayList alParametros)
        {
            int res;

            using (SqlConnection connection = new SqlConnection(cnx))
            {
                connection.Open();

                SqlCommand cmd = getCommand(connection, spName, alParametros);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                res = cmd.ExecuteNonQuery();

            }

            return res;
        }

        public static int executeNonQueryText(String cnx, string sqlQuery)
        {
            int res;

            using (SqlConnection connection = new SqlConnection(cnx))
            {
                connection.Open();

                SqlCommand cmd = getCommand(connection, sqlQuery);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                res = cmd.ExecuteNonQuery();
            }

            return res;
        }


        private static SqlCommand getCommand(SqlConnection conn, string spName, ArrayList alParametros)
        {
            SqlCommand cmd = new SqlCommand(spName, conn);

            IEnumerator ieParametros = alParametros.GetEnumerator();
            while (ieParametros.MoveNext())
            { cmd.Parameters.Add((SqlParameter)ieParametros.Current); }

            return cmd;
        }

        private static SqlCommand getCommand(SqlConnection conn, string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            return cmd;
        }

        public static SqlConnection getConnection(String strcnx)
        {
            SqlConnection conn = new SqlConnection(strcnx);
            conn.Open();
            return conn;
        }

        public static async Task<string> publicDataTable(string connectionString, DataTable dataTableSource, string destinationTableName)
        {
            return await Task.Run(() =>
            {
                using (SqlConnection conn = getConnection(connectionString))
                {
                    SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn)
                    {
                        BulkCopyTimeout = 0,
                        DestinationTableName = destinationTableName
                    };

                    try
                    {
                        sqlBulkCopy.WithColumnMappings(dataTableSource.Columns)
                            .GetColumsTableDestination(conn, out DataRow[] lookup)
                            .ValidationOfColums(conn, lookup, dataTableSource, out DataTable dataTableDestination)
                            .WriteToServer(dataTableDestination);

                        return "OK";
                    }
                    catch (SqlException ex)
                    {
                        if
                        (
                            ex.Message.Contains("Received an invalid column length from the bcp client for colid") ||
                            ex.Message.Contains("La longitud de la columna que se recibió del cliente bcp para colid")
                        )
                        {
                            string pattern = @"\d+";
                            Match match = Regex.Match(ex.Message.ToString(), pattern);
                            var index = Convert.ToInt32(match.Value) - 1;

                            FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                            var sortedColumns = fi.GetValue(sqlBulkCopy);
                            var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                            FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                            var metadata = itemdata.GetValue(items[index]);

                            var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                            var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                          //  throw new DataFormatException(String.Format("La columna {0} contiene datos con una longitud mayor a {1}.", column, length));
                        }

                        return ex.Message.ToString();
                    }
                    catch (Exception ex)
                    {
                        return ex.Message.ToString();
                    }
                }
            });
        }

        public static int ConvertStringToInt(string str)
        {
            try
            {
                string cleanStr = Regex.Replace(str.Trim(), @"\s+", "");
                if (int.TryParse(cleanStr, out int returnIntVal))
                    return returnIntVal;
                else
                    throw new Exception("Error al convertir STRING a INT");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static decimal ConvertStringToDecimal(string str)
        {
            try
            {
                // Decimal separator is ",".
                CultureInfo culture = new CultureInfo("tr-TR");
                // Decimal sepereator is ".".
                CultureInfo culture1 = new CultureInfo("en-US");

                string cleanStr = Regex.Replace(str.Trim(), @"\s+", "");
                bool isNegative = cleanStr.Contains('-');
                if (isNegative)
                {
                    cleanStr=cleanStr.Replace('-',' ').Trim();
                }

                bool success = decimal.TryParse(cleanStr, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, culture, out decimal result);
                bool success1 = decimal.TryParse(cleanStr, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, culture1, out decimal result1);

                if (success && success1)
                    return isNegative ? ((result > result1) ?  result1 : result) * -1 : (result > result1) ? result1 : result;
                else if (success && !success1)
                    return isNegative ? result*-1:result ;
                else if (!success && success1)
                    return isNegative ? result1*-1:result1;
                else
                    throw new Exception("Error al convertir STRING a DECIMAL");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ConvertStringToBool(string str)
        {
            try
            {
                switch (str.Trim().ToLower())
                {
                    case "true":
                    case "yes":
                    case "y":
                    case "1":
                        return true;
                    case "false":
                    case "no":
                    case "n":
                    case "0":
                        return false;
                    default:
                        throw new Exception("Error al convertir STRING a BOOL");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public static class SqlBulkCopyExtensions
    {
        public static SqlBulkCopy WithColumnMappings(this SqlBulkCopy bcp, DataColumnCollection columns) =>
            WithColumnMappings(bcp, columns.Cast<DataColumn>());

        public static SqlBulkCopy WithColumnMappings(this SqlBulkCopy bcp, IEnumerable<DataColumn> columns)
        {
            bcp.ColumnMappings.Clear();

            foreach (DataColumn column in columns)
            {
                bcp.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            return bcp;
        }

        public static SqlBulkCopy GetColumsTableDestination(this SqlBulkCopy bcp, SqlConnection conn, out DataRow[] lookup)
        {
            DataTable schemaTable = null;

            #region get columns of table destination

            ConnectionState origState = conn.State;

            if (origState == ConnectionState.Closed)
            {
                conn.Open();
            }

            try
            {
                using (SqlCommand select = new SqlCommand("SELECT TOP 0 * FROM " + bcp.DestinationTableName, conn))
                {
                    using (SqlDataReader destReader = select.ExecuteReader())
                    {
                        schemaTable = destReader.GetSchemaTable();
                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Message.StartsWith("Invalid object name"))
                {
                    throw new Exception(
                        "Tabla de destino " + bcp.DestinationTableName + " no existe en la base de datos"
                        //+ conn.Database + " en el servidor " + conn.DataSource 
                        + "."
                    );
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (origState == ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            #endregion

            if (schemaTable != null)
            {
                if (bcp.ColumnMappings.Count > 0)
                {
                    lookup = new DataRow[bcp.ColumnMappings.Count];

                    #region validate destination

                    DataRow[] columns = new DataRow[schemaTable.Rows.Count];
                    Hashtable columnLookup = new Hashtable();

                    foreach (DataRow column in schemaTable.Rows)
                    {
                        columns[(int)column["ColumnOrdinal"]] = column;
                        columnLookup[column["ColumnName"]] = column["ColumnOrdinal"];
                    }

                    foreach (SqlBulkCopyColumnMapping mapping in bcp.ColumnMappings)
                    {
                        string destColumn = mapping.DestinationColumn;

                        if (destColumn.StartsWith("[") && destColumn.EndsWith("]"))
                        {
                            destColumn = destColumn.Substring(1, destColumn.Length - 2);
                        }

                        if (destColumn != "")
                        {
                            if (!columnLookup.ContainsKey(destColumn))
                            {
                                string bestFit = null;

                                foreach (string existingColumn in columnLookup.Keys)
                                {
                                    if (String.Equals(existingColumn, destColumn, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        bestFit = existingColumn;
                                    }
                                }

                                if (bestFit == null)
                                {
                                    throw new Exception(
                                        "Columna de destino " + mapping.DestinationColumn + " no existe en la tabla de destino " + bcp.DestinationTableName
                                        //+ " en la base de datos " + conn.Database + " en el servidor " + conn.DataSource 
                                        + "."
                                    );
                                }
                                else
                                {
                                    throw new Exception(
                                        "Columna de destino " + mapping.DestinationColumn + " no existe en la tabla de destino " + bcp.DestinationTableName
                                        //+ " en la base de datos " + conn.Database + " en el servidor " + conn.DataSource 
                                        + "." +
                                        " Las asignaciones de nombres de columna son específicas (mayúsculas y/o minúsculas) y la mejor coincidencia es " + bestFit + "."
                                    );
                                }
                            }
                        }
                        else
                        {
                            if (mapping.DestinationOrdinal < 0 || mapping.DestinationOrdinal >= columns.Length)
                            {
                                throw new Exception(
                                    "No existe ninguna columna en el índice " + mapping.DestinationOrdinal + " en la tabla de destino " + bcp.DestinationTableName
                                    //+ " en la base de datos " + conn.Database + " en el servidor " + conn.DataSource 
                                    + "."
                                );
                            }
                        }
                    }

                    #endregion

                    #region create lookup for per record validation

                    int index = 0;
                    foreach (SqlBulkCopyColumnMapping mapping in bcp.ColumnMappings)
                    {
                        int sourceIndex = -1;

                        string sourceColumn = mapping.SourceColumn;

                        if (sourceColumn.StartsWith("[") && sourceColumn.EndsWith("]"))
                        {
                            sourceColumn = sourceColumn.Substring(1, sourceColumn.Length - 2);
                        }

                        if (sourceColumn != "")
                        {
                            //sourceIndex = reader.GetOrdinal(sourceColumn);
                            sourceIndex = index;
                        }
                        else
                        {
                            sourceIndex = mapping.SourceOrdinal;
                        }

                        DataRow destColumnDef = null;

                        string destColumn = mapping.DestinationColumn;

                        if (destColumn.StartsWith("[") && destColumn.EndsWith("]"))
                        {
                            destColumn = destColumn.Substring(1, destColumn.Length - 2);
                        }

                        if (destColumn != "")
                        {
                            foreach (DataRow column in schemaTable.Rows)
                            {
                                if ((string)column["ColumnName"] == destColumn)
                                {
                                    destColumnDef = column;
                                }
                            }
                        }
                        else
                        {
                            foreach (DataRow column in schemaTable.Rows)
                            {
                                if ((int)column["ColumnOrdinal"] == mapping.DestinationOrdinal)
                                {
                                    destColumnDef = column;
                                }
                            }
                        }

                        lookup[sourceIndex] = destColumnDef;
                        index++;
                    }

                    #endregion
                }
                else
                {
                    lookup = new DataRow[schemaTable.Rows.Count];

                    foreach (DataRow column in schemaTable.Rows)
                    {
                        lookup[(int)column["ColumnOrdinal"]] = column;
                    }
                }
            }
            else
            {
                lookup = null;
            }

            return bcp;
        }

        public static SqlBulkCopy ValidationOfColums(this SqlBulkCopy bcp, SqlConnection conn, DataRow[] lookup, DataTable dataTableSource, out DataTable dataTableDestination)
        {
            dataTableDestination = new DataTable();
            int indexRow = 0;
            string nameColumn = "";

            List<DataColumn> IntColumns = new List<DataColumn>();
            List<DataColumn> DecimalColumns = new List<DataColumn>();
            List<DataColumn> BoolColumns = new List<DataColumn>();
            List<DataColumn> DateColumns = new List<DataColumn>();
            List<DataColumn> StringColumns = new List<DataColumn>();

            try
            {
                foreach (DataRow column in lookup)
                {
                    Type FieldDataType = (Type)column["DataType"];
                    DataColumn newColumn = new DataColumn(column["ColumnName"].ToString(), FieldDataType);
                    dataTableDestination.Columns.Add(newColumn);

                    switch (Type.GetTypeCode(FieldDataType))
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64: { IntColumns.Add(newColumn); break; }
                        case TypeCode.Boolean: { BoolColumns.Add(newColumn); break; }
                        case TypeCode.Double:
                        case TypeCode.Decimal: { DecimalColumns.Add(newColumn); break; }
                        case TypeCode.DateTime: { DateColumns.Add(newColumn); break; }
                        default: { newColumn.MaxLength = (int)column["ColumnSize"]; StringColumns.Add(newColumn); break; }
                    }
                }

                indexRow = 2;

                foreach (DataRow dr in dataTableSource.Rows)
                {
                    DataRow row = dataTableDestination.NewRow();

                    foreach (DataColumn IntCol in IntColumns)
                    {
                        nameColumn = IntCol.ColumnName;
                        if (string.IsNullOrEmpty(dr[nameColumn].ToString()) || dr[nameColumn].ToString().Trim() == "")
                            row[nameColumn] = DBNull.Value;
                        else
                            row[nameColumn] = SqlConnector.ConvertStringToInt(dr[nameColumn].ToString());
                    }

                    foreach (DataColumn DecCol in DecimalColumns)
                    {
                        nameColumn = DecCol.ColumnName;
                        if (string.IsNullOrEmpty(dr[nameColumn].ToString()) || dr[nameColumn].ToString().Trim() == "")
                            row[nameColumn] = DBNull.Value;
                        else
                            row[nameColumn] = SqlConnector.ConvertStringToDecimal(dr[nameColumn].ToString());
                    }

                    foreach (DataColumn BoolCol in BoolColumns)
                    {
                        nameColumn = BoolCol.ColumnName;
                        if (string.IsNullOrEmpty(dr[nameColumn].ToString()) || dr[nameColumn].ToString().Trim() == "")
                            row[nameColumn] = DBNull.Value;
                        else
                            row[nameColumn] = SqlConnector.ConvertStringToBool(dr[nameColumn].ToString());
                    }

                    foreach (DataColumn DateCol in DateColumns)
                    {
                        nameColumn = DateCol.ColumnName;
                        if (string.IsNullOrEmpty(dr[nameColumn].ToString()) || dr[nameColumn].ToString().Trim() == "")
                            row[nameColumn] = DBNull.Value;
                        else
                            row[nameColumn] = dr[nameColumn].ToString().Replace("T", " ");
                    }

                    foreach (DataColumn StringCol in StringColumns)
                    {
                        nameColumn = StringCol.ColumnName;
                        if (string.IsNullOrEmpty(dr[nameColumn].ToString()) || dr[nameColumn].ToString().Trim() == "")
                        {
                            row[nameColumn] = DBNull.Value;
                        }
                        else
                        {
                            if (dr[nameColumn].ToString().Trim().Length > StringCol.MaxLength)
                            {
                                string message =
                                    "Valor de columna \"" + dr[nameColumn].ToString().Trim() + "\"" +
                                    " con longitud " + dr[nameColumn].ToString().Trim().Length.ToString("###,##0") +
                                    " de la columna fuente " + nameColumn +
                                    " en la fila " + indexRow.ToString("###,##0") +
                                    " no cabe en la columna de destino " + nameColumn +
                                    " con longitud " + StringCol.MaxLength.ToString("###,##0") +
                                    " en la tabla " + bcp.DestinationTableName;
                                //+ " en la base de datos " + conn.Database
                                //+ " en el servidor " + conn.DataSource + ".";

                                throw DataException(message);
                            }
                            else
                            {
                                row[nameColumn] = dr[nameColumn].ToString().Trim();
                            }
                        }
                    }

                    dataTableDestination.Rows.Add(row);
                    indexRow++;
                }
            }
            catch (Exception ex)
            {
                if (indexRow == 0 && nameColumn == "")
                {
                    throw DataException(ex.Message);
                }
                else
                {
                    throw DataException(String.Format("{0}: [ Fila: {1}, Columna: {2} ].", ex.Message, indexRow, nameColumn));
                }
            }

            return bcp;
        }

        private static Exception DataException(string message)
        {
            throw new Exception(message);
        }
    }
}
