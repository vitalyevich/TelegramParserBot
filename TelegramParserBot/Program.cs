using System;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using Telegram.Bot;
using System.IO;

namespace TelegramParserBot
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = File.ReadAllText(@"token.txt");
            TelegramBotClient telegramBot = new TelegramBotClient(token);

            telegramBot.OnMessage += (s, arg) =>
            {
                Console.WriteLine($"{arg.Message.Chat.FirstName}: {arg.Message.Text}");
                telegramBot.SendTextMessageAsync(
                    arg.Message.Chat.Id,
                    TemperatureInCity(arg.Message.Text)
                    );
            };

            telegramBot.StartReceiving();
            Console.ReadKey();

            string cityName = "Столбцы";
            string result = String.Empty;
            result = TemperatureInCity(cityName);

            Console.WriteLine(result);

        }

        private static string TemperatureInCity(string cityName)
        {
            string result;
            switch (cityName.ToLower())
            {
                case "/столбцы": result = GetTemperature(url: @"https://xml.meteoservice.ru/export/gismeteo/point/9189.xml"); break;
                case "/минск": result = GetTemperature(url: @"https://xml.meteoservice.ru/export/gismeteo/point/34.xml"); break;
                case "/барановичи": result = GetTemperature(url: @"https://xml.meteoservice.ru/export/gismeteo/point/9156.xml"); break;
                default: result = "Такого города я не знаю\n/столбцы\n/минск\n/барановичи\n"; break;
            }

            return result;
        }
        #region Парсинг XML
        private static string GetTemperature(string url)
        {
            string xml = new WebClient().DownloadString(url);
            /*Console.WriteLine(xml);*/

            var doc = XDocument.Parse(xml);
            var forecast = doc.Descendants("MMWEATHER")
                               .Descendants("REPORT")
                               .Descendants("TOWN")
                               .Descendants("FORECAST")
                               .ToList()[0];

            //Console.WriteLine(town);

            string temperatureMax = forecast.Element("TEMPERATURE").Attribute("max").Value;
            string temperatureMin = forecast.Element("TEMPERATURE").Attribute("min").Value;

            string result = $"Температура в городе от {temperatureMin} до {temperatureMax}";
            return result;
        }
        #endregion
    }
}
