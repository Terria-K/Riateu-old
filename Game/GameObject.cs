using System.Collections.Generic;
using Riateu;

namespace SimpleGame;

public class GameObject 
{
    private string name;
    private int zIndex;
    private List<Component> components = new List<Component>();
    public Transform Transform = new Transform();
    public int ZIndex => zIndex;

    public GameObject(string name) 
    {
        this.name = name;
        zIndex = 0;
    }

    public GameObject(string name, Transform transform, int zIndex = 0) : this(name)
    {
        Transform = transform;
        this.zIndex = zIndex;
    }

    public void AddComponent(Component comp) 
    {
        comp.GameObject = this;
        components.Add(comp);
    }

    public T GetComponent<T>() 
    where T : Component
    {
        foreach (var component in components) 
        {
            if (component is T comp)
                return comp;
        }
        return default;
    }

    public void RemoveComponent<T>() 
    where T : Component
    {
        foreach (var component in components) 
        {
            if (component is T comp) 
            {
                components.Remove(component);        
                return;
            }
        }
    }

    public void Ready() 
    {
        foreach (var component in components) 
        {
            component.Ready();
        }
    }

    public void Update(float dt) 
    {
        foreach (var component in components) 
        {
            component.Update(dt);
        }
    }

    public void Render() 
    {
        foreach (var component in components) 
        {
            component.Render();
        }
    }
}