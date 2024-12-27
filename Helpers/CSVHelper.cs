using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

public static class CSVHelper
{
    public static (DataTable, List<string>) LoadCSVWithValidation(string filePath, char separator = ',', bool detectSeparators = false)
    {
        var dataTable = new DataTable();
        var inconsistentRows = new List<string>();

        if (!File.Exists(filePath))
            throw new FileNotFoundException("CSV file not found.", filePath);

        using (var reader = new StreamReader(filePath))
        {
            // Read and parse the header line
            string headerLine = reader.ReadLine();
            if (headerLine == null)
                throw new Exception("CSV file is empty.");

            if (detectSeparators)
            {
                separator = DetectSeparator(headerLine);
            }

            // Create columns from header
            var headers = headerLine.Split(separator);
            foreach (var header in headers)
            {
                dataTable.Columns.Add(header.Trim());
            }

            // Read and parse the rows
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(line))
                    continue; // Skip empty lines

                var values = line.Split(separator);

                // Create a new row
                var row = dataTable.NewRow();

                if (values.Length < headers.Length)
                {
                    // Pad missing columns with empty strings
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row[i] = i < values.Length ? values[i]?.Trim() : string.Empty;
                    }

                    // Log the inconsistent row
                    inconsistentRows.Add($"Row padded: {line}");
                }
                else if (values.Length > headers.Length)
                {
                    // Truncate extra columns and log the inconsistency
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row[i] = values[i]?.Trim();
                    }

                    // Log the extra data
                    inconsistentRows.Add($"Row truncated: {line}");
                }
                else
                {
                    // Add valid row
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row[i] = values[i]?.Trim();
                    }
                }

                // Add row to DataTable
                dataTable.Rows.Add(row);
            }
        }

        return (dataTable, inconsistentRows);
    }


    private static char DetectSeparator(string headerLine)
    {
        // Detect the most likely separator from the header line
        var possibleSeparators = new[] { ',', ';', '\t', '|' };
        return possibleSeparators
            .Select(sep => new { Separator = sep, Count = headerLine.Count(c => c == sep) })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault()?.Separator ?? ','; // Default to comma
    }
}
