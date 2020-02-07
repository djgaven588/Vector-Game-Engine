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

        public abstract void BeforeRenderGroup();

        public abstract void BeforeRenderIndividual();

        public abstract void AfterRenderObject();

        public abstract void AfterRenderShader();

        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(programID, uniformName);
        }

        public void EnableShader()
        {
            GL.UseProgram(programID);
        }

        public void DisableShader()
        {
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

        public void LoadDouble(int location, double value)
        {
            GL.ProgramUniform1(programID, location, value);
        }

        public void LoadVector(int location, Vector3 vector)
        {
            GL.ProgramUniform3(programID, location, vector);
        }

        public void LoadBoolean(int location, bool value)
        {
            int toLoad = 0;
            if (value)
            {
                toLoad = 1;
            }

            GL.ProgramUniform1(programID, location, toLoad);
        }

        public void LoadMatrix4(int location, Matrix4 matrix)
        {
            GL.ProgramUniformMatrix4(programID, location, false, ref matrix);
        }

        public void LoadVectorArray(int location, Vector3[] vectors)
        {
            float[] values = new float[vectors.Length * 3];
            for (int i = 0; i < vectors.Length; i+=3)
            {
                values[i] = vectors[i].X;
                values[i + 1] = vectors[i].Y;
                values[i + 2] = vectors[i].Z;
            }
            GL.ProgramUniform3(programID, location, vectors.Length, values);
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
