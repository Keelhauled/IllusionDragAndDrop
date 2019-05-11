using B83.Win32;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Logger = BepInEx.Logger;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public class CardHandlerMain
    {
        static Dictionary<Type, CardHandlerMain> cardHandlers = new Dictionary<Type, CardHandlerMain>();

        public static CardHandlerMain GetActiveCardHandler()
        {
            var mainType = typeof(CardHandlerMain);
            var inheritingTypes = mainType.Assembly.GetTypes().Where(x => x.IsSubclassOf(mainType));

            foreach(var type in inheritingTypes)
            {
                if(!cardHandlers.TryGetValue(type, out var handler))
                {
                    handler = (CardHandlerMain)Activator.CreateInstance(type);
                    cardHandlers.Add(type, handler);
                }

                if(handler.Condition)
                    return handler;
            }

            Logger.Log(LogLevel.Message, "No handler found for this scene");
            return null;
        }

        public virtual bool Condition => throw new NotImplementedException();

        public virtual void Scene_Load(string path, POINT pos) { }
        public virtual void Scene_Import(string path, POINT pos) { }
        public virtual void Character_Load(string path, POINT pos, byte sex) { }
        public virtual void Coordinate_Load(string path, POINT pos) { }
        public virtual void PoseData_Load(string path, POINT pos) { }
    }
}
