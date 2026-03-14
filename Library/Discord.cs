using System.Text;
using System.Text.Json;

namespace DiscordWebhookSender.Library
{
    static class Discord
    {
        static readonly HttpClient httpClient = new HttpClient();
        private static string? Token;
        private static string? BotName;
        private static string? AvatarUrl;
        public static void SetToken(string token)
        {
            Token = token;
        }

        public static void SetBotName(string name)
        {
            BotName = name;
        }

        public static void SetBotAvatarUrl(string url)
        {
            AvatarUrl = url;
        }

        public static async Task SendMessage(string msg)
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new InvalidOperationException("You forgot initialize the token");
            }

            var message = new
            {
                content = msg,
                username = BotName,
                avatar_url = AvatarUrl

            };

            string json = JsonSerializer.Serialize(message);
            try
            {

                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, Token)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                var response = await httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task SendRepeatedMessages(string msg, int amount, int delaySecond = 1, bool marker = false)
        {
            for (int i = 1; i <= amount; i++)
            {
                string message = marker ? $"{msg} [{i}]" : msg;
                await SendMessage(message);
                await Task.Delay(delaySecond * 1000);
            }
        }

        public static async Task SendFile(string filePath, string? message = null)
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new InvalidOperationException("You forgot initialize the token");
            }
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }
            using var formData = new MultipartFormDataContent();

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(fileBytes);

            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            formData.Add(fileContent, "file", Path.GetFileName(filePath));

            if (!string.IsNullOrEmpty(message))
            {
                formData.Add(new StringContent(message), "content");
            }
            if (!string.IsNullOrEmpty(BotName))
            {
                formData.Add(new StringContent(BotName), "username");
            }
            if (!string.IsNullOrEmpty(AvatarUrl))
            {
                formData.Add(new StringContent(AvatarUrl), "avatar_url");
            }

            var response = await httpClient.PostAsync(Token, formData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error: {response.StatusCode} - {error}");
            }
        }
    }
}
