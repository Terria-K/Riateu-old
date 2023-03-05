using System;
using Silk.NET.OpenGL;

namespace Riateu;

public class RenderTarget 
{
    public uint RendererID = 0;
    public Texture2D Texture = null;
    private GraphicsDevice device;
    public RenderTarget(GraphicsDevice device, int width, int height) 
    {
        this.device = device;
        RendererID = device.CreateFrameBuffer();

        Texture = new Texture2D(device, width, height);
        device.CreateFrameBufferTexture2D(Texture.Texture, (uint)width, (uint)height);

        device.CreateRenderBuffer((uint)width, (uint)height);
#if DEBUG
        device.CheckFramebuffer();
#endif
        device.UnbindRenderTarget();

    } 

    public void Bind() 
    {
        device.BindRenderTarget(RendererID);
    }

    public void Unbind() 
    {
        device.UnbindRenderTarget();
    }
}