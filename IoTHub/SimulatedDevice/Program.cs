using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedDevice
{ 

    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "IvaTelemtry.azure-devices.net";
        static string deviceId = "myFirstDevice";
        static string connectionString = "HostName=IvaTelemtry.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=n41FtpXu1dxjGz0mJl+GtT95c7FtaMgbWgjXr5l9Y5U=";

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        static string deviceKey = Base64Encode("myFirstDevice");
        static int curHeartRate = 0;
        private static async void SendDeviceToCloudMessagesAsync()
        {
            
            Random rand = new Random();

            while (true)
            {
                curHeartRate = rand.Next(60, 120);
                var telemetryDataPoint = new
                {
                    clientName = "Jean",
                    heartRate = curHeartRate,
                    measureTime = DateTime.UtcNow

                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Thread.Sleep(1000);
            }
        }

        private static async void SendDeviceToCloudInteractiveMessagesAsync()
        {
            while (true)
            {
                var interactiveMessageString = "Alert message!";
                var interactiveMessage = new Message(Encoding.ASCII.GetBytes(interactiveMessageString));
                interactiveMessage.Properties["messageType"] = "interactive";
                interactiveMessage.MessageId = Guid.NewGuid().ToString();

                await deviceClient.SendEventAsync(interactiveMessage);
                Console.WriteLine("{0} > Sending interactive message: {1}", DateTime.Now, interactiveMessageString);

                Thread.Sleep(1000);
            }
        }
        private static async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, deviceId);

            //ReceiveC2dAsync();
            SendDeviceToCloudMessagesAsync();
            //SendDeviceToCloudInteractiveMessagesAsync();
            Console.ReadLine();
        }
    }
}
