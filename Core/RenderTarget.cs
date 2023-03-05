using System;
using Silk.NET.OpenGL;

namespace Riateu;

public class RenderTarget 
{
    public uint RendererID = 0;
    public Texture2D Texture = null;
    private GL gl;
    public RenderTarget(GL gl, int width, int height) 
    {
        this.gl = gl;
        gl.GenFramebuffers(1, out RendererID);
        gl.BindFramebuffer(GLEnum.Framebuffer, RendererID);

        Texture = new Texture2D(gl, width, height);
        gl.FramebufferTexture2D(
            GLEnum.Framebuffer, 
            GLEnum.ColorAttachment0, 
            GLEnum.Texture2D, 
            Texture.Texture, 0
        );

        gl.GenRenderbuffers(1, out uint rboID);
        gl.BindRenderbuffer(GLEnum.Renderbuffer, rboID);
        gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.DepthComponent32, (uint)width, (uint)height);
        gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Renderbuffer, rboID);

#if DEBUG
        if (gl.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete) 
        {
            Console.WriteLine("Framebuffer is incompleted");
        }
#endif
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);
    } 

    public void Bind() 
    {
        gl.BindFramebuffer(GLEnum.Framebuffer, RendererID);
    }

    public void Unbind() 
    {
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);
    }
}