﻿using Intro3DFramework.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace Sample
{
    class Map
    {
        #region Shader

        [StructLayout(LayoutKind.Sequential)]
        struct PerBlockUniformData
        {
            public Vector3 position;
            public float scale;
        }

        private PerBlockUniformData perBlockUniformData;
        private UniformBuffer<PerBlockUniformData> perBlockUniformGPUBuffer;

        private Shader blockShader;

        #endregion


        #region Blocks

        enum BlockType
        {
            NONE = 0,
            WALL = 1,
            COIN = 2
        };

        private Model[] blockModels;

        private BlockType[,] map;
        private Model ground;
        private const float blockSize = 10;
        private const float groundHeight = 4;

        #endregion

        /// <summary>
        /// Size in render units, not block units.
        /// </summary>
        public Vector2 Size
        {
            get { return new Vector2(map.GetLength(0), map.GetLength(1)) * blockSize; }
        }
        
        /// <summary>
        /// Checks if rect can walk onto the given area and gathers coins.
        /// </summary>
        /// <returns>true if it is valid to walk to the specified rect.</returns>
        public bool TryWalk(Vector2 min, Vector2 max, out int gatheredCoins)
        {
            int minX = Math.Max((int)(min.X / blockSize + map.GetLength(0) / 2), 0);
            int minY = Math.Max((int)(min.Y / blockSize + map.GetLength(1) / 2), 0);
            int maxX = Math.Min((int)Math.Ceiling(max.X / blockSize + map.GetLength(0) / 2), map.GetLength(0));
            int maxY = Math.Min((int)Math.Ceiling(max.Y / blockSize + map.GetLength(1) / 2), map.GetLength(1));

            gatheredCoins = 0;

            for (int x = minX; x < maxX; ++x)
            {
                for (int y = minY; y < maxY; ++y)
                {
                    if (map[x, y] == BlockType.WALL)
                        return false;
                }
            }

            for (int x = minX; x < maxX; ++x)
            {
                for (int y = minY; y < maxY; ++y)
                {
                    if (map[x, y] == BlockType.COIN)
                    {
                        map[x, y] = BlockType.NONE;
                        ++gatheredCoins;
                    }
                }
            }

            return true;
        }

        public Map()
        {
            int[,] mapData = 
            {
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 1 },
                { 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
            };

            map = new BlockType[mapData.GetLength(0), mapData.GetLength(1)];
            for(int x = 0; x < mapData.GetLength(0); ++x)
            {
                for(int y = 0; y < mapData.GetLength(1); ++y)
                {
                    map[x, y] = (BlockType)mapData[x, y];
                }
            }


            blockShader = Shader.GetResource(new Shader.LoadDescription("Content/block.vert", "Content/default.frag"));

            // Need to assign block binding indices. Shader memorizes these indices.
            GL.UniformBlockBinding(blockShader.Program, GL.GetUniformBlockIndex(blockShader.Program, "PerFrame"), 0);
            GL.UniformBlockBinding(blockShader.Program, GL.GetUniformBlockIndex(blockShader.Program, "PerBlock"), 1);

            ground = Model.GetResource("Content/Models/ground.obj");
            ground.Meshes[0].texture = Texture2D.GetResource("Content/Models/Texture/ground0.png");

            blockModels = new Model[Enum.GetValues(typeof(BlockType)).Length];
            blockModels[0] = null;
            blockModels[1] = Model.GetResource("Content/Models/stone.obj");
            blockModels[1].Meshes[0].texture = Texture2D.GetResource("Content/Models/Texture/Bamboo.png");
            blockModels[2] = Model.GetResource("Content/Models/gras0.obj");
            blockModels[2].Meshes[0].texture = Texture2D.GetResource("Content/Models/Texture/Bamboo.png");

            perBlockUniformGPUBuffer = new UniformBuffer<PerBlockUniformData>();
        }
        public void Render(float totalTime, Terrain terrain)
        {
            GL.UseProgram(blockShader.Program);

            perBlockUniformData.scale = map.GetLength(0);
            perBlockUniformData.position = (-1) * groundHeight * (perBlockUniformData.scale - 1) * Vector3.UnitY; 
            //perBlockUniformGPUBuffer.UpdateGPUData(ref perBlockUniformData);
            perBlockUniformGPUBuffer.BindBuffer(1);
            //ground.Draw();

            // Draw the blocks.
            perBlockUniformData.scale = 1;
            for (int x = 0; x < map.GetLength(0); ++x)
            {
                for (int y = 0; y < map.GetLength(1); ++y)
                {
                    if (map[x, y] == BlockType.NONE)
                        continue;

                    Vector2 pos2d = new Vector2((x - map.GetLength(0) / 2 + 0.5f) * blockSize, (y - map.GetLength(1) / 2 + 0.5f) * blockSize);
                    perBlockUniformData.position = new Vector3(pos2d.X, terrain.GetHeight(new Vector2(pos2d.X, pos2d.Y)), pos2d.Y);

                    perBlockUniformGPUBuffer.UpdateGPUData(ref perBlockUniformData);
                    blockModels[(int)map[x, y]].Draw();
                }
            }
        }
    }
}
