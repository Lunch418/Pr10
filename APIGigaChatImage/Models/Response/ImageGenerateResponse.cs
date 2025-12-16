using System.Collections.Generic;

namespace APIGigaChatImage.Models.Response
{
    public class ImageGenerateResponse
    {
        public List<Choice> choices { get; set; }
        public long created { get; set; }
        public string model { get; set; }
        public Usage usage { get; set; }

        public class Choice
        {
            public Message message { get; set; }
            public string finish_reason { get; set; }
            public int index { get; set; }
        }

        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
            public List<ImagePart> image_parts { get; set; }
        }

        public class ImagePart
        {
            public string type { get; set; }
            public ImageData image { get; set; }
        }

        public class ImageData
        {
            public string image_id { get; set; }
            public string data { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }
    }
}