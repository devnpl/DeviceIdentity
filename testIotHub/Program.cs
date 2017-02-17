using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;

namespace testIotHub

{
    class Program
    {


        private const string DeviceConnectionString = "HostName=daenethub.azure-devices.net;DeviceId=IgusDE023;SharedAccessKeyName=mqtttest;SharedAccessKey=/xHcc1oTsW9bJb2NFYPTSFZOUM2eMdL0bGgTjxJSkUY=";
        private static int MESSAGE_COUNT = 10;

        //static void test()
        //{
        //    try
        //    {
              DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);


        //        string str = "{\"Temp\" : @1,\"Humidity\": @2}";

        //        deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(str)));

        //        Console.WriteLine("message hello world sent");

        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //    Console.ReadLine();
        //}

        static void Main(string[] args)
        {
            //test(); return;

            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);

                deviceClient.OpenAsync().Wait();
                SendEvent(deviceClient).Wait();
                ReceiveCommands(deviceClient).Wait();

                Console.WriteLine("Exited!");
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error in sample: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        static async Task SendEvent(DeviceClient deviceClient)
        {
            string dataBuffer;

            Console.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);

            for (int count = 0; count < MESSAGE_COUNT; count++)
            {
                dataBuffer = Guid.NewGuid().ToString();
                var eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
                Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

                await deviceClient.SendEventAsync(eventMessage);
            }
        }

        static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage = null;

            while (true)
            {
                try
                {
                    receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));

                    if (receivedMessage != null)
                    {
                        string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                        int propCount = 0;
                        foreach (var prop in receivedMessage.Properties)
                        {
                            Console.WriteLine("\t\tProperty[{0}> Key={1} : Value={2}", propCount++, prop.Key, prop.Value);
                        }

                        await deviceClient.CompleteAsync(receivedMessage);
                    }
                }
                finally
                {
                    if (receivedMessage != null)
                    {
                        receivedMessage.Dispose();
                    }
                }
            }
        }
    }
}
