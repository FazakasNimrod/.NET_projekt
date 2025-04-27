using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace projekt.Services
{
    public static class CsvSetupHelper
    {
        /// <summary>
        /// Ensures that the data directory exists and creates sample CSV files if none exist.
        /// This is primarily for development purposes.
        /// </summary>
        /// <param name="environment">Web hosting environment</param>
        public static void EnsureDataDirectoryAndSampleFiles(IWebHostEnvironment environment)
        {
            // Get a logger
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger("CsvSetupHelper");

            // Create the data directory if it doesn't exist
            string dataDirectory = Path.Combine(environment.WebRootPath, "data");
            logger.LogInformation($"Checking for data directory: {dataDirectory}");

            if (!Directory.Exists(dataDirectory))
            {
                logger.LogInformation($"Creating data directory: {dataDirectory}");
                Directory.CreateDirectory(dataDirectory);
            }

            // Check if any CSV files exist
            var files = Directory.GetFiles(dataDirectory, "*_final.csv");
            logger.LogInformation($"Found {files.Length} CSV files in {dataDirectory}");

            if (files.Length == 0)
            {
                logger.LogInformation("No CSV files found, creating sample data");

                // Create sample CSV files for testing
                CreateSampleCsvFile(dataDirectory, 2021, logger);
                CreateSampleCsvFile(dataDirectory, 2022, logger);
                CreateSampleCsvFile(dataDirectory, 2023, logger);

                // List the new files
                var newFiles = Directory.GetFiles(dataDirectory, "*_final.csv");
                logger.LogInformation($"Created {newFiles.Length} sample CSV files");
                foreach (var file in newFiles)
                {
                    logger.LogInformation($"  - {Path.GetFileName(file)}");
                }
            }
            else
            {
                // List existing files
                logger.LogInformation("Existing CSV files:");
                foreach (var file in files)
                {
                    logger.LogInformation($"  - {Path.GetFileName(file)} ({new FileInfo(file).Length} bytes)");
                }
            }
        }

        private static void CreateSampleCsvFile(string directory, int year, ILogger logger)
        {
            string filePath = Path.Combine(directory, $"{year}_final.csv");
            logger.LogInformation($"Creating sample CSV file: {filePath}");

            try
            {
                // Create sample header and data
                var lines = new List<string>
                {
                    "code,school,county,previous_prom,education_form,specialization,romanian_level,native_level,native_subject,extra_language,extra_language_grades,tic,mandatory_subject,chosen_subject,avg,passed,year,full_school_name,city,medium,school_code,name_hu,city_ro,city_hu,county_ro,county_hu,school_type,mandatory_grade_initial,mandatory_grade_final,mandatory_grade_contestation,chosen_grade_initial,chosen_grade_final,chosen_grade_contestation,romanian_grade_initial,romanian_grade_final,romanian_grade_contestation,native_grade_initial,native_grade_final,native_grade_contestation,nationality,specialization_type"
                };

                // Generate 20 sample records
                var random = new Random(year); // Use year as seed for consistent randomization
                var schools = new[] { "Colegiul National", "Liceul Teoretic", "Colegiul Tehnic", "Liceul Vocational" };
                var counties = new[] { "Bucuresti", "Cluj", "Iasi", "Timis", "Brasov", "Constanta" };
                var specializations = new[] { "Matematica-Informatica", "Stiinte ale Naturii", "Filologie", "Stiinte Sociale" };

                for (int i = 1; i <= 50; i++)
                {
                    string school = schools[random.Next(schools.Length)];
                    string county = counties[random.Next(counties.Length)];
                    string specialization = specializations[random.Next(specializations.Length)];

                    // Generate random grades between 5 and 10
                    double romanianGrade = Math.Round(5 + random.NextDouble() * 5, 2);
                    double mandatoryGrade = Math.Round(5 + random.NextDouble() * 5, 2);
                    double chosenGrade = Math.Round(5 + random.NextDouble() * 5, 2);

                    // Calculate average
                    double avg = Math.Round((romanianGrade + mandatoryGrade + chosenGrade) / 3, 2);

                    // Determine if passed (avg >= 6.0 and all grades >= 5.0)
                    int passed = (avg >= 6.0) ? 1 : 0;

                    string schoolName = $"{school} \"{i}\"";
                    string city = county;

                    lines.Add($"CODE{year}{i:000},{school},{county},True,Zi,{specialization},Avansat,Neprezentat,Franceza,B1-B1-A2-B1-A2,Mediu,Istorie,Geografie,{avg},{passed},{year},{schoolName},{city},{city},1000{i},{schoolName},{city},{city},{county},{county},{school},{mandatoryGrade},{mandatoryGrade},0,{chosenGrade},{chosenGrade},0,{romanianGrade},{romanianGrade},0,0,0,0,Romana,Uman");
                }

                // Write to file
                File.WriteAllLines(filePath, lines);
                logger.LogInformation($"Successfully created sample CSV file with {lines.Count - 1} records: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error creating sample CSV file {filePath}: {ex.Message}");
                throw;
            }
        }
    }
}