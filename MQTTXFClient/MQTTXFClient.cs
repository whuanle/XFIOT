
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static uPLibrary.Networking.M2Mqtt.MqttClient;

namespace MQTTXFClient
{
    //  封装 M2MQTT ，配合阿里云IOT
    public class XFMQTT
    {
        public XFMQTT()
        {

        }
       
        string TopicHead;         // 每个设备都有一个用于通讯的特定 URL 头
        string targetServer;      // 生成 MQTT 通讯地址
        /// <summary>
        /// 连接客户端
        /// </summary>
        MqttClient client;
        string mqttUserName;
        string mqttPassword;
        string mqttClientId;

        public void Init(string ProductKey, string DeviceName, string DeviceSecret, string RegionId)
        {
            
            TopicHead = "/" + ProductKey + "/" + DeviceName;    // 针对设备生成 Topic 头

            // 生成唯一 ClientID
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string clientId = host.AddressList.FirstOrDefault(
                ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();



            string t = Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            string signmethod = "hmacmd5";  // 使用hmacmd5加密 ,可使用其它验证方式：HmacMD5,HmacSHA1,HmacSHA256 签名算法验证

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("productKey", ProductKey);
            dict.Add("deviceName", DeviceName);
            dict.Add("clientId", clientId);
            dict.Add("timestamp", t);

            // 阿里云客户端连接认证说明 https://help.aliyun.com/document_detail/73742.html?spm=a2c4g.11186623.2.15.52003f86YTzGCo
            mqttUserName = DeviceName + "&" + ProductKey;
            mqttPassword = MQTTService.Sign(dict, DeviceSecret, signmethod);
            mqttClientId = clientId + "|securemode=3,signmethod=" + signmethod + ",timestamp=" + t + "|"; ;
            targetServer = ProductKey + ".iot-as-mqtt." + RegionId + ".aliyuncs.com";


        }


        /// <summary>
        /// 创建MQTT连接并与服务器通讯，订阅消息
        /// </summary>
        /// <param name="SubTopic">订阅列表</param>
        /// <param name="QOS">QOS，0或1，默认全为0</param>
        public void ConnectMqtt(string[] SubTopic, byte[] QOS = null)
        {
            if (QOS == null)
            {
                QOS = new byte[SubTopic.Length];
                for (int i = 0; i < SubTopic.Length; i++)
                {
                    QOS[i] = 0;
                }
            }

            client = new MqttClient(targetServer);


            client.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;

            // 建立连接
            client.Connect(mqttClientId, mqttUserName, mqttPassword, false, 60);

            //订阅消息
            client.Subscribe(SubTopic, QOS);
            AddPublishEvent();
        }



        /// <summary>
        /// 向服务器发布消息
        /// </summary>
        /// <param name="PubTopic">要发布到的Topic名称</param>
        /// <param name="content">消息内容</param>
        /// <returns></returns>
        public int Subscribe(string PubTopic, string content)
        {
            //发布消息
            //String content = "{'content':'msg from :" + mqttClientId + ", 这里是.NET设备'}";

            var id = client.Publish(PubTopic, Encoding.ASCII.GetBytes(content));
            return id;
        }



        /// <summary>
        /// 添加订阅回调事件
        /// </summary>
        private void AddPublishEvent()
        {
            //订阅事件
            client.MqttMsgPublishReceived += PubEventHandler;
        }

        /// <summary>
        /// 订阅回调
        /// </summary>
        public uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgPublishEventHandler PubEventHandler;

        /// <summary>
        /// 默认的回调方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Default_PublishEvent(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received
            string topic = e.Topic;
            string message = Encoding.ASCII.GetString(e.Message);
            Console.WriteLine("topic: " + topic);
            Console.WriteLine("get messgae :" + message);
        }
    }


    class MQTTService
    {
        /// <summary>
        /// 获取设备身份三元组，ProductKey，DeviceName，DeviceSecret
        /// </summary>
        /// <param name="param"></param>
        /// <param name="deviceSecret"></param>
        /// <param name="signMethod"></param>
        /// <returns></returns>
        public static string Sign(Dictionary<string, string> param, string deviceSecret, string signMethod)
        {
            string[] sortedKey = param.Keys.ToArray();
            Array.Sort(sortedKey);

            StringBuilder builder = new StringBuilder();
            foreach (var i in sortedKey)
            {
                builder.Append(i).Append(param[i]);
            }
            // deviceSecret
            byte[] key = Encoding.UTF8.GetBytes(deviceSecret);
            byte[] signContent = Encoding.UTF8.GetBytes(builder.ToString());
            //这里根据signMethod动态调整，本例子硬编码了： 'hmacmd5'
            var hmac = new HMACMD5(key); // deviceSecret
            byte[] hashBytes = hmac.ComputeHash(signContent);

            StringBuilder signBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
                signBuilder.AppendFormat("{0:x2}", b);

            return signBuilder.ToString();
        }
    }
}
