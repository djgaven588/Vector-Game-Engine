namespace VectorEngine.Engine.Common
{
    public interface IStartEngine
    {
        void OnLoad();
        void OnClose();
        void OnUpdate(double timeDelta);
        void OnRender(double timeDelta);
    }
}
