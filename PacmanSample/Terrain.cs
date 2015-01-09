using Intro3DFramework.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Sample
{
    /// <summary>
    /// Class for drawing a screen aligned quad.
    /// </summary>
    class Terrain
    {
        /// <summary>
        /// Vertex for a terrain.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = sizeof(float) * 5)]
        private struct VertexTerrain
        {
            [FieldOffset(0)]
            public OpenTK.Vector2 position;
            [FieldOffset(sizeof(float) * 2)]
            public OpenTK.Vector2 texcoord;
            [FieldOffset(sizeof(float) * 4)]
            public float height;
        }

        private Vector2 FieldSize;
        private VectorInt NumFields;

        /// <summary>
        /// Index of Vertex Buffer.
        /// </summary>
        private int vertexBuffer;

        VertexTerrain[] vertices;
        private float GetHeight(VectorInt pos)
        {
            return vertices[pos.X + pos.Y*(NumFields.X+1)].height;
        }

        /// <summary>
        /// Index of Index Buffer.
        /// </summary>
        private int indexBuffer;
        private int numIndices;

        /// <summary>
        /// The texture that will be drawn to the quad.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// A class for creating a simple quad-based Terrain.
        /// </summary>
        /// <param name="sizeX">The number of quads in x direction.</param>
        /// <param name="sizeY">The number of quads in y direction.</param>
        public Terrain(int numFieldsX, int numFieldsY, float fieldSizeX, float fieldSizeY)
        {
            NumFields = new VectorInt(numFieldsX, numFieldsY);
            FieldSize = new Vector2(fieldSizeX, fieldSizeY);


            numIndices = numFieldsX * numFieldsY * 2 * 3;
            System.Diagnostics.Debug.Assert((ulong)(numFieldsX * numFieldsY * 2 * 3) <= uint.MaxValue);
            // Saving vertices in array
            vertices = new VertexTerrain[(numFieldsX + 1) * (numFieldsY + 1)];

            for (int x = 0; x <= numFieldsX; ++x)
                for (int y = 0; y <= numFieldsY; ++y)
                {
                    float xPos = (float)x / numFieldsX * fieldSizeX - fieldSizeX / 2;
                    float yPos = (float)y / numFieldsY * fieldSizeY - fieldSizeY / 2;
                    // Assign position and texcoord basedon index. 
                    vertices[y*(numFieldsX+1) + x] = new VertexTerrain{
                        position = new OpenTK.Vector2(xPos, yPos),
                        texcoord = new OpenTK.Vector2((float)x/numFieldsX, (float)y/numFieldsY),
                        // Boring function to add some height.
                        height = (float)(Math.Sin(xPos/10) - Math.Cos(yPos/10))*30
                    };
                }

            // Link the vertices to triangles via index buffer.
            // Creating sizeX * sizeY quads with 2 triangles at 3 vertices each.

            uint[] indices = new uint[numIndices];

            for (int x = 0; x < numFieldsX; ++x)
                for (int y = 0; y < numFieldsY; ++y)
                {
                    uint arrayIndex = (uint)(x + (y * numFieldsX)) * 6;
                    indices[arrayIndex + 0] = (uint)(x + 1 + (y * (numFieldsX + 1)));
                    indices[arrayIndex + 1] = (uint)(x + (y * (numFieldsX + 1)));
                    indices[arrayIndex + 2] = (uint)(x + ((y + 1) * (numFieldsX + 1)));
                    indices[arrayIndex + 3] = indices[arrayIndex + 0];
                    indices[arrayIndex + 4] = indices[arrayIndex + 2];
                    indices[arrayIndex + 5] = (uint)(x + 1 + ((y + 1) * (numFieldsX + 1)));
                }

            // Create and fill OpenGL vertex buffer.
            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Marshal.SizeOf(typeof(VertexTerrain)) * (numFieldsX + 1) * (numFieldsY + 1)), vertices, BufferUsageHint.StaticDraw);

            // Create and fill OpenGL index buffer.
            indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            IntPtr tmp = (IntPtr)(Marshal.SizeOf(typeof(VertexTerrain)) * numIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * numIndices), indices, BufferUsageHint.StaticDraw);
        }

        public void Draw()
        {
            // Assert the object exists and is valid.
            System.Diagnostics.Debug.Assert(vertexBuffer > 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);

            // Set vertex type
            int vertexSize = Marshal.SizeOf(typeof(VertexTerrain));
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, vertexSize, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexSize, sizeof(float) * 2);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, vertexSize, sizeof(float) * 4);
            GL.EnableVertexAttribArray(2);

            // Setting the texture.
            if (Texture != null)
                GL.BindTexture(TextureTarget.Texture2D, Texture.Texture);
            else
                GL.BindTexture(TextureTarget.Texture2D, 0);

            // Finally drawing the vertex buffer.
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);
        }

        public Vector3 GetGradient(Vector2 pos)
        {
            Vector2 localCoord = pos + FieldSize / 2;
            localCoord = new Vector2(localCoord.X / FieldSize.X, localCoord.Y / FieldSize.Y);
            // Lower left, lower right, upper left, upper right positions.
            VectorInt llPos = new VectorInt((int)localCoord.X, (int)localCoord.Y);
            VectorInt lrPos = llPos + VectorInt.UnitX;
            VectorInt ulPos = llPos + VectorInt.UnitY;
            VectorInt urPos = lrPos + VectorInt.UnitY;

            Vector2 posUV = localCoord - (Vector2)ulPos;

            //Vector3 lPoint = new Vector3((Vector2)llPos + Vector2.UnitX*((1 - posUV.X) * GetHeight(llPos) + posUV.X * GetHeight(lrPos));

            return new Vector3(1, 0, 0);
        }

        public float GetHeight(Vector2 pos)
        {
            Vector2 localCoord = pos + FieldSize / 2;
            localCoord = new Vector2(localCoord.X / FieldSize.X * NumFields.X, localCoord.Y / FieldSize.Y * NumFields.Y);
            // Lower left, lower right, upper left, upper right positions.
            VectorInt llPos = new VectorInt((int)localCoord.X, (int)localCoord.Y);
            VectorInt lrPos = llPos + VectorInt.UnitX;
            VectorInt ulPos = llPos + VectorInt.UnitY;
            VectorInt urPos = lrPos + VectorInt.UnitY;

            Vector2 posUV = localCoord - (Vector2)llPos;

            float posValue = (1 - posUV.Y) * ((1 - posUV.X) * GetHeight(llPos) + posUV.X * GetHeight(lrPos)) +
                             posUV.Y *       ((1 - posUV.X) * GetHeight(ulPos) + posUV.X * GetHeight(urPos));

            return posValue;
        }
    }
}
