
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
    //  封装 M2MQTT ，配合阿里云IOT，目前仅支持透传
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

        #region 用于自定义功能的属性、服务、事件Topic(共用)
        // 上行用于上传数据等，下行用于发布指令
        // 上报 请求 Topic
        string up_raw;
        // 上报 响应 Topic
        string up_raw_reply;
        // 下行 请求 Topic
        string down_raw;
        // 下行 响应 Topic
        string down_raw_reply;
        #endregion

        /// <summary>
        /// 初始话连接设置
        /// </summary>
        /// <param name="ProductKey"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DeviceSecret"></param>
        /// <param name="RegionId"></param>
        public void Init(string ProductKey, string DeviceName, string DeviceSecret, string RegionId)
        {

            TopicHead = "/" + ProductKey + "/" + DeviceName;    // 针对设备生成 Topic 头

            #region 设置设备属性、服务、事件通讯
            up_raw = $"/sys/{ProductKey}/{DeviceName}/thing/model/up_raw";
            up_raw_reply = $"/sys/{ProductKey}/{DeviceName}/thing/model/up_raw_reply";

            down_raw = $"/sys/{ProductKey}/{DeviceName}/thing/model/down_raw";
            down_raw_reply = $"/sys/{ProductKey}/{DeviceName}/thing/model/down_raw_reply";
            #endregion

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
            mqttClientId = clientId + "|securemode=3,signmethod=" + signmethod + ",timestamp=" + t + "|";
            mqttUserName = DeviceName + "&" + ProductKey;
            mqttPassword = MQTTService.Sign(dict, DeviceSecret, signmethod);
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
            // 订阅消息，若不指定Topic的QOS，则全部为 0
            client.Subscribe(SubTopic, QOS);
            // 设置各种触发事件
            AddPublishEvent();
            // 建立连接
            client.Connect(mqttClientId, mqttUserName, mqttPassword, true, 60);


        }



        #region 发布消息

        /*
         发布功能仅用于上传数据、发布、上传属性，格式由自己在服务器和本地定义
         */

        /// <summary>
        /// 上传属性或发布 Topic
        /// </summary>
        /// <param name="PubTopic"></param>
        /// <param name="content"></param>
        /// <returns></returns>

        public int Subscribe(string PubTopic, byte[] content)
        {
            var id = client.Publish(PubTopic,content);
            return id;
        }

        /// <summary>
        /// 上传属性或发布 Topic
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
        /// 上传属性或发布 Topic，会将源数据进行 Base 64位加密再上传
        /// </summary>
        /// <param name="PubTopic"></param>
        /// <param name="betys">源数据</param>
        /// <returns></returns>
        public int SubscribeToBase(string PubTopic, byte[] betys)
        {
            string base64 = Convert.ToBase64String(betys);
            int id = client.Publish(PubTopic, Encoding.ASCII.GetBytes(base64));
            return id;
        }

        /// <summary>
        /// 设备上传属性--透传
        /// </summary>
        /// <param name="betys">要发布的数据</param>
        public int Up_Raw(byte[] bytes)
        {
            int id=Subscribe(up_raw,bytes);
            return id;
        }

        /// <summary>
        /// 设备上传属性--透传,转为 Base 64位加密后上传
        /// </summary>
        /// <param name="betys">要发布的数据</param>
        public int Up_RawToBase64(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            int id = Subscribe(up_raw, base64);
            return id;
        }


        /// <summary>
        /// 设备上传属性--透传
        /// </summary>
        /// <param name="up_rawTopic">自定义设备上报属性地址</param>
        /// <param name="betys">要发布的二进制数据</param>
        public int Up_Raw(string up_rawTopic,byte[] bytes)
        {
            int id = Subscribe(up_rawTopic, bytes);
            return id;
        }

        /// <summary>
        /// 设备上传属性--透传,Base 64 位加密后上传
        /// </summary>
        /// <param name="up_rawTopic">自定义设备上报属性地址</param>
        /// <param name="betys">要发布的二进制数据</param>
        public int Up_Raw_ToBase64(string up_rawTopic, byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            int id = Subscribe(up_rawTopic, base64);
            return id;
        }

        #endregion

        /// <summary>
        /// 添加订阅回调事件
        /// </summary>
        private void AddPublishEvent()
        {
            //订阅事件
            client.MqttMsgPublishReceived += PubEventHandler;
            client.MqttMsgPublished += PubedEventHandler;
            client.MqttMsgSubscribed += SubedEventHandler;
            client.MqttMsgUnsubscribed += UnSubedEventHandler;
            client.ConnectionClosed += ConnectionClosedEventHandler;
        }

        /// <summary>
        /// 取消已订阅的Topic
        /// </summary>
        public void UnSubEvent(string[] topics)
        {
            client.Unsubscribe(topics);

        }
        /// <summary>
        /// 全部使用预设的事件
        /// </summary>
        public void UseDefaultEventHandler()
        {
            PubEventHandler += Default_PubEventHandler;
            PubedEventHandler += Default_PubedEventHandler;
            SubedEventHandler += Default_SubedEventHandler;
            UnSubedEventHandler += Default_UnSubedEventHandler;
            ConnectionClosedEventHandler += Default_ConnectionClosedEventHandler;
        }


        #region 设置回调事件
        /// <summary>
        /// 订阅回调 - 当收到服务器消息时
        /// </summary>
        public uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgPublishEventHandler PubEventHandler;

        /// <summary>
        /// 当 QOS=1或2时，收到订阅触发
        /// </summary>
        public uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgPublishedEventHandler PubedEventHandler;

        /// <summary>
        /// 向服务器发布 Topic 时
        /// </summary>
        public uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgSubscribedEventHandler SubedEventHandler;

        /// <summary>
        /// 向服务器发布 Topic 失败时
        /// </summary>
        public uPLibrary.Networking.M2Mqtt.MqttClient.MqttMsgUnsubscribedEventHandler UnSubedEventHandler;


        /// <summary>
        /// 断开连接时
        /// </summary>
        public uPLibrary.Networking.M2Mqtt.MqttClient.ConnectionClosedEventHandler ConnectionClosedEventHandler;

        #endregion


        #region 事件的默认方法


        public void Default_PubEventHandler(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received
            string topic = e.Topic;
            string message = Encoding.ASCII.GetString(e.Message);
            Console.WriteLine("- - - - - - - - - - ");
            Console.WriteLine("get topic message,Date: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("topic: " + topic);
            Console.WriteLine("get messgae :" + message);
        }

        public void Default_PubedEventHandler(object sender, MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine("- - - - - - - - - - ");
            Console.WriteLine("published,Date: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("MessageId: " + e.MessageId + "    Is Published: " + e.IsPublished);
        }

        public void Default_SubedEventHandler(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("- - - - - - - - - - ");
            Console.WriteLine("Sub topic,Date: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("MessageId: " + e.MessageId);
            Console.WriteLine("List of granted QOS Levels:    " + Encoding.UTF8.GetString(e.GrantedQoSLevels));
        }
        public void Default_UnSubedEventHandler(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            Console.WriteLine("- - - - - - - - - - ");
            Console.WriteLine("Sub topic error,Date: " + DateTime.Now.ToLongTimeString());
            Console.WriteLine("MessageId:    " + e.MessageId);
        }

        public void Default_ConnectionClosedEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("- - - - - - - - - - ");
            Console.WriteLine("Connect Closed error,Date: " + DateTime.Now.ToLongTimeString());
        }
        #endregion




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
            //这里根据signMethod动态调整，本例子硬编码了： 'hmacmd5' 其它验证方式：HmacMD5,HmacSHA1,HmacSHA256 签名算法验证 
            var hmac = new HMACMD5(key); // deviceSecret

            byte[] hashBytes = hmac.ComputeHash(signContent);

            StringBuilder signBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
                signBuilder.AppendFormat("{0:x2}", b);

            return signBuilder.ToString();
        }
    }
}
