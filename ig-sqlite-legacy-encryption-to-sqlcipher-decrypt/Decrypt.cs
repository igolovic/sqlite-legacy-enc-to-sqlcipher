using System.Data.SQLite;
using System.IO;

using ig_sqlite_legacy_to_sqlcipher_common;

namespace ig_sqlite_legacy_encryption_to_sqlcipher_decrypt
{
    public class Decrypt
    {
        public static bool DecryptLegacy(string legacyEncryptionFilePath, string password, out string clearFilePath)
        {
            clearFilePath = null;

            var legacyEncrtyptionFile = new FileInfo(legacyEncryptionFilePath);
            if (legacyEncrtyptionFile.Exists == false)
                return false;

            clearFilePath = Common.GetClearDatabaseFilePath(legacyEncryptionFilePath);

            File.Copy(legacyEncryptionFilePath, clearFilePath, true);

            using (var connection = new SQLiteConnection(GetSQLCEConnectionStringLegacyEncryptedDb(clearFilePath, password)))
            using (var commandKey = new SQLiteCommand($"PRAGMA key = {password};", connection))
            using (var commandRekey = new SQLiteCommand($"PRAGMA rekey = '';", connection))
            {
                connection.Open();

                commandKey.ExecuteNonQuery();
                commandRekey.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }



        private static string GetSQLCEConnectionStringLegacyEncryptedDb(string filePath, string password)
        {
            var connectionString = new SQLiteConnectionStringBuilder()
            {
                DataSource = filePath,
                Password = password
            };

            return connectionString + ";";
        }
    }
}
