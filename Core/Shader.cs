using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using Silk.NET.OpenGL;

namespace Riateu;

public class Shader : IDisposable
{
    private uint rendererID;
    private string filePath;
    private GraphicsDevice device;
    private Dictionary<string, int> locCache = new Dictionary<string, int>();

    public Shader(GraphicsDevice device, string path) 
    {
        this.device = device;
        filePath = path;
        var source = ParseShader(path);
        rendererID = CreateShader(source);
    }

    public Shader(GraphicsDevice device, string vertexPath, string fragmentPath) 
    {
        this.device = device;
        var source = ParseShader(vertexPath, fragmentPath);
        rendererID = CreateShader(source);
    }

    public void Bind() 
    {
        device.UseShader(rendererID);
    }

    public void Unbind() 
    {
        device.DetachShader();
    }

    private int GetUniformLocation(string name) 
    {
        if (locCache.TryGetValue(name, out int loc)) 
            return loc;    

        var location = device.GetUniformLocation(rendererID, name);
        locCache[name] = location;
        return location;
    }

    private uint CreateShader(ShaderBlock shaderBlock) 
    {
        return CreateShader(shaderBlock.VertexSource, shaderBlock.FragmentSource);
    }

    private uint CreateShader(string vertexShader, string fragmentShader) 
    {
        uint vs = device.CreateShader(vertexShader, GLEnum.VertexShader);
        uint fs = device.CreateShader(fragmentShader, GLEnum.FragmentShader);

        uint program = device.CreateShaderProgram(vs, fs);

        return program;
    }

    private ShaderBlock ParseShader(string vertexFile, string fragmentFile) 
    {
        using var vfs = File.OpenRead(vertexFile);
        using var ffs = File.OpenRead(fragmentFile);
        using TextReader vtr = new StreamReader(vfs);
        using TextReader ftr = new StreamReader(ffs);

        return new ShaderBlock(vtr.ReadToEnd(), ftr.ReadToEnd());
    }

    private ShaderBlock ParseShader(string file) 
    {
        using var fs = File.OpenRead(file);
        using TextReader tr = new StreamReader(fs);

        string line;
        StringBuilder[] builders = new StringBuilder[2] {
            new StringBuilder(), new StringBuilder()
        };
        ShaderType shaderType = ShaderType.None;
        while ((line = tr.ReadLine()) != null) 
        {
            if (line.StartsWith("#shader")) 
            {
                if (line.Contains("vertex")) 
                    shaderType = ShaderType.Vertex;
                
                else if (line.Contains("fragment")) 
                    shaderType = ShaderType.Fragment;
                continue;                
            }
            builders[(int)shaderType].AppendLine(line);
        }

        return new ShaderBlock(builders[0].ToString(), builders[1].ToString());
    }

    public void SetUniformArrayI(string name, uint count, int[] array) 
    {
        device.SetUniformArrayI(GetUniformLocation(name), count, array);
    }

    public void SetUniform1i(string name, int value) 
    {
        device.SetUniform1i(GetUniformLocation(name), value);
    }

    public void SetUniform2i(string name, int i1, int i2) 
    {
        device.SetUniform2i(GetUniformLocation(name), i1, i2);
    }

    public void SetUniform3i(string name, int i1, int i2, int i3) 
    {
        device.SetUniform3i(GetUniformLocation(name), i1, i2, i3);
    }

    public void SetUniform4i(string name, int i1, int i2, int i3, int i4) 
    {
        device.SetUniform4i(GetUniformLocation(name), i1, i2, i3, i4);
    }

    public void SetUniform1f(string name, float value) 
    {
        device.SetUniform1f(GetUniformLocation(name), value);
    }

    public void SetUniform2f(string name, float f1, float f2) 
    {
        device.SetUniform2f(GetUniformLocation(name), f1, f2);
    }

    public void SetUniform3f(string name, float f1, float f2, float f3) 
    {
        device.SetUniform3f(GetUniformLocation(name), f1, f2, f3);
    }

    public void SetUniform4f(string name, float f1, float f2, float f3, float f4) 
    {
        device.SetUniform4f(GetUniformLocation(name), f1, f2, f3, f4);
    }

    public void SetUniformVector2(string name, Vector2 v2) 
    {
        device.SetUniformVector2(GetUniformLocation(name), v2);
    }

    public void SetUniformVector3(string name, Vector3 v3) 
    {
        device.SetUniformVector3(GetUniformLocation(name), v3);
    }

    public void SetUniformVector4(string name, Vector4 v4) 
    {
        device.SetUniformVector4(GetUniformLocation(name), v4);
    }

    public void SetUniformMat3f(string name, Matrix3x2 mat) 
    {
        device.SetUniformMat3f(GetUniformLocation(name), mat);
    }

    public void SetUniformMat4f(string name, Matrix4x4 mat) 
    {
        device.SetUniformMat4f(GetUniformLocation(name), mat);
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

    public void Dispose()
    {
        device.DeleteProgram(rendererID);
    }
}

public struct ShaderBlock 
{
    public string FragmentSource;
    public string VertexSource;

    public ShaderBlock(string vertexSource, string fragmentSource) 
    {
        VertexSource = vertexSource;
        FragmentSource = fragmentSource;
    }
}

public enum ShaderType 
{
    None = -1,
    Vertex,
    Fragment
}
