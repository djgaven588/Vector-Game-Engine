using Svelto.ECS;
using System;
using System.Collections;

namespace VectorEngine.Engine.Rendering
{
    class RenderingEngine : IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { set => throw new NotImplementedException(); }

        public void Ready()
        {
            RenderUpdate().RunOnScheduler();
        }

        IEnumerator RenderUpdate()
        {
            while (true)
            {
                yield return null;
            }
        }
    }
}
