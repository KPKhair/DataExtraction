# DataExtraction

A clean and extensible C# application for extracting and transforming data from multiple bank CSV files.  
Built using **Hexagonal Architecture** and **SOLID principles** for maintainability and easy extension.

---

## Features
- Handles different bank CSV formats and headers  
- Cleans and preprocesses invalid or inconsistent CSV files  
- Extracts both simple and complex fields (e.g., parses `AlgoParams` → `ContractSize`)  
- Easily extendable — add new banks by creating new parser classes  
- Uses dependency injection for pluggability  

---

## How It Works
1. The application accepts:
   DataExtraction.exe BankName InputFilePath

   e.g. DataExtraction.exe Barclay C:\Banks\Barclay\DataExtractor_Example_Input.csv
3. There is parser of Bank name Barclay created in this application for testing.
4. It automatically detects and uses the correct parser for the given bank.  
5. The CSV file is cleaned and processed.  
6. The output CSV is saved in the same folder with `_Output` added to the filename. 

---
  
## Technologies
1. .NET 8 / C# 12
2. CsvHelper
3. xUnit / NUnit for testing
