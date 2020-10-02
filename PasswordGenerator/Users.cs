using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Text;

namespace PasswordGenerator
{
    public class Users
    {
        public string UserPrincipalName { get; private set; }
        public string DisplayName { get; private set; }
        public SecureString Password { get; private set; }
        public static List<UserPrincipal> UsersPrincip { get; set; }
        public Users(string _DisplayName, string _UserPrincipalName, SecureString _Password) { DisplayName = _DisplayName; UserPrincipalName = _UserPrincipalName; Password = _Password; }
    }

    public class DomainUsers 
    {
        string Path { get; set; }

        public DomainUsers(string path) { Path = path; }

        public void ChangePasswordUsers(List<Users> users)
        {
            foreach (var item in users)
            {
                var user = Users.UsersPrincip.Find(x=>x.UserPrincipalName == item.UserPrincipalName);
                //user.SetPassword(Password.GetStringPassword(item.password));
            }
        }

        public List<UserPrincipal> GetUsers(List<string> groupeName)
        { //ContextType contextType, string name, string userName, string password)
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, Path, "kryukov.vn", "Cisco28!!"))// Path = "ssn.agrom.local"
            {
                List<UserPrincipal> users = new List<UserPrincipal>();
                using (PrincipalSearcher searcher = new PrincipalSearcher())
                {
                    foreach (var item in groupeName)
                    {
                        GroupPrincipal groupe = new GroupPrincipal(principalContext);
                        groupe.Name = item;
                        searcher.QueryFilter = groupe;
                        RecursiveGetUser(searcher.FindOne(), users);
                    }
                }
                return users;
            }
        }

        private void RecursiveGetUser(Principal principal, List<UserPrincipal> list)
        {
            if (principal is GroupPrincipal)
            {
                foreach (var item in ((GroupPrincipal)principal).Members)
                {
                    RecursiveGetUser(item, list);
                }
            }
            else if (principal is UserPrincipal)
            {
                list.Add((UserPrincipal)principal);
            }
        }

    }
}
