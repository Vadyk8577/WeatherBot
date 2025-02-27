using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Telegram.Bot.Types.ReplyMarkups;

namespace WeatherHistory
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationTokenSource _cts = new();

        private static readonly string BotToken = "7584568967:AAEkQZaEA2hcHRUuUxdssK5w3fYUQ1YHE9Y";
        private static readonly string WeatherApiKey = "24359dc6112a45c55d2c9d0458bbbd5b";
        private static readonly HttpClient httpClient = new HttpClient();

        public TelegramBotService()
        {
            _botClient = new TelegramBotClient(BotToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Starting Telegram bot...");

            _botClient.StartReceiving(
                UpdateHandler,
                ErrorHandler,
                new ReceiverOptions { AllowedUpdates = { } },
                _cts.Token
            );

            await Task.Delay(-1, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            Console.WriteLine("Telegram bot stopped.");
            await base.StopAsync(cancellationToken);
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            
            if (update.CallbackQuery != null)
            {
                string selectedCity = update.CallbackQuery.Data;
                long chatId = update.CallbackQuery.Message.Chat.Id;

                
                await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);

                
                string weatherInfo = await GetWeatherAsync(selectedCity, chatId);
                await client.SendTextMessageAsync(chatId, weatherInfo, cancellationToken: token);

                return; 
            }

            
            if (update.Message?.Text == null) return;

            var chatIdFromMessage = update.Message.Chat.Id;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Berlin", "Berlin"),
                    InlineKeyboardButton.WithCallbackData("Monaco", "Monaco")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("London", "London"),
                    InlineKeyboardButton.WithCallbackData("Tokyo", "Tokyo")
                }
            });

            string userMessage = update.Message.Text.ToLower();

            if (userMessage == "/start")
            {
                await client.SendTextMessageAsync(
                    chatId: chatIdFromMessage,
                    text: "Welcome to the bot! Click a city to check the weather:",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: token);
                DatabaseHelper.SaveUser(update.Message.Chat.Id, update.Message.Chat.FirstName, update.Message.Chat.LastName);

                return;
            }

            DatabaseHelper.SaveUser(update.Message.Chat.Id, update.Message.Chat.FirstName, update.Message.Chat.LastName);

            string weatherResponse = await GetWeatherAsync(userMessage, update.Message.Chat.Id);
            await client.SendTextMessageAsync(chatIdFromMessage, weatherResponse, cancellationToken: token);
        }


        private static async Task<string> GetWeatherAsync(string city, long userId)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={WeatherApiKey}&units=metric";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return "❌ City not found. Please try again.";

                string json = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(json);

                string weatherDesc = data["weather"][0]["description"].ToString();
                decimal temp = (decimal)data["main"]["temp"];
                int humidity = (int)data["main"]["humidity"];
                decimal windSpeed = (decimal)data["wind"]["speed"];

                
                DatabaseHelper.SaveWeather(userId, city, temp, Convert.ToDecimal(humidity), windSpeed, weatherDesc);

                return $"🌍 Weather in {char.ToUpper(city[0]) + city.Substring(1).ToLower()}:\n" +
                       $"🌡 Temperature: {temp}°C\n" +
                       $"💧 Humidity: {humidity}%\n" +
                       $"💨 Wind Speed: {windSpeed} m/s\n" +
                       $"☁️ Condition: {weatherDesc}";
            }
            catch (Exception ex)
            {
                return $"⚠️ Error fetching weather data: {ex.Message}";
            }
        }

        private Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"⚠️ Error: {exception.Message}");
            return Task.CompletedTask;
        }
        public static async Task SendWeatherToUser(Models.User user, string city)
        {
            string weatherResponse = await GetWeatherAsync(city, user.UserId);

            TelegramBotClient client = new TelegramBotClient(BotToken);
            
            await client.SendTextMessageAsync(user.UserId, weatherResponse);
        }
        
    }
}
