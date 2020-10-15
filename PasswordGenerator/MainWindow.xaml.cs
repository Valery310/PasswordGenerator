using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using PasswordGenerator.Properties;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Media.Animation;

namespace PasswordGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Start();
        }

        public static ObservableCollection<Group> TargetGroup { get; set; }
        public static ObservableCollection<Group> ExcludedGroup { get; set; }
        private static ObservableCollection<Group> _Groups;
        public ObservableCollection<Group> Groups
        {
            get { return _Groups; }
            set { _Groups = value; }
        }
        public static ObservableCollection<Users> UsersList;
        public static DomainUsers domainUsers { get; set; }
        public static FileStream fs { get; set; }
        public static event EventHandler CheckConnectDomain;

        public void Start()
        {
            Setting.mainWindow = this;
            Setting.CheckSettings();

            Connect.ResultTestConnection += WriteConsole;
            Password.ResultPass += WriteConsole;
            Table.ResultTable += WriteConsole;
            DomainUsers.ResultDomainUsers += WriteConsole;
            Connect.ResultTestConnection += WriteLog;
            Password.ResultPass += WriteLog;
            Table.ResultTable += WriteLog;
            DomainUsers.ResultDomainUsers += WriteLog;

            Connect.TestConnection(Settings.Default.Domain, (TypeLogin)Settings.Default.TypeLogin);

            CheckConnectDomain += MainWindow_CheckConnectDomain;
            domainUsers = new DomainUsers(Settings.Default.Domain, (TypeLogin)Settings.Default.TypeLogin);
            _Groups = new ObservableCollection<Group>();
            cmbAddRowTargetGroup.ItemsSource = Groups;

            this.DataContext = this;
            cmbAddRowExcludedGroup.ItemsSource = Groups;
            TargetGroup = new ObservableCollection<Group>();
            TargetGroup.CollectionChanged += TargetGroup_CollectionChanged;
            ExcludedGroup = new ObservableCollection<Group>();
            ExcludedGroup.CollectionChanged += ExcludedGroup_CollectionChanged;
            dgTargetGroup.ItemsSource = TargetGroup;
            dgExcludedtGroup.ItemsSource = ExcludedGroup;
            dgUsers.ItemsSource = UsersList;
        }

        private void MainWindow_CheckConnectDomain(object sender, EventArgs e)
        {
            Connect.TestConnection(Settings.Default.Domain, (TypeLogin)Settings.Default.TypeLogin);
        }

       

        private void TargetGroup_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //   MessageBox.Show("Группа добавлена");
        }

        private void ExcludedGroup_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //  MessageBox.Show("Группа исключена");
            //  throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "BUILTIN\\Users")]
        static void ShowMessage()
        {
            Console.WriteLine("Текущий принципиал зарегистрировался локально и является членом группы Users");
        }

        private void btnAddRowExcludedGroup_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAddRowExcludedGroup.SelectedItem as Group != null)
            {
                ExcludedGroup.Add(cmbAddRowExcludedGroup.SelectedItem as Group);
                Groups.Remove(cmbAddRowExcludedGroup.SelectedItem as Group);
            }
        }

        private void btnAddRowTargetGroup_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAddRowTargetGroup.SelectedItem as Group != null)
            {
                TargetGroup.Add(cmbAddRowTargetGroup.SelectedItem as Group);
                Groups.Remove(cmbAddRowTargetGroup.SelectedItem as Group);
            }
        }

        private void btnDelRowTargetGroup_Click(object sender, RoutedEventArgs e)
        {
            Groups.Add(dgTargetGroup.SelectedItem as Group);
            Groups = new ObservableCollection<Group>(Groups.OrderBy(i => i));
            cmbAddRowTargetGroup.ItemsSource = Groups;
            cmbAddRowExcludedGroup.ItemsSource = Groups;
            TargetGroup.Remove(dgTargetGroup.SelectedItem as Group);
        }

        private void btnDelRowExcludedGroup_Click(object sender, RoutedEventArgs e)
        {
            Groups.Add(dgExcludedtGroup.SelectedItem as Group);
            Groups = new ObservableCollection<Group>(Groups.OrderBy(i => i));
            cmbAddRowTargetGroup.ItemsSource = Groups;
            cmbAddRowExcludedGroup.ItemsSource = Groups;
            ExcludedGroup.Remove(dgExcludedtGroup.SelectedItem as Group);
        }

        private void btnDelRowUsers_Click(object sender, RoutedEventArgs e)
        {
            var user = dgUsers?.SelectedItem as Users;           
            Predicate<UserPrincipal> FindUserPrincipal = delegate (UserPrincipal x) { return x.UserPrincipalName == user.UserPrincipalName; };
            Users.UsersPrincip.Remove(Users.UsersPrincip.Find(FindUserPrincipal));
            UsersList.Remove(user);
            // Users.UsersPrincip.Remove(Users.UsersPrincip.Find((x) => { return x.UserPrincipalName == user.UserPrincipalName; }));
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Domain = tbxServer.Text;
            Settings.Default.Login = tbxLogin.Text;
            Settings.Default.Password = Encryption.Encrypt(pbxPassword.Password);
            Settings.Default.Save();
        }

        private void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            LogoPas login = new LogoPas() { Login = tbxLogin.Text, Password = pbxPassword.Password };
            Connect.TestConnection(tbxServer.Text, (TypeLogin)int.Parse((string)((ComboBoxItem)((ComboBox)cmbbxTypeLogin).SelectedItem).Tag), login);
        }



      

        public static bool ConnectionDomain(string server)
        {
            PrincipalContext testDomain = new PrincipalContext(ContextType.Domain, server);
            return false;
        }

        public static bool ConnectionDomain(TypeLogin typeLogin, string server, string Login, string password)
        {
            PrincipalContext testDomain = new PrincipalContext(ContextType.Domain, server, Login, password);
            return false;
        }

     

        private async void btnGetDomainUsers_Click(object sender, RoutedEventArgs e)
        {
            UsersList = new ObservableCollection<Users>();           
            foreach (var user in await domainUsers.GetUsers(TargetGroup, ExcludedGroup))
            {
                UsersList.Add(new Users(user.DisplayName, user.UserPrincipalName));
            }
            UsersList = new ObservableCollection<Users>(UsersList.OrderBy(i => i));
            dgUsers.ItemsSource = UsersList;
        }

        private async void btnGetDomainGroups_Click(object sender, RoutedEventArgs e)
        {
            Groups = await domainUsers.GetGroup();
            cmbAddRowExcludedGroup.ItemsSource = Groups;
            cmbAddRowTargetGroup.ItemsSource = Groups;
        }

        private async void btnGenerationPasswordAndSaveTable_Click(object sender, RoutedEventArgs e)
        {
            var users = Password.CreateNewPasswordUsers(Users.UsersPrincip);         
            Table.Write(Directory.GetCurrentDirectory(), await users);
        }

        private async void btnChangePasswordUsers_Click(object sender, RoutedEventArgs e)
        {
            List<Users> users = await Table.ReadAsync();
            DomainUsers.ChangePasswordUsers(users);
        }

        private void cmbbxTypeLogin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int temp = int.Parse((string)((ComboBoxItem)((ComboBox)sender).SelectedItem).Tag);
            if (tbxLogin !=null && pbxPassword != null)
            {
                switch (temp)
                {
                    case (int)TypeLogin.LoginPass:
                        tbxLogin.IsEnabled = true;
                        pbxPassword.IsEnabled = true;
                        break;
                    case (int)TypeLogin.CurrentUser:
                        tbxLogin.IsEnabled = false;
                        pbxPassword.IsEnabled = false;
                        break;
                    default:
                        tbxLogin.IsEnabled = false;
                        pbxPassword.IsEnabled = false;
                        break;
                }
            }       
        }

        public enum TypeLogin { CurrentUser, LoginPass }

        private void cmbAddRowTargetGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void WriteConsole(object o, EventArgsResultTest message)
        {
            ConsoleWindow(message.Message);
        }

        public void WriteConsole(object o, EventArgsResultPass message)
        {
            ConsoleWindow(message.Message);
        }

        public void WriteConsole(object o, EventArgsResultTable message)
        {
            ConsoleWindow(message.Message);
        }

        private void ConsoleWindow(string message) 
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
           {
               lblResult.Text += "\n" + message;
               svConsole.ScrollToEnd();
           });
            
        }

        public void WriteConsole(object o, EventArgsResultDomainUsers message)
        {
            ConsoleWindow(message.Message);
        }

        public void WriteLog(object o, EventArgsResultDomainUsers message)
        {
            WriteLog(message.Message);
        }

        public void WriteLog(object o, EventArgsResultTest message)
        {
            WriteLog(message.Message);
        }

        public void WriteLog(object o, EventArgsResultPass message)
        {
            WriteLog(message.Message);
        }

        public void WriteLog(object o, EventArgsResultTable message)
        {
            WriteLog(message.Message);
        }

        public static void FileCreate()
        {
            string path = Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("dd - MM - yyyy") + "log.txt";

            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }

        }

        public async void WriteLog(string eventMessage) 
        {
            FileCreate();
            await Task.Run(() => {
                fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("dd - MM - yyyy") + "log.txt",
                FileMode.Append, FileAccess.Write, FileShare.Write,
                bufferSize: 4096, useAsync: true);
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLineAsync(eventMessage);
                    sw.Close();
                }
            });
        }
    }
}