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
            client = new XFMQTT();
            string ProductKey = "a1ZcYCgOOdz";
            string DeviceName = "OrangePi";
            string DeviceSecret = "PMlBrgIu42Fd5SciFrkMuGimVeBHKiWm";
            string RegionId = "cn-shanghai";


            client.Init(ProductKey,DeviceName,DeviceSecret,RegionId);
            string[] topic = { "/user/led" };

            //client.PubEventHandler += Default_PublishEvent;
            //client.PubedEventHandler += Default_testPublishEvent;

            client.UseDefaultEventHandler();
            client.ConnectMqtt(topic);


            //uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgPublishEventHandler


            while (true)
            {
                Console.WriteLine("输入发布");
                var str = Console.ReadLine();
                client.Subscribe("/user/update",str);
                Thread.Sleep(1000);
            }
           
        }
        public static void Default_PublishEvent(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received
            string topic = e.Topic;
            
            string message = Encoding.ASCII.GetString(e.Message);
            Console.WriteLine("topic 名称: " + topic);
            Console.WriteLine("收到服务器的消息 :" + message);

            Console.WriteLine(e.DupFlag+"|"+e.QosLevel+"|"+e.Retain);
        }

        public static void Default_testPublishEvent(object sender, MqttMsgPublishedEventArgs e)
        {
            // handle message received

            Console.WriteLine("测试topic 名称: " + e.IsPublished);
            Console.WriteLine("测试收到服务器的消息 :" + e.MessageId);
        }
    }
}
