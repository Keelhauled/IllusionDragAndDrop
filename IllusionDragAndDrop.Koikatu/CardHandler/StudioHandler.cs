using BepInEx;
using BepInEx.Logging;
using Manager;
using System.Linq;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public class StudioHandler : CardHandlerMain
    {
        public override bool Condition => Scene.Instance && Scene.Instance.NowSceneNames.Any(x => x == "Studio");

        public override void Scene_Load(string path)
        {
            Logger.Log(LogLevel.Message, "Loading scene");
            Studio.Studio.Instance.StartCoroutine(Studio.Studio.Instance.LoadSceneCoroutine(path));
        }

        public override void Scene_Import(string path)
        {
            Logger.Log(LogLevel.Message, "Importing scene");
            Studio.Studio.Instance.ImportScene(path);
        }

        public override void Character_LoadFemale(string path)
        {
            Logger.Log(LogLevel.Message, "Loading female");
            Studio.Studio.Instance.AddFemale(path);
        }

        public override void Character_LoadMale(string path)
        {
            Logger.Log(LogLevel.Message, "Loading male");
            Studio.Studio.Instance.AddMale(path);
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
