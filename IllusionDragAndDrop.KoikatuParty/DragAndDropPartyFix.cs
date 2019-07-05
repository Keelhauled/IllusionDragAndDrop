using B83.Win32;
using BepInEx;
using ChaCustom;
using Harmony;
using IllusionDragAndDrop.Koikatu.CardHandler;
using Manager;
using System;
using System.Linq;
using UnityEngine;

namespace IllusionDragAndDrop.KoikatuParty
{
    [BepInPlugin("keelhauled.draganddroppartyfix", "Illusion Drag & Drop Partyfix", Version)]
    public class DragAndDropPartyFix : BaseUnityPlugin
    {
        public const string Version = "1.0.0";
        public const string Process = "Koikatsu Party";
    }

    public class MakerHandlerParty : CardHandlerMethods
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "CustomScene") && Paths.ProcessName == DragAndDropPartyFix.Process;

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            var customCharaFile = GameObject.FindObjectOfType<CustomCharaFile>();
            var traverse = Traverse.Create(customCharaFile);
            var fileWindow = traverse.Field("fileWindow").GetValue<CustomFileWindow>();
            var listCtrl = traverse.Field("listCtrl").GetValue<CustomFileListCtrl>();

            var index = listCtrl.GetInclusiveCount() + 1;
            listCtrl.AddList(new CustomFileInfo(new FolderAssist.FileInfo(path, "", new DateTime())) { index = index, name = "" });
            listCtrl.Create(customCharaFile.OnChangeSelect);
            listCtrl.SelectItem(index);
            fileWindow.btnChaLoadLoad.onClick.Invoke();
            listCtrl.RemoveList(index);
            listCtrl.Create(customCharaFile.OnChangeSelect);

            var customBase = CustomBase.Instance;
            customBase.chaCtrl.chaFile.parameter.sex = (byte)customBase.modeSex;
        }

        public override void Coordinate_Load(string path, POINT pos)
        {
            var customCoordinateFile = GameObject.FindObjectOfType<CustomCoordinateFile>();
            var traverse = Traverse.Create(customCoordinateFile);
            var fileWindow = traverse.Field("fileWindow").GetValue<CustomFileWindow>();
            var listCtrl = traverse.Field("listCtrl").GetValue<CustomFileListCtrl>();

            var index = listCtrl.GetInclusiveCount() + 1;
            listCtrl.AddList(new CustomFileInfo(new FolderAssist.FileInfo(path, "", new DateTime())) { index = index, name = "" });
            listCtrl.Create(customCoordinateFile.OnChangeSelect);
            listCtrl.SelectItem(index);
            fileWindow.btnCoordeLoadLoad.onClick.Invoke();
            listCtrl.RemoveList(index);
            listCtrl.Create(customCoordinateFile.OnChangeSelect);
        }
    }
}
