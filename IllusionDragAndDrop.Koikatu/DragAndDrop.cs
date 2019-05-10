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

namespace IllusionDragAndDrop.Koikatu
{
    [BepInPlugin("keelhauled.draganddrop", "Drag and Drop", "1.0.0")]
    class DragAndDrop : BaseUnityPlugin
    {
        static readonly byte[] CharaToken = Encoding.UTF8.GetBytes("【KoiKatuChara】");
        static readonly byte[] StudioToken = Encoding.UTF8.GetBytes("【KStudio】");
        static readonly byte[] SexToken = Encoding.UTF8.GetBytes("【sex】");
        static readonly byte[] CoordinateToken = Encoding.UTF8.GetBytes("【KoiKatuClothes】");
        static readonly byte[] PoseToken = Encoding.UTF8.GetBytes("【pose】");

        UnityDragAndDropHook hook;

        void OnEnable()
        {
            hook = new UnityDragAndDropHook();
            hook.InstallHook();
            hook.OnDroppedFiles += OnFiles;
        }

        void OnDisable()
        {
            hook.UninstallHook();
        }

        void OnFiles(List<string> aFiles, POINT aPos)
        {
            var goodFiles = aFiles.Where(x => {
                var ext = Path.GetExtension(x).ToLower();
                return ext == ".png" || ext == ".dat";
            }).ToList();

            if(goodFiles.Count == 0)
            {
                Logger.Log(LogLevel.Message, "No files to handle");
                return;
            }

            var cardHandler = CardHandlerMain.GetActiveCardHandler();
            if(cardHandler != null)
            {
                foreach(var file in goodFiles)
                {
                    var cardType = GetCardType(file);

                    switch(cardType)
                    {
                        case CardType.CharaFemale:
                            cardHandler.Character_LoadFemale(file);
                            break;

                        case CardType.CharaMale:
                            cardHandler.Character_LoadMale(file);
                            break;

                        case CardType.StudioScene:
                            cardHandler.Scene_Load(file);
                            break;

                        case CardType.Coordinate:
                            cardHandler.Coordinate_Load(file);
                            break;

                        case CardType.Unknown:
                            Logger.Log(LogLevel.Message, "This file does not contain any koikatu related data");
                            break;
                    }
                }
            }
        }

        CardType GetCardType(string path)
        {
            var bytes = File.ReadAllBytes(path);

            if(BoyerMoore.ContainsSequence(bytes, StudioToken))
            {
                return CardType.StudioScene;
            }
            else if(BoyerMoore.ContainsSequence(bytes, CharaToken))
            {
                var index = new BoyerMoore(SexToken).Search(bytes).First();
                var sex = bytes[index + SexToken.Length];
                return sex == 1 ? CardType.CharaFemale : CardType.CharaMale;
            }
            else if(BoyerMoore.ContainsSequence(bytes, CoordinateToken))
            {
                return CardType.Coordinate;
            }
            else if(BoyerMoore.ContainsSequence(bytes, PoseToken))
            {
                return CardType.Pose;
            }

            return CardType.Unknown;
        }

        enum CardType
        {
            Unknown,
            CharaFemale,
            CharaMale,
            StudioScene,
            Coordinate,
            Pose
        }
    }
}
