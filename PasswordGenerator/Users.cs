using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Text;

namespace PasswordGenerator
{
    public class Users
    {
        public string FIO { get; set; }
        //public SecureString password { get; private set; }
        public String password { get; set; }
        public static List<UserPrincipal> users { get; set; }
        public Users(string fio, String _password) { FIO = fio; password = _password; }
    }

    public class DomainUsers 
    {
        string Path { get; set; }

        public DomainUsers(string path) { Path = path; }

        public void ChangePasswordUsers(List<Users> users)
        {
            foreach (var item in users)
            {
                var user = Users.users.Find(x=>x.Name == item.FIO);
                //user.SetPassword(Password.GetStringPassword(item.password));
            }
        }

        public List<UserPrincipal> GetUsers(List<string> groupeName) 
        {
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, Path))// Path = "ssn.agrom.local"
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
