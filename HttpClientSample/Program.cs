using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HttpClientSample
{
    class Program
    {
        private const string APP_PATH = "http://localhost:53128/api/auth";
        private static string token;

        static void Main(string[] args)
        {
            Console.WriteLine("Введите логин:");
            string email = Console.ReadLine();

            Console.WriteLine("Введите пароль:");
            string password = Console.ReadLine();

            GetTest(email,password);

            #region
            //var registerResult = Register(email, password);
            //Console.WriteLine("Статусный код регистрации: {0}", registerResult);

            //Dictionary<string, string> tokenDictionary = GetTokenDictionary(email, password);
            //token = tokenDictionary["access_token"];

            //Console.WriteLine();
            //Console.WriteLine("Access Token:");
            //Console.WriteLine(token);

            //Console.WriteLine();
            //string userInfo = GetUserInfo(token);
            //Console.WriteLine("Пользователь:");
            //Console.WriteLine(userInfo);

            //Console.WriteLine();
            //string values = GetValues(token);
            //Console.WriteLine("Values:");
            //Console.WriteLine(values);
            #endregion


            Console.Read();
        }

        // регистрация
        static string Register(string email, string password)
        {
            var registerModel = new
            {
                Email = email,
                Password = password,
                ConfirmPassword = password
            };
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(APP_PATH + "/api/Account/Register", registerModel).Result;
                return response.StatusCode.ToString();
            }
        }
        // получение токена
        static Dictionary<string, string> GetTokenDictionary(string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password" ),
                    new KeyValuePair<string, string>( "username", userName ),
                    new KeyValuePair<string, string> ( "Password", password )
                };
            var content = new FormUrlEncodedContent(pairs);

            using (var client = new HttpClient())
            {
                var response =
                    client.PostAsync(APP_PATH + "/Token", content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                // Десериализация полученного JSON-объекта
                Dictionary<string, string> tokenDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                return tokenDictionary;
            }
        }

        // создаем http-клиента с токеном 
        static HttpClient CreateClient(string accessToken = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }
            return client;
        }

        // получаем информацию о клиенте 
        static string GetUserInfo(string token)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/api/Account/UserInfo").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        // обращаемся по маршруту api/values 
        static string GetValues(string token)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/api/values").Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

       public static void GetTest(string email,string pasw)
        {
            HttpClient httpClient = new HttpClient();
                var response = httpClient.GetAsync($"http://localhost:53128/api/auth/login?email={email}&pass={pasw}").Result;

            using (var client = new HttpClient())
            {
                var result = client.GetAsync("http://aspnetmonsters.com").Result;
                Console.WriteLine(result.StatusCode);
                Console.WriteLine(result.Content.ToString());
                Console.WriteLine(result.Content);
            }


        }

    }
}

//   using(var client = new HttpClient())
//   {
//    var result = client.GetAsync("http://aspnetmonsters.com").Result;
//    Console.WriteLine(result.StatusCode);
//   }