using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Azimecha.Stupidchat.Core {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ProcessorAttribute : Attribute {
        public ProcessorAttribute(Type typeToProcess) {
            TypeProcessed = typeToProcess;
        }

        public Type TypeProcessed { get; set; }

        public static IDictionary<Type, MethodInfo> BuildProcessorsList<TProcessorClass, TToProcess>()
            => BuildProcessorsList(typeof(TProcessorClass), typeof(TToProcess));

        public static IDictionary<Type, MethodInfo> BuildProcessorsList(Type typeContainingProcessorMethods, Type typeToProcess) {
            IDictionary<Type, MethodInfo> dicMethods = new Dictionary<Type, MethodInfo>();

            foreach (MethodInfo infMethod in typeContainingProcessorMethods.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
            {
                ProcessorAttribute attrib = infMethod.GetCustomAttribute<ProcessorAttribute>();
                if (attrib is null) continue;
                if (!typeToProcess.IsAssignableFrom(attrib.TypeProcessed)) continue;
                dicMethods.Add(attrib.TypeProcessed, infMethod);
            }

            return dicMethods;
        }

        public static IDictionary<Type, Action<TToProcess>> BindProcessorsList<TProcessorClass, TToProcess>
            (TProcessorClass objBindTo, IDictionary<Type, MethodInfo> dicProcessors)
        {
            IDictionary<Type, Action<TToProcess>> dicBound = new Dictionary<Type, Action<TToProcess>>();

            foreach (KeyValuePair<Type, MethodInfo> kvp in dicProcessors) {
                Delegate procTakesDerived = kvp.Value.CreateDelegate(typeof(Action<>).MakeGenericType(kvp.Key), objBindTo);
                Action<TToProcess> procTakesBase = (TToProcess obj) => procTakesDerived.DynamicInvoke(obj);
                dicBound.Add(kvp.Key, procTakesBase);
            }

            return dicBound;
        }
    }
}
