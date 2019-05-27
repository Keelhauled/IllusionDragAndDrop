using B83.Win32;
using ChaCustom;
using Harmony;
using Manager;
using System;
using System.Linq;
using UnityEngine;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public class MakerHandler : CardHandlerMethods
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "CustomScene");

        public override void Character_Load(string path, POINT pos, byte sex)
        {
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
            listCtrl.AddList(index, "", "", "", path, "", new DateTime());
            listCtrl.Create(customCoordinateFile.OnChangeSelect);
            listCtrl.SelectItem(index);
            fileWindow.btnCoordeLoadLoad.onClick.Invoke();
            listCtrl.RemoveList(index);
            listCtrl.Create(customCoordinateFile.OnChangeSelect);
        }
    }
}
