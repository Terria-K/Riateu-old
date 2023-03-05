using System;
using System.IO;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Riateu;

public class Texture2D : IDisposable, IEquatable<Texture2D>
{
    private uint rendererID;
    private string filePath;
    private byte[] localBuffer;
    private int width, height;
    private ColorComponents components;
    private GraphicsDevice device;

    public int Width => width;
    public int Height => height;
    public uint Texture => rendererID;

    public Texture2D(GraphicsDevice device, string path) 
    {
        this.device = device;
        StbImage.stbi_set_flip_vertically_on_load(1);
        using var fs = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(fs, ColorComponents.RedGreenBlueAlpha);
        width = image.Width;
        height = image.Height;
        components = image.Comp;
        localBuffer = image.Data;
        filePath = path;
        rendererID = device.CreateTextureBuffer();

        device.CreateTexture2D((uint)width, (uint)height, localBuffer);
    }

    public Texture2D(GraphicsDevice device, int width, int height) 
    {
        rendererID = device.CreateTextureBuffer();
        device.CreateTexture2D((uint)width, (uint)height);
    }

    public void Bind() 
    {
        device.BindTexture2D(rendererID);
    }

    public void Unbind() 
    {
        device.UnbindTexture2D();
    }

    public void Dispose()
    {
        device.DeleteTexture(rendererID);
    }

    public bool Equals(Texture2D other)
    {
        return other.width == width && 
        other.height == height && 
        other.rendererID == rendererID && 
        other.filePath == filePath;
    }
}