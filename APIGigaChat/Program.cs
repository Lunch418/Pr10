using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using APIGigaChat.Models;
using APIGigaChat.Models.Response;

namespace APIGigaChat
{
    class Program
    {
        public static string ClientId = "019b285e-c114-72cc-b636-b11cfb5fd760";
        public static string AuthorizationKey = "MDE5YjI4NWUtYzExNC03MmNjLWI2MzYtYjExY2ZiNWZkNzYwOjllOGM5OGRjLTc5MDgtNDAwMi1hN2QwLWNmZWU0ZmE2NGM0OQ==";

        private static List<Request.Message> conversationHistory = new List<Request.Message>();

        static async Task Main(string[] args)
        {
            string Token = await GetToken(ClientId, AuthorizationKey);

            if (Token == null)
            {
                Console.WriteLine("Не удалось получить токен");
                return;
            }

            Console.WriteLine("Диалог с GigaChat начат. Для выхода введите 'выход'.\n");

            while (true)
            {
                Console.Write("Вы: ");
                string userMessage = Console.ReadLine();

                if (userMessage.ToLower() == "выход" || userMessage.ToLower() == "exit")
                {
                    Console.WriteLine("Диалог завершен.");
                    break;
                }

                conversationHistory.Add(new Request.Message
                {
                    role = "user",
                    content = userMessage
                });

                ResponseMessage answer = await GetAnswerWithHistory(Token);

                if (answer != null && answer.choices != null && answer.choices.Count > 0)
                {
                    string botResponse = answer.choices[0].message.content;
                    Console.WriteLine($"\nGigaChat: {botResponse}\n");
                    conversationHistory.Add(new Request.Message
                    {
                        role = "assistant",
                        content = botResponse
                    });
                }
                else
                {
                    Console.WriteLine("Ошибка: Не удалось получить ответ от GigaChat");
                }
            }
        }
        public static async Task<ResponseMessage> GetAnswerWithHistory(string token)
        {
            ResponseMessage responseMessage = null;
            string Url = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

            using (HttpClientHandler Handler = new HttpClientHandler())
            {
                Handler.ServerCertificateCustomValidationCallback = (messages, cert, chain, sslPolicyErrors) => true;

                using (HttpClient Client = new HttpClient(Handler))
                {
                    HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Post, Url);

                    Request.Headers.Add("Accept", "application/json");
                    Request.Headers.Add("Authorization", $"Bearer {token}");
                    Request DataRequest = new Request()
                    {
                        model = "GigaChat",
                        stream = false,
                        repetition_penalty = 1,
                        messages = conversationHistory 
                    };

                    string JsonContent = JsonConvert.SerializeObject(DataRequest);
                    Request.Content = new StringContent(JsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage Response = await Client.SendAsync(Request);

                    if (Response.IsSuccessStatusCode)
                    {
                        string ResponseContent = await Response.Content.ReadAsStringAsync();
                        responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(ResponseContent);
                    }
                }
            }
            return responseMessage;
        }

        public static async Task<string> GetToken(string rqUID, string bearer)
        {
            string ReturnToken = null;
            string Url = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";

            using (HttpClientHandler Handler = new HttpClientHandler())
            {
                Handler.ServerCertificateCustomValidationCallback = (message, cert, chain, ss1PolicyErrors) => true;

                using (HttpClient Client = new HttpClient(Handler))
                {
                    HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Post, Url);

                    Request.Headers.Add("Accept", "application/json");
                    Request.Headers.Add("RqUID", rqUID);
                    Request.Headers.Add("Authorization", $"Bearer {bearer}");

                    var Data = new List<KeyValuePair<string, string>> {
                        new KeyValuePair<string, string>("scope", "GIGACHAT_API_PERS")
                    };

                    Request.Content = new FormUrlEncodedContent(Data);

                    HttpResponseMessage Response = await Client.SendAsync(Request);

                    if (Response.IsSuccessStatusCode)
                    {
                        string ResponseContent = await Response.Content.ReadAsStringAsync();
                        ResponseToken Token = JsonConvert.DeserializeObject<ResponseToken>(ResponseContent);
                        ReturnToken = Token.access_token;
                    }
                }
            }
            return ReturnToken;
        }
    }
}