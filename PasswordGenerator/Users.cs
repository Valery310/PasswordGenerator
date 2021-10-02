using Newtonsoft.Json;
using PasswordGenerator.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static PasswordGenerator.MainWindow;

namespace PasswordGenerator
{
    public class Users: IComparable
    {
        public string UserPrincipalName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public static List<UserPrincipal> UsersPrincip { get; set; }       

        [JsonConstructor]
        public Users(string _DisplayName, string _UserPrincipalName, string _Password) { DisplayName = _DisplayName; UserPrincipalName = _UserPrincipalName; Password = _Password; }

        public Users(string displayName, string userPrincipalName)
        {
            DisplayName = displayName;
            UserPrincipalName = userPrincipalName;
        }

        public int CompareTo(object obj)
        {
            return string.Compare(DisplayName, (obj as Users).DisplayName);
        }
    }

    public class DomainUsers
    {
        static string Path { get; set; }

        public static TypeLogin typeLogin { get; set; }

        public static event EventHandler<EventArgsResultDomainUsers> ResultDomainUsers;

        public DomainUsers(string path, TypeLogin _typeLogin) { Path = path; typeLogin = _typeLogin; }

        public static async void ChangePasswordUsers(List<Users> users)
        {
            ResultDomainUsers(null, new EventArgsResultDomainUsers("Начинаю смену паролей."));

            await Task.Run(()=> {
                foreach (var item in users)
                {
                    try
                    {
                        if (Users.UsersPrincip == null || (Users.UsersPrincip?.Find(x => x.UserPrincipalName == item.UserPrincipalName) == null))
                        {
                            GetUser(item);
                        }

                        var user = Users.UsersPrincip.Find(x => x.UserPrincipalName == item.UserPrincipalName);
                        user.SetPassword(Encryption.Decrypt(item.Password));
                        ResultDomainUsers(null, new EventArgsResultDomainUsers($"Пароль для пользователя {user.DisplayName} сменен успешно."));
                    }
                    catch (Exception ex)
                    {
                        ResultDomainUsers(null, new EventArgsResultDomainUsers(ex.Message + $"Данные пользователя: {item.UserPrincipalName}, {item.DisplayName}, {Encryption.Decrypt(item.Password)}"));
                    }
                    
                }
            });
            
            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Смена паролей завершена."));
        }

        public static void GetUser(Users user)
        {

            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Запрос пользователя из домена..."));
            using (PrincipalContext principalContext = GetPrincipalContext(Path).Result)
            {
                ResultDomainUsers(null, new EventArgsResultDomainUsers($"Соединение с доменом установлено."));
                UserPrincipal userPrincipal = null;

                    using (PrincipalSearcher searcher = new PrincipalSearcher())
                    {
                        ResultDomainUsers(null, new EventArgsResultDomainUsers($"Получение пользователя..."));
                        userPrincipal = UserPrincipal.FindByIdentity(principalContext, IdentityType.UserPrincipalName, user.UserPrincipalName);

                        if (userPrincipal == null)
                        {
                            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Пользователь {user.DisplayName} не найден."));
                        }
                        else
                        {
                            if (Users.UsersPrincip == null) 
                            {
                            Users.UsersPrincip = new List<UserPrincipal>();
                            }
                            Users.UsersPrincip.Add(userPrincipal);
                            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Пользователь {user.DisplayName} найден."));
                        }
                    }
            }
        }

        public async Task<ObservableCollection<UserPrincipal>> GetUsers(ObservableCollection<Group> TargetGroup, ObservableCollection<Group> ExcludedGroup)
        {            
            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Запрос пользователей из домена..."));
            using (PrincipalContext principalContext = GetPrincipalContext(Path).Result)
            {             
                    ResultDomainUsers(null, new EventArgsResultDomainUsers($"Соединение с доменом установлено."));
                    ObservableCollection<UserPrincipal> users = new ObservableCollection<UserPrincipal>();
                await Task.Run(() => {
                    using (PrincipalSearcher searcher = new PrincipalSearcher())
                    { 
                   
                            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Получение пользователей..."));
                            GroupPrincipal groupe = new GroupPrincipal(principalContext);
                            foreach (var item in TargetGroup)
                            {
                                groupe.Name = item.GroupName;
                                searcher.QueryFilter = groupe;
                                RecursiveGetUser(searcher.FindOne(), users, ExcludedGroup);
                            } 
               
                    }
                    ResultDomainUsers(null, new EventArgsResultDomainUsers($"Пользователи получены."));
                });
                Users.UsersPrincip = users.ToList();
                return users;
            }            
        }

        public async Task<ObservableCollection<Group>> GetGroup()
        { 
            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Запрос групп из домена..."));
            using (PrincipalContext principalContext = GetPrincipalContext(Path).Result)
            {
                ResultDomainUsers(null, new EventArgsResultDomainUsers($"Соединение с доменом установлено."));
                ObservableCollection<Group> groups = new ObservableCollection<Group>();
                await Task.Run(() => {
                    GroupPrincipal findAllGroups = new GroupPrincipal(principalContext);
                    using (PrincipalSearcher searcher = new PrincipalSearcher(findAllGroups))
                    {
                   
                            ResultDomainUsers(null, new EventArgsResultDomainUsers($"Получение групп..."));
                            foreach (GroupPrincipal group in searcher.FindAll())
                            {
                                groups.Add(new Group(group.Name));
                            }
               
                    }
                });
                ResultDomainUsers(null, new EventArgsResultDomainUsers($"Группы получены."));
                return new ObservableCollection<Group>(groups.OrderBy(i => i));
            }
        }

        public static async Task<PrincipalContext> GetPrincipalContext(string Path, LogoPas logoPas = null) 
        {
            return await Task<PrincipalContext>.Run(() =>
            {

                PrincipalContext temp = null;
                try
                {
                    switch (Settings.Default.TypeLogin)
                    {
                        case (int)TypeLogin.CurrentUser:
                            if (!String.IsNullOrWhiteSpace(Path))
                            {
                                temp = new PrincipalContext(ContextType.Domain, Path);
                            }
                            else
                            {
                                ResultDomainUsers(null, new EventArgsResultDomainUsers("Проверьте имя домена!"));
                            }
                            break;
                        case (int)TypeLogin.LoginPass:
                            if (string.IsNullOrWhiteSpace(Path))
                            {
                                ResultDomainUsers(null, new EventArgsResultDomainUsers("Проверьте имя домена!"));
                            }
                            if (logoPas != null && !string.IsNullOrWhiteSpace(logoPas.Login) && !string.IsNullOrWhiteSpace(logoPas.Password))
                            {
                                temp = new PrincipalContext(ContextType.Domain, Path, logoPas.Login, logoPas.Password);
                            }
                            else if (!string.IsNullOrWhiteSpace(Settings.Default.Login) && !string.IsNullOrWhiteSpace(Encryption.Decrypt(Settings.Default.Password)))
                            {
                                temp = new PrincipalContext(ContextType.Domain, Path, Settings.Default.Login, Encryption.Decrypt(Settings.Default.Password));
                            }
                            else
                            {
                                ResultDomainUsers(null, new EventArgsResultDomainUsers("Проверьте логин и пароль!"));
                            }
                            break;
                        default:
                            temp = new PrincipalContext(ContextType.Domain, Path);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
                return temp;
            });
           
        }

        private void RecursiveGetUser(Principal principal, ObservableCollection<UserPrincipal> list, ObservableCollection<Group> ExcludedGroup)
        {
            if (principal is GroupPrincipal && CheckExcludeGroup(ExcludedGroup, principal as GroupPrincipal))
            {
                        foreach (var item in ((GroupPrincipal)principal).Members)
                        {
                            RecursiveGetUser(item, list, ExcludedGroup);
                        }             
            }
            else if (principal is UserPrincipal)
            {
                // Users.UsersPrincip.Add((UserPrincipal)principal);
                if (((UserPrincipal)principal).Enabled == true && !list.Contains((UserPrincipal)principal))
                {
                    list.Add((UserPrincipal)principal);
                }
            }
        }

        private bool CheckExcludeGroup(ObservableCollection<Group> ExcludedGroup, GroupPrincipal principal) 
        {
            foreach (var item in ExcludedGroup)
            {
                if (item.GroupName == principal.Name)
                {
                    return false;
                }
            }
            return true;
        }

    }

    public class EventArgsResultDomainUsers
    {
        public string Message { get; set; }
        public EventArgsResultDomainUsers(string message)
        {
            Message = message;
        }
    }
}
