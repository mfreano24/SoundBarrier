using UnityEngine;

public class Paintable : MonoBehaviour
{
    private readonly Color c_color = new Color(0, 0, 0, 0);
    int TEX_WIDTH = 10240, TEX_HEIGHT = 10240;
    public Material wallMat;
    Texture2D mTexture;
    bool PaintEnabled;
    private void Start()
    {
        mTexture = new Texture2D(TEX_WIDTH, TEX_HEIGHT);
        for (int x = 0; x < TEX_WIDTH; x++)
        {
            for (int y = 0; y < TEX_HEIGHT; y++)
            {
                mTexture.SetPixel(x, y, c_color);
            }
        }
        mTexture.Apply();
        wallMat.SetTexture("_DrawingTex", mTexture);

        PaintEnabled = true;
    }


    public void PaintOn(Vector2 textureCoord, Texture2D splashTex)
    {
        //TEXTURE COORD IS A DECIMAL POSITION FROM 0 TO 1
        if (PaintEnabled)
        {
            int x = (int)(textureCoord.x * TEX_WIDTH) - (splashTex.width / 2);
            int y = (int)(textureCoord.y * TEX_HEIGHT) - (splashTex.height / 2);
            for (int i = 0; i < splashTex.width; i++)
            {
                for (int j = 0; j < splashTex.height; j++)
                {
                    Color res = Color.Lerp(mTexture.GetPixel(i, j), new Color(1, 1, 1, 1), wallMat.color.a);
                    res.a = mTexture.GetPixel(i, j).a + wallMat.color.a;
                    mTexture.SetPixel(i, j, res);
                }
            }
            mTexture.Apply();
        }
    }


    /*
     * public void PaintOn(Vector2 textureCoord, Texture2D splashTexture)
{
    if (isEnabled)
    {
        int x = (int)(textureCoord.x * textureWidth) - (splashTexture.width / 2);
        int y = (int)(textureCoord.y * textureHeight) - (splashTexture.height / 2);
        for (int i = 0; i < splashTexture.width; ++i)
            for (int j = 0; j  0)
                {
                    Color result = Color.Lerp(existingColor, targetColor, alpha);   // resulting color is an addition of splash texture to the texture based on alpha
                    result.a = existingColor.a + alpha;                             // but resulting alpha is a sum of alphas (adding transparent color should not make base color more transparent)
                    m_texture.SetPixel(newX, newY, result);
                }
            }

        m_texture.Apply();
    }
}
     * 
     * 
     * 
     * 
     */

}
