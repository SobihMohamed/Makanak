using Microsoft.AspNetCore.SignalR.Client;

namespace TestNotification
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string myToken = "حط_التوكن_الطويل_هنا_بتاع_بوست_مان";

            // تأكد إن ده البورت بتاعك صح (زي ما هو في المتصفح)
            string hubUrl = "https://localhost:7148/notify";

            Console.WriteLine("🔄 Connecting to SignalR...");

            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    // الكود ده هو اللي بيبعت التوكن صح للسيرفر
                    options.AccessTokenProvider = () => Task.FromResult<string?>(myToken);

                    // عشان يطنش مشاكل شهادة الأمان (Localhost SSL)
                    options.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            clientHandler.ServerCertificateCustomValidationCallback +=
                                (sender, certificate, chain, sslPolicyErrors) => { return true; };
                        return message;
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            // 👇 هنا بنقوله: "خلي ودنك مع السيرفر"
            connection.On<object>("ReceiveNotification", (data) =>
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\n🔔🔔 BINGO! Notification Received! 🔔🔔");
                Console.ResetColor();
                Console.WriteLine($"📦 Data: {data}");
                Console.WriteLine("------------------------------------------");
            });

            try
            {
                connection.StartAsync();
                Console.WriteLine($"✅ Connected Successfully to: {hubUrl}");
                Console.WriteLine("👂 I am listening for 'ReceiveNotification'...");
                Console.WriteLine("🚀 Go to Postman NOW and send the request!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Connection Error: {ex.Message}");
            }

            // عشان الشاشة متقفلش
            Console.ReadLine();
        }
    }
}
