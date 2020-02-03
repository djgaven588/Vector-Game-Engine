using OpenTK;
using System.Collections.Generic;
using VectorEngine.Core.Rendering.Objects;
using VectorEngine.Core.Rendering.Shaders;

namespace VectorEngine.Engine.Rendering
{
    /// <summary>
    /// A wrapper for a shader and its properties.
    /// </summary>
    public class Material
    {
        public bool InstancedMaterial { get; private set; }
        public bool UsesLights { get; private set; }
        public bool UsesViewMatrix { get; private set; }
        public bool UsesTime { get; private set; }
        public ShaderProgram Shader
        {
            get => InstancedMaterial ? instancedMaterialShader : staticMaterialShader;
            private set
            {
                if (InstancedMaterial)
                {
                    instancedMaterialShader = value;
                }
                else
                {
                    staticMaterialShader = value;
                }
            }
        }

        public Dictionary<string, int> UniformLocations => InstancedMaterial ? instancedUniformLocations : staticUniformLocations;

        private ShaderProgram instancedMaterialShader;
        private static ShaderProgram staticMaterialShader;
        private Dictionary<string, int> instancedUniformLocations;
        private static Dictionary<string, int> staticUniformLocations;

        public Material(ShaderProgram shader, bool isInstanced = false, bool useLights = true, bool viewBased = true, bool useTime = true)
        {
            InstancedMaterial = isInstanced;
            UsesLights = useLights;
            UsesViewMatrix = viewBased;
            UsesTime = useTime;
            Shader = shader;
        }

        /// <summary>
        /// Sets a variable of type double with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetDouble(string uniformName, double value)
        {
            bool shaderPreviouslyEnabled = Shader.wasEnabled;
            if (!shaderPreviouslyEnabled)
            {
                Shader.EnableShader();
            }

            int uniformLocation = HandleUniformGet(uniformName);

            ShaderProgram.LoadDouble(uniformLocation, value);

            if (!shaderPreviouslyEnabled)
            {
                Shader.DisableShader();
            }
        }

        /// <summary>
        /// Sets a variable of type matrix with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetMatrix(string uniformName, Matrix4 value)
        {
            bool shaderPreviouslyEnabled = Shader.wasEnabled;
            if (!shaderPreviouslyEnabled)
            {
                Shader.EnableShader();
            }

            int uniformLocation = HandleUniformGet(uniformName);

            ShaderProgram.LoadMatrix4(uniformLocation, value);

            if (!shaderPreviouslyEnabled)
            {
                Shader.DisableShader();
            }
        }

        /// <summary>
        /// Sets a variable of type vector with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetVector3(string uniformName, Vector3 value)
        {
            bool shaderPreviouslyEnabled = Shader.wasEnabled;
            if (!shaderPreviouslyEnabled)
            {
                Shader.EnableShader();
            }

            int uniformLocation = HandleUniformGet(uniformName);

            ShaderProgram.LoadVector(uniformLocation, value);

            if (!shaderPreviouslyEnabled)
            {
                Shader.DisableShader();
            }
        }

        /// <summary>
        /// !@! - INTERNAL USAGE - !@!
        /// Set the time value of the shader, this variable should be called
        /// "timeSinceStart", and if not found should be ignored.
        /// </summary>
        /// <param name="timeSinceStart">The time since the engine core start.</param>
        public void SetTimeData(double timeSinceStart)
        {
            bool shaderPreviouslyEnabled = Shader.wasEnabled;
            if (!shaderPreviouslyEnabled)
            {
                Shader.EnableShader();
            }

            int uniformLocation = HandleUniformGet("timeSinceStart");

            ShaderProgram.LoadDouble(uniformLocation, timeSinceStart);

            if (!shaderPreviouslyEnabled)
            {
                Shader.DisableShader();
            }
        }

        /// <summary>
        /// !@! - INTERNAL USAGE - !@!
        /// Sets the light variables "lightPositions", "lightColors", "lightTypes", and "lightRanges",
        /// allowing for lighting to be dealt with within the shader.
        /// </summary>
        /// <param name="lights"></param>
        public void SetLights(Light[] lights)
        {
            bool shaderPreviouslyEnabled = Shader.wasEnabled;
            if (!shaderPreviouslyEnabled)
            {
                Shader.EnableShader();
            }

            int uniformLocationPos = HandleUniformGet("lightPositions");
            int uniformLocationCol = HandleUniformGet("lightColors");

            Vector3d[] positions = new Vector3d[lights.Length];
            Vector3d[] colors = new Vector3d[lights.Length];

            for (int i = 0; i < lights.Length; i++)
            {
                positions[i] = lights[i].Position;
                colors[i] = lights[i].Color;
            }

            ShaderProgram.LoadVectorArray(uniformLocationPos, positions);
            ShaderProgram.LoadVectorArray(uniformLocationCol, colors);

            if (!shaderPreviouslyEnabled)
            {
                Shader.DisableShader();
            }
        }

        /// <summary>
        /// !@! - INTERNAL USAGE - !@!
        /// Sets the view matrix that should be used for this shader,
        /// UI shaders would be a 2d view, with first person cameras
        /// being a 3d view from a changing position.
        /// </summary>
        /// <param name="viewMatrix"></param>
        public void SetViewMatrix(Matrix4 viewMatrix)
        {
            bool shaderPreviouslyEnabled = Shader.wasEnabled;
            if (!shaderPreviouslyEnabled)
            {
                Shader.EnableShader();
            }

            int uniformLocation = HandleUniformGet("viewMatrix");

            ShaderProgram.LoadMatrix4(uniformLocation, viewMatrix);

            if (!shaderPreviouslyEnabled)
            {
                Shader.DisableShader();
            }
        }

        private int HandleUniformGet(string uniformName)
        {
            int uniformLocation;
            if (!UniformLocations.ContainsKey(uniformName))
            {
                uniformLocation = Shader.GetUniformLocation(uniformName);
                if (uniformLocation >= 0)
                {
                    UniformLocations[uniformName] = uniformLocation;
                }
                else
                {
                    throw new System.Exception("Uniform location invalid!");
                }
            }
            else
            {
                uniformLocation = UniformLocations[uniformName];
            }

            return uniformLocation;
        }
    }
}
