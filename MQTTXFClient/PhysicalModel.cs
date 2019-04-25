using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTXFClient
{
    public class PhysicalModel
    {
        public string schema { get { return "https://iotx-tsl.oss-ap-southeast-1.aliyuncs.com/schema.json"; } set { schema = value; } }
        /// <summary>
        /// 产品
        /// </summary>
        public Profile profile;
        public class Profile
        {
            public string productKey { get; set; }
        }

        public class Services
        {
            public List<OutPutDate> outputData { get; set; }

            public class OutPutDate
            {
                public string identifier { get; set; }
                public string name { get; set; }
                public List<DataType> dataType { get; set; }
                public class DataType
                {
                   public Specs specs { get; set; }
                    public class Specs
                    {
                       
                    }
                }
            }
        }
    }
}
