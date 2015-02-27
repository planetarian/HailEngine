using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hail.Helpers
{
    public static class Primitives
    {
        #region PrimitiveType enum

        public enum PrimitiveType
        {
            BoundingBox,
            Grid,
            Sphere,
        }

        #endregion

        private const float size = 10;
        private const float gridMulti = 10;
        private const short defaultBufferSize = 10000;
        private const short indicesMultiplier = 4;
        private const int numTypes = 3;
        private static readonly Color vertColor = Color.White;

        private static BasicEffect effect;
        private static short bufferPos;
        private static short indicesPos;
        private static VertexPositionColor[] vertices;
        private static short[] indices;

        private static readonly short[][] presetIndices =
            new short[numTypes][];

        private static readonly VertexPositionColor[][] presetVertices =
            new VertexPositionColor[numTypes][];

        /// <summary>
        /// Initializes the index and vertex arrays for the given primitive type.
        /// </summary>
        /// <param name="type">PrimitiveType to initialize</param>
        private static void InitPrimitive(PrimitiveType type)
        {
            var typeVal = (int) type;
            switch (type)
            {
                    #region BoundingBox

                case PrimitiveType.BoundingBox:
                    if (presetIndices[typeVal] != null && presetVertices[typeVal] != null) break;
                    presetIndices[typeVal] = new short[]
                                                 {
                                                     0, 1, 1, 2, 2, 3, 3, 0, // top square
                                                     0, 4, 1, 5, 2, 6, 3, 7, // bottom square
                                                     4, 5, 5, 6, 6, 7, 7, 4, // connecting lines
                                                     8, 9, // forward
                                                     10, 11 // up
                                                 };
                    presetVertices[typeVal] = new[]
                                                  {
                                                      new VertexPositionColor(new Vector3(size, size, size), vertColor),
                                                      new VertexPositionColor(new Vector3(size, -size, size), vertColor)
                                                      ,
                                                      new VertexPositionColor(new Vector3(-size, -size, size), vertColor)
                                                      ,
                                                      new VertexPositionColor(new Vector3(-size, size, size), vertColor)
                                                      ,
                                                      new VertexPositionColor(new Vector3(size, size, -size), vertColor)
                                                      ,
                                                      new VertexPositionColor(new Vector3(size, -size, -size), vertColor)
                                                      ,
                                                      new VertexPositionColor(new Vector3(-size, -size, -size),
                                                                              vertColor),
                                                      new VertexPositionColor(new Vector3(-size, size, -size), vertColor)
                                                      ,
                                                      new VertexPositionColor(new Vector3(0, 0, 0), vertColor),
                                                      new VertexPositionColor(new Vector3(0, 0, -size*2), vertColor),
                                                      new VertexPositionColor(new Vector3(0, size/5f, 0), vertColor),
                                                      new VertexPositionColor(new Vector3(0, size*1.2f, 0), vertColor)
                                                  };

                    break;

                    #endregion

                    #region Grid

                case PrimitiveType.Grid:
                    if (presetIndices[typeVal] != null && presetVertices[typeVal] != null) break;
                    presetIndices[typeVal] = new short[]
                                                 {
                                                     // Vertical lines
                                                     0, 1,
                                                     2, 3,
                                                     4, 5,
                                                     6, 7,
                                                     8, 9,
                                                     10, 11,
                                                     12, 13,
                                                     14, 15,
                                                     16, 17,
                                                     18, 19,
                                                     20, 21,
                                          
                                                     // Horizontal lines
                                                     0, 20,
                                                     22, 23,
                                                     24, 25,
                                                     26, 27,
                                                     28, 29,
                                                     30, 31,
                                                     32, 33,
                                                     34, 35,
                                                     36, 37,
                                                     38, 39,
                                                     1, 21
                                                 };
                    presetVertices[typeVal] = new VertexPositionColor[40];

                    // Vertical lines
                    float x = -size*(gridMulti/2); // e.g. -5
                    float z = -size*(gridMulti/2); // e.g. -5
                    for (int i = 0; i < 22; i++)
                    {
                        presetVertices[typeVal][i++] = new VertexPositionColor(
                            new Vector3(x, 0, z), vertColor);
                        presetVertices[typeVal][i] = new VertexPositionColor(
                            new Vector3(x, 0, z + size*gridMulti), vertColor);
                        x += size*gridMulti/10; // increment x
                      
                    }

                    // Horizontal lines
                    x = -size*(gridMulti/2);
                    z = -size*(gridMulti/2) + size*gridMulti/10;
                    for (int i = 22; i < 40; i++)
                    {
                        presetVertices[typeVal][i++] = new VertexPositionColor(
                            new Vector3(x, 0, z), vertColor);
                        presetVertices[typeVal][i] = new VertexPositionColor(
                            new Vector3(x + size*gridMulti, 0, z), vertColor);
                        z += size*gridMulti/10; // increment z
                    }
                    break;

                    #endregion

                    #region Sphere

                    #endregion
            }
        }

        /// <summary>
        /// Initializes a BasicEffect for drawing primitives.
        /// </summary>
        /// <param name="graphics">GraphicsDevice used to draw the primitive</param>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        /// <returns>BasicEffect initialized with given values</returns>
        private static void InitEffect(
            GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            if (effect == null)
                effect = new BasicEffect(graphics);
            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;
            effect.VertexColorEnabled = true;
        }

        /// <summary>
        /// Flushes the Primitives draw buffer and renders its contents.
        /// </summary>
        /// <param name="graphics">GraphicsDevice used to draw the primitive</param>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        public static void Flush(GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            if (bufferPos < 1 || indicesPos < 1) return; // Nothing to flush

            InitEffect(graphics, view, projection);
            var primitives = (short) (indicesPos/2);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserIndexedPrimitives(
                    Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList,
                    vertices, 0, bufferPos,
                    indices, 0, primitives);
            }

            bufferPos = 0;
            indicesPos = 0;
        }


        /// <summary>
        /// Adds a custom primitive with given vertices/indices to the Primitives draw buffer.
        /// </summary>
        /// <param name="graphics">GraphicsDevice used to draw the primitive</param>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        /// <param name="world">World Matrix used for rendering the primitive</param>
        /// <param name="customVertices">VertexPositionColor array contain vertex data</param>
        /// <param name="customIndices">Array containing vertex pair indices</param>
        public static void AddPrimitive(GraphicsDevice graphics,
                                        Matrix view, Matrix projection, Matrix world,
                                        VertexPositionColor[] customVertices, short[] customIndices)
        {
            if (graphics == null) throw new ArgumentNullException("graphics");

            if (vertices == null) vertices = new VertexPositionColor[defaultBufferSize];
            if (indices == null) indices = new short[defaultBufferSize*indicesMultiplier];


            // Flush the buffers if there is not enough space to fit the primitive
            if (vertices.Length - bufferPos < customVertices.Length ||
                indices.Length - indicesPos < customIndices.Length)
            {
                Flush(graphics, view, projection);
            }

            // Store the current vertex buffer position for calculating index offsets
            short verticesOffset = bufferPos;

            // Place the transformed vertices into the vertex buffer
            for (int i = 0; i < customVertices.Length; i++)
            {
                vertices[bufferPos++] = new VertexPositionColor(
                    Vector3.Transform(customVertices[i].Position, world),
                    vertColor
                    );
            }

            // Add the indices to the index buffer
            for (int i = 0; i < customIndices.Length; i++)
            {
                indices[indicesPos++] = (short) (customIndices[i] + verticesOffset);
            }
        }

        /// <summary>
        /// Adds a primitive of the specified built-in type to the Primitives draw buffer.
        /// </summary>
        /// <param name="graphics">GraphicsDevice used to draw the primitive</param>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        /// <param name="world">World Matrix used for rendering the primitive</param>
        /// <param name="type">Preset PrimitiveType to be drawn</param>
        public static void AddPrimitive(GraphicsDevice graphics,
                                        Matrix view, Matrix projection, Matrix world,
                                        PrimitiveType type)
        {
            InitPrimitive(type);

            // Avoid constant casting
            var typeVal = (int) type;

            AddPrimitive(graphics, view, projection, world, presetVertices[typeVal], presetIndices[typeVal]);
        }

        /// <summary>
        /// Adds a bounding box to the Primitives draw buffer, with markers for forward/up.
        /// </summary>
        /// <param name="graphics">GraphicsDevice used to draw the primitive</param>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        /// <param name="world">World Matrix used for rendering the primitive</param>
        public static void AddBoundingBox(GraphicsDevice graphics,
                                          Matrix view, Matrix projection, Matrix world)
        {
            AddPrimitive(graphics, view, projection, world, PrimitiveType.BoundingBox);
        }

        /// <summary>
        /// Adds a 10x10 grid to the Primitives draw buffer.
        /// </summary>
        /// <param name="graphics">GraphicsDevice used to draw the primitive</param>
        /// <param name="view">Camera view matrix</param>
        /// <param name="projection">Camera projection matrix</param>
        /// <param name="world">World Matrix used for rendering the primitive</param>
        public static void AddGrid(GraphicsDevice graphics,
                                   Matrix view, Matrix projection, Matrix world)
        {
            AddPrimitive(graphics, view, projection, world, PrimitiveType.Grid);
        }
    }
}