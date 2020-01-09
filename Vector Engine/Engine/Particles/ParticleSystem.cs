using OpenTK;
using System;
using System.Collections.Generic;

namespace VectorEngine.Engine.Particles
{
    public class ParticleSystem
    {
        private readonly int MaxParticles;
        private Func<float, float> scaleOverLifeTimeFunction;
        private Func<float, Vector3, Vector3> velocityChangeOverLifeTimeFunction;
        private Func<(Vector3, Vector3, float)> particleCreatorFunction;
        private Func<(int, float), int> particleGeneratorFunction;

        private float constantScale = 1f;
        private Vector3 constantVelocityValue = new Vector3(0, 0, 0);
        private int lastEnabledParticleIndex = -1;
        private float systemLifetime = 0;

        // Always enabled data
        private Vector3[] particlePositions;
        private Vector3[] particleVelocities;
        private float[] remainingParticleLifetime;
        private bool[] enabledParticles;
        private int enabledParticleCount = 0;

        // Conditionally enabled data
        private bool scaleConstant = true;
        private bool isScaleOverLifeTime = false;

        private bool isVelocityConstant = true;
        private bool isVelocityOverLifeTime = false;

        private Queue<Matrix4> toRender;

        public ParticleSystem(int maxParticles, Func<(int, float), int> particleGenerator, Func<(Vector3, Vector3, float)> particleCreation)
        {
            if(maxParticles <= 0)
            {
                throw new Exception("Particle system with no or negative particle count are not allowed!");
            }

            if(particleGenerator == null || particleCreation == null)
            {
                throw new Exception("Particle generator and particle creation values must not be null!");
            }

            particleGeneratorFunction = particleGenerator;
            particleCreatorFunction = particleCreation;
            
            MaxParticles = maxParticles;

            particlePositions = new Vector3[maxParticles];
            particleVelocities = new Vector3[maxParticles];
            remainingParticleLifetime = new float[maxParticles];
            enabledParticles = new bool[maxParticles];
            toRender = new Queue<Matrix4>(maxParticles);
        }

        /// <summary>
        /// Sets the scale over lifetime function, the parameter is particle lifetime,
        /// and the return value should be the scale at that point in time.
        /// </summary>
        /// <param name="scalingFunction">Input is time, output is scale</param>
        public void SetScaleOverLifeTime(Func<float, float> scalingFunction)
        {
            scaleOverLifeTimeFunction = scalingFunction;
            isScaleOverLifeTime = true;
        }

        /// <summary>
        /// Sets the scale to a constant number
        /// </summary>
        /// <param name="scale">The scale of the particle</param>
        public void SetScale(float scale)
        {
            scaleConstant = true;
            isScaleOverLifeTime = false;
            constantScale = scale;
        }

        /// <summary>
        /// Sets the change over lifetime of the velocity, input is lifetime,
        /// output should be the velocity modification (X, Y, Z)
        /// </summary>
        /// <param name="velocityChangeFunction"></param>
        public void VelocityChangeOverLifeTime(Func<float, Vector3, Vector3> velocityChangeFunction)
        {
            isVelocityConstant = false;
            isVelocityOverLifeTime = true;
            velocityChangeOverLifeTimeFunction = velocityChangeFunction;
        }

        public void SetVelocityChange(Vector3 velocityChangeValue)
        {
            isVelocityConstant = true;
            isVelocityOverLifeTime = false;
            constantVelocityValue = velocityChangeValue;
        }

        public void RunUpdate(float timeDelta)
        {
            for (int i = 0; i < MaxParticles; i++)
            {
                if (enabledParticles[i] == false)
                    continue;

                remainingParticleLifetime[i] -= timeDelta;

                if (remainingParticleLifetime[i] <= 0)
                {
                    enabledParticles[i] = false;
                    enabledParticleCount--;
                }

                if (isVelocityOverLifeTime && !isVelocityConstant)
                {
                    particleVelocities[i] += velocityChangeOverLifeTimeFunction(remainingParticleLifetime[i], particlePositions[i]) * timeDelta;
                    particlePositions[i] += particleVelocities[i] * timeDelta;
                }
                else
                {
                    particlePositions[i] += constantVelocityValue;
                }
            }

            int particlesToSpawn = particleGeneratorFunction((enabledParticleCount, systemLifetime));
            (Vector3 position, Vector3 velocity, float lifetime) newParticleData;
            for (int i = 0; i < particlesToSpawn; i++)
            {
                newParticleData = particleCreatorFunction();
                lastEnabledParticleIndex++;

                if(lastEnabledParticleIndex >= MaxParticles)
                {
                    lastEnabledParticleIndex = 0;
                }

                if (enabledParticles[lastEnabledParticleIndex] == true)
                    enabledParticleCount--;

                particlePositions[lastEnabledParticleIndex] = newParticleData.position;
                particleVelocities[lastEnabledParticleIndex] = newParticleData.velocity;
                remainingParticleLifetime[lastEnabledParticleIndex] = newParticleData.lifetime;
                enabledParticles[lastEnabledParticleIndex] = true;
                enabledParticleCount++;
            }

            systemLifetime += timeDelta;
        }

        public void RenderParticles(Matrix4 renderAt, Core.Rendering.LowLevel.Mesh meshToRender, int texture)
        {
            float particleSize = constantScale;
            for (int i = 0; i < MaxParticles; i++)
            {
                if (enabledParticles[i] == false)
                    continue;

                if (isScaleOverLifeTime)
                {
                    particleSize = scaleOverLifeTimeFunction(remainingParticleLifetime[i]);
                }

                toRender.Enqueue(Core.Common.Mathmatics.CreateTransformationMatrix(particlePositions[i], Quaternion.Identity, Vector3.One * particleSize));
            }

            Core.Rendering.LowLevel.RenderEngine.RenderMeshInstanced(toRender.ToArray(), meshToRender, texture);
            toRender.Clear();
        }
    }
}
