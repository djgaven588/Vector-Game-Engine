using OpenTK;
using VectorEngine.Core.Rendering.LowLevel;

namespace VectorEngine.Core.Rendering.LowLevel
{
    public class Mesh
    {
        private int vaoID;
        private int vertexCount;

        public bool IsDirty { get; private set; }


        public Mesh(int vaoID, int vertexCount)
        {
            this.vaoID = vaoID;
            this.vertexCount = vertexCount;
        }

        public int GetVAOID()
        {
            return vaoID;
        }

        public int GetVertexCount() => vertexCount;

        public void UpdateMesh(Vector3d[] positions, Vector3d[] normals, Vector2d[] textureCoords, int[] indices)
        {
            Mesh newMesh = RenderDataLoader.LoadMeshData(positions, indices, textureCoords, normals, vaoID);
            vaoID = newMesh.GetVAOID();
            vertexCount = newMesh.vertexCount;
        }
    }
}
