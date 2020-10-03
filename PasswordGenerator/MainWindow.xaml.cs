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
using System.Windows.Threading;
using System.Collections.ObjectModel;

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

        public ObservableCollection<Group> TargetGroup { get; set; }
        public ObservableCollection<Group> ExcludedGroup { get; set; } 

        public async void Start() 
        {
            TargetGroup = new ObservableCollection<Group>();
            TargetGroup.Add(new Group("Группа"));
            TargetGroup.CollectionChanged += TargetGroup_CollectionChanged;
            ExcludedGroup = new ObservableCollection<Group>();
            ExcludedGroup.Add(new Group("Группа"));
            ExcludedGroup.CollectionChanged += ExcludedGroup_CollectionChanged;
            dgTargetGroup.ItemsSource = TargetGroup;
           // dgTargetGroup.DataContext = TargetGroup;
            dgExcludedtGroup.ItemsSource = ExcludedGroup;
            //dgExcludedtGroup.DataContext = ExcludedGroup;

            //await Task.Run(() =>
            //{
            //    Console.WriteLine("Hello World!");
            //    Password password = new Password(Environment.TickCount);
            //    for (int i = 0; i < 20; i++)
            //    {
            //        Console.WriteLine(Password.GetStringPassword(password.GeneratePassword()));
            //    }
            //    DomainUsers domainUsers = new DomainUsers("ssn.agrom.local");
            //    var users = password.CreateNewPasswordUsers(domainUsers.GetUsers(new List<string>() { "ИТ" }));
            //    Table table = new Table(users);
            //    table.Write(System.AppDomain.CurrentDomain.BaseDirectory);
            //  //  domainUsers.ChangePasswordUsers(users);
            //    //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            //    //{
            //    //    lblResult.Content = "Готово";
            //    //}));
            //    lblResult.Dispatcher.Invoke(new Action(() => { lblResult.Content = "Готово"; })); 
            //});
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
            if (!string.IsNullOrWhiteSpace(tbxAddRowExcludedGroup.Text))
            {
                ExcludedGroup.Add(new Group(tbxAddRowExcludedGroup.Text)); ;
            }
            else
            {
                MessageBox.Show("Введите имя группы!");
            }
        }

        private void btnAddRowTargetGroup_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbxAddRowTargetGroup.Text))
            {
                TargetGroup.Add(new Group(tbxAddRowTargetGroup.Text)); ;
            }
            else
            {
                MessageBox.Show("Введите имя группы!");
            }
        }

        private void btnDelRowTargetGroup_Click(object sender, RoutedEventArgs e)
        {
                TargetGroup.Remove(dgTargetGroup.SelectedItem as Group);                         
        }

        private void btnDelRowExcludedGroup_Click(object sender, RoutedEventArgs e)
        {
                ExcludedGroup.Remove(dgExcludedtGroup.SelectedItem as Group);
        }
    }
}



//bool valid = false;
//using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
//{
//    valid = context.ValidateCredentials(username, password);
//https://you-hands.ru/2018/08/31/wpf-sistema-avtorizacii-i-registracii/
//}