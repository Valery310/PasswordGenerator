using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordGenerator
{
    public class Group
    {
        public string GroupName { get; set; }
        public Group(string grupName) => GroupName = grupName;
        public Group() { }
    }
}
