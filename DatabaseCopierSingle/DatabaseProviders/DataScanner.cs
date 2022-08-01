using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.TableDataComponents;
using System;
using System.Data.Common;

namespace DatabaseCopierSingle.DatabaseProviders
{
    internal class DataScanner
    {
        /*public static async Task<TableData> GetDataInTable(string tableName, NpgsqlConnection conn)
        {
            var ammountOfRows = await GetNumberOfRowsInTheTableAsync(tableName, conn);

            var tableData = new TableData();

            Task<TableDataRows>[] dataAwaiter = new Task<TableDataRows>[ammountOfRows / 100 + 1];
            for (int i = 0; i <= ammountOfRows / 100; i++)
            {
                int rowsТumberToGet = i == (ammountOfRows / 100) ? ammountOfRows % 100 : 100;
                dataAwaiter[i] = GetRangeOfRowsFromTableAsync(tableName, conn, i * 100, rowsТumberToGet);
            }

            await Task.WhenAll(dataAwaiter);
            for (int i = 0; i < dataAwaiter.Length; i++)
            {
                tableData.AddData(dataAwaiter[i].Result);
            }
            return tableData;
        }*/

        /*public static async Task<TableDataRows> GetRangeOfRowsFromTableAsync(string tableName, NpgsqlConnection conn, int startWith = 0, int ammountOfRows = 100)
        {
            var queryString =
                $"SELECT *\n" +
                $"FROM {tableName}\n" +
                $"OFFSET {startWith}\n" +
                $"FETCH NEXT {ammountOfRows} ROWS ONLY;";
            try
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = queryString;
                var reader = await cmd.ExecuteReaderAsync();
                var ammountOfColumns = reader.FieldCount;

                TableDataRow[] dataRows = new TableDataRow[ammountOfRows];
                int ch = 0;

                object[] tmp = new object[ammountOfColumns];
                while (reader.Read())
                {
                    for (int i = 0; i < ammountOfColumns; i++)
                    {

                        reader.GetValues(tmp);
                        TableDataRow row = new TableDataRow((object[])tmp.Clone());
                        dataRows[ch] = row;
                    }
                    ch++;
                }

                return new TableDataRows(dataRows);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                throw;
            }

        }*/

        /*public static async Task<int> GetNumberOfRowsInTheTableAsync(string tableName, DbConnection conn)
        {
            try
            {
                var quaryString =
                    $"SELECT COUNT(*)" +
                    $"FROM {tableName}";
                var cmd = conn.CreateCommand();
                cmd.CommandText = quaryString;
                var quaryTask = cmd.ExecuteScalarAsync();
                var res = (long)(await quaryTask);
                int numberOfRows = (int)res;
                return numberOfRows;
            }
            catch (SocketException)
            {
                throw;
            }
            catch (DbException)
            {
                throw;
            }
        }*/
    }
}
