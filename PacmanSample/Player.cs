using Intro3DFramework.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Player
    {
        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private Vector2 viewDir = Vector2.UnitX;

        private int score = 0;

        #region Shader

        [StructLayout(LayoutKind.Sequential)]
        struct PlayerUniformData
        {
            public Matrix4 world; 
        }

        private PlayerUniformData uniformData;
        private UniformBuffer<PlayerUniformData> uniformGPUBuffer;

        private Shader shader;

        #endregion

        private Model model;

        private const float accelerationFactor = 0.2f;
        private const float playerSize = 10.0f;

        public Player(Vector2 startPosition)
        {
            position = startPosition;

            shader = Shader.GetResource(new Shader.LoadDescription("Content/player.vert", "Content/default.frag"));
            
            // Need to assign block binding indices. Shader memorizes these indices.
            GL.UniformBlockBinding(shader.Program, GL.GetUniformBlockIndex(shader.Program, "PerFrame"), 0);
            GL.UniformBlockBinding(shader.Program, GL.GetUniformBlockIndex(shader.Program, "Player"), 1);

            uniformGPUBuffer = new UniformBuffer<PlayerUniformData>();

            model = Model.GetResource("Content/Models/bamboo.obj");
            model.Meshes[0].texture = Texture2D.GetResource("Content/Models/Texture/Bamboo.png");
        }

        public void Update(float timeSinceLastFrame, Map map, Terrain terrain, Matrix4 worldOrientation)
        {
            // Project normal back to 2D field, but keep original speed.
            Vector3 terrainNormal = terrain.GetNormal(position);
            Vector2 terrainDirection = -Vector3.Transform(terrainNormal, worldOrientation).Xz;
            float acceleration = terrainDirection.Length;
            terrainDirection.Normalize();

            // Move in terrain gradient direction.
            Vector2 nextVelocity = velocity + terrainDirection * (timeSinceLastFrame * accelerationFactor * acceleration) * 10;
            Vector2 nextPosition = position + nextVelocity;

            // Check if we would now touch a non walkable field
            int gatheredCoins;
            if (map.TryWalk(nextPosition - playerSize / 2 * Vector2.One, nextPosition + playerSize / 2 * Vector2.One, out gatheredCoins))
            {
                position = nextPosition;
                velocity = nextVelocity;
                score += gatheredCoins;
            }
            else
                velocity = Vector2.Zero;

            // Simplistic rotation adaption to velocity - the higher velocity is the faster it will rotate
            viewDir += velocity * 0.001f;
            viewDir.Normalize();

            uniformData.world = Matrix4.CreateRotationY((float)Math.Acos(Vector2.Dot(viewDir, Vector2.UnitX))) *
                                Matrix4.CreateTranslation(position.X, terrain.GetHeight(new Vector2(position.X, position.Y)), position.Y);
            uniformGPUBuffer.UpdateGPUData(ref uniformData);
        }

        public void Render()
        {
            GL.UseProgram(shader.Program);
            uniformGPUBuffer.BindBuffer(1);
            
            model.Draw();
        }
    }
}
