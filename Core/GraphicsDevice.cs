using System;
using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Riateu;

public class GraphicsDevice 
{
    private GL gl;

    public GraphicsDevice(GL gl) 
    {
        this.gl = gl;
    }

    public void Clear(Vector4 color) 
    {
        gl.ClearColor(color.X, color.Y, color.Z, color.W);
        gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void Clear(Vector4 color, ClearBufferMask buffer) 
    {
        gl.ClearColor(color.X, color.Y, color.Z, color.W);
        gl.Clear(buffer);
    }

    public void Clear(Color color) 
    {
        gl.ClearColor(color);
        gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void Clear(Color color, ClearBufferMask buffer) 
    {
        gl.ClearColor(color);
        gl.Clear(buffer);
    }

    public void Clear(float r, float g, float b, float a) 
    {
        gl.ClearColor(r, g, b, a);
        gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void Clear(float r, float g, float b, float a, ClearBufferMask buffer) 
    {
        gl.ClearColor(r, g, b, a);
        gl.Clear(buffer);
    }

    public void SetRenderTarget(RenderTarget target) 
    {
        if (target == null) 
        {
            target.Bind();
            return;
        }
        BindRenderTarget(0);
    }

    public void SetRenderTarget(uint id) 
    {
        BindRenderTarget(id);
    }

    internal uint CreateTextureBuffer() 
    {
        gl.GenTextures(1, out uint rendererID);
        gl.BindTexture(GLEnum.Texture2D, rendererID);

        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.ClampToEdge);
        gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.ClampToEdge);
        return rendererID;
    }

    internal unsafe void CreateTexture2D(uint width, uint height, void* data) 
    {
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba8, (uint)width, (uint)height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, data);
    }

    internal unsafe void CreateTexture2D(uint width, uint height, byte[] data) 
    {
        fixed (void* ptr = &data[0])
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba8, (uint)width, (uint)height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
    }

    internal unsafe void CreateTexture2D(uint width, uint height) 
    {
        gl.TexImage2D(
            GLEnum.Texture2D, 0, (int)GLEnum.Rgba8, width, height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, (void*)0);
    }

    internal unsafe void CreateFrameBufferTexture2D(uint textureID, uint width, uint height) 
    {
        gl.FramebufferTexture2D(
            GLEnum.Framebuffer, 
            GLEnum.ColorAttachment0, 
            GLEnum.Texture2D, 
            textureID, 0
        );
    }

    internal uint CreateFrameBuffer() 
    {
        gl.GenFramebuffers(1, out uint fbo);
        gl.BindFramebuffer(GLEnum.Framebuffer, fbo);
        return fbo;
    }

    internal uint CreateRenderBuffer(uint width, uint height) 
    {
        gl.GenRenderbuffers(1, out uint rboID);
        gl.BindRenderbuffer(GLEnum.Renderbuffer, rboID);
        gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.DepthComponent32, width, height);
        gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Renderbuffer, rboID);
        return rboID;
    }

    internal uint CreateRenderBuffer(uint width, uint height, GLEnum depthComponent) 
    {
        gl.GenRenderbuffers(1, out uint rboID);
        gl.BindRenderbuffer(GLEnum.Renderbuffer, rboID);
        gl.RenderbufferStorage(GLEnum.Renderbuffer, depthComponent, width, height);
        gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Renderbuffer, rboID);
        return rboID;
    }

    internal void CheckFramebuffer() 
    {
#if DEBUG
        if (gl.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete) 
        {
            Console.WriteLine("Framebuffer is incompleted");
        }
#endif
    }

    internal void DeleteTexture(uint id) 
    {
        gl.DeleteTextures(1, id);
    }

    internal void BindTexture2D(uint id) 
    {
        gl.BindTexture(GLEnum.Texture2D, id);
    }

    internal void UnbindTexture2D() 
    {
        gl.BindTexture(GLEnum.Texture2D, 0);
    }

    internal void BindRenderTarget(uint id) 
    {
        gl.BindFramebuffer(GLEnum.Framebuffer, id);
    }

    internal void UnbindRenderTarget() 
    {
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);
    }
}