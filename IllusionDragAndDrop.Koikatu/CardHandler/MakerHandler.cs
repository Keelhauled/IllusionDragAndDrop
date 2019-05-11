﻿using B83.Win32;
using BepInEx.Logging;
using ChaCustom;
using Harmony;
using Manager;
using System;
using System.Linq;
using UnityEngine;
using Logger = BepInEx.Logger;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public class MakerHandler : CardHandlerMain
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "CustomScene");

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            Logger.Log(LogLevel.Message, "Loading character");

            var customCharaFile = GameObject.FindObjectOfType<CustomCharaFile>();
            var traverse = Traverse.Create(customCharaFile);
            var fileWindow = traverse.Field("fileWindow").GetValue<CustomFileWindow>();
            var listCtrl = traverse.Field("listCtrl").GetValue<CustomFileListCtrl>();

            var index = listCtrl.GetInclusiveCount() + 1;
            listCtrl.AddList(index, "", "", "", path, "", new DateTime());
            listCtrl.Create(customCharaFile.OnChangeSelect);
            listCtrl.SelectItem(index);
            fileWindow.btnChaLoadLoad.onClick.Invoke();
            listCtrl.RemoveList(index);
            listCtrl.Create(customCharaFile.OnChangeSelect);
        }

        public override void Coordinate_Load(string path, POINT pos)
        {
            Logger.Log(LogLevel.Message, "Loading coordinate");

            var customCoordinateFile = GameObject.FindObjectOfType<CustomCoordinateFile>();
            var traverse = Traverse.Create(customCoordinateFile);
            var fileWindow = traverse.Field("fileWindow").GetValue<CustomFileWindow>();
            var listCtrl = traverse.Field("listCtrl").GetValue<CustomFileListCtrl>();

            var index = listCtrl.GetInclusiveCount() + 1;
            listCtrl.AddList(index, "", "", "", path, "", new DateTime());
            listCtrl.Create(customCoordinateFile.OnChangeSelect);
            listCtrl.SelectItem(index);
            fileWindow.btnCoordeLoadLoad.onClick.Invoke();
            listCtrl.RemoveList(index);
            listCtrl.Create(customCoordinateFile.OnChangeSelect);
        }
    }
}
