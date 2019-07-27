using IllusionDragAndDrop.Shared;
using IllusionDragAndDrop.Shared.WinAPI;

namespace IllusionDragAndDrop.Koikatu.CardHandler
{
    public abstract class CardHandlerMethods : CardHandlerCommon<CardHandlerMethods>
    {
        public virtual void Scene_Load(string path, POINT pos) { }
        public virtual void Scene_Import(string path, POINT pos) { }
        public virtual void Character_Load(string path, POINT pos, byte sex) { }
        public virtual void Coordinate_Load(string path, POINT pos) { }
        public virtual void PoseData_Load(string path, POINT pos) { }
    }
}
