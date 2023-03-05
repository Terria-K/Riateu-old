using System.Collections.Generic;
using System.Numerics;

namespace Riateu;

public class Spritesheet 
{
    private Texture2D texture;
    private List<Sprite> sprites = new List<Sprite>();

    public Sprite this[int idx] => sprites[idx];

    public Spritesheet(Texture2D texture, int spriteWidth, int spriteHeight, int numSprites, int spacing) 
    {
        this.texture = texture;
        int currentX = 0;
        int currentY = texture.Height - spriteHeight;
        for (int i = 0; i < numSprites; i++) 
        {
            float topY = (currentY + spriteHeight) / (float)texture.Height;
            float rightX = (currentX + spriteWidth) / (float)texture.Width;
            float leftX = currentX / (float)texture.Width;
            float bottomY = currentY / (float)texture.Height;

            var texCoords = new Vector2[4] {
                new Vector2(rightX, topY),
                new Vector2(rightX, bottomY),
                new Vector2(leftX, bottomY),
                new Vector2(leftX, topY)
            };
            var sprite = new Sprite(texture, texCoords);
            sprites.Add(sprite);
            currentX += spriteWidth + spacing;
            if (currentX >= texture.Width) 
            {
                currentX = 0;
                currentY -= spriteHeight + spacing;
            }
        }
    }
}