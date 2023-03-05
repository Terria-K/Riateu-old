namespace SimpleGame;

public abstract class Component 
{
    public GameObject GameObject;
    public virtual void Ready() {}
    public virtual void Update(float dt) {}
    public virtual void Render() {}
}