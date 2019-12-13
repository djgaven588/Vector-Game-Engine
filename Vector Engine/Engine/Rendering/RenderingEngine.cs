﻿using OpenTK;
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
                var objectsToRender = entitiesDB.QueryEntities<Transform, RendersMesh>(ExclusiveGroups.RenderedGroup);

                foreach (var entry in objectsToRender)
                {
                    Transform transform = entry.Item1;
                    RenderEngine.RenderMesh(Mathmatics.CreateTransformationMatrix(transform.position, transform.rotation, transform.scale), entry.Item2.mesh, GameEngine.treeTexture);
                }

                yield return null;
            }
        }
    }
}
