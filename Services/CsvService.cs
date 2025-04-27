using CsvHelper;
using CsvHelper.Configuration;
using projekt.Models;
using System.Globalization;

namespace projekt.Services
{
    public class CsvService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CsvService> _logger;

        public CsvService(IWebHostEnvironment environment, ILogger<CsvService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<List<BacResult>> GetBacResultsForYearAsync(int year)
        {
            // Filename format like "2014_final.csv"
            string fileName = $"{year}_final.csv";
            string filePath = Path.Combine(_environment.WebRootPath, "data", fileName);

            _logger.LogInformation($"Looking for CSV file at: {filePath}");

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"CSV file not found: {filePath}");
                // Return empty list if file doesn't exist
                return new List<BacResult>();
            }

            try
            {
                _logger.LogInformation($"Reading CSV file: {filePath}");

                // First, try to read the file contents
                string fileContent = await File.ReadAllTextAsync(filePath);
                _logger.LogInformation($"Successfully read {fileContent.Length} characters from {fileName}");

                // Log the first 100 characters to see what the file looks like
                _logger.LogInformation($"File preview: {fileContent.Substring(0, Math.Min(100, fileContent.Length))}");

                // Create a more flexible CSV configuration
                using var reader = new StringReader(fileContent);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null,
                    TrimOptions = TrimOptions.Trim,
                    ShouldSkipRecord = record => string.IsNullOrWhiteSpace(record.ToString()),
                    IgnoreBlankLines = true,
                    HeaderValidated = null,
                    PrepareHeaderForMatch = args => args.Header.ToLower(),
                    AllowComments = true,
                    Comment = '#'
                };

                using var csv = new CsvReader(reader, config);

                // Register the class map to handle the BacResult parsing
                csv.Context.RegisterClassMap<BacResultMap>();

                try
                {
                    var results = csv.GetRecords<BacResult>().ToList();
                    _logger.LogInformation($"Successfully parsed {results.Count} records from {fileName}");
                    return results;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error parsing CSV records: {ex.Message}");

                    // Try a manual approach if CSV parsing fails
                    return ParseManually(fileContent, year);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, $"Error loading CSV file {fileName}: {ex.Message}");
                return new List<BacResult>();
            }
        }

        private List<BacResult> ParseManually(string fileContent, int year)
        {
            _logger.LogInformation("Attempting manual CSV parsing...");
            var results = new List<BacResult>();

            try
            {
                // Split file into lines
                var lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Skip header line
                if (lines.Length > 1)
                {
                    // Log the header to understand the structure
                    _logger.LogInformation($"CSV header: {lines[0]}");

                    // Process data lines
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        try
                        {
                            var parts = line.Split(',');
                            if (parts.Length < 20) continue; // Skip lines with insufficient data

                            var result = new BacResult
                            {
                                Code = parts[0],
                                School = parts.Length > 1 ? parts[1] : null,
                                County = parts.Length > 2 ? parts[2] : null,
                                PreviousProm = parts.Length > 3 && bool.TryParse(parts[3], out bool prevProm) ? prevProm : false,
                                EducationForm = parts.Length > 4 ? parts[4] : null,
                                Specialization = parts.Length > 5 ? parts[5] : null,
                                RomanianLevel = parts.Length > 6 ? parts[6] : null,
                                NativeLevel = parts.Length > 7 ? parts[7] : null,
                                NativeSubject = parts.Length > 8 ? parts[8] : null,
                                ExtraLanguage = parts.Length > 9 ? parts[9] : null,
                                ExtraLanguageGrades = parts.Length > 10 ? parts[10] : null,
                                Tic = parts.Length > 11 ? parts[11] : null,
                                MandatorySubject = parts.Length > 12 ? parts[12] : null,
                                ChosenSubject = parts.Length > 13 ? parts[13] : null,
                                Average = parts.Length > 14 && double.TryParse(parts[14], out double avg) ? avg : 0,
                                Passed = parts.Length > 15 && int.TryParse(parts[15], out int passed) ? passed : 0,
                                Year = parts.Length > 16 && int.TryParse(parts[16], out int yr) ? yr : year,
                                FullSchoolName = parts.Length > 17 ? parts[17] : null,
                                City = parts.Length > 18 ? parts[18] : null,
                                Medium = parts.Length > 19 ? parts[19] : null,
                                SchoolCode = parts.Length > 20 ? parts[20] : null,
                                NameHu = parts.Length > 21 ? parts[21] : null,
                                CityRo = parts.Length > 22 ? parts[22] : null,
                                CityHu = parts.Length > 23 ? parts[23] : null,
                                CountyRo = parts.Length > 24 ? parts[24] : null,
                                CountyHu = parts.Length > 25 ? parts[25] : null,
                                SchoolType = parts.Length > 26 ? parts[26] : null,
                                MandatoryGradeInitial = parts.Length > 27 && double.TryParse(parts[27], out double mgInit) ? mgInit : 0,
                                MandatoryGradeFinal = parts.Length > 28 && double.TryParse(parts[28], out double mgFinal) ? mgFinal : 0,
                                MandatoryGradeContestation = parts.Length > 29 && double.TryParse(parts[29], out double mgCont) ? mgCont : 0,
                                ChosenGradeInitial = parts.Length > 30 && double.TryParse(parts[30], out double cgInit) ? cgInit : 0,
                                ChosenGradeFinal = parts.Length > 31 && double.TryParse(parts[31], out double cgFinal) ? cgFinal : 0,
                                ChosenGradeContestation = parts.Length > 32 && double.TryParse(parts[32], out double cgCont) ? cgCont : 0,
                                RomanianGradeInitial = parts.Length > 33 && double.TryParse(parts[33], out double rgInit) ? rgInit : 0,
                                RomanianGradeFinal = parts.Length > 34 && double.TryParse(parts[34], out double rgFinal) ? rgFinal : 0,
                                RomanianGradeContestation = parts.Length > 35 && double.TryParse(parts[35], out double rgCont) ? rgCont : 0,
                                NativeGradeInitial = parts.Length > 36 && double.TryParse(parts[36], out double ngInit) ? ngInit : 0,
                                NativeGradeFinal = parts.Length > 37 && double.TryParse(parts[37], out double ngFinal) ? ngFinal : 0,
                                NativeGradeContestation = parts.Length > 38 && double.TryParse(parts[38], out double ngCont) ? ngCont : 0,
                                Nationality = parts.Length > 39 ? parts[39] : null,
                                SpecializationType = parts.Length > 40 ? parts[40] : null
                            };

                            results.Add(result);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Error parsing line {i}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in manual parsing: {ex.Message}");
            }

            _logger.LogInformation($"Manual parsing completed with {results.Count} results");
            return results;
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            string dataDir = Path.Combine(_environment.WebRootPath, "data");
            _logger.LogInformation($"Looking for CSV files in: {dataDir}");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(dataDir))
            {
                _logger.LogWarning($"Data directory not found. Creating: {dataDir}");
                Directory.CreateDirectory(dataDir);
                return new List<int>();
            }

            var files = Directory.GetFiles(dataDir, "*_final.csv");
            _logger.LogInformation($"Found {files.Length} CSV files in {dataDir}");

            var years = new List<int>();

            foreach (var file in files)
            {
                _logger.LogInformation($"Processing file: {Path.GetFileName(file)}");
                var fileName = Path.GetFileNameWithoutExtension(file);
                var parts = fileName.Split('_');
                if (parts.Length > 0 && int.TryParse(parts[0], out int year))
                {
                    _logger.LogInformation($"Found year: {year}");
                    years.Add(year);
                }
                else
                {
                    _logger.LogWarning($"Could not parse year from filename: {fileName}");
                }
            }

            return await Task.FromResult(years.OrderBy(y => y).ToList());
        }
    }

    // This class maps the CSV fields to the BacResult properties
    public sealed class BacResultMap : ClassMap<BacResult>
    {
        public BacResultMap()
        {
            Map(m => m.Code).Name("code");
            Map(m => m.School).Name("school");
            Map(m => m.County).Name("county");
            Map(m => m.PreviousProm).Name("previous_prom").Default(false);
            Map(m => m.EducationForm).Name("education_form");
            Map(m => m.Specialization).Name("specialization");
            Map(m => m.RomanianLevel).Name("romanian_level");
            Map(m => m.NativeLevel).Name("native_level");
            Map(m => m.NativeSubject).Name("native_subject");
            Map(m => m.ExtraLanguage).Name("extra_language");
            Map(m => m.ExtraLanguageGrades).Name("extra_language_grades");
            Map(m => m.Tic).Name("tic");
            Map(m => m.MandatorySubject).Name("mandatory_subject");
            Map(m => m.ChosenSubject).Name("chosen_subject");
            Map(m => m.Average).Name("avg").Default(0);
            Map(m => m.Passed).Name("passed").Default(0);
            Map(m => m.Year).Name("year").Default(0);
            Map(m => m.FullSchoolName).Name("full_school_name");
            Map(m => m.City).Name("city");
            Map(m => m.Medium).Name("medium");
            Map(m => m.SchoolCode).Name("school_code");
            Map(m => m.NameHu).Name("name_hu");
            Map(m => m.CityRo).Name("city_ro");
            Map(m => m.CityHu).Name("city_hu");
            Map(m => m.CountyRo).Name("county_ro");
            Map(m => m.CountyHu).Name("county_hu");
            Map(m => m.SchoolType).Name("school_type");
            Map(m => m.MandatoryGradeInitial).Name("mandatory_grade_initial").Default(0);
            Map(m => m.MandatoryGradeFinal).Name("mandatory_grade_final").Default(0);
            Map(m => m.MandatoryGradeContestation).Name("mandatory_grade_contestation").Default(0);
            Map(m => m.ChosenGradeInitial).Name("chosen_grade_initial").Default(0);
            Map(m => m.ChosenGradeFinal).Name("chosen_grade_final").Default(0);
            Map(m => m.ChosenGradeContestation).Name("chosen_grade_contestation").Default(0);
            Map(m => m.RomanianGradeInitial).Name("romanian_grade_initial").Default(0);
            Map(m => m.RomanianGradeFinal).Name("romanian_grade_final").Default(0);
            Map(m => m.RomanianGradeContestation).Name("romanian_grade_contestation").Default(0);
            Map(m => m.NativeGradeInitial).Name("native_grade_initial").Default(0);
            Map(m => m.NativeGradeFinal).Name("native_grade_final").Default(0);
            Map(m => m.NativeGradeContestation).Name("native_grade_contestation").Default(0);
            Map(m => m.Nationality).Name("nationality");
            Map(m => m.SpecializationType).Name("specialization_type");
        }
    }
}