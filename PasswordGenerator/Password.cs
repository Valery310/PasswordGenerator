using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PasswordGenerator
{
    public static class Password
    {
        public static event EventHandler<EventArgsResultPass> ResultPass;

        private static ThreadLocal<Random> random;
        static string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z" };
        static string[] vowels = { "a", "e", "i", "o", "u", "y" };
        static Dictionary<string, string> pairs = new Dictionary<string, string>() { { "l", "!" },{ "o", "0" },{ "a", "@" },{ "s", "$" } };

        static Password()
        {
            int i = Environment.TickCount;
            random = new ThreadLocal<Random>(() =>
            new Random(Interlocked.Increment( ref i)));
        }

        public async static Task<List<Users>> CreateNewPasswordUsers(ObservableCollection<UserPrincipal> principals, int lengthPasswotd = 8)
        {
            List<Users> users = new List<Users>();
            Users.UsersPrincip = principals.ToList();
            ResultPass(null, new EventArgsResultPass("Начинаю генерацию паролей."));

            await Task.Run(()=> {
                foreach (UserPrincipal user in principals)
                {
                    users.Add(new Users(user.DisplayName, user.UserPrincipalName, Encryption.Encrypt(GeneratePassword(lengthPasswotd))));
                    ResultPass(null, new EventArgsResultPass($"Сгенерирован новый пароль для {user.DisplayName}."));
                }
            });
            
            ResultPass(null, new EventArgsResultPass("Пароли сгенерированы."));
            users.Sort();
            return users;
        }

       

        public static string GeneratePassword(int length = 8) 
        {
            StringBuilder password = new StringBuilder();
            int number = random.Value.Next(0, 9 * Convert.ToInt32(Math.Pow(10, (length / 2)-1)));
            PlaceNumber PositionNumber = NextGenericEnum<PlaceNumber>();

            if (PositionNumber == 0)
            {
                password.Append(number.ToString());
                GenerateWord(ref password, length);
            }
            else
            {
                GenerateWord(ref password, length - number.ToString().Length);
                password.Append(number.ToString());             
            }

            //SecureString result = new SecureString();
            //foreach (var item in password.ToString())
            //{
            //    result.AppendChar(item);
            //}
            //result.MakeReadOnly();
            //return result;     
            return password.ToString();
        }

        private static void GenerateWord(ref StringBuilder password, int length) 
        {
            TypeSymbol temp = TypeSymbol.Consonats;
            while (password.Length < length)
            {          
                if (temp == TypeSymbol.Consonats)
                {
                    var ch = consonants[random.Value.Next(0, consonants.Length - 1)];
                    GetSymbol(ref ch);
                    password.Append(ch);
                    temp = TypeSymbol.Vowels;
                }
                else
                {
                    var ch = vowels[random.Value.Next(0, vowels.Length - 1)];
                    GetSymbol(ref ch);
                    password.Append(ch);
                    temp = TypeSymbol.Consonats;
                }
            }
        }

        private static void GetSymbol(ref string ch) 
        {
            LetterCase letterCase = NextGenericEnum<LetterCase>();
            switch (letterCase)
            {
                case LetterCase.Lower:
                    ch = ch.ToLower();
                    break;
                case LetterCase.Top:
                    ch = ch.ToUpper();
                    break;
                case LetterCase.Alternative:
                    ch = GetAlternativeSymbol(ch);
                    break;
            }
        } 

        private static string GetAlternativeSymbol(string ch) 
        {
            string result;
            if (pairs.TryGetValue(ch, out result))
            {
                return result;
            }
            else
            {
                return ch;
            }
            
        }

        internal static string GetStringPassword(SecureString password)
        {
            var returnValue = IntPtr.Zero;
            try
            {
                returnValue = Marshal.SecureStringToGlobalAllocUnicode(password);
                return Marshal.PtrToStringUni(returnValue);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(returnValue);
            }
        }
        /// <summary>
        /// Генерация рандомного значения перечисления
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T NextGenericEnum<T>()
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            T[] values = (T[])(Enum.GetValues(typeof(T)));
            T randomEnum = values[random.Value.Next(0, values.Length)];
            return randomEnum;
        }
    }

    public class EventArgsResultPass
    {
        public string Message { get; set; }
        public EventArgsResultPass(string message)
        {
            Message = message;
        }
    }

    enum PlaceNumber { Begin, End }
    enum LetterCase { Top, Lower, Alternative } 
    enum TypeSymbol { Consonats, Vowels, Number }
}
