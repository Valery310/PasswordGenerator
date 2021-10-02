using PasswordGenerator.Properties;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static PasswordGenerator.MainWindow;

namespace PasswordGenerator
{
    public static class Connect
    {
        public static event EventHandler<EventArgsResultTest> ResultTestConnection;

        public static bool TestConnection(string server, TypeLogin typeLogin, LogoPas logoPas = null)//Общая проверка связи
        {
            if (!string.IsNullOrWhiteSpace(server) && PingHost(server))
            {
                ResultTestConnection(null, new EventArgsResultTest("Проверка связи с сервером прошла успешно."));

                PrincipalContext temp;
                try
                {
                    switch (typeLogin)
                    {
                        case TypeLogin.CurrentUser:
                            temp = DomainUsers.GetPrincipalContext(server).Result;
                            break;
                        case TypeLogin.LoginPass:
                            if (logoPas is null)
                            {
                                logoPas = new LogoPas() { Login = Settings.Default.Login, Password = Encryption.Decrypt(Settings.Default.Password) };
                            }
                            temp = DomainUsers.GetPrincipalContext(server, logoPas).Result;
                            if (logoPas!=null && !string.IsNullOrWhiteSpace(logoPas.Login) && !string.IsNullOrWhiteSpace(logoPas.Password) && temp.ValidateCredentials(logoPas.Login, logoPas.Password))
                            {
                                ResultTestConnection(null, new EventArgsResultTest("Проверка авторизации на сервере прошла успешно."));
                                return true;
                            }
                            else
                            {
                                ResultTestConnection(null, new EventArgsResultTest("Проверка авторизации на сервере прошла неудачно."));
                                return false;
                            }                           
                        default:
                            temp = DomainUsers.GetPrincipalContext(server).Result;
                            break;
                    }
                    if (temp != null)
                    {
                        ResultTestConnection(null, new EventArgsResultTest("Проверка авторизации на сервере прошла успешно."));
                        return true;
                    }
                    else
                    {
                        ResultTestConnection(null, new EventArgsResultTest("Проверка авторизации на сервере прошла неудачно."));
                    }
                }
                catch (Exception ex)
                {
                    ResultTestConnection(null, new EventArgsResultTest("Проверка авторизации на сервере прошла неудачно."));
                }
            }
            else
            {
                ResultTestConnection(null, new EventArgsResultTest("Проверка связи с сервером прошла неудачно."));
            }
            //lblResult.Content = "Проверьте введенные данные.";
            return false;
        }

        private static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;
            try 
            { 
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                ResultTestConnection(null, new EventArgsResultTest(ex.Message));
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;          
        }
    }

    public class EventArgsResultTest
    {
        public string Message { get; set; }
        public EventArgsResultTest(string message) 
        {
            Message = message;
        }
    }
}
