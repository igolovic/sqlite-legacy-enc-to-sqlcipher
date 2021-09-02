# sqlite-legacy-enc-to-sqlcipher-1

Conversion of SQLite CryptoAPI-legacy-encrypted database to SqlCipher-encrypted database   
     
Libraries used:   
System.Data.SQLite 1.0.112.1 - official arhcived version - http://nuget.yuanbei.biz/feeds/Default/System.Data.SQLite.Core/1.0.112.1   
System.Data.SQLite 4.4.3.0 - sqlite with sqlcipher, unlicensed demo version - https://www.zetetic.net/sqlcipher/   
    
Application decrypts legacy System.Data.SQLite-encrypted database file in console application.   
It encrypts produced clear database file using SqlCipher in main Windows application.    
                 
Implementation with two process (console and Windows application) is used because of "SQL logic error - No such function: sqlcipher_export()" problem, where seemingly versions of System.Data.SQLite get mixed up.   
Once the old version of sqlite (in System.Data.SQLite 1.0.112.1 - pure sqlite) is executed for decryption of legacy database, new version of sqlite with sqlcipher (in System.Data.SQLite 4.4.3.0 - sqlite with sqlcipher, unlicensed demo version) cannot be used in process.   
Commands used to encrypt clear database into SqlCipher cannot succeed because of said error.   
    
Although Modules view shows correct System.Data.SQLite 4.4.3.0 loaded and not the old version (especially in experiment with separate AppDomains - https://github.com/igolovic/sqlite-legacy-enc-to-sqlcipher-appdomains-error).   
Error is thrown as if the sqlcipher_export does not exist.   
   
If decryption with old System.Data.SQLite 1.0.112.1 is performed in separate process and new encryption in another, everything works.   

Screenshot while running on Windows 10:
![screenshot](./screenshot.png?raw=true)
