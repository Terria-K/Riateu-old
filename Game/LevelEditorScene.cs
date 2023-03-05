using System;
using System.Numerics;
using Riateu;
using Silk.NET.OpenGL;

namespace SimpleGame;

public class LevelEditorScene : Scene
{
    private GL gl;
    private Texture2D player;
    private GameObject randomPlayer;
    private Spritesheet sprites;

    public LevelEditorScene(GL gl) : base(gl)
    {
        this.gl = gl;

    }

    public override void Initialize()
    {
        var textures = ResourcePool.GetTexture(gl, "res/textures/cavern_deco_aseprite-Sheet.png");
        ResourcePool.AddSpritesheet("cavern_sheet", new Spritesheet(textures, 32, 32, 8, 0));
        GameCamera = new Camera(Vector2.Zero);
        player = ResourcePool.GetTexture(gl, "res/textures/logo.png");

        sprites = ResourcePool.GetSpritesheet("cavern_sheet");

        randomPlayer = new GameObject("Player", new Transform(new Vector2(400, 400), new Vector2(32, 32)), 1);
        randomPlayer.AddComponent(new SpriteRenderer(sprites[4]));
        Add(randomPlayer);

        var randomPlayer2 = new GameObject("Player", new Transform(new Vector2(380, 400), new Vector2(32, 32)), -1);
        randomPlayer2.AddComponent(new SpriteRenderer(sprites[3]));
        Add(randomPlayer2);

        const int xOffset = 10;
        const int yOffset = 10;
        const float totalWidth = (float)(600 - xOffset * 2);
        const float totalHeight = (float)(300 - yOffset * 2);
        const float sizeX = totalWidth / 100.0f;
        const float sizeY = totalHeight / 100.0f;

        for (int x = 0; x < 100; x++)
            for (int y = 0; y < 100; y++) {
                float xPos = xOffset + (x * sizeX);
                float yPos = yOffset + (y * sizeY);

                var ent = new GameObject($"Obj {x}:{y}", new Transform(new Vector2(xPos, yPos), new Vector2(sizeX, sizeY)));
                ent.AddComponent(new SpriteRenderer(new Vector4(xPos/ totalWidth, yPos /totalHeight, 1, 1)));
                Add(ent);
            }
        base.Initialize();
    }

    public override void Update(float dt) 
    {
        base.Update(dt);
    }
    public override void Render()
    {
        Painter.Render(GameCamera);
        base.Render();
    }
}
