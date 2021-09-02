using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                OnPropertyChanged(nameof(SqliteLegacyEncryptionFilePath));
            }
        }

        private string _sqliteLegacyEncryptionPassword;
        public string SqliteLegacyEncryptionPassword
        {
            get { return _sqliteLegacyEncryptionPassword; }
            set
            {
                _sqliteLegacyEncryptionPassword = value;
                OnPropertyChanged(nameof(SqliteLegacyEncryptionPassword));
            }
        }
        
        private string _clearPath;
        public string ClearPath
        {
            get { return _clearPath; }
            set
            {
                _clearPath = value;
                OnPropertyChanged(nameof(ClearPath));
            }
        }

        private string _encryptedPath;
        public string EncryptedPath
        {
            get { return _encryptedPath; }
            set
            {
                _encryptedPath = value;
                OnPropertyChanged(nameof(EncryptedPath));
            }
        }

        private string _statusMessage = "Please populate path to legacy encrypted file and password";
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
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
                
        public async void ConvertSqliteLegacyEncryptionFile()
        {
            try
            {
                StatusMessage = String.Concat(StatusMessage, Environment.NewLine, DateTime.Now.ToLongTimeString(), " - Deleting .clear.sqlite and .clear.sqlcipher.sqlite files if any are found...");

                Common.DeleteCreatedDatabaseFiles(SqliteLegacyEncryptionFilePath);

                StatusMessage = String.Concat(StatusMessage, Environment.NewLine, DateTime.Now.ToLongTimeString(), " - Please wait while decryption process finishes...");

                await Task.Run(() => {
                    var process = Process.Start(@"DECRYPT\ig.sqlite-legacy-encryption-to-sqlcipher.decrypt.exe", $"\"{SqliteLegacyEncryptionFilePath}\" \"{SqliteLegacyEncryptionPassword}\"");
                    process.WaitForExit();
                });

                StatusMessage = String.Concat(StatusMessage, Environment.NewLine, DateTime.Now.ToLongTimeString(), " - Decryption process finished.");

                var clearFilePath = Common.GetClearDatabaseFilePath(SqliteLegacyEncryptionFilePath);
                if (File.Exists(clearFilePath))
                {
                    ClearPath = clearFilePath;
                    StatusMessage = String.Concat(StatusMessage, Environment.NewLine, DateTime.Now.ToLongTimeString(), " - Please wait while encryption process finishes...");

                    await Task.Run(() =>
                    {
                        var success = Encrypt.EncryptSqlCipher(clearFilePath, SqliteLegacyEncryptionPassword);
                        if (success)
                        {
                            var sqlCipherPath = Common.GetSqlCipherEncryptedDatabasePath(clearFilePath);

                            EncryptedPath = sqlCipherPath;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                StatusMessage = String.Concat(StatusMessage, Environment.NewLine, DateTime.Now.ToLongTimeString(), " - Encryption process finished.");
                            });
                        }
                    });
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
