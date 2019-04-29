using System;
using System.Text;
using System.Threading;
using MQTTTest;
using MQTTXFClient;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTTest
{
    class Program
    {
        static XFMQTT client;
        static void Main(string[] args)
        {
            client = new XFMQTT("a1u8YIGj2Dl", "RaspberryJson");
            //string ProductKey = "a1ZcYCgOOdz";
            //string DeviceName = "OrangePi";
            //string DeviceSecret = "PMlBrgIu42Fd5SciFrkMuGimVeBHKiWm";
            //string RegionId = "cn-shanghai";


            client.Init("WsEWfiDLIiqazSFjrWJPsslzkVt8cktN", "cn-shanghai");
            string[] topic = { client.CombineHeadTopic("get") };

            //client.PubEventHandler += Default_PublishEvent;
            //client.PubedEventHandler += Default_testPublishEvent;

            client.UseDefaultEventHandler();
            client.ConnectMqtt(topic);

            while (true)
            {
                Console.ReadKey();
                float a = (float)(new Random()).Next(0, 100);
                float b = (float)(new Random()).NextDouble();
                float c = a + b;
                string up_Raw = "{\"id\":\"123\",\"version\":\"1.0\",\"params\":{\"temperature_CPU\":{\"value\":" + c + ",\"time\":1524448722000}},\"method\":\"thing.event.property.post\"}";
                Console.WriteLine(client.Thing_Property_Post(up_Raw));
                Thread.Sleep(2000);
            }
            //uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgPublishEventHandler


            //while (true)
            //{
            //    Console.WriteLine("输入发布");
            //    var str = Console.ReadLine();
            //    client.Subscribe("/user/update",str);
            //    Thread.Sleep(1000);
            //}

        }
        public static void Default_PublishEvent(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received
            string topic = e.Topic;

            string message = Encoding.ASCII.GetString(e.Message);
            Console.WriteLine("topic 名称: " + topic);
            Console.WriteLine("收到服务器的消息 :" + message);

            Console.WriteLine(e.DupFlag + "|" + e.QosLevel + "|" + e.Retain);
        }

        public static void Default_testPublishEvent(object sender, MqttMsgPublishedEventArgs e)
        {
            // handle message received

            Console.WriteLine("测试topic 名称: " + e.IsPublished);
            Console.WriteLine("测试收到服务器的消息 :" + e.MessageId);
        }
    }
}
