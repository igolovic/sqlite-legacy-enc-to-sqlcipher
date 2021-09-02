using System;
using System.IO;
using System.Windows;

namespace ig_sqlite_legacy_to_sqlcipher_common
{
    public static class Common
    {
        public static string GetClearDatabaseFilePath(string legacyEncryptionFilePath)
        {
            return  Path.Combine(
                    Path.GetDirectoryName(legacyEncryptionFilePath),
                    Path.GetFileNameWithoutExtension(legacyEncryptionFilePath) + ".clear" + Path.GetExtension(legacyEncryptionFilePath));
        }

        public static string GetSqlCipherEncryptedDatabasePath(string clearFilePath)
        {
            return Path.Combine(
                   Path.GetDirectoryName(clearFilePath),
                   Path.GetFileNameWithoutExtension(clearFilePath) + ".sqlcipher" + Path.GetExtension(clearFilePath));
        }

        public static void DeleteCreatedDatabaseFiles(string legacyEncryptionFilePath)
        {
            // Delete .clear.sqlite file if it exists
            var clearFilePath = GetClearDatabaseFilePath(legacyEncryptionFilePath);
            if (File.Exists(clearFilePath))
            {
                try
                {
                    File.Delete(clearFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Delete .clear.sqlcipher.sqlite file if it exists
            var sqlCipherPath = GetSqlCipherEncryptedDatabasePath(clearFilePath);
            if (File.Exists(sqlCipherPath))
            {
                try
                {
                    File.Delete(sqlCipherPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
