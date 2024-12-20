using System.Data;
using System.IO;

namespace SwissAddressManager.Helpers
{
    public static class CSVHelper
    {
        public static DataTable LoadCSV(string filePath)
        {
            var dt = new DataTable();
            using (var reader = new StreamReader(filePath))
            {
                var headers = reader.ReadLine().Split(',');
                foreach (var header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine().Split(',');
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }
    }
}
