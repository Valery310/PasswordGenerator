using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace PasswordGenerator
{
    public class Group: IComparable
    {
        public string GroupName { get; set; }

        public Group(string grupName) => GroupName = grupName;
        public Group() { }

        public int CompareTo(object obj)
        {
            return string.Compare(GroupName, (obj as Group).GroupName);
        }

        public override string ToString()
        {
            return string.Format($"{GroupName}");
        }
    }
}
