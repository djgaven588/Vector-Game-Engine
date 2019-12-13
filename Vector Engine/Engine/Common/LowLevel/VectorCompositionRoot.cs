using Svelto.ECS;
using VectorEngine.Core;
using VectorEngine.Engine.Components;
using VectorEngine.Engine.Rendering;

namespace VectorEngine.Engine.Common.LowLevel
{
    public class VectorCompositionRoot
    {
        private readonly EnginesRoot _enginesRoot;

        public VectorCompositionRoot()
        {
            _enginesRoot = new EnginesRoot(VectorSchedulers.EntityScheduler);

            IEntityFactory entityFactory = _enginesRoot.GenerateEntityFactory();
            IEntityFunctions entityFunctions = _enginesRoot.GenerateEntityFunctions();

            RenderingEngine renderEngine = new RenderingEngine();

            _enginesRoot.AddEngine(renderEngine);

            var entity = entityFactory.BuildEntity<TestEntity>(new EGID(0, ExclusiveGroups.DefaultGroup));
            entity.Init(new Transform() { position = new OpenTK.Vector3d(-1, 1, -2), rotation = OpenTK.Quaterniond.Identity, scale = OpenTK.Vector3d.One });
            entity.Init(new RendersMesh() { mesh = GameEngine.treeMesh });
        }
    }

    class TestEntity : GenericEntityDescriptor<Transform, RendersMesh> { }
}
