using System.Data.SQLite;
using System.IO;

using ig_sqlite_legacy_to_sqlcipher_common;

namespace ig_sqlite_legacy_to_sqlcipher_ui
{
    public static class Encrypt
    {
        public static bool EncryptSqlCipher(string clearFilePath, string pasword)
        {
            var clearFile = new FileInfo(clearFilePath);
            if (clearFile.Exists == false)
                return false;

            var sqlCipherPath = Common.GetSqlCipherEncryptedDatabasePath(clearFilePath);

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
