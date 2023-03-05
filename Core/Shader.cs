using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Riateu;

public class Shader : IDisposable
{
    private uint rendererID;
    private string filePath;
    private GL gl;
    private Dictionary<string, int> locCache = new Dictionary<string, int>();

    public Shader(GL gl, string path) 
    {
        this.gl = gl;
        filePath = path;
        var source = ParseShader(path);
        rendererID = CreateShader(source);
    }

    public Shader(GL gl, string vertexPath, string fragmentPath) 
    {
        this.gl = gl;
        var source = ParseShader(vertexPath, fragmentPath);
        rendererID = CreateShader(source);
    }

    public void Bind() 
    {
        gl.UseProgram(rendererID);
    }

    public void Unbind() 
    {
        gl.UseProgram(0);
    }

    public void SetUniformArrayI(string name, uint count, int[] array) 
    {
        gl.Uniform1(GetUniformLocation(name), count, array);
    }

    public void SetUniform1i(string name, int value) 
    {
        gl.Uniform1(GetUniformLocation(name), value);
    }

    public void SetUniform2i(string name, int i1, int i2) 
    {
        gl.Uniform2(GetUniformLocation(name), i1, i2);
    }

    public void SetUniform3i(string name, int i1, int i2, int i3) 
    {
        gl.Uniform3(GetUniformLocation(name), i1, i2, i3);
    }

    public void SetUniform4i(string name, int i1, int i2, int i3, int i4) 
    {
        gl.Uniform4(GetUniformLocation(name), i1, i2, i3, i4);
    }

    public void SetUniform1f(string name, float value) 
    {
        gl.Uniform1(GetUniformLocation(name), value);
    }

    public void SetUniform2f(string name, float f1, float f2) 
    {
        gl.Uniform2(GetUniformLocation(name), f1, f2);
    }

    public void SetUniform3f(string name, float f1, float f2, float f3) 
    {
        gl.Uniform3(GetUniformLocation(name), f1, f2, f3);
    }

    public void SetUniform4f(string name, float f1, float f2, float f3, float f4) 
    {
        gl.Uniform4(GetUniformLocation(name), f1, f2, f3, f4);
    }

    public void SetUniformVector2(string name, Vector2 v2) 
    {
        gl.Uniform2(GetUniformLocation(name), v2);
    }

    public void SetUniformVector3(string name, Vector3 v3) 
    {
        gl.Uniform3(GetUniformLocation(name), v3);
    }

    public void SetUniformVector4(string name, Vector4 v4) 
    {
        gl.Uniform4(GetUniformLocation(name), v4);
    }

    public void SetUniformMat3f(string name, Matrix3x2 mat) 
    {
        gl.UniformMatrix3(GetUniformLocation(name), 1u, false, GetMatrix3x2Value(mat));
    }

    public void SetUniformMat4f(string name, Matrix4x4 mat) 
    {
        gl.UniformMatrix4(GetUniformLocation(name), 1u, false, GetMatrix4x4Value(mat));
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

    private int GetUniformLocation(string name) 
    {
        if (locCache.TryGetValue(name, out int loc)) 
            return loc;    
        
        var location = gl.GetUniformLocation(rendererID, name);
        if (location == -1)
            Console.WriteLine($"Warning: uniform {name} does not exists!");
        locCache[name] = location;
        return location;
    }

    private uint CompileShader(string source, GLEnum type) 
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

    private uint CreateShader(ShaderBlock shaderBlock) 
    {
        return CreateShader(shaderBlock.VertexSource, shaderBlock.FragmentSource);
    }

    private uint CreateShader(string vertexShader, string fragmentShader) 
    {
        uint program = gl.CreateProgram();
        uint vs = CompileShader(vertexShader, GLEnum.VertexShader);
        uint fs = CompileShader(fragmentShader, GLEnum.FragmentShader);

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



    public void Dispose()
    {
        gl.DeleteProgram(rendererID);
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
