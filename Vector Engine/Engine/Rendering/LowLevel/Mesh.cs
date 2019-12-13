using OpenTK;

namespace VectorEngine.Core.Rendering.LowLevel
{
    public struct Mesh
    {
        public int VaoID        { get; private set; }
        public int VertexCount  { get; private set; }

        public Mesh(int vaoID, int vertexCount)
        {
            VaoID = vaoID;
            VertexCount = vertexCount;
        }

        public void UpdateMesh(Vector3d[] positions, Vector3d[] normals, Vector2d[] textureCoords, int[] indices)
        {
            Mesh newMesh = RenderDataLoader.LoadMeshData(positions, indices, textureCoords, normals, VaoID);
            VaoID = newMesh.VaoID;
            VertexCount = newMesh.VertexCount;
        }
    }
}
