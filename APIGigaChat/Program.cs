using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using APIGigaChat.Models;
using APIGigaChat.Models.Response;
using APIGigaChat.Models.Yandex;

namespace APIGigaChat
{
    class Program
    {
        public static string GigaChatClientId = "019b285e-c114-72cc-b636-b11cfb5fd760";
        public static string GigaChatAuthorizationKey = "MDE5YjI4NWUtYzExNC03MmNjLWI2MzYtYjExY2ZiNWZkNzYwOjllOGM5OGRjLTc5MDgtNDAwMi1hN2QwLWNmZWU0ZmE2NGM0OQ==";

        public static string YandexFolderId = "b1gftehghm6he7padu45"; 
        public static string YandexOAuthToken = "y0__xDF-fn7BBjB3RMgxsyJ2RXyQlmzBvWemximRIQ6ClsapFgQ8Q";

        private static List<Request.Message> gigaChatHistory = new List<Request.Message>();
        private static List<YandexRequest.Message> yandexHistory = new List<YandexRequest.Message>();

        static async Task Main(string[] args)
        {
            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("Выберите режим работы:");
                Console.WriteLine("1 - Диалог с GigaChat");
                Console.WriteLine("2 - Диалог с YandexGPT");
                Console.WriteLine("3 - Диалог между GigaChat и YandexGPT");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await RunGigaChatDialog();
                        break;
                    case "2":
                        await RunYandexGPTDialog();
                        break;
                    case "3":
                        await RunDialogBetweenModels();
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task RunGigaChatDialog()
        {
            string token = await GetGigaChatToken(GigaChatClientId, GigaChatAuthorizationKey);

            if (token == null)
            {
                Console.WriteLine("Не удалось получить токен GigaChat. Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nДиалог с GigaChat начат. Для возврата в меню введите 'меню'.\n");

            while (true)
            {
                Console.Write("Вы: ");
                string userMessage = Console.ReadLine();

                if (userMessage.ToLower() == "меню" || userMessage.ToLower() == "menu")
                {
                    Console.WriteLine("Возврат в главное меню...");
                    gigaChatHistory.Clear();
                    return;
                }

                gigaChatHistory.Add(new Request.Message
                {
                    role = "user",
                    content = userMessage
                });

                ResponseMessage answer = await GetGigaChatAnswer(token);

                if (answer != null && answer.choices != null && answer.choices.Count > 0)
                {
                    string botResponse = answer.choices[0].message.content;
                    Console.WriteLine($"\nGigaChat: {botResponse}\n");

                    gigaChatHistory.Add(new Request.Message
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

        static async Task RunYandexGPTDialog()
        {
            string iamToken = await GetYandexIamToken(YandexOAuthToken);

            if (iamToken == null)
            {
                Console.WriteLine("Не удалось получить IAM токен Yandex. Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nДиалог с YandexGPT начат. Для возврата в меню введите 'меню'.\n");

            while (true)
            {
                Console.Write("Вы: ");
                string userMessage = Console.ReadLine();

                if (userMessage.ToLower() == "меню" || userMessage.ToLower() == "menu")
                {
                    Console.WriteLine("Возврат в главное меню...");
                    yandexHistory.Clear();
                    return;
                }

                yandexHistory.Add(new YandexRequest.Message
                {
                    role = "user",
                    text = userMessage
                });

                string response = await GetYandexGPTAnswer(iamToken);

                if (response != null)
                {
                    Console.WriteLine($"\nYandexGPT: {response}\n");

                    yandexHistory.Add(new YandexRequest.Message
                    {
                        role = "assistant",
                        text = response
                    });
                }
                else
                {
                    Console.WriteLine("Ошибка: Не удалось получить ответ от YandexGPT");
                }
            }
        }

        static async Task RunDialogBetweenModels()
        {
            Console.WriteLine("\n=== Диалог между GigaChat и YandexGPT ===\n");

            string gigaToken = await GetGigaChatToken(GigaChatClientId, GigaChatAuthorizationKey);
            string yandexToken = await GetYandexIamToken(YandexOAuthToken);

            if (gigaToken == null || yandexToken == null)
            {
                Console.WriteLine("Не удалось получить токены для одной из моделей. Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите начальный вопрос для начала диалога: ");
            string initialQuestion = Console.ReadLine();

            Console.WriteLine($"\nНачальный вопрос: {initialQuestion}\n");

            gigaChatHistory.Clear();
            yandexHistory.Clear();

            gigaChatHistory.Add(new Request.Message
            {
                role = "user",
                content = initialQuestion
            });

            bool gigaChatTurn = true;
            int round = 1;

            Console.WriteLine($"Раунд {round}:");

            while (true)
            {
                if (gigaChatTurn)
                {
                    ResponseMessage gigaResponse = await GetGigaChatAnswer(gigaToken);

                    if (gigaResponse != null && gigaResponse.choices != null && gigaResponse.choices.Count > 0)
                    {
                        string gigaMessage = gigaResponse.choices[0].message.content;
                        Console.WriteLine($"GigaChat: {gigaMessage}\n");

                        gigaChatHistory.Add(new Request.Message
                        {
                            role = "assistant",
                            content = gigaMessage
                        });

                        yandexHistory.Add(new YandexRequest.Message
                        {
                            role = "user",
                            text = gigaMessage
                        });
                    }
                    else
                    {
                        Console.WriteLine("GigaChat не ответил. Возврат в главное меню...");
                        gigaChatHistory.Clear();
                        yandexHistory.Clear();
                        return;
                    }
                }
                else
                {
                    // YandexGPT отвечает
                    string yandexResponse = await GetYandexGPTAnswer(yandexToken);

                    if (yandexResponse != null)
                    {
                        Console.WriteLine($"YandexGPT: {yandexResponse}\n");

                        yandexHistory.Add(new YandexRequest.Message
                        {
                            role = "assistant",
                            text = yandexResponse
                        });

                        gigaChatHistory.Add(new Request.Message
                        {
                            role = "user",
                            content = yandexResponse
                        });
                    }
                    else
                    {
                        Console.WriteLine("YandexGPT не ответил. Возврат в главное меню...");
                        gigaChatHistory.Clear();
                        yandexHistory.Clear();
                        return;
                    }
                }

                gigaChatTurn = !gigaChatTurn;
                round++;

                Console.WriteLine($"Раунд {round}:");
                Console.WriteLine("Нажмите Enter для продолжения...");
                string input = Console.ReadLine();

                if (input.ToLower() == "меню" || input.ToLower() == "menu")
                {
                    Console.WriteLine("Возврат в главное меню...");
                    gigaChatHistory.Clear();
                    yandexHistory.Clear();
                    return;
                }
            }
        }

        public static async Task<string> GetGigaChatToken(string rqUID, string bearer)
        {
            string returnToken = null;
            string url = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("RqUID", rqUID);
                    request.Headers.Add("Authorization", $"Bearer {bearer}");

                    var data = new List<KeyValuePair<string, string>> {
                        new KeyValuePair<string, string>("scope", "GIGACHAT_API_PERS")
                    };

                    request.Content = new FormUrlEncodedContent(data);

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        ResponseToken token = JsonConvert.DeserializeObject<ResponseToken>(responseContent);
                        returnToken = token.access_token;
                    }
                }
            }
            return returnToken;
        }

        public static async Task<ResponseMessage> GetGigaChatAnswer(string token)
        {
            ResponseMessage responseMessage = null;
            string url = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (messages, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

                    request.Headers.Add("Accept", "application/json");
                    request.Headers.Add("Authorization", $"Bearer {token}");

                    Request dataRequest = new Request()
                    {
                        model = "GigaChat",
                        stream = false,
                        repetition_penalty = 1,
                        messages = gigaChatHistory
                    };

                    string jsonContent = JsonConvert.SerializeObject(dataRequest);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(responseContent);
                    }
                }
            }
            return responseMessage;
        }

        public static async Task<string> GetYandexIamToken(string oauthToken)
        {
            string url = "https://iam.api.cloud.yandex.net/iam/v1/tokens";

            using (HttpClient client = new HttpClient())
            {
                var requestData = new
                {
                    yandexPassportOauthToken = oauthToken
                };

                string jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<YandexTokenResponse>(responseContent);
                    return tokenResponse.iamToken;
                }
            }

            return null;
        }

        public static async Task<string> GetYandexGPTAnswer(string iamToken)
        {
            string url = "https://llm.api.cloud.yandex.net/foundationModels/v1/completion";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {iamToken}");

                var request = new YandexRequest
                {
                    modelUri = $"gpt://{YandexFolderId}/yandexgpt-lite",
                    completionOptions = new YandexRequest.CompletionOptions
                    {
                        stream = false,
                        temperature = 0.6,
                        maxTokens = 2000
                    },
                    messages = yandexHistory
                };

                string jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var yandexResponse = JsonConvert.DeserializeObject<YandexResponse>(responseContent);

                    if (yandexResponse?.result?.alternatives?.Count > 0)
                    {
                        return yandexResponse.result.alternatives[0].message.text;
                    }
                }
            }

            return null;
        }
    }
}