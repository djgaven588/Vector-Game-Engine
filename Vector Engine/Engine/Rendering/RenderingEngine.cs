using Svelto.ECS;
using System;
using System.Collections;
using VectorEngine.Core;
using VectorEngine.Core.Common;
using VectorEngine.Core.Rendering.LowLevel;
using VectorEngine.Engine.Common;
using VectorEngine.Engine.Components;

namespace VectorEngine.Engine.Rendering
{
    class RenderingEngine : IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { set; get; }

        public void Ready()
        {
            RenderUpdate().RunOnScheduler(VectorSchedulers.RenderScheduler);
        }

        IEnumerator RenderUpdate()
        {
            while (true)
            {
                var objectsToRender = entitiesDB.QueryEntities<Transform, RendersMesh>(ReservedGroups.DefaultGroup, out uint renderCount);

                for (int i = 0; i < renderCount; i++)
                {
                    Transform transform = objectsToRender.Item1[i];
                    RenderEngine.RenderMesh(Mathmatics.CreateTransformationMatrix(transform.position, transform.rotation, transform.scale), objectsToRender.Item2[i].mesh, GameEngine.treeTexture);
                }

                yield return null;
            }
        }
    }
}
