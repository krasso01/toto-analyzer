# Toto Analyzer

## Description
Toto Analyzer is a .NET console application that downloads historical Sport Toto 6/49 draw data from the official public website, parses both TXT and DOCX files, and provides statistical analysis and ASCII visualizations directly in the console.

The application supports:
- loading draw files from the official Toto page
- parsing both legacy and newer TXT formats
- parsing DOCX files
- combining all parsed draws into one collection
- statistical analysis with LINQ
- console visualizations
- interactive menu with period filtering

---

## Features

### Data Loading
- Downloads the main Toto statistics page
- Extracts file URLs for both `.txt` and `.docx` files
- Downloads TXT files as text
- Downloads DOCX files as byte arrays

### Parsing
- Parses TXT files in multiple formats
- Extracts text from DOCX files using Open XML SDK
- Converts all valid records into a unified `TotoDraw` model

### Statistics
- Top N most frequent numbers
- Hot pairs (most frequent number pairs)
- Distribution by tens:
  - 1-10
  - 11-20
  - 21-30
  - 31-40
  - 41-49

### Visualizations
- Horizontal ASCII bar chart
- 7x7 heat map using console colors

### User Interface
- Interactive console menu
- Period selection by year range
- Input validation
- Menu-based access to statistics and visualizations

---

## Technologies Used
- C#
- .NET 8
- LINQ
- HttpClient
- Regex
- Open XML SDK (`DocumentFormat.OpenXml`)
- Console application architecture

---

## Project Structure

- `Models/`
  - `TotoDraw.cs` – internal data model for a single draw

- `Services/`
  - `DataLoader.cs` – downloads HTML and files, extracts URLs, extracts year from URL
  - `Statistics.cs` – statistical calculations with LINQ
  - `Visualizer.cs` – console visualizations

- `Parsers/`
  - `TxtParser.cs` – parses TXT files in different layouts
  - `DocxParser.cs` – extracts text from DOCX and parses draw data

- `UI/`
  - `Menu.cs` – interactive console menu

- `Program.cs`
  - entry point
  - initializes loaders, parsers, and menu
  - loads all draws before starting the UI

---

## Internal Data Model

Each parsed draw is represented by the `TotoDraw` model with:
- `Year`
- `DrawNumber`
- `CombinationIndex`
- `WinningNumbers`

This allows all TXT and DOCX records to be stored in one common collection and analyzed uniformly.

---

## How It Works

1. The application downloads the Toto 6/49 statistics page
2. It extracts all relevant TXT and DOCX file links
3. Each file is downloaded and parsed with the appropriate parser
4. All parsed draws are combined into one collection
5. The user selects a time period and analysis from the menu
6. Results are shown as raw statistics or ASCII visualizations

---

## Example Functionalities
- Show the 10 most frequent numbers for all years
- Show the top hot pairs in a selected period
- Show number distribution by tens
- Display a bar chart of top numbers
- Display a 7x7 heat map for the selected data

---

## How to Run
1. Open the project in Visual Studio
2. Restore NuGet packages
3. Build the solution
4. Run the console application
5. Use the interactive menu to explore the statistics

---

## Notes
- The project works with real public Toto data
- Both TXT and DOCX file formats are supported
- Some TXT files use different layouts, so multiple parsing strategies are implemented
- DOCX files are processed through text extraction with Open XML SDK before parsing

---

## Educational Goals
This project demonstrates practical work with:
- real-world external data
- HTTP requests
- text and document parsing
- data modeling
- LINQ analysis
- console UI design
- Git/GitHub workflow

---

## Future Improvements
- export results to file
- richer console UI
- additional statistics
- improved data validation
- caching downloaded files locally

## Author
Krasimir Blagov - krasso01