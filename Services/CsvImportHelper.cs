using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Components.Forms;
using projekt.Models;
using System.Globalization;

namespace projekt.Services
{
    public static class CsvImportHelper
    {
        /// <summary>
        /// Process an uploaded CSV file and save it to the data directory
        /// </summary>
        public static async Task<bool> ProcessUploadedCsvFile(IBrowserFile file, int year, IWebHostEnvironment environment)
        {
            try
            {
                // Create data directory if it doesn't exist
                string dataDirectory = Path.Combine(environment.WebRootPath, "data");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                // Define the file path
                string filePath = Path.Combine(dataDirectory, $"{year}_final.csv");

                // Read the uploaded file
                using var stream = file.OpenReadStream(maxAllowedSize: 10485760); // 10 MB max
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();

                // Validate CSV structure
                if (!ValidateCsvStructure(content))
                {
                    return false;
                }

                // Save the file
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validate the structure of the CSV file
        /// </summary>
        private static bool ValidateCsvStructure(string csvContent)
        {
            try
            {
                using var reader = new StringReader(csvContent);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null
                };

                using var csv = new CsvReader(reader, config);
                var header = csv.GetRecords<BacResult>().FirstOrDefault();

                return header != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse CSV content to BacResult objects
        /// </summary>
        public static List<BacResult> ParseCsvContent(string csvContent)
        {
            try
            {
                using var reader = new StringReader(csvContent);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null,
                    TrimOptions = TrimOptions.Trim
                };

                using var csv = new CsvReader(reader, config);
                return csv.GetRecords<BacResult>().ToList();
            }
            catch
            {
                return new List<BacResult>();
            }
        }
    }
}