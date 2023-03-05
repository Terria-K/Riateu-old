using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using SimpleGame;

namespace Riateu;


//TODO Make a Graphics Class to replace SpriteRenderer
public class VertexBatch : IComparable<VertexBatch>
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Vertex
    {
        public Vector2 Position;
        public Vector4 Color;
        public Vector2 TexCoord;
        public float TexID;

    }
    private const int POSITION_SIZE = 2;
    private const int COLOR_SIZE = 4;
    private const int TEX_COORD_SIZE = 2;
    private const int TEX_ID_SIZE = 1;

    private const int POSITION_OFFSET = 0;
    private const int COLOR_OFFSET = POSITION_OFFSET + POSITION_SIZE * sizeof(float);
    private const int TEX_COORDS_OFFSET = COLOR_OFFSET + COLOR_SIZE * sizeof(float);
    private const int TEX_ID_OFFSET = TEX_COORDS_OFFSET + TEX_COORD_SIZE * sizeof(float);
    private const int VERTEX_SIZE = 9;
    private const int VERTEX_SIZE_IN_BYTES = VERTEX_SIZE * sizeof(float);

    private SpriteRenderer[] sprites;
    private List<Texture2D> textures; 
    private int numSprites;
    private bool hasRoom;
    public bool HasRoom => hasRoom;
    public bool HasTextureRoom => textures.Count < 8;

    private float[] vertices;
    private int[] texSlots = { 0, 1, 2, 3, 4, 5, 6, 7 };

    private uint vaoID, vboID;
    private int maxBatchSize;
    public int ZIndex;
    private Shader shader;
    private GL gl;



    public VertexBatch(GL gl, int maxBatchSize, int zIndex) 
    {
        this.gl = gl;
        this.maxBatchSize = maxBatchSize;
        this.ZIndex = zIndex;
        shader = ResourcePool.GetShader(gl, "res/shaders/draw");
        vertices = new float[maxBatchSize * 4 * VERTEX_SIZE];
        sprites = new SpriteRenderer[maxBatchSize];
        numSprites = 0;
        hasRoom = true;
        textures = new List<Texture2D>();
    }

    public void Start() 
    {
        InternalStart();
    }

    public unsafe void InternalStart() 
    {
        gl.Enable(GLEnum.Blend);
        gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        gl.GenVertexArrays(1, out vaoID);
        gl.BindVertexArray(vaoID);

        gl.GenBuffers(1, out vboID);
        gl.BindBuffer(GLEnum.ArrayBuffer, vboID);
        gl.BufferData(GLEnum.ArrayBuffer, (nuint)vertices.Length * sizeof(float), null, GLEnum.DynamicDraw);

        gl.GenBuffers(1, out uint eboID);
        gl.BindBuffer(GLEnum.ElementArrayBuffer, eboID);
        // Generate Indices
        int[] indices = new int[6 * maxBatchSize];
        for (int i = 0; i < maxBatchSize; i++) 
        {
            int offsetArrIdx = 6 * i;
            int offset = 4 * i;

            indices[offsetArrIdx + 0] = offset + 3;
            indices[offsetArrIdx + 1] = offset + 2;
            indices[offsetArrIdx + 2] = offset + 0;

            indices[offsetArrIdx + 3] = offset + 0;
            indices[offsetArrIdx + 4] = offset + 2;
            indices[offsetArrIdx + 5] = offset + 1;
        }

        fixed (void* ptr = &indices[0])
            gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)indices.Length * sizeof(int), ptr, GLEnum.StaticDraw);

        gl.VertexAttribPointer(0, POSITION_SIZE, GLEnum.Float, false, VERTEX_SIZE_IN_BYTES, (void*)POSITION_OFFSET);
        gl.EnableVertexAttribArray(0);

        gl.VertexAttribPointer(1, COLOR_SIZE, GLEnum.Float, false, VERTEX_SIZE_IN_BYTES, (void*)COLOR_OFFSET);
        gl.EnableVertexAttribArray(1);

        gl.VertexAttribPointer(2, TEX_COORD_SIZE, GLEnum.Float, false, VERTEX_SIZE_IN_BYTES, (void*)TEX_COORDS_OFFSET);
        gl.EnableVertexAttribArray(2);

        gl.VertexAttribPointer(3, TEX_ID_SIZE, GLEnum.Float, false, VERTEX_SIZE_IN_BYTES, (void*)TEX_ID_OFFSET);
        gl.EnableVertexAttribArray(3);
    }

    public void AddToBatch(SpriteRenderer spr) 
    {
        int idx = numSprites;
        sprites[idx] = spr;
        numSprites++;
        if (spr.Texture != null) 
        {
            if (!textures.Contains(spr.Texture)) 
                textures.Add(spr.Texture);
            
        }

        PushVertex(idx);

        if (numSprites >= maxBatchSize)
            hasRoom = false;
    }

    public void Draw(Camera camera) 
    {
        InternalDraw(camera);
    }

    public unsafe void InternalDraw(Camera camera) 
    {
        bool rebufferData = false;
        for (int i = 0; i < numSprites; i++) 
        {
            var spr = sprites[i];
            if (spr.Dirty) {
                PushVertex(i);
                spr.Clean();
                rebufferData = true;
            }
        }
        if (rebufferData) 
        {
            gl.BindBuffer(GLEnum.ArrayBuffer, vboID);
            fixed(void* ptr = &vertices[0])
                gl.BufferSubData(GLEnum.ArrayBuffer, 0, (nuint)vertices.Length * sizeof(float), ptr);
        }

        shader.Bind();
        shader.SetUniformMat4f("uProjection", camera.GetProjectionMatrix());
        shader.SetUniformMat4f("uView", camera.GetViewMatrix());
        for (int i = 0; i < textures.Count; i++) 
        {
            gl.ActiveTexture(GLEnum.Texture1 + i);
            textures[i].Bind();
        }
        shader.SetUniformArrayI("uTextures", 8, texSlots);

        gl.BindVertexArray(vaoID);
        gl.EnableVertexAttribArray(0);
        gl.EnableVertexAttribArray(1);
        gl.EnableVertexAttribArray(2);
        gl.EnableVertexAttribArray(3);

        gl.DrawElements(GLEnum.Triangles, (uint)numSprites * 6, GLEnum.UnsignedInt, null);
        gl.DisableVertexAttribArray(0);
        gl.DisableVertexAttribArray(1);
        gl.DisableVertexAttribArray(2);
        gl.DisableVertexAttribArray(3);

        gl.BindVertexArray(0);
        for (int i = 0; i < textures.Count; i++) 
        {
            gl.ActiveTexture(GLEnum.Texture0 + i);
            textures[i].Unbind();
        }
        shader.Unbind();
    }

    private void PushVertex(int idx) 
    {
        var sprite = sprites[idx];
        int offset = idx * 4 * VERTEX_SIZE;

        var color = sprite.Color;
        var texCoords = sprite.TexCoords;
        int texID = 0;
        if (sprite.Texture != null)
            for (int i = 0; i < textures.Count; i++) 
            {
                if (textures[i].Equals(sprite.Texture))
                {
                    texID = i + 1;
                    break;
                }
            }

        float xAdd = 1.0f;
        float yAdd = 1.0f;

        for (int i = 0; i < 4; i++) 
        {
            if (i == 1) 
                yAdd = 0.0f;
            
            else if (i == 2) 
                xAdd = 0.0f;
            
            else if (i == 3) 
                yAdd = 1.0f;

            vertices[offset + 0] = sprite.GameObject.Transform.Position.X + (xAdd * sprite.GameObject.Transform.Scale.X);
            vertices[offset + 1] = sprite.GameObject.Transform.Position.Y + (yAdd * sprite.GameObject.Transform.Scale.Y);

            vertices[offset + 2] = color.X;
            vertices[offset + 3] = color.Y;
            vertices[offset + 4] = color.Z;
            vertices[offset + 5] = color.W;

            vertices[offset + 6] = texCoords[i].X;
            vertices[offset + 7] = texCoords[i].Y;

            vertices[offset + 8] = texID;
            
            // vertices[offset + 0].Position.X = sprite.GameObject.Transform.Position.X + (xAdd * sprite.GameObject.Transform.Scale.X);
            // vertices[offset + 1].Position.Y = sprite.GameObject.Transform.Position.Y + (yAdd * sprite.GameObject.Transform.Scale.Y);

            // vertices[offset + 2].Color.X = color.X;
            // vertices[offset + 3].Color.Y = color.Y;
            // vertices[offset + 4].Color.Z = color.Z;
            // vertices[offset + 5].Color.W = color.W;

            // vertices[offset + 6].TexCoord.X = texCoords[i].X;
            // vertices[offset + 7].TexCoord.Y = texCoords[i].Y;

            // vertices[offset + 8].TexID = texID;

            offset += VERTEX_SIZE;
        }
    }

    public bool HasTextures(Texture2D tex) 
    {
        return textures.Contains(tex);
    }

    public int CompareTo(VertexBatch other)
    {
        if (ZIndex > other.ZIndex)
            return 1;
        if (ZIndex < other.ZIndex)
            return -1;
        return 0;
    }
}