using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordGenerator
{
    public class Table
    { 
        DateTime now { get; set; }
        List<Users> Users { get; set; }

        public Table(List<Users> users) => Users = users;

        public async void Write(string path) 
        {
            StringBuilder csv = new StringBuilder();

            foreach (var user in Users)
            {
                csv.AppendLine($"{DateTime.Now},{user.DisplayName},{user.UserPrincipalName},{Password.GetStringPassword(user.Password)}");
            }

            try
            {
                File.WriteAllText(path + "Password " + DateTime.Now.ToString("dd-MM-yyyy-HH-mm") + ".csv", csv.ToString(), Encoding.Unicode);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось сохранить таблицу с паролями. Попробуйте еще раз или смените директорию сохранения.");
            }
            
            try
            {
                using (FileStream fs = File.Create(Application.ResourceAssembly.Location + "ListUser.jsn"))
                {
                    await JsonSerializer.SerializeAsync<List<Users>>(fs, Users);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось успешно сохранить файл с настройками пользователей. Попробуйте еще раз.");
            }
           
        }

        public async Task<List<Users>> ReadAsync() 
        {
            try
            {
                using (FileStream fs = File.OpenRead(Application.ResourceAssembly.Location + "ListUser.jsn"))
                {
                    return await JsonSerializer.DeserializeAsync<List<Users>>(fs);
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Не найден файл настроек пользователей! Сгенерируйте новый.");
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Поврежден файл настроек пользователей! Сгенерируйте новый.");
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            throw new Exception("Не удалось загрузить настройки.");
        }
    }
}
