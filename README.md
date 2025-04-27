# Romanian Baccalaureate Results Analyzer

![BAC Results Analyzer](https://img.shields.io/badge/BAC%20Results-Analyzer-blue)
![Blazor](https://img.shields.io/badge/Blazor-NET%208.0-purple)
![CSV](https://img.shields.io/badge/CSV-Analysis-green)
![License](https://img.shields.io/badge/license-MIT-orange)

A powerful, modern web application built with Blazor for analyzing Romanian Baccalaureate examination results from 2014 to 2023.

![App Screenshot](https://via.placeholder.com/800x400.png?text=BAC+Results+Analyzer)

## 🌟 Features

- **Comprehensive Data Exploration**: Browse and search through 10 years of Romanian Baccalaureate results.
- **Interactive Statistics**: View pass rates, average scores, and other key metrics at a glance.
- **Advanced Filtering**: Search by school, county, city, or specialization to find specific results.
- **Year-by-Year Comparison**: Compare statistics between different years to identify trends.
- **Detailed Analysis**: Drill down into specific schools, counties, or specializations.
- **Data Upload**: Easily upload and integrate new CSV data files up to 100MB.
- **Responsive Design**: Works seamlessly on desktop and mobile devices.

## 📋 Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or Visual Studio Code
- Basic understanding of Blazor and C#

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/bac-results-analyzer.git
cd bac-results-analyzer
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
dotnet run
```

The application will be available at `https://localhost:7146` or `http://localhost:5041`.

## 📊 Data Structure

The application works with CSV files containing the following data structure:

```
code,school,county,previous_prom,education_form,specialization,romanian_level,native_level,native_subject,extra_language,extra_language_grades,tic,mandatory_subject,chosen_subject,avg,passed,year,full_school_name,city,medium,school_code,name_hu,city_ro,city_hu,county_ro,county_hu,school_type,mandatory_grade_initial,mandatory_grade_final,mandatory_grade_contestation,chosen_grade_initial,chosen_grade_final,chosen_grade_contestation,romanian_grade_initial,romanian_grade_final,romanian_grade_contestation,native_grade_initial,native_grade_final,native_grade_contestation,nationality,specialization_type
```

## 📁 Project Structure

```
projekt/
├── Components/          # Blazor components
│   ├── Pages/           # Application pages
│   └── Shared/          # Reusable components
├── Models/              # Data models
├── Services/            # Business logic & data access
├── wwwroot/             # Static assets
│   ├── css/             # Stylesheets
│   └── data/            # CSV data files
└── Program.cs           # Application entry point
```

## 🔍 Key Components

- **BacResults**: Main page for browsing examination results
- **BacStatistics**: Component for displaying summarized statistics
- **BacDetailedAnalysis**: Detailed analysis of specific entities
- **BacCompare**: Year-to-year comparison tool
- **BacUpload**: CSV file upload and management
- **CsvService**: Handles loading and parsing of CSV data

## 🔧 Configuration

### File Size Limits

By default, the application supports CSV files up to 100MB. This can be modified in the `Program.cs` file:

```csharp
options.Limits.MaxRequestBodySize = 104857600; // 100 MB
```

### Data Directory

CSV files are stored in the `/wwwroot/data/` directory with the naming convention `YYYY_final.csv`.

## 📚 Example Usage

### Viewing BAC Results

1. Navigate to the "BAC Results" page
2. Select a year from the sidebar
3. Use the search box to filter results
4. Click on a school, county, or specialization to see detailed analysis

### Comparing Years

1. Navigate to the "Compare Years" page
2. Select exactly 2 years to compare
3. Choose between Overall, Specialization, or County comparison
4. View the changes and trends between the selected years

### Uploading New Data

1. Navigate to the "Upload Data" page
2. Select the year for the data
3. Choose a CSV file from your computer
4. Click "Upload File" and wait for processing

## 💡 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgements

- [CsvHelper](https://joshclose.github.io/CsvHelper/) - Used for CSV parsing
- [Bootstrap](https://getbootstrap.com/) - UI framework
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - Web framework

---

**Developed as a project for .NET 2025 @ Sapientia University**