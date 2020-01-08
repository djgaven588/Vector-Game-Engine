using Svelto.ECS.Internal;
using System;

namespace Svelto.ECS
{
    public interface IEntityBuilder
    {
        void BuildEntityAndAddToList(ref ITypeSafeDictionary dictionary, EGID entityID, object[] implementors);
        ITypeSafeDictionary Preallocate(ref ITypeSafeDictionary dictionary, uint size);

        Type GetEntityType();
    }
}