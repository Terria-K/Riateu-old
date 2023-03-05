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

#region Shaders
    internal uint CreateShaderProgram(uint vs, uint fs) 
    {
        var program = gl.CreateProgram();
        gl.AttachShader(program, vs);
        gl.AttachShader(program, fs);
        gl.LinkProgram(program);
        gl.ValidateProgram(program);

        gl.DetachShader(program, vs);
        gl.DetachShader(program, fs);
        gl.DeleteShader(vs);
        gl.DeleteShader(fs);
        return program;
    }

    internal void LinkShaderProgram(uint program, uint compShader) 
    {
        gl.AttachShader(program, compShader);
        gl.LinkProgram(program);
        gl.ValidateProgram(program);

        gl.DetachShader(program, compShader);
        gl.DeleteShader(compShader);
    }

    internal void LinkShaderProgram(uint program, uint vs, uint fs) 
    {
        gl.AttachShader(program, vs);
        gl.AttachShader(program, fs);
        gl.LinkProgram(program);
        gl.ValidateProgram(program);

        gl.DetachShader(program, vs);
        gl.DetachShader(program, fs);
        gl.DeleteShader(vs);
        gl.DeleteShader(fs);
    }
    
    internal void DeleteProgram(uint program) 
    {
        gl.DeleteProgram(program);
    }

    internal uint CreateShader(string source, GLEnum type) 
    {
        uint id = gl.CreateShader(type);
        gl.ShaderSource(id, source);
        gl.CompileShader(id);

        gl.GetShader(id, GLEnum.CompileStatus, out int status);
        if (status == (int)GLEnum.False) 
        {
            var message = $"{type} failed to compile: " + gl.GetShaderInfoLog(id);
            Console.WriteLine(message);
            gl.DeleteShader(id);
            throw new Exception(message);
        }

        return id;
    }

    internal void UseShader(uint id) 
    {
        gl.UseProgram(id);
    }
    
    internal void DetachShader() 
    {
        gl.UseProgram(0);
    }

    internal int GetUniformLocation(uint id, string name) 
    {
        var location = gl.GetUniformLocation(id, name);
#if DEBUG
        if (location == -1)
            Console.WriteLine($"Warning: uniform {name} does not exists!");
        return location;
#endif
    }
#endregion

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

#region uniform
    public void SetUniformArrayI(int id, uint count, int[] array) 
    {
        gl.Uniform1(id, count, array);
    }

    public void SetUniform1i(int id, int value) 
    {
        gl.Uniform1(id, value);
    }

    public void SetUniform2i(int id, int i1, int i2) 
    {
        gl.Uniform2(id, i1, i2);
    }

    public void SetUniform3i(int id, int i1, int i2, int i3) 
    {
        gl.Uniform3(id, i1, i2, i3);
    }

    public void SetUniform4i(int id, int i1, int i2, int i3, int i4) 
    {
        gl.Uniform4(id, i1, i2, i3, i4);
    }

    public void SetUniform1f(int id, float value) 
    {
        gl.Uniform1(id, value);
    }

    public void SetUniform2f(int id, float f1, float f2) 
    {
        gl.Uniform2(id, f1, f2);
    }

    public void SetUniform3f(int id, float f1, float f2, float f3) 
    {
        gl.Uniform3(id, f1, f2, f3);
    }

    public void SetUniform4f(int id, float f1, float f2, float f3, float f4) 
    {
        gl.Uniform4(id, f1, f2, f3, f4);
    }

    public void SetUniformVector2(int id, Vector2 v2) 
    {
        gl.Uniform2(id, v2);
    }

    public void SetUniformVector3(int id, Vector3 v3) 
    {
        gl.Uniform3(id, v3);
    }

    public void SetUniformVector4(int id, Vector4 v4) 
    {
        gl.Uniform4(id, v4);
    }

    public void SetUniformMat3f(int id, Matrix3x2 mat) 
    {
        gl.UniformMatrix3(id, 1u, false, GetMatrix3x2Value(mat));
    }

    public void SetUniformMat4f(int id, Matrix4x4 mat) 
    {
        gl.UniformMatrix4(id, 1u, false, GetMatrix4x4Value(mat));
    }

    private ReadOnlySpan<float> GetMatrix3x2Value(Matrix3x2 mat) 
    {
        ReadOnlySpan<float> mats = new [] {
            mat.M11, mat.M12,
            mat.M21, mat.M22,
            mat.M31, mat.M32, 
        };
        return mats;
    }

    private ReadOnlySpan<float> GetMatrix4x4Value(Matrix4x4 mat) 
    {
        ReadOnlySpan<float> mats = new [] {
            mat.M11, mat.M12, mat.M13, mat.M14,
            mat.M21, mat.M22, mat.M23, mat.M24,
            mat.M31, mat.M32, mat.M33, mat.M34,
            mat.M41, mat.M42, mat.M43, mat.M44
        };
        return mats;
    }
#endregion
}