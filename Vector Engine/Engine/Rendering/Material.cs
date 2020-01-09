using OpenTK;
using VectorEngine.Core.Rendering.Objects;
using VectorEngine.Core.Rendering.Shaders;

namespace VectorEngine.Engine.Rendering
{
    /// <summary>
    /// A wrapper for a shader and its properties.
    /// </summary>
    public class Material
    {
        public virtual bool InstancedMaterial => false;
        public virtual bool UsesLights => true;
        public virtual bool UsesViewMatrix => true;
        public virtual bool UsesTime => true;
        public virtual ShaderProgram Shader => materialShader;

        private static ShaderProgram materialShader = new StaticShader();

        /// <summary>
        /// Sets a variable of type double with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetDouble(string uniformName, double value)
        {
            
        }

        /// <summary>
        /// Sets a variable of type matrix with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetMatrix(string uniformName, Matrix4 value)
        {

        }

        /// <summary>
        /// Sets a variable of type vector with the given name
        /// inside the shader.
        /// </summary>
        /// <param name="uniformName">Name of the variable</param>
        /// <param name="value">The value to set it to</param>
        public void SetVector3(string uniformName, Vector3 value)
        {

        }

        /// <summary>
        /// !@! - INTERNAL USAGE - !@!
        /// Set the time value of the shader, this variable should be called
        /// "timeSinceStart", and if not found should be ignored.
        /// </summary>
        /// <param name="timeSinceStart">The time since the engine core start.</param>
        public void SetTimeData(double timeSinceStart)
        {

        }

        /// <summary>
        /// !@! - INTERNAL USAGE - !@!
        /// Sets the light variables "lightPositions", "lightColors", "lightTypes", and "lightRanges",
        /// allowing for lighting to be dealt with within the shader.
        /// </summary>
        /// <param name="lights"></param>
        public void SetLights(Light[] lights)
        {

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

        }
    }
}
