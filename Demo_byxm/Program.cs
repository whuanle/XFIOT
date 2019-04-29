using System;
using System.Threading;
using MQTTXFClient;
using System.Text;
namespace Demo_byxm
{
    class Program
    {
        static XFMQTT client;
        static void Main(string[] args)
        {
            //client = new XFMQTT();
            //string ProductKey = "a1ZR7cgC9gW";
            //string DeviceName = "Raspberry";
            //string DeviceSecret = "96KeL0p5Cty74JDDmv9e8s3cya4sAl1l";
            //string RegionId = "cn-shanghai";


            //client.Init(ProductKey, DeviceName, DeviceSecret, RegionId);
            //string[] topic = { "/user/get" };

            ////client.PubEventHandler += Default_PublishEvent;
            ////client.PubedEventHandler += Default_testPublishEvent;

            //client.UseDefaultEventHandler();
            //client.ConnectMqtt(topic);


            ////uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgPublishEventHandler


            //while (true)
            //{
            //    Console.WriteLine("输入发布");
            //    //var str = Console.ReadLine();
            //    byte[] b = { 0x00,
            //        0x00,0x00,0x01,0x02,
            //        0x00,0x00,0x00,0x14};
            //    client.Up_raw(b);
            //    Thread.Sleep(5000);
            //}
        }
    }
}
