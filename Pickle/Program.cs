using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace Pickle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string loginPage = "https://testshop8.utypia.com/UI/en-US/Account/B2CLogin?returnUrl=%2FUI%2Fen-US";
            string url = "https://testshop8.utypia.com/UI/en-US/Account/B2CLogin?returnUrl=%2FUI%2Fen-US";

            // get login page

            HttpClient client = new HttpClient();

            string html = client.GetAsync(loginPage).Result.Content.ReadAsStringAsync().Result;

            string patternToken = "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"";

            string requestVerificationToken = Regex.Match(html, patternToken).Groups[1].Value;

            // login
           

            string username = "admin";

            for (int i = 0; i < 50; i++) {
                byte[] ff = new byte[10];
                Random.Shared.NextBytes(ff);
                // base64
                string password = Convert.ToBase64String(ff);

                string postData = $"__RequestVerificationToken={requestVerificationToken}&AccountLoginViewModel.Username={username}&AccountLoginViewModel.Password={password}";

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(postData);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                // get response with status code and all
                var response = client.SendAsync(request).Result;

                lock(patternToken)
                {
                    // date/time with ms
                    Console.WriteLine(DateTime.Now.ToString("[dd.MM.yy HH:mm:ss:fff] "));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(response.StatusCode);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(response.StatusCode);
                    }
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine($" {username} {password}");
                    string resBody = response.Content.ReadAsStringAsync().Result;
                    bool isLoginFailed = resBody.Contains("password provided is incorrect");

                    if (!isLoginFailed)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Login successful");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Denied");
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.WriteLine(resBody);
                    Console.WriteLine();
                }
            }

            Console.ReadKey();
        }
    }
}
