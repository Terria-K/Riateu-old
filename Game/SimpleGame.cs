using System.Numerics;
using Riateu;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace SimpleGame;

public class SimpleGame : Game
{
    public Scene currentScene;
    public Vector4 Color = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
    public RenderTarget buffer;



    public SimpleGame(int width, int height, string title) : base(width, height, title)
    {
    }

    protected override void Initialize()
    {
        buffer = new RenderTarget(GL, 1024, 768);
        currentScene = new LevelEditorScene(GL);
        currentScene.Initialize();
        currentScene.Ready();
    }

    protected override void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
    }

    protected override void Render()
    {
        GL.ClearColor(Color.X, Color.Y, Color.Z, Color.W);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        currentScene.Render();
    }

    protected override void Unload()
    {
    }

    protected override void Update(double delta)
    {
        currentScene.Update((float)delta);
    }
}
