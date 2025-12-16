using System.Collections.Generic;

namespace APIGigaChat.Models.Yandex
{
    public class YandexRequest
    {
        public string modelUri { get; set; }
        public CompletionOptions completionOptions { get; set; }
        public List<Message> messages { get; set; }

        public class CompletionOptions
        {
            public bool stream { get; set; } = false;
            public double temperature { get; set; } = 0.6;
            public int maxTokens { get; set; } = 2000;
        }

        public class Message
        {
            public string role { get; set; }
            public string text { get; set; }
        }
    }
}