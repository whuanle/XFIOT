using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTXFClient
{
    /*
     https://help.aliyun.com/document_detail/89301.html?spm=a2c4g.11174283.6.661.3a8b1668RBT8Nv
     */
    public class ThingModel
    {
        /// <summary>
        /// 设备上传属性
        /// </summary>
        public UpTopic upTopic { get; set; }
        /// <summary>
        /// 设置设备属性
        /// </summary>
        public SetTopic setTopic { get; set; }
        /// <summary>
        /// 设备上报事件
        /// </summary>
        public EventTopic eventTopic { get; set; }
        /// <summary>
        /// 服务器调用设备服务
        /// </summary>
        public ServiceTopic serviceTopic { get; set; }
        public ThingModel(string productKey, string deviceName)
        {
            upTopic = new UpTopic(productKey, deviceName);
            setTopic = new SetTopic(productKey, deviceName);
            eventTopic = new EventTopic(productKey, deviceName);
            serviceTopic = new ServiceTopic(productKey, deviceName);

        }
        public class UpTopic
        {
            /// <summary>
            /// 设备上报属性请求Topic--透传
            /// </summary>
            public string up_raw { get; set; }

            /// <summary>
            /// 设备上报属性响应Topic--透传
            /// </summary>
            public string up_raw_reply { get; set; }

            /// <summary>
            /// 设备上报属性请求Topic--Alink JSON
            /// </summary>
            public string post { get; set; }

            /// <summary>
            /// 设备上报属性响应Topic--Alink JSON
            /// </summary>
            public string post_reply { get; set; }

            public UpTopic(string productKey, string deviceName)
            {
                up_raw = $"/sys/{productKey}/{deviceName}/thing/model/up_raw";
                up_raw_reply = $"/sys/{productKey}/{deviceName}/thing/model/up_raw_reply";
                post = $"/sys/{productKey}/{deviceName}/thing/event/property/post";
                post_reply = $"/sys/{productKey}/{deviceName}/thing/event/property/post_reply";
            }

        }

        /// <summary>
        /// 设置设备属性
        /// </summary>
        public class SetTopic
        {
            /// <summary>
            /// 下行（透传）请求Topic
            /// </summary>
            public string down_raw { get; set; }
            /// <summary>
            /// 下行（透传）响应Topic
            /// </summary>
            public string down_raw_reply { get; set; }
            /// <summary>
            /// 下行（Alink JSON）请求Topic
            /// </summary>
            public string set { get; set; }
            /// <summary>
            /// 下行（Alink JSON）响应Topic
            /// </summary>
            public string set_reply { get; set; }

            public SetTopic(string productKey, string deviceName)
            {
                down_raw = $"/sys/{productKey}/{deviceName}/thing/model/down_raw";
                down_raw_reply = $"/sys/{productKey}/{deviceName}/thing/model/down_raw_reply";
                set = $"/sys/{productKey}/{deviceName}/thing/service/property/set";
                set_reply = $"/sys/{productKey}/{deviceName}/thing/service/property/set_reply";
            }
        }

        /// <summary>
        /// 设备事件上报
        /// </summary>
        public class EventTopic
        {
            /// <summary>
            /// 上行（透传） 请求Topic
            /// </summary>
            public string up_raw { get; set; }
            /// <summary>
            /// 上行（透传）响应Topic
            /// </summary>
            public string up_raw_reply { get; set; }

            /// <summary>
            /// 上行（Alink JSON）请求Topic
            /// </summary>
            public string post { get; set; }
            /// <summary>
            /// 上行（Alink JSON）响应Topic
            /// </summary>
            public string post_reply { get; set; }

            public EventTopic(string productKey, string deviceName)
            {
                up_raw = $"/sys/{productKey}/{deviceName}/thing/model/up_raw";
                up_raw_reply = $"/sys/{productKey}/{deviceName}/thing/model/up_raw_reply";
                post = $"/sys/{productKey}/{deviceName}/thing/event/" + "{tsl.event.identifier}/post";
                post_reply = $"/sys/{productKey}/{deviceName}/thing/event/" + "{tsl.event.identifier}/post_reply";
            }
        }

        /// <summary>
        /// 设备服务调用
        /// </summary>
        public class ServiceTopic
        {
            /// <summary>
            /// 下行（透传）请求Topic
            /// </summary>
            public string down_raw { get; set; }
            /// <summary>
            /// 下行（透传）响应Topic
            /// </summary>
            public string down_raw_reply { get; set; }

            /// <summary>
            ///  下行（Alink JSON）请求Topic
            /// </summary>
            public string identifier { get; set; }
            /// <summary>
            /// 下行（Alink JSON）Topic
            /// </summary>
            public string identifier_reply { get; set; }

            public ServiceTopic(string productKey, string deviceName)
            {
                down_raw = $"/sys/{productKey}/{deviceName}/thing/model/down_raw";
                down_raw_reply = $"/sys/{productKey}/{deviceName}/thing/model/down_raw_reply";
                identifier = $"/sys/{productKey}/{deviceName}/thing/service/" + "{tsl.service.identifier}";
                identifier_reply = $"/sys/{productKey}/{deviceName}/thing/service/" + "{tsl.service.identifier}_reply";
            }
        }

        /// <summary>
        /// 透传-请求属性、事件、服务返回格式
        /// </summary>
        public class up_rawModel
        {
            public string id { get; set; }
            public int code { get; set; }
            public string method { get; set; }
            public Data data { get; set; }
            public class Data
            {

            }
        }

        /// <summary>
        /// 属性、事件、服务 Alink响应数据格式
        /// </summary>
        public class post_replyModel
        {

            public string id { get; set; }
            public int code { get; set; }
            public Data data { get; set; }
            public class Data
            {

            }

        }
    }
    /// <summary>
    /// 设备上报属性
    /// </summary>


}
