using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;
using SimpleGame;

namespace Riateu;

public class Painter 
{
    private const int MAX_BATCH_SIZE = 1000;
    private List<VertexBatch> batches = new List<VertexBatch>();
    private GL gl;
    private GraphicsDevice device;

    public Painter(GL gl, GraphicsDevice device) 
    {
        this.gl = gl;
        this.device = device;
    }

    public void Add(GameObject gameObject) 
    {
        var spr = gameObject.GetComponent<SpriteRenderer>();
        if (spr != null) {
            Add(spr);
        }
    }

    private void Add(SpriteRenderer sprite) 
    {
        bool added = false;
        foreach (var batch in batches) 
        {
            if (batch.HasRoom && batch.ZIndex == sprite.GameObject.ZIndex) 
            {
                Texture2D tex = sprite.Texture;
                if (tex == null && batch.HasTextures(tex) || batch.HasTextureRoom) 
                {
                    batch.AddToBatch(sprite);
                    added = true;
                    break;
                }
            }
        }
        if (!added) 
        {
            VertexBatch batch = new VertexBatch(gl, device, MAX_BATCH_SIZE, sprite.GameObject.ZIndex);
            batch.Start();
            batches.Add(batch);
            batch.AddToBatch(sprite);
            batches.Sort();
        }
    }

    public void Render(Camera camera) 
    {
        foreach (var batch in batches) 
        {
            batch.Draw(camera);
        }
    }
}