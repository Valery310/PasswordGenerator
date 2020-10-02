using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Security.Permissions;

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

            Console.WriteLine("Hello World!");
            Password password = new Password(Environment.TickCount);
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(Password.GetStringPassword(password.GeneratePassword()));
            }
            DomainUsers domainUsers = new DomainUsers("ssn.agrom.local");
            var users = password.CreateNewPasswordUsers(domainUsers.GetUsers(new List<string>() { "ИТ" }));
            Table table = new Table(users);
            table.Write(System.AppDomain.CurrentDomain.BaseDirectory);
            //  domainUsers.ChangePasswordUsers(users);
        }




        [PrincipalPermission(SecurityAction.Demand, Role = "BUILTIN\\Users")]
        static void ShowMessage()
        {
            Console.WriteLine("Текущий принципиал зарегистрировался локально и является членом группы Users");
        }
    }
}



//bool valid = false;
//using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
//{
//    valid = context.ValidateCredentials(username, password);
//https://you-hands.ru/2018/08/31/wpf-sistema-avtorizacii-i-registracii/
//}