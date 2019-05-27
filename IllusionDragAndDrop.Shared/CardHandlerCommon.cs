using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IllusionDragAndDrop.Shared
{
    public abstract class CardHandlerCommon<T> where T : CardHandlerCommon<T>
    {
        static Dictionary<Type, T> CardHandlers = new Dictionary<Type, T>();
        public abstract bool Condition { get; }

        public static T GetActiveCardHandler()
        {
            var mainType = typeof(T);
            var inheritingTypes = mainType.Assembly.GetTypes().Where(x => x.IsSubclassOf(mainType));

            foreach(var type in inheritingTypes)
            {
                if(!CardHandlers.TryGetValue(type, out var handler))
                {
                    handler = (T)Activator.CreateInstance(type);
                    CardHandlers.Add(type, handler);
                }

                if(handler.Condition)
                    return handler;
            }

            Logger.Log(LogLevel.Message, "No handler found for this scene");
            return default(T);
        }
    }
}
