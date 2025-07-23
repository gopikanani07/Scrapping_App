using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebScrapper
{
    internal class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveItems(List<ScrapedItem> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var createTableSql = @"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Auctions')
                    BEGIN
                        CREATE TABLE Auctions (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Title NVARCHAR(MAX),
                            Description NVARCHAR(MAX),
                            ImageUrl NVARCHAR(MAX),
                            Link NVARCHAR(MAX),
                            LotCount INT,
                            StartDate INT,
                            StartMonth NVARCHAR(50),
                            StartYear INT,
                            StartTime NVARCHAR(50),
                            EndDate INT,
                            EndMonth NVARCHAR(50),
                            EndYear INT,
                            EndTime NVARCHAR(50),
                            Location NVARCHAR(MAX)
                        )
                    END";

                connection.Execute(createTableSql);

              
                foreach (var item in items)
                {
                    try
                    {
                        var insertSql = @"
                            INSERT INTO Auctions (
                                Title, Description, ImageUrl, Link, LotCount,
                                StartDate, StartMonth, StartYear, StartTime,
                                EndDate, EndMonth, EndYear, EndTime, Location
                            )
                            VALUES (
                                @Title, @Description, @ImageUrl, @Link, @LotCount,
                                @StartDate, @StartMonth, @StartYear, @StartTime,
                                @EndDate, @EndMonth, @EndYear, @EndTime, @Location
                            );";

                        connection.Execute(insertSql, item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" Error saving item '{item.Title}': {ex.Message}");
                    }
                }
            }
        }

    }
}
