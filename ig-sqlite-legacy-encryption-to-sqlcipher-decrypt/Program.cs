using System;

namespace ig_sqlite_legacy_encryption_to_sqlcipher_decrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            var password = args[1];
            Console.WriteLine("Source database (legacy encrypted database): {0}", path);

            Decrypt.DecryptLegacy(path, password, out string clearFilePath);

            Console.WriteLine("Destination database (clear database): {0}", clearFilePath);
        }
    }
}
