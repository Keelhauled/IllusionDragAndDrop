using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
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

        public virtual void Scene_Load(string path) { }
        public virtual void Scene_Import(string path) { }
        public virtual void Character_LoadFemale(string path) { }
        public virtual void Character_LoadMale(string path) { }
        public virtual void Coordinate_Load(string path) { }
        public virtual void PoseData_Load(string path) { }
    }
}
