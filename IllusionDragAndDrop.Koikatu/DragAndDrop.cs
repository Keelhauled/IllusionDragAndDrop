using B83.Win32;
using BepInEx;
using BepInEx.Logging;
using IllusionDragAndDrop.Koikatu.CardHandler;
using IllusionDragAndDrop.Shared;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Logger = BepInEx.Logger;
using UniRx;

namespace IllusionDragAndDrop.Koikatu
{
    [BepInPlugin(GUID, "Illusion Drag & Drop", Version)]
    public class DragAndDrop : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.draganddrop";
        public const string Version = "1.0.1";

        static readonly byte[] StudioToken = Encoding.UTF8.GetBytes("【KStudio】");
        static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【KoiKatuChara】");
        static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");
        static readonly byte[] CoordinateToken = Encoding.UTF8.GetBytes("【KoiKatuClothes】");
        static readonly byte[] PoseToken = Encoding.UTF8.GetBytes("【pose】");

        UnityDragAndDropHook hook;

        void OnEnable()
        {
            hook = new UnityDragAndDropHook();
            hook.InstallHook();
            hook.OnDroppedFiles += (aFiles, aPos) => MainThreadDispatcher.Post((x) => OnFiles(aFiles, aPos), null);
        }

        void OnDisable()
        {
            hook.UninstallHook();
        }

        void OnFiles(List<string> aFiles, POINT aPos)
        {
            var goodFiles = aFiles.Where(x =>
            {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png" || ext == ".dat";
            });

            if(goodFiles.Count() == 0)
            {
                Logger.Log(LogLevel.Message, "No files to handle");
                return;
            }

            var cardHandler = CardHandlerMethods.GetActiveCardHandler();
            if(cardHandler != null)
            {
                foreach(var file in goodFiles)
                {
                    var bytes = File.ReadAllBytes(file);

                    if(BoyerMoore.ContainsSequence(bytes, StudioToken))
                    {
                        cardHandler.Scene_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CharaToken))
                    {
                        var index = new BoyerMoore(SexToken).Search(bytes).First();
                        var sex = bytes[index + SexToken.Length];
                        cardHandler.Character_Load(file, aPos, sex);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, CoordinateToken))
                    {
                        cardHandler.Coordinate_Load(file, aPos);
                    }
                    else if(BoyerMoore.ContainsSequence(bytes, PoseToken))
                    {
                        cardHandler.PoseData_Load(file, aPos);
                    }
                    else
                    {
                        Logger.Log(LogLevel.Message, "This file does not contain any koikatu related data");
                    }
                }
            }
        }
    }
}
