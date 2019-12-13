using OpenTK;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorEngine.Engine.Components
{
    public struct Transform : IEntityStruct
    {
        public Vector3d position;
        public Vector3d scale;
        public Quaterniond rotation;
    }
}
