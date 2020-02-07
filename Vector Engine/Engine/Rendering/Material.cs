using OpenTK;
using OpenTK.Graphics.OpenGL4;
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
        public bool UsesLights { get; private set; }
        public bool UsesViewMatrix { get; private set; }
        public bool UsesTime { get; private set; }
        public ShaderProgram Shader { get; private set; }
        private readonly Dictionary<string, int> UniformLocations;

        public Material(ShaderProgram shader, bool useLights = true, bool viewBased = true, bool useTime = true)
        {
            UsesLights = useLights;
            UsesViewMatrix = viewBased;
            UsesTime = useTime;
            Shader = shader;

            UniformLocations = new Dictionary<string, int>();
        }

        /// <summary>
        /// Sets a variable of type double with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetDouble(string uniformName, double value)
        {
            int uniformLocation = HandleUniformGet(uniformName);

            Shader.LoadDouble(uniformLocation, value);
        }

        /// <summary>
        /// Sets a variable of type matrix with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetMatrix(string uniformName, Matrix4 value)
        {
            int uniformLocation = HandleUniformGet(uniformName);

            Shader.LoadMatrix4(uniformLocation, value);
        }

        /// <summary>
        /// Sets a variable of type vector with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetVector3(string uniformName, Vector3 value)
        {
            int uniformLocation = HandleUniformGet(uniformName);

            Shader.LoadVector(uniformLocation, value);
        }

        /// <summary>
        /// !@! - INTERNAL USAGE - !@!
        /// Set the time value of the shader, this variable should be called
        /// "timeSinceStart", and if not found should be ignored.
        /// </summary>
        /// <param name="timeSinceStart">The time since the engine core start.</param>
        public void SetTimeData(double timeSinceStart)
        {
            int uniformLocation = HandleUniformGet("timeSinceStart");

            Shader.LoadDouble(uniformLocation, timeSinceStart);
        }

        /// <summary>
        /// !@! - INTERNAL USAGE - !@!
        /// Sets the light variables "lightPositions", "lightColors", "lightTypes", and "lightRanges",
        /// allowing for lighting to be dealt with within the shader.
        /// </summary>
        /// <param name="lights"></param>
        public void SetLights(Light[] lights)
        {
            Vector3[] positions = new Vector3[12];
            Vector3[] colors = new Vector3[12];
            float[] distances = new float[12];
            float[] intensities = new float[12];

            int uniformLocationPos = HandleUniformGet("lightPositions");
            int uniformLocationCol = HandleUniformGet("lightColors");
            int uniformLocationDis = HandleUniformGet("lightDistances");
            int uniformLocationInt = HandleUniformGet("lightIntensities");

            for (int i = 0; i < lights.Length; i++)
            {
                if (i >= 12)
                    break;

                positions[i] = (Vector3)lights[i].Position;
                colors[i] = (Vector3)lights[i].Color;
                distances[i] = lights[i].Distance;
                intensities[i] = lights[i].Intensity;
            }

            Shader.LoadVectorArray(uniformLocationPos, positions);
            Shader.LoadVectorArray(uniformLocationCol, colors);
            Shader.LoadFloatArray(uniformLocationDis, distances);
            Shader.LoadFloatArray(uniformLocationInt, intensities);
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
            int uniformLocation = HandleUniformGet("viewMatrix");

            Shader.LoadMatrix4(uniformLocation, viewMatrix);
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
