using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(programID, uniformName);
        }

        public void EnableShader()
        {
            GL.UseProgram(programID);
        }

        public static void DisableShader()
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

        protected static void LoadDouble(int location, double value)
        {
            GL.Uniform1(location, value);
        }

        protected static void LoadVector(int location, Vector3 vector)
        {
            GL.Uniform3(location, vector.X, vector.Y, vector.Z);
        }

        protected static void LoadBoolean(int location, bool value)
        {
            int toLoad = 0;
            if (value)
                toLoad = 1;

            GL.Uniform1(location, toLoad);
        }

        protected static void LoadMatrix4(int location, Matrix4 matrix)
        {
            GL.UniformMatrix4(location, false, ref matrix);
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
