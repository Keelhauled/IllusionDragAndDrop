using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manager;
using ChaCustom;
using UnityEngine;
using Logger = BepInEx.Logger;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public class MakerHandler : CardHandlerMain
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "Studio");

        public override void Scene_Load(string path)
        {
            Logger.Log(LogLevel.Message, "Cannot load a scene in maker");
        }

        public override void Scene_Import(string path)
        {
            Logger.Log(LogLevel.Message, "Cannot import a scene into maker");
        }

        public override void Character_LoadFemale(string path)
        {
            Logger.Log(LogLevel.Message, "Loading female");
            
        }

        public override void Character_LoadMale(string path)
        {
            Logger.Log(LogLevel.Message, "Loading male");
            
        }

        public override void Coordinate_Load(string path)
        {
            Logger.Log(LogLevel.Message, "Loading coordinate");

        }

        public override void PoseData_Load(string path)
        {
            Logger.Log(LogLevel.Message, "Loading pose");

        }
    }
}
