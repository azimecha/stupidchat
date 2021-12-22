using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;

namespace Azimecha.Stupidchat.Core {
    public static class ProtocolMessageSerializer {
        public static string Serialize(Protocol.MessageContainer mc) {
            if (!_dicMessageTypes.Value.ContainsKey(mc.MessageType))
                throw new ProtocolMessageFormatException($"Cannot serialize message - type {mc.MessageType} is not a valid message type");

            Type typeMessage = mc.Message.GetType();
            if (!_dicMessageTypes.Value[mc.MessageType].IsAssignableFrom(typeMessage))
                throw new ProtocolMessageFormatException($"Cannot serialize message - declared type {mc.MessageType} does not match actual type {typeMessage.FullName}");

            return JsonConvert.SerializeObject(mc);
        }

        public static string Serialize(this Protocol.IMessage msg)
            => Serialize(new Protocol.MessageContainer() { Message = msg, MessageType = msg.GetType().FullName, SentAt = DateTime.Now.Ticks });

        public static Protocol.MessageContainer Deserialize(string str) {
            JObject obj = JObject.Parse(str);

            Protocol.MessageContainer mc = new Protocol.MessageContainer() {
                MessageType = obj["MessageType"].Value<string>(),
                SentAt = obj["SentAt"].Value<long>()
            };

            if (!_dicMessageTypes.Value.ContainsKey(mc.MessageType))
                throw new ProtocolMessageFormatException($"Cannot deserialize message - type {mc.MessageType} is not a valid message type");

            mc.Message = (Protocol.IMessage)obj["Message"].ToObject(_dicMessageTypes.Value[mc.MessageType]);
            return mc;
        }

        private static readonly Type MESSAGE_INTERFACE = typeof(Protocol.IMessage);
        private static Lazy<IDictionary<string, Type>> _dicMessageTypes = new Lazy<IDictionary<string, Type>>(GetValidMessageTypes, 
            System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        private static IDictionary<string, Type> GetValidMessageTypes() {
            Dictionary<string, Type> dicTypes = new Dictionary<string, Type>();

            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies()) {
                if (ass.IsDynamic) continue;
                foreach (Type typeCur in ass.GetExportedTypes().Where(t => MESSAGE_INTERFACE.IsAssignableFrom(t)))
                    dicTypes.Add(typeCur.FullName, typeCur);
            }

            return dicTypes;
        }
    }

    public class ProtocolMessageFormatException : Exception {
        public ProtocolMessageFormatException(string strMessage) : base(strMessage) { }
        public ProtocolMessageFormatException(string strMessage, Exception exInner) : base(strMessage, exInner) { }
    }
}
