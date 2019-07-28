using BepInEx;
using BepInEx.Logging;
using IllusionDragAndDrop.Koikatu.CardHandler;
using IllusionDragAndDrop.Shared;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UniRx;
using Logger = BepInEx.Logger;

namespace IllusionDragAndDrop.Koikatu
{
    [BepInPlugin(GUID, "Illusion Drag & Drop", Version)]
    public class DragAndDrop : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.draganddrop";
        public const string Version = "1.1.0";

        static readonly byte[] StudioToken = Encoding.UTF8.GetBytes("【KStudio】");
        static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【KoiKatuChara");
        static readonly byte[] SexToken = Encoding.UTF8.GetBytes("sex");
        static readonly byte[] CoordinateToken = Encoding.UTF8.GetBytes("【KoiKatuClothes");
        static readonly byte[] PoseToken = Encoding.UTF8.GetBytes("【pose】");

        DragDropWndProc hook;

        void OnEnable()
        {
            hook = new DragDropWndProc();
            hook.InstallHook();
            hook.OnDragDrop += (aFiles, aPos) => MainThreadDispatcher.Post((x) => OnFiles(aFiles, aPos), null);
            hook.OnDragHover += (aPos) => MainThreadDispatcher.Post((x) => OnHover(aPos), null);
        }

        void OnDisable()
        {
            hook.UninstallHook();
        }

        void OnHover(WinAPI.POINT aPos)
        {

        }

        void OnFiles(List<string> aFiles, WinAPI.POINT aPos)
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
