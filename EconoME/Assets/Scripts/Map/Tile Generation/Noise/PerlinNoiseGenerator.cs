using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PerlinNoiseGenerator : MonoBehaviour
{
    public int width = 32;
    public int height = 32;

    public float scale = 3.5f;

    const int DIVIDER = 42069;
    public string seed;

    public float offsetX { get; set; }
    public float offsetY { get; set; }

    [SerializeField] RawImage rawImage;
    [SerializeField] RawImage colorImage;
    [SerializeField] List<RangeForColor> colorRanges = new();
    [SerializeField] List<RangeForColorOverride> colorOverrides = new();
    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    [ContextMenu("Seed to int")]
    public void GenerateNoise()
    {
        Debug.Log($"{seed} is {seed.GetHashCode()} as an int");
        offsetX = seed.GetHashCode() / 42069;
        offsetY = seed.GetHashCode() / 42069;
        rawImage.texture = GenerateTexture(out _);


    }

    public float[,] GeneratePelinData()
    {
        rawImage.texture = GenerateTexture(out var data);
        return data;
    }

    Texture2D GenerateTexture(out float[,] data)
    {
        Texture2D texture = new(width, height);
        Texture2D colorTexture = new(width, height);
        data = new float[width, height];
        //Generate perline noise map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y, data);
                Color visualColor = CalculateVisualColor(x, y);
                colorTexture.SetPixel(x, y, visualColor);
                texture.SetPixel(x, y, color);
            }
        }


        colorTexture.filterMode = FilterMode.Point;
        colorTexture.wrapMode = TextureWrapMode.Clamp;

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.Apply();
        colorTexture.Apply();

        colorImage.texture = colorTexture;
        return texture;

        Color CalculateVisualColor(int x, int y)
        {
            float xCoord = (float)x / width * scale + offsetX;
            float yCoord = (float)y / height * scale + offsetY;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);
            //float sample = OctavePerlin(xCoord, yCoord, 4, 2);

            for (int i = 0; i < colorOverrides.Count; i++)
            {
                var colorOverride = colorOverrides[i];
                foreach (var cOveride in colorOverride.ranges)
                {
                    if (sample.isBetween(cOveride.x, cOveride.y))
                        return colorOverrides[i].color;
                }

            }

            for (int i = 0; i < colorRanges.Count; i++)
            {
                if (sample < colorRanges[i].lessThan)
                {
                    return colorRanges[i].color;
                }
            }
            return Color.black;
            /*
            switch (sample)
            {
                case var _ when sample < 0.4f:
                    return Color.white;
                case var _ when sample < 0.6f:
                    return Color.yellow;
                case var _ when sample < 0.7f:
                    return Color.red;
                case var _ when sample < 0.8f:
                    return Color.blue;
                case var _ when sample <= 1f:
                    return Color.magenta;
                default:
                    return Color.black;
            }
            */
        }
        Color CalculateColor(int x, int y, float[,] data)
        {
            float xCoord = (float)x / width * scale + offsetX;
            float yCoord = (float)y / height * scale + offsetY;


            float sample = Mathf.PerlinNoise(xCoord, yCoord);
            //float sample = OctavePerlin(xCoord, yCoord, 4, 2);
            data[x, y] = sample;
            return new Color(sample, sample, sample);
        }
    }

    public static float GetPerlineNoiseFromSeed(int width, int height, int seed, int octaveCount = 0, float scale = 2.5f)
    {
        float offsetX = seed / DIVIDER;
        float offsetY = seed / DIVIDER;
        int x = 0;
        int y = 0;

        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);


    }

    public static float[,] GeneratePerlinNoise2D(int width, int height, int seed, int octaveCount = 0, float scale = 2.5f)
    {
        float[,] data = new float[width, height];

        float offsetX = seed / 42069;
        float offsetY = seed / 42069;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;

                data[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        return data;

    }

    public static float OctavePerlin(float x, float y, int octaves, float persistence)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;

    }

}

[System.Serializable]
public class RangeForColor
{
    public Color color;
    [Range(0, 1)] public float lessThan;
}

[System.Serializable]
public class RangeForColorOverride
{
    public Color color;

    public List<Vector2> ranges;
}
