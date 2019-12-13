using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine.Common
{
    public static class ReservedGroups
    {
        public static readonly ExclusiveGroup.ExclusiveGroupStruct DefaultGroup = ExclusiveGroup.ExclusiveGroupStruct.Generate();
    }
}
