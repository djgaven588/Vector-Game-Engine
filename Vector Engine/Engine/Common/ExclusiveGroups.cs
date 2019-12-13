using Svelto.ECS;
using System.Collections.Generic;

namespace VectorEngine.Engine.Common
{
    public static class ExclusiveGroups
    {
        public static readonly ExclusiveGroup DefaultGroup = new ExclusiveGroup("Default Group");

#pragma warning disable CA1819 // Properties should not return arrays
        public static ExclusiveGroup[] RenderedGroup { get; private set; }
#pragma warning restore CA1819 // Properties should not return arrays

        private static readonly List<ExclusiveGroup> renderedGroup;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static ExclusiveGroups()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            renderedGroup = new List<ExclusiveGroup>();
            AddToRenderedGroup(DefaultGroup);
        }

        public static void AddToRenderedGroup(ExclusiveGroup group)
        {
            renderedGroup.Add(group);
            RenderedGroup = renderedGroup.ToArray();
        }
    }
}
