using System;
using System.Numerics;
using Riateu;

namespace SimpleGame;

public class SpriteRenderer : Component 
{
    public Vector4 Color 
    {
        get => color;
        set 
        {
            if (color == value)
                return;
            color = value;
            isDirty = true;
        }
    }
    private Transform lastTransform;
    public Vector2[] TexCoords => Sprite.TexCoords;
    public Texture2D Texture => Sprite.Texture;
    public Sprite Sprite 
    {
        get => sprite;
        set 
        {
            sprite = value;
            isDirty = true;
        }
    }
    private Sprite sprite;
    private Vector4 color;
    private bool isDirty;
    public bool Dirty => isDirty;

    public SpriteRenderer(Sprite sprite) 
    {
        this.sprite = sprite;
        color = Vector4.One;
        isDirty = true;
    }

    public SpriteRenderer(Vector4 color) 
    {
        this.color = color;
        this.sprite = new Sprite(null);
        isDirty = true; 
    }

    public override void Ready()
    {
        lastTransform = GameObject.Transform.Copy();
        base.Ready();
    }

    public override void Update(float dt)
    {
        if (lastTransform != GameObject.Transform) 
        {
            GameObject.Transform.Copy(lastTransform);
            isDirty = true;
        }
    }

    public void Clean() 
    {
        isDirty = true;
    }
    
}