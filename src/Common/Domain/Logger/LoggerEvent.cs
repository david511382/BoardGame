using System;
using System.Collections;
using System.Collections.Generic;

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
        public string Message { get; set; }

        public StructLoggerEvent()
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
