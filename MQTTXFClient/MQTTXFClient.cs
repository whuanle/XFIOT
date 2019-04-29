using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;

namespace MQTTXFClient
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class XFMQTT
    {
        public XFMQTT(string _ProductKey, string _DeviceName)
        {
            ProductKey = _ProductKey;
            DeviceName = _DeviceName;


            TopicHead = "/" + ProductKey + "/" + DeviceName + "/user/";    // 针对设备生成 Topic 头
            // 生成属性事件服务地址
            thingModel = new ThingModel(ProductKey, DeviceName);
        }

        string TopicHead;         // 每个设备都有一个用于通讯的特定 URL 头
        string targetServer;      // 生成 MQTT 通讯地址

        readonly string ProductKey;
        readonly string DeviceName;

        /// <summary>
        /// 连接客户端
        /// </summary>
        MqttClient client;
        string mqttUserName;
        string mqttPassword;
        string mqttClientId;

        // 生成设备属性、服务、事件通讯的topic
        public readonly ThingModel thingModel;

        /// <summary>
        /// 初始化连接设置
        /// </summary>
        /// <param name="ProductKey"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DeviceSecret"></param>
        /// <param name="RegionId"></param>
        public void Init(string DeviceSecret, string RegionId)
        {
            // 生成唯一 ClientID
            string clientId = CreateClientId();

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
            mqttPassword = MQTTEncryption.Sign(dict, DeviceSecret, signmethod);
            targetServer = ProductKey + ".iot-as-mqtt." + RegionId + ".aliyuncs.com";
        }


        /// <summary>
        /// 用于生成唯一clientId
        /// </summary>
        /// <returns></returns>
        public string CreateClientId()
        {
            // 生成唯一 ClientID
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string clientId = host.AddressList.FirstOrDefault(
                ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            return clientId;
        }


        /// <summary>
        /// 创建MQTT连接并与服务器通讯，订阅需要的Topic
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

        /*
         * 最终必须为 byte[]才能上传，默认以10进制处理。
         * 考虑到可能需要使用2进制或16进制，可先转换好byte[]再调用
         */


        #region 通用发布

        /// <summary>
        /// 上传属性或发布 Topic
        /// </summary>
        /// <param name="PubTopic">要发布的 Topic 名称</param>
        /// <param name="content">上传的内容</param>
        /// <returns></returns>
        public int Subscribe(string PubTopic, byte[] content)
        {
            var id = client.Publish(PubTopic, content);
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

        #endregion



        #region 设备属性上报


        #region 透传
        /// <summary>
        /// 设备上传属性--透传
        /// </summary>
        /// <param name="betys">要发布的数据</param>
        public int Thing_Property_Up_Raw(byte[] bytes)
        {
            int id = Subscribe(thingModel.upTopic.up_raw, bytes);
            return id;
        }

        /// <summary>
        /// 自定义设备上传属性地址、上传属性--透传。不建议使用，建议使用 Up_Raw(byte[] bytes)
        /// </summary>
        /// <param name="up_rawTopic">自定义设备上报属性地址</param>
        /// <param name="betys">要发布的二进制数据</param>
        public int Thing_Property_Up_Raw(string up_rawTopic, byte[] bytes)
        {
            int id = Subscribe(up_rawTopic, bytes);
            return id;
        }

        /// <summary>
        /// 设备上传属性--透传,转为 Base 64位加密后上传
        /// </summary>
        /// <param name="betys">要发布的数据</param>
        public int Thing_Property_Up_RawToBase64(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            int id = Subscribe(thingModel.upTopic.up_raw, base64);
            return id;
        }


        /// <summary>
        /// 设备上传属性--透传,Base 64 位加密后上传--不建议使用此方法
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

        #region json上传
        //后面要加泛型，暂时这样

        /// <summary>
        /// 上传设备属性--Alink Json
        /// </summary>
        /// <param name="json">josn</param>
        /// <param name="isToLower">是否转为小写</param>
        /// <returns></returns>
        public int Thing_Property_Post(string json, bool isToLwer = true)
        {
            if (isToLwer == true)
                json = json.ToLower();

            int id = Subscribe(thingModel.upTopic.post, json);
            return id;
        }
        public int Thing_Property_Post(byte[] json)
        {
            int id = Subscribe(thingModel.upTopic.post, json);
            return id;
        }

        /// <summary>
        /// 上传设备属性--Alink Json
        /// </summary>
        /// <typeparam name="AlinkModel">模型类型</typeparam>
        /// <param name="model">模型</param>
        /// <param name="isToLower">是否全部转为小写</param>
        /// <returns></returns>
        public int Thing_Property_Post<AlinkModel>(AlinkModel model, bool isToLower = true)
        {
            string json = JsonConvert.SerializeObject(model);
            if (isToLower == true)
                json = json.ToLower();

            int id = Subscribe(thingModel.upTopic.post, json);
            return id;
        }

        #endregion
        #endregion

        #region 设置设备属性

        #region 透传
        /// <summary>
        /// 收到服务器属性设置命令，返回响应
        /// </summary>
        /// <param name="content">响应内容</param>
        /// <returns></returns>
        public int Thing_Property_down_raw_reply(byte[] content)
        {
            int id = Subscribe(thingModel.setTopic.down_raw_reply, content);
            return id;
        }

        #endregion

        #region Alink Json
        /// <summary>
        /// 收到服务器属性设置命令，返回响应
        /// </summary>
        /// <param name="content">响应内容</param>
        /// <param name="isToLower">是否转为小写</param>
        /// <returns></returns>
        public int Thing_Property_set(string content, bool isToLower = true)
        {
            if (isToLower == true)
                content = content.ToLower();
            int id = Subscribe(thingModel.setTopic.down_raw_reply, content);
            return id;
        }
        public int Thing_Property_set(byte[] content)
        {
            int id = Subscribe(thingModel.setTopic.down_raw_reply, content);
            return id;
        }
        /// <summary>
        /// 设备属性下发设置
        /// </summary>
        /// <typeparam name="SetJson">收到的json</typeparam>
        /// <param name="model">模型</param>
        /// <param name="isToLower">是否转为小写</param>
        /// <returns></returns>
        public int Thing_Property_set<SetJson>(SetJson model, bool isToLower = true)
        {
            string json = JsonConvert.SerializeObject(model);
            if (isToLower == true)
                json = json.ToLower();

            int id = Subscribe(thingModel.setTopic.down_raw_reply, json);
            return id;
        }
        #endregion



        #endregion

        #region 设备事件上报

        #region 透传

        /// <summary>
        /// 设备事件上报,以字符串内容上传
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Thing_Event_up_raw(string content)
        {
            int id = Subscribe(thingModel.eventTopic.up_raw, content);
            return id;
        }

        /// <summary>
        /// 设备事件上报,把原始报文 Base64 加密后上传
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Thing_Event_up_raw_Base64(byte[] content)
        {
            int id = SubscribeToBase(thingModel.eventTopic.up_raw, content);
            return id;
        }

        #endregion

        #region Alink Json

        /// <summary>
        /// 设备事件上报 Alink JSON
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Thing_Event_Post(string content, bool isToLower = true)
        {
            if (isToLower == true)
                content = content.ToLower();
            int id = Subscribe(thingModel.eventTopic.post, content);
            return id;
        }

        /// <summary>
        /// 设备事件上报 Alink JSON
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isToLower">是否转为小写</param>
        /// <returns></returns>
        public int Thing_Event_Post<EventJson>(EventJson model, bool isToLower = true)
        {
            string json = JsonConvert.SerializeObject(model);
            if (isToLower == true)
                json = json.ToLower();
            int id = Subscribe(thingModel.eventTopic.post, json);
            return id;
        }



        #endregion

        #endregion


        #region 设备服务调用

        #region 透传

        /// <summary>
        /// 设备服务调用--透传
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Thing_Service_Down_Reply(byte[] content)
        {
            int id = Subscribe(thingModel.serviceTopic.down_raw_reply, content);
            return id;
        }

        #endregion

        #region Alink Json

        /// <summary>
        /// 设备服务调用
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isToLower">是否转为小写</param>
        /// <returns></returns>
        public int THing_Service_Identifier_Reply(string content, bool isToLower = true)
        {
            if (isToLower == true)
                content = content.ToLower();
            int id = Subscribe(thingModel.serviceTopic.identifier_reply, content);
            return id;
        }
        /// <summary>
        /// 设备服务调用
        /// </summary>
        /// <typeparam name="ServiceJsonModel"></typeparam>
        /// <param name="model">模型</param>
        /// <param name="isToLower">是否转为小写</param>
        /// <returns></returns>
        public int THing_Service_Identifier_Reply<ServiceJsonModel>(ServiceJsonModel model, bool isToLower = true)
        {
            string json = JsonConvert.SerializeObject(model);
            if (isToLower == true)
                json = json.ToLower();
            int id = Subscribe(thingModel.serviceTopic.identifier_reply, json);
            return id;
        }

        #endregion

        #endregion


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

        /// <summary>
        /// 生成完整的Topic，只需传入 /user/后面的内容即可，如 update
        /// </summary>
        /// <param name="topicend"></param>
        /// <returns></returns>
        public string CombineHeadTopic(string topicend)
        {
            return TopicHead + topicend;
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
            Console.WriteLine("get messgae :\n" + message);
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

    /// <summary>
    /// MQTT 加密服务
    /// </summary>
    class MQTTEncryption
    {
        /// <summary>
        /// 转换密码，设备身份三元组，ProductKey，DeviceName，DeviceSecret
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