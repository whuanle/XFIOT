using System;
using System.Text;
using System.Threading;
using MQTTXFClient;
using uPLibrary.Networking.M2Mqtt.Messages;
using WiringXF;
namespace Demo_Raspberry_LED
{
    class Program
    {
        static XFMQTT client;
        static void Main(string[] args)
        {
            string ProductKey = "a1ZcYCgOOdz";
            string DeviceName = "OrangePi";
            string DeviceSecret = "PMlBrgIu42Fd5SciFrkMuGimVeBHKiWm";
            string RegionId = "cn-shanghai";

            client = new XFMQTT();
            client.Init(ProductKey, DeviceName, DeviceSecret, RegionId);

            string[] topic = { "/user/led" };
            client.UseDefaultEventHandler();
            client.PubEventHandler -= client.Default_PubEventHandler;
            client.PubEventHandler += Default_PubEventHandler;
            client.ConnectMqtt(topic);



            Thread thread = new Thread(OpenOrClose);
            thread.Start();
            Console.WriteLine("Runing...");
            Console.ReadKey();
        }

        public static void Default_PubEventHandler(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received
            string topic = e.Topic;
            string message = System.Text.Encoding.ASCII.GetString(e.Message);
            Console.WriteLine("- - - - - - - - - - ");
            Console.WriteLine("get topic message,Date: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("topic: " + topic);
            Console.WriteLine("get messgae :" + message);


            // 假设服务器发送格式为 open/close pin
            // 0为close，1为open，pin是针脚，格式 1 27
            string[] str = message.Split(" ");
            if (str.Length == 2)
            {
            code = Convert.ToInt32(str[0]);
            pin = Convert.ToInt32(str[1]);
            }


        }
        static int code { get; set; }
        static int pin;
        public static void OpenOrClose()
        {
            Console.WriteLine(wiringPi_Setup.wiringPiSetupGpio_());

            while (true)
            {
                if (code == 0)
                {
                    wiringPi_Core.pinMode_(pin, code);
                    Thread.Sleep(1000);
                    continue;
                }
                Console.WriteLine("openorclose:"+code+"     pin:"+pin);
                wiringPi_Core.pinMode_(pin, code);
                wiringPi_Core.digitalWrite_(pin, 1);
                Thread.Sleep(500);
                wiringPi_Core.digitalWrite_(pin, 0);
                Thread.Sleep(500);

            }
        }
    }
}
