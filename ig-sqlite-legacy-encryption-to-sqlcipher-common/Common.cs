using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
