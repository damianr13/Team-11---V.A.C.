using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace IoTHub
{
    class Program
    {
        static System.IO.StreamWriter file;
        static RegistryManager registryManager;
        static String connectionString = "HostName=IvaTelemtry.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=n41FtpXu1dxjGz0mJl+GtT95c7FtaMgbWgjXr5l9Y5U=";

        private async static Task AddDeviceAsync()
        {
            string deviceId = "myFirstDevice";
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            file = new System.IO.StreamWriter("c:\\users\\damian\\desktop\\text.txt");
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
            file.WriteLine(device.Authentication.SymmetricKey.PrimaryKey);
            file.Close();

        }

        static void Main(string[] args)
        {

            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }
    }
}
