using System.Numerics;

namespace Riateu;

public class Sprite 
{
    public Texture2D Texture;
    public Vector2[] TexCoords = new Vector2[4];

    public Sprite(Texture2D texture) 
    {
        this.Texture = texture;
        UpdateCoords();
    }

    public Sprite(Texture2D texture, Vector2[] texCoords) 
    {
        Texture = texture;
        TexCoords = texCoords;
    }

    private void UpdateCoords() 
    {
        TexCoords[0] = new Vector2(1f, 1f);
        TexCoords[1] = new Vector2(1f, 0f);
        TexCoords[2] = new Vector2(0f, 0f);
        TexCoords[3] = new Vector2(0f, 1f);
    }
}