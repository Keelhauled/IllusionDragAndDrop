using B83.Win32;
using Harmony;
using UnityEngine;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public class FreeHSelectHandler : CardHandlerMethods
    {
        const string NormalMaleCanvas = "FreeHScene/Canvas/Panel/Normal/MaleCard";
        const string NormalFemaleCanvas = "FreeHScene/Canvas/Panel/Normal/FemaleCard";
        const string MasturbationCanvas = "FreeHScene/Canvas/Panel/Masturbation/FemaleCard";
        const string LesbianFemaleCanvas = "FreeHScene/Canvas/Panel/Lesbian/FemaleCard";
        const string LesbianPartnerCanvas = "FreeHScene/Canvas/Panel/Lesbian/PartnerCard";
        const string ThreesomeMaleCanvas = "FreeHScene/Canvas/Panel/3P/MaleCard";
        const string ThreesomeFemaleCanvas = "FreeHScene/Canvas/Panel/3P/FemaleCard";
        const string Stage1Canvas = "FreeHScene/Canvas/Panel/3P/Stage1";

        public override bool Condition => GameObject.FindObjectOfType<FreeHScene>() && !GameObject.FindObjectOfType<FreeHCharaSelect>();

        public override void Character_Load(string path, POINT pos, byte sex)
        {
            if(ActiveAndInBounds(NormalMaleCanvas, pos))
                SetupCharacter(path, ResultType.Player);

            else if(ActiveAndInBounds(NormalFemaleCanvas, pos))
                SetupCharacter(path, ResultType.Heroine);

            else if(ActiveAndInBounds(MasturbationCanvas, pos))
                SetupCharacter(path, ResultType.Heroine);

            else if(ActiveAndInBounds(LesbianFemaleCanvas, pos))
                SetupCharacter(path, ResultType.Heroine);

            else if(ActiveAndInBounds(LesbianPartnerCanvas, pos))
                SetupCharacter(path, ResultType.Partner);

            else if(ActiveAndInBounds(ThreesomeMaleCanvas, pos))
                SetupCharacter(path, ResultType.Player);

            else if(ActiveAndInBounds(ThreesomeFemaleCanvas, pos))
            {
                if(GameObject.Find(Stage1Canvas).activeInHierarchy)
                    SetupCharacter(path, ResultType.Heroine);
                else
                    SetupCharacter(path, ResultType.Partner);
            }
        }

        bool ActiveAndInBounds(string gameObjectPath, POINT pos)
        {
            var gameObject = GameObject.Find(gameObjectPath);

            if(gameObject && gameObject.activeInHierarchy)
            {
                var rectTransform = gameObject.GetComponent<RectTransform>();
                if(rectTransform)
                {
                    var left = rectTransform.position.x;
                    var top = Screen.height - rectTransform.position.y;
                    var right = left + rectTransform.rect.width;
                    var bottom = top + rectTransform.rect.height;
                    return left < pos.x && pos.x < right && top < pos.y && pos.y < bottom;
                }
            }

            return false;
        }

        void SetupCharacter(string path, ResultType type)
        {
            var chaFileControl = new ChaFileControl();
            if(chaFileControl.LoadCharaFile(path, 255, false, true))
            {
                var hscene = GameObject.FindObjectOfType<FreeHScene>();
                var member = Traverse.Create(hscene).Field("member");

                switch(type)
                {
                    case ResultType.Heroine:
                        member.Field("resultHeroine").Property("Value").SetValue(new SaveData.Heroine(chaFileControl, false));
                        break;

                    case ResultType.Player:
                        member.Field("resultPlayer").Property("Value").SetValue(new SaveData.Player(chaFileControl, false));
                        break;

                    case ResultType.Partner:
                        member.Field("resultPartner").Property("Value").SetValue(new SaveData.Heroine(chaFileControl, false));
                        break;
                }
            }
        }

        enum ResultType
        {
            Heroine,
            Player,
            Partner,
        }
    }
}
