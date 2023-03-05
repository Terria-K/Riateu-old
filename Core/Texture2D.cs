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
    private GL gl;

    public int Width => width;
    public int Height => height;
    public uint Texture => rendererID;

    public Texture2D(GL gl, string path) 
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        using var fs = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(fs, ColorComponents.RedGreenBlueAlpha);
        this.gl = gl;
        width = image.Width;
        height = image.Height;
        components = image.Comp;
        localBuffer = image.Data;
        filePath = path;
        gl.GenTextures(1, out rendererID);
        gl.BindTexture(GLEnum.Texture2D, rendererID);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.ClampToEdge);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.ClampToEdge);

        unsafe 
        {
            fixed (void* ptr = &localBuffer[0]) 
            {
                gl.TexImage2D(
                    GLEnum.Texture2D, 0, 
                    (int)GLEnum.Rgba8, 
                    (uint)width, (uint)height, 
                    0, GLEnum.Rgba, GLEnum.UnsignedByte, 
                    ptr);
            }
        }


        gl.BindTexture(GLEnum.Texture2D, 0);
    }

    public Texture2D(GL gl, int width, int height) 
    {
        gl.GenTextures(1, out rendererID);
        gl.BindTexture(GLEnum.Texture2D, rendererID);

        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.ClampToEdge);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.ClampToEdge);

        unsafe 
        {
            gl.TexImage2D(
                GLEnum.Texture2D, 0, 
                (int)GLEnum.Rgba8, 
                (uint)width, (uint)height, 
                0, GLEnum.Rgba, GLEnum.UnsignedByte, 
                (void*)0);
        }

    }

    public void Bind() 
    {
        gl.BindTexture(GLEnum.Texture2D, rendererID);
    }

    public void Unbind() 
    {
        gl.BindTexture(GLEnum.Texture2D, 0);
    }

    public void Dispose()
    {
        gl.DeleteTextures(1, rendererID);
    }

    public bool Equals(Texture2D other)
    {
        return other.width == width && 
        other.height == height && 
        other.rendererID == rendererID && 
        other.filePath == filePath;
    }
}