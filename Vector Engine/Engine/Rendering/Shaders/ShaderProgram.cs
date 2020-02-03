using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using VectorEngine.Engine;

namespace VectorEngine.Core.Rendering.Shaders
{
    public abstract class ShaderProgram
    {
        private readonly int programID;
        private readonly int vertexShaderID;
        private readonly int fragmentShaderID;
        public bool wasEnabled { get; private set; }

        protected ShaderProgram(string vertexFile, string fragmentFile)
        {
            vertexShaderID = LoadShader(vertexFile, ShaderType.VertexShader);
            fragmentShaderID = LoadShader(fragmentFile, ShaderType.FragmentShader);
            programID = GL.CreateProgram();

            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);

            BindAttributes();

            GL.LinkProgram(programID);
            GL.ValidateProgram(programID);

            GetAllUniformLocations();

            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
        }

        protected abstract void GetAllUniformLocations();

        public abstract void BeforeRenderShader();

        public abstract void BeforeRenderObject();

        public abstract void AfterRenderObject();

        public abstract void AfterRenderShader();

        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(programID, uniformName);
        }

        public void EnableShader()
        {
            wasEnabled = true;
            GL.UseProgram(programID);
        }

        public void DisableShader()
        {
            wasEnabled = false;
            GL.UseProgram(0);
        }

        public void CleanUp()
        {
            DisableShader();
            GL.DeleteProgram(programID);
        }

        protected abstract void BindAttributes();

        protected void BindAttribute(int attribute, string variableName)
        {
            GL.BindAttribLocation(programID, attribute, variableName);
        }

        public static void LoadDouble(int location, double value)
        {
            GL.Uniform1(location, value);
        }

        public static void LoadVector(int location, Vector3 vector)
        {
            GL.Uniform3(location, vector.X, vector.Y, vector.Z);
        }

        public static void LoadBoolean(int location, bool value)
        {
            int toLoad = 0;
            if (value)
            {
                toLoad = 1;
            }

            GL.Uniform1(location, toLoad);
        }

        public static void LoadMatrix4(int location, Matrix4 matrix)
        {
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public static void LoadVectorArray(int location, Vector3d[] vectors)
        {
            double[] data = new double[vectors.Length * 3];
            for (int i = 0; i < vectors.Length; i++)
            {
                data[i] = vectors[i].X;
                data[i + 1] = vectors[i].Y;
                data[i + 2] = vectors[i].Z;
            }
            GL.Uniform3(location, vectors.Length, data);
        }

        private static int LoadShader(string file, ShaderType type)
        {
            int shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, File.ReadAllText(file));
            GL.CompileShader(shaderID);
            Debug.Log($"Shader Info Log: {GL.GetShaderInfoLog(shaderID)} END");
            return shaderID;
        }
    }
}
