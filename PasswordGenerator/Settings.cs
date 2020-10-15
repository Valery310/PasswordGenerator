using PasswordGenerator.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using static PasswordGenerator.MainWindow;

namespace PasswordGenerator
{
    public static class Setting
    {
        static public MainWindow mainWindow;
        static public TypeLogin typeLogin { get; private set; }
        static public string Domain { get; set; }
        static public string Login { get; set; }
        static public string Password { get; set; }
        // public Login;

        public static void ReadSetting() { }

        public static void WriteSetting() { }

        public static void CheckSettings()//приводит активност ьконтролов в соотвествие с настройками
        {
            mainWindow.tbxServer.Text = PasswordGenerator.Properties.Settings.Default.Domain;
            mainWindow.tbxLogin.Text = Settings.Default.Login;
            mainWindow.pbxPassword.Password = Encryption.Decrypt(Settings.Default.Password);
            switch (Settings.Default.TypeLogin)
            {
                case (int)TypeLogin.LoginPass:
                    mainWindow.tbxLogin.IsEnabled = true;
                    mainWindow.pbxPassword.IsEnabled = true;
                    break;
                case (int)TypeLogin.CurrentUser:
                    mainWindow.tbxLogin.IsEnabled = false;
                    mainWindow.pbxPassword.IsEnabled = false;
                    break;
                default:
                    mainWindow.tbxLogin.IsEnabled = false;
                    mainWindow.pbxPassword.IsEnabled = false;
                    break;
            }
        }

        public static bool ApplySetting() { return true; }
        public static bool WriteJsonSetting() { return true; }
        public static bool ReadJsonSetting() { return true; }

    }
}
