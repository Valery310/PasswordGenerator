using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PasswordGenerator
{
    public class Table
    { 
        DateTime now { get; set; }
        List<Users> Users { get; set; }

        public Table(List<Users> users) => Users = users;

        public void Write(string path) 
        {
            StringBuilder csv = new StringBuilder();

            foreach (var user in Users)
            {
                csv.AppendLine($"{DateTime.Now},{user.FIO},{Password.GetStringPassword(user.password)}");
            }

            File.WriteAllText(path + "Password " + DateTime.Now + ".csv", csv.ToString());
        }
    }
}
