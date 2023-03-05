using System.Collections.Generic;
using System.IO;
using Silk.NET.OpenGL;

namespace Riateu;

public static class ResourcePool
{
    private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
    private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    private static Dictionary<string, Spritesheet> spritesheets = new Dictionary<string, Spritesheet>();

    public static Shader GetShader(GL gl, string resourcePath) 
    {
        if (shaders.TryGetValue(resourcePath, out Shader shader))
            return shader;
        var shd = new Shader(gl, resourcePath + ".vert", resourcePath + ".frag");
        shaders.Add(resourcePath, shd);
        return shd;
    }

    public static Texture2D GetTexture(GL gl, string resourcePath) 
    {
        if (textures.TryGetValue(resourcePath, out Texture2D texture))
            return texture;
        var tex = new Texture2D(gl, resourcePath);
        textures.Add(resourcePath, tex);
        return tex;
    }

    public static void AddSpritesheet(string resourceName, Spritesheet spritesheet) 
    {
        if (!spritesheets.ContainsKey(resourceName)) 
            spritesheets.Add(resourceName, spritesheet);
    }

    public static Spritesheet GetSpritesheet(string resourceName) 
    {
        if (spritesheets.ContainsKey(resourceName)) 
        {
            return spritesheets[resourceName];
        }
        return null;
    }
}