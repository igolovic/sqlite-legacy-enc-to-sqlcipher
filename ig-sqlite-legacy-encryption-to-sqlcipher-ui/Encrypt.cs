using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ig_sqlite_legacy_to_sqlcipher_common;

namespace ig_sqlite_legacy_to_sqlcipher_ui
{
    public static class Encrypt
    {
        public static bool EncryptSqlCipher(string clearFilePath, string pasword)
        {
            string sqlCipherPath = Common.GetSqlCipherEncryptedDatabasePath(clearFilePath);

            // Delete .clear.sqlcipher file if it exists
            if (File.Exists(sqlCipherPath))
            {
                try
                {
                    File.Delete(sqlCipherPath);
                }
                catch
                {
                    // If some process is using this file, e.g. database editor
                }
            }

            var queryEncrypt = $"ATTACH DATABASE '{sqlCipherPath}' AS encrypted KEY '{pasword}'; SELECT sqlcipher_export('encrypted'); DETACH DATABASE encrypted;";
            var queryLicense = $"PRAGMA cipher_license = 'OmNpZDowMDEzbzAwMDAyV3NOV2dBQU46cGxhdGZvcm06MzI6ZXhwaXJlOjE2MzA3NjM1NjI6dmVyc2lvbjoxOmhtYWM6ODE0NjlkODM3YjBiOWIzYzEyOTA5YzhkNGNhN2M4OWYyNGE1ZGM2MQ=='";

            using (var connection = new SQLiteConnection(GetSQLCEConnectionStringClearDb(clearFilePath)))
            using (var commandEncrypt = new SQLiteCommand(queryEncrypt, connection))
            using (var commandLicense = new SQLiteCommand(queryLicense, connection))
            {
                connection.Open();

                commandLicense.ExecuteNonQuery();
                commandEncrypt.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }

        private static string GetSQLCEConnectionStringClearDb(string filePath)
        {
            var connectionString = new SQLiteConnectionStringBuilder()
            {
                DataSource = filePath
            };

            return connectionString.ToString();
        }
    }
}
