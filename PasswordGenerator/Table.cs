using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace PasswordGenerator
{
    public static class Table
    { 
        static DateTime now { get; set; }
        //static List<Users> Users { get; set; }

        // public static void SetUsers(List<Users> users) => Users = users;

        public static event EventHandler<EventArgsResultTable> ResultTable;

        public static async void Write(string path, List<Users> Users) 
        {
            StringBuilder csv = new StringBuilder();

            foreach (var user in Users)
            {
                csv.AppendLine($"{DateTime.Now},{user.DisplayName},{user.UserPrincipalName},{Encryption.Decrypt(user.Password)}");//{Password.GetStringPassword(user.Password)}");
            }           
            try
            {
               File.WriteAllText(path + "\\Password " + DateTime.Now.ToString("dd-MM-yyyy-HH-mm") + ".csv", csv.ToString(), Encoding.Unicode);                
            }
            catch (Exception ex)
            {
                ResultTable(null, new EventArgsResultTable("Не удалось сохранить таблицу с паролями. Попробуйте еще раз или смените директорию сохранения."));
            }
            
            try
            {
                string ser = JsonConvert.SerializeObject(Users);
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\ListUser.json", ser, Encoding.Unicode);
            }
            catch (Exception ex)
            {
                ResultTable(null, new EventArgsResultTable("Не удалось сохранить файл с настройками пользователей. Попробуйте еще раз."));
            }
            ResultTable(null, new EventArgsResultTable("Сохранение выполнено."));
        }

        public static async Task<List<Users>> ReadAsyncCSV()
        {
            ResultTable(null, new EventArgsResultTable("Чтение настроек из CSV."));
            List<Users> users = null;
            OpenFileDialog ofd = new OpenFileDialog();
        //    ofd.ShowDialog();
            if (ofd.ShowDialog() == true)
            {
                if (File.Exists(ofd.FileName))
                {
                    try
                    {
                        //  string des1 = File.ReadAllText(Directory.GetCurrentDirectory() + "\\ListUser.json", Encoding.Unicode);
                        string[] des = await File.ReadAllLinesAsync(ofd.FileName, Encoding.Unicode);//await File.ReadAllTextAsync(ofd.FileName, Encoding.Unicode);
                        // users = JsonConvert.DeserializeObject<List<Users>>(des);
                        ResultTable(null, new EventArgsResultTable("Данные считаны."));
                        var temp =  des.Select(line =>
                        {
                            string[] data = line.Split(",");
                            // We return a person with the data in order.
                            return new Users(data[1], data[2], data[3]);
                        });

                        users = temp.ToList();
                    }
                    catch (Exception ex)
                    {
                        ResultTable(null, new EventArgsResultTable(ex.Message));
                    }
                }
                   
            }
            else
            {
                ResultTable(null, new EventArgsResultTable("Не выбран файл настроек пользователей!"));
            }
            return users;
        }
        public static async Task<List<Users>> ReadAsync() 
        {
            ResultTable(null, new EventArgsResultTable("Чтение настроек из json."));
            List<Users> users = null; 
            if (File.Exists(Directory.GetCurrentDirectory() + "\\ListUser.json"))
            {
                try
                {
                  //  string des1 = File.ReadAllText(Directory.GetCurrentDirectory() + "\\ListUser.json", Encoding.Unicode);
                    string des = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "\\ListUser.json", Encoding.Unicode);
                    users = JsonConvert.DeserializeObject<List<Users>>(des);
                    ResultTable(null, new EventArgsResultTable("Данные считаны."));
                }
                catch (Exception ex)
                {
                    ResultTable(null, new EventArgsResultTable(ex.Message));
                }
            }
            else
            {
                ResultTable(null, new EventArgsResultTable("Не найден файл настроек пользователей! Сгенерируйте новый."));
            }
                     
            return users;
        }
    }

    public class EventArgsResultTable
    {
        public string Message { get; set; }
        public EventArgsResultTable(string message)
        {
            Message = message;
        }
    }
}
