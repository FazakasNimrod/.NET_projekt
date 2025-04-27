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

                // For large files, use a streaming approach with buffer
                const int bufferSize = 4096; // 4 KB buffer
                using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
                {
                    using var inputStream = file.OpenReadStream(maxAllowedSize: 104857600); // 100 MB max

                    // Copy the file in chunks to avoid loading it all into memory
                    await inputStream.CopyToAsync(outputStream);
                }

                // Validate the first part of the file to ensure it's a valid CSV
                bool isValidCsv = true;
                try
                {
                    using (var validateStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using var reader = new StreamReader(validateStream);

                        // Read just the first 1000 characters to validate structure
                        char[] buffer = new char[1000];
                        await reader.ReadAsync(buffer, 0, buffer.Length);
                        string sampleContent = new string(buffer);

                        // Basic validation - just check if it has headers and commas
                        isValidCsv = sampleContent.Contains(",") &&
                                     (sampleContent.Contains("code") ||
                                      sampleContent.Contains("school") ||
                                      sampleContent.Contains("county"));
                    }
                }
                catch
                {
                    // If there's an error reading the file, assume it's still valid
                    // since we already wrote it successfully
                    isValidCsv = true;
                }

                return isValidCsv;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing uploaded file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validate the structure of the CSV file
        /// </summary>
        private static bool ValidateCsvStructure(string csvContent)
        {
            if (string.IsNullOrWhiteSpace(csvContent))
                return false;

            // Basic validation - just check if it has commas and some expected header fields
            return csvContent.Contains(",") &&
                  (csvContent.Contains("code") ||
                   csvContent.Contains("school") ||
                   csvContent.Contains("county"));
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