# Sql Text Search Tool

A simple and efficient C# console application that scans all `varchar` and `nvarchar` columns in a SQL Server database to find where a specific text value appears.

## 🔧 Features
- Searches all tables and columns dynamically using `INFORMATION_SCHEMA.COLUMNS`
- Supports partial text matches (`LIKE '%text%'`)
- Writes search results to a `.txt` file
- Displays live progress in the console

## 🚀 Usage
1. Add your SQL Server connection string in the code.
2. Run the program.
3. Enter the text you want to search.
4. Check the generated `SearchResults.txt` file for matches.

## 📂 Output Example
