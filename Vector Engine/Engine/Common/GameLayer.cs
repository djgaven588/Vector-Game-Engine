namespace VectorEngine.Engine.Common
{
    public class GameLayer
    {
        public virtual string DEBUG_NAME { get; private set; } = "Default Layer";

        public GameLayer(string name = "Default Layer")
        {
            DEBUG_NAME = name;
        }

        /// <summary>
        /// Called when the layer receives a logic update
        /// </summary>
        public virtual void OnUpdate(double timeDelta)
        {

        }

        /// <summary>
        /// Called when the layer receives a render update
        /// </summary>
        public virtual void OnRender(double timeDelta)
        {

        }

        /// <summary>
        /// Called when the layer receives an event, the layer can "eat" the event
        /// to prevent layers beneath it from receiving it.
        /// </summary>
        /// <param name="eatEvent"></param>
        public virtual void OnEvent(ref bool eatEvent)
        {
            eatEvent = false;
        }
    }
}
