namespace VectorEngine.Engine.Common
{
    public interface IStartEngine
    {
        int TargetFrameRate { get; }
        int MaxFrameRate { get; }
        bool UseVSync { get; }
        void OnLoad();
        void OnClose();
        void OnUpdate(double timeDelta);
        void OnRender(double timeDelta);
    }
}
