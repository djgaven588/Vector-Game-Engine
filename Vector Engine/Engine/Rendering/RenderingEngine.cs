using Svelto.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
