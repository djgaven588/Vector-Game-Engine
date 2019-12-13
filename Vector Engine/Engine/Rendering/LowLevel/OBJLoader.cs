using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using VectorEngine.Core.Rendering.LowLevel;

namespace VectorEngine.Core.Rendering.LowLevel
{
    public static class OBJLoader
    {

        public static Mesh LoadObjModel(string fileName)
        {
            StreamReader stream;
            if (File.Exists("Game/Models/" + fileName + ".obj"))
            {
                stream = new StreamReader("Game/Models/" + fileName + ".obj");
            }
            else
            {
                Console.WriteLine("Model file with name " + fileName + " doesn't exist!");
                return new Mesh(0, 0);
            }

            Vector3d[] verts;
            Vector3d[] norms;
            Vector2d[] textCoords;

            string line = stream.ReadLine();
            string[] lineData;
            using (null)
            {
                Queue<Vector3d> vertices = new Queue<Vector3d>();
                Queue<Vector3d> normals = new Queue<Vector3d>();
                Queue<Vector2d> textureCoords = new Queue<Vector2d>();

                while (line[0] != 'f')
                {
                    lineData = line.Split(' ');
                    switch (lineData[0])
                    {
                        case "v":
                            vertices.Enqueue(new Vector3d(double.Parse(lineData[1]), double.Parse(lineData[2]), double.Parse(lineData[3])));
                            break;

                        case "vn":
                            normals.Enqueue(new Vector3d(double.Parse(lineData[1]), double.Parse(lineData[2]), double.Parse(lineData[3])));
                            break;

                        case "vt":
                            textureCoords.Enqueue(new Vector2d(double.Parse(lineData[1]), double.Parse(lineData[2])));
                            break;

                        default:
                            break;
                    }

                    line = stream.ReadLine();
                }

                verts = vertices.ToArray();
                norms = normals.ToArray();
                textCoords = textureCoords.ToArray();
            }

            Queue<int> indices = new Queue<int>();
            Vector3d[] normsFinal = new Vector3d[verts.Length];
            Vector2d[] textureCoordsFinal = new Vector2d[verts.Length];

            string[] tri1;
            string[] tri2;
            string[] tri3;
            while (line != null)
            {
                lineData = line.Split(' ');
                tri1 = lineData[1].Split('/');
                tri2 = lineData[2].Split('/');
                tri3 = lineData[3].Split('/');

                ProcessVertex(int.Parse(tri1[0]) - 1, int.Parse(tri1[2]) - 1, int.Parse(tri1[1]) - 1, ref indices, ref normsFinal, ref textureCoordsFinal, ref norms, ref textCoords);
                ProcessVertex(int.Parse(tri2[0]) - 1, int.Parse(tri2[2]) - 1, int.Parse(tri2[1]) - 1, ref indices, ref normsFinal, ref textureCoordsFinal, ref norms, ref textCoords);
                ProcessVertex(int.Parse(tri3[0]) - 1, int.Parse(tri3[2]) - 1, int.Parse(tri3[1]) - 1, ref indices, ref normsFinal, ref textureCoordsFinal, ref norms, ref textCoords);

                line = stream.ReadLine();
            }

            Mesh mesh = RenderDataLoader.LoadMeshData(verts, indices.ToArray(), textureCoordsFinal, normsFinal);
            stream.Dispose();
            return mesh;
        }

        private static void ProcessVertex(int posID, int normID, int texID, ref Queue<int> indices, ref Vector3d[] finalNorms, ref Vector2d[] finalTexCoords, ref Vector3d[] norms, ref Vector2d[] texCoords)
        {
            indices.Enqueue(posID);
            finalNorms[posID] = norms[normID];
            finalTexCoords[posID] = texCoords[texID];
        }
    }
}
