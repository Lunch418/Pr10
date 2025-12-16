using System.Collections.Generic;

namespace APIGigaChat.Models.Yandex
{
    public class YandexResponse
    {
        public Result result { get; set; }

        public class Result
        {
            public List<Alternative> alternatives { get; set; }
            public Usage usage { get; set; }
            public string modelVersion { get; set; }
        }

        public class Alternative
        {
            public Message message { get; set; }
            public string status { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string text { get; set; }
        }

        public class Usage
        {
            public string inputTextTokens { get; set; }
            public string completionTokens { get; set; }
            public string totalTokens { get; set; }
        }
    }
}