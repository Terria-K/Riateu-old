using System;
using System.Numerics;

namespace Riateu;

public class Transform : IEquatable<Transform>
{
    public Vector2 Position;
    public Vector2 Scale;

    public Transform() {}

    public Transform(Vector2 position) 
    {
        Position = position;
    }

    public Transform(Vector2 position, Vector2 scale) 
    {
        Position = position;
        Scale = scale;
    }

    public Transform Copy() 
    {
        return new Transform(Position, Scale);
    }

    public void Copy(Transform to) 
    {
        to = new Transform(Position, Scale);
    }

    public bool Equals(Transform other)
    {
        return other.Position == Position && other.Scale == Scale;
    }
}