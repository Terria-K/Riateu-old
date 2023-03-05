using System;
using System.Collections.Generic;
using System.Numerics;
using Riateu;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace SimpleGame;

public abstract class Scene 
{
    public Painter Painter;
    public Camera GameCamera;
    public List<GameObject> gameObjects = new List<GameObject>();
    public bool Paused;
    private bool hasRun = false;

    public Scene(GL gl) 
    {
        Painter = new Painter(gl);
    }

    public void Add(GameObject gameObject) 
    {
        gameObjects.Add(gameObject);
        if (hasRun)
            Painter.Add(gameObject);
    }

    public virtual void Initialize() {}
    public virtual void Ready() 
    {
        foreach (var gameObject in gameObjects) 
        {
            gameObject.Ready();
            Painter.Add(gameObject);
        }
        hasRun = true;
    }
    public virtual void Update(float dt) 
    {
        if (Paused)
            return;

        foreach (var gameObject in gameObjects) 
        {
            gameObject.Update(dt);
        }
    }

    public virtual void Render() 
    {
        foreach (var gameObject in gameObjects) 
        {
            gameObject.Render();
        }
    }
}

public class LevelScene : Scene
{
    public LevelScene(GL gl) : base(gl)
    {
    }

    public override void Update(float dt)
    {
    }
}
