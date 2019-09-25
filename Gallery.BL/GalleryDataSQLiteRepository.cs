using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Gallery.BL
{
    public class GalleryDataSQLiteRepository
    {
        private readonly string connectionString;
        private readonly string dbFileName = "sqliteDb.db";
        private readonly string table_galleryLocations = "GalleryLocations";

        public GalleryDataSQLiteRepository(string programPath)
        {
            string sqlitePath = Path.Combine(programPath, dbFileName);
            connectionString = $"Data Source={sqlitePath};";

            VerifyTablesExist();
        }

        public IEnumerable<GalleryLocation> RetrieveGalleryLocations()
        {
            IList<GalleryLocation> galleryLocations = new List<GalleryLocation>();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // SQL Command
                string sqlStatement = $"Select name, path from {table_galleryLocations}";
                using (SQLiteCommand command = new SQLiteCommand(sqlStatement, conn))
                {
                    // SQL Execute
                    SQLiteDataReader reader = command.ExecuteReader();

                    // SQL Read
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string path = reader.GetString(1);

                        GalleryLocation galleryLocation = new GalleryLocation()
                        {
                            Name = name,
                            Path = path
                        };

                        galleryLocations.Add(galleryLocation);
                    }

                    reader.Close();
                }
            }

            return galleryLocations;
        }

        public void AddGalleryLocation(string name, string path)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // SQL Command
                string sqlStatement = $"INSERT INTO {table_galleryLocations} (name, path) VALUES (@name, @path)";
                using (SQLiteCommand command = new SQLiteCommand(sqlStatement, conn))
                {
                    // SQL Parameters
                    var param_name = new SQLiteParameter("@name", DbType.String) { Value = name };
                    var param_path = new SQLiteParameter("@path", DbType.String) { Value = path };
                    command.Parameters.Add(param_name);
                    command.Parameters.Add(param_path);

                    // SQL Execute
                    command.ExecuteNonQuery();
                }
            }
        }

        private void VerifyTablesExist()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string sqlStatement = $"CREATE TABLE IF NOT EXISTS {table_galleryLocations}(" +
                    "id INTEGER PRIMARY KEY," +
                    "name TEXT NOT NULL," +
                    "path TEXT NOT NULL" +
                    ")";

                using (SQLiteCommand command = new SQLiteCommand(sqlStatement, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}