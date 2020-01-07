using BoardGameWebService.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BoardGameWebService
{
    public class ApiParameter
    {
        public static ApiParameter Create(string strData)
        {
            return JsonConvert.DeserializeObject<ApiParameter>(strData);
        }

        private const string PARAMETER_NAME = "parameter=";

        public Dictionary<ApiParameterEnum, string> Parameters;

        public ApiParameter()
        {
            Parameters = new Dictionary<ApiParameterEnum, string>();
        }

        public void AddParameter(ApiParameterEnum key, object data)
        {
            string value = JsonConvert.SerializeObject(data);
            Parameters[key] = value;
        }

        public T GetParameter<T>(ApiParameterEnum key)
        {
            string objStr;
            try
            {
                objStr = Parameters[key];
            }
            catch
            {
                throw new Exception("no key");
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(objStr);
            }
            catch
            {
                throw new Exception("parse fail");
            }
        }

        public string MakeStrData()
        {
            return PARAMETER_NAME + JsonConvert.SerializeObject(this);
        }
    }
}
