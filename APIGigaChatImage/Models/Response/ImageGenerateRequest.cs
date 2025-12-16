namespace APIGigaChatImage.Models.Request
{
    public class ImageGenerateRequest
    {
        public string model { get; set; } = "GigaChat";
        public string prompt { get; set; }
        public StyleSettings style_settings { get; set; }
        public bool with_description { get; set; } = false;
    }

    public class StyleSettings
    {
        public string style { get; set; } = "realistic";
        public string color_palette { get; set; } = "vibrant";
        public string aspect_ratio { get; set; } = "16:9";
        public string theme { get; set; } = "neutral";
        public string holiday { get; set; } = null;
    }
}