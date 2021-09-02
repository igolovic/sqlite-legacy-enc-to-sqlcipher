using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

using ig_sqlite_legacy_to_sqlcipher_common;

using Microsoft.Win32;

namespace ig_sqlite_legacy_to_sqlcipher_ui
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _sqliteLegacyEncryptionFilePath;
        public string SqliteLegacyEncryptionFilePath
        {
            get { return _sqliteLegacyEncryptionFilePath; }
            set
            {
                _sqliteLegacyEncryptionFilePath = value;
                OnPropertyChanged();
            }
        }

        private string _sqliteLegacyEncryptionPassword;
        public string SqliteLegacyEncryptionPassword
        {
            get { return _sqliteLegacyEncryptionPassword; }
            set
            {
                _sqliteLegacyEncryptionPassword = value;
                OnPropertyChanged();
            }
        }
        
        private string _clearPath;
        public string ClearPath
        {
            get { return _clearPath; }
            set
            {
                _clearPath = value;
                OnPropertyChanged();
            }
        }

        private string _encryptedPath;
        public string EncryptedPath
        {
            get { return _encryptedPath; }
            set
            {
                _encryptedPath = value;
                OnPropertyChanged();
            }
        }

        private ICommand _sqliteLegacyEncryptedFileSelectCommand;
        public ICommand SqliteLegacyEncryptedFileSelectCommand
        {
            get
            {
                return _sqliteLegacyEncryptedFileSelectCommand ?? (_sqliteLegacyEncryptedFileSelectCommand = new CommandHandler(() => SelectSqliteLegacyEncryptionFile(), () => true));
            }
        }

        private ICommand _sqliteLegacyEncryptionFileConvertCommand;
        public ICommand SqliteLegacyEncryptionFileConvertCommand
        {
            get
            {
                return _sqliteLegacyEncryptionFileConvertCommand ?? (_sqliteLegacyEncryptionFileConvertCommand = new CommandHandler(() => ConvertSqliteLegacyEncryptionFile(), () => CanExecuteConvertSqliteLegacyEncryptionFile));
            }
        }

        public bool CanExecuteConvertSqliteLegacyEncryptionFile
        {
            get
            {
                return String.IsNullOrWhiteSpace(SqliteLegacyEncryptionFilePath) == false 
                    && File.Exists(SqliteLegacyEncryptionFilePath)
                    && String.IsNullOrWhiteSpace(SqliteLegacyEncryptionPassword) == false;
            }
        }

        private void SelectSqliteLegacyEncryptionFile()
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.Filter = "(*.sqlite)|*.sqlite";

                if (dlg.ShowDialog() is true)
                {
                    SqliteLegacyEncryptionFilePath = dlg.FileName.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
                
        public void ConvertSqliteLegacyEncryptionFile()
        {
            try
            {
                /* Decrypt legacy encrypted file by starting console application that decrypts legacy System.Data.SQLite encryption.
                 * 
                 * This is used because of "SQL logic error - No such function: sqlcipher_export()" problem, where seemingly versions
                 * of System.Data.SQLite get mixed up. 
                 * Once the old version of sqlite (in System.Data.SQLite 1.0.112.1 - pure sqlite) is executed for decryption of legacy database, 
                 * new version of sqlite with sqlcipher (in System.Data.SQLite 4.4.3.0 - sqlite with sqlcipher) cannot be used in process.
                 * Commands used to encrypt clear database into SqlCipher cannot succeed because of said error.
                 * 
                 * Although Modules view shows correct System.Data.SQLite 4.4.3.0 loaded and not the old version (especially in experiment in separate AppDomains).
                 * Error is thrown as if the sqlcipher_export does not exist.
                 * 
                 * If decryption with old System.Data.SQLite 1.0.112.1 is performed in separate process and new encryption in another, everything works.
                */

                Process.Start(@"DECRYPT\ig.sqlite-legacy-encryption-to-sqlcipher.decrypt.exe", $"\"{SqliteLegacyEncryptionFilePath}\" \"{SqliteLegacyEncryptionPassword}\"");

                var clearFilePath = Common.GetClearDatabaseFilePath(SqliteLegacyEncryptionFilePath);
                if (File.Exists(clearFilePath))
                {
                    ClearPath = clearFilePath;

                    var success = Encrypt.EncryptSqlCipher(clearFilePath, SqliteLegacyEncryptionPassword);
                    if (success)
                    {
                        var sqlCipherPath = Common.GetSqlCipherEncryptedDatabasePath(clearFilePath);
                        EncryptedPath = sqlCipherPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
