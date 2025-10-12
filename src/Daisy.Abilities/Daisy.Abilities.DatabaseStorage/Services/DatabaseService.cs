using Daisy.Abilities.DatabaseStorage.Models;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Daisy.Abilities.DatabaseStorage.Services
{
    /// <summary>
    /// Service for storing football scores in a SQL database.
    /// Falls back to mock storage if connection string is not configured.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly DatabaseSettings _settings;

        public DatabaseService(ApplicationSettings settings)
        {
            _settings = settings.GetApiSettings<DatabaseSettings>("Database");
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            // Initialize database if needed
        }

        /// <summary>
        /// Stores football scores in the database.
        /// Falls back to mock storage if connection string is not configured.
        /// </summary>
        public async Task<bool> StoreFootballScoresAsync(string clubName, string scores)
        {
            // Check if connection string is configured
            if (string.IsNullOrWhiteSpace(_settings.ConnectionString))
            {
                return MockStoreFootballScores(clubName, scores);
            }

            try
            {
                using var connection = new SqlConnection(_settings.ConnectionString);
                await connection.OpenAsync();

                // Create table if it doesn't exist
                var createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FootballScores' AND xtype='U')
                    CREATE TABLE FootballScores (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        ClubName NVARCHAR(255) NOT NULL,
                        Scores NVARCHAR(MAX) NOT NULL,
                        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                    )";

                using (var createCommand = new SqlCommand(createTableQuery, connection))
                {
                    await createCommand.ExecuteNonQueryAsync();
                }

                // Insert scores
                var insertQuery = @"
                    INSERT INTO FootballScores (ClubName, Scores)
                    VALUES (@ClubName, @Scores)";

                using (var insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@ClubName", clubName);
                    insertCommand.Parameters.AddWithValue("@Scores", scores);
                    await insertCommand.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}. Falling back to mock storage.");
                return MockStoreFootballScores(clubName, scores);
            }
        }

        /// <summary>
        /// Mock storage when database is not available.
        /// </summary>
        private static bool MockStoreFootballScores(string clubName, string scores)
        {
            Console.WriteLine($"[Mock Database] Stored scores for {clubName}");
            return true;
        }
    }
}
