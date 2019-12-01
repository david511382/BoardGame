using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Domain.Logger
{
    /// <summary>
    /// nlog config 用 <field name="ClientIP" layout="${event-properties:item=ClientIP}" /> 讀取_properties的ClientIP
    /// </summary>
    public class LoggerEvent : IEnumerable<KeyValuePair<string, object>>
    {
        public static Func<LoggerEvent, Exception, string> Formatter { get; } = (l, e) => l.Message;

        List<KeyValuePair<string, object>> _properties = new List<KeyValuePair<string, object>>();

        public string Message { get; }

        public LoggerEvent(string message = "")
        {
            Message = message;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public LoggerEvent AddProp(string name, object value)
        {
            _properties.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }
    }

    public class StructLoggerEvent
    {
        public string Message
        {
            get
            {
                return _logMsg.ToString();
            }
            set
            {
                _logMsg.Clear();
                _logMsg.Append(value);
            }
        }
        private StringBuilder _logMsg;

        public StructLoggerEvent(string message = null)
        {
            _logMsg = new StringBuilder();
            Message = message;
        }

        public void Log(string msg)
        {
            _logMsg.AppendLine(msg);
        }

        public void Log(string title, string msg)
        {
            Log($"{title}:{msg}");
        }

        public void Log(string title, object obj)
        {
            Log(title, JsonConvert.SerializeObject(obj));
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
