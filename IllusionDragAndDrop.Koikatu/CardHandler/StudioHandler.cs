using B83.Win32;
using BepInEx.Logging;
using Harmony;
using Manager;
using Studio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = BepInEx.Logger;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public class StudioHandler : CardHandlerMain
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "Studio");

        public override void Scene_Load(string path, POINT pos)
        {
            Logger.Log(LogLevel.Message, "Loading scene");
            Studio.Studio.Instance.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(path));
        }

        public override void Scene_Import(string path, POINT pos)
        {
            Logger.Log(LogLevel.Message, "Importing scene");
            Studio.Studio.Instance.ImportScene(path);
        }

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            Logger.Log(LogLevel.Message, "Loading character");

            var characters = GetSelectedCharacters();
            if(characters.Count > 0)
            {
                foreach(var chara in characters)
                {
                    chara.charInfo.fileParam.sex = sex;
                    chara.ChangeChara(path);
                }

                UpdateStateInfo();
            }
            else if(sex == 1)
            {
                Studio.Studio.Instance.AddFemale(path);
            }
            else if(sex == 0)
            {
                Studio.Studio.Instance.AddMale(path);
            }
        }

        public override void Coordinate_Load(string path, POINT pos)
        {
            Logger.Log(LogLevel.Message, "Loading coordinate");
        }

        public override void PoseData_Load(string path, POINT pos)
        {
            Logger.Log(LogLevel.Message, "Loading pose");
        }

        List<OCIChar> GetSelectedCharacters()
        {
            return GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();
        }

        void UpdateStateInfo()
        {
            var mpCharCtrl = GameObject.FindObjectOfType<MPCharCtrl>();
            if(mpCharCtrl)
            {
                int select = Traverse.Create(mpCharCtrl).Field("select").GetValue<int>();
                if(select == 0) mpCharCtrl.OnClickRoot(0);
            }
        }
    }
}
