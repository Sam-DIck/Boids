using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralTerrain : MonoBehaviour
{
    public Terrain terrain;
    public float height = 0.1f;
    public int octaves = 10;
    public float lacunarity = 2;
    public float persistance = 0.5f;
    Vector2[] octaveOffsets;
    public int Blocks = 16;
    int _blocks = 16;
    // Start is called before the first frame update
    void Start()
    {
        _blocks = Blocks;
        StartCoroutine(GenerateTerrainCoroutine());
        //terrain.terrainData.SyncHeightmap();
    }
    void createOctaveOffsets()
    {
        octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(Random.value, Random.value) * 1000f;
        }
    }

    void generateTerrain() {
        generateTerrain(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
    }
    void generateTerrain( int startX, int startY, int sizeX, int sizeY, int tileOffsetX=0, int tileOffsetY = 0)
    {
        if (octaveOffsets == null)
        {
            createOctaveOffsets();
        }
        float[,] heights = new float[sizeX,sizeY];
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                float X = (x + tileOffsetX * sizeX) / (float)terrain.terrainData.heightmapResolution;
                float Y = (y + tileOffsetY * sizeY) / (float)terrain.terrainData.heightmapResolution;

                heights[x, y] = getHeight(X, Y);
            }
        }
        terrain.terrainData.SetHeightsDelayLOD(startX, startY, heights);
    }
    
    IEnumerator GenerateTerrainCoroutine()
    {
        int blockSize = terrain.terrainData.heightmapResolution / _blocks;
        for (int i = 0; i < _blocks; i++)
        {
            for (int j = 0; j < _blocks; j++)
            {
                generateTerrain(i * blockSize, j * blockSize, blockSize+1, blockSize+1, j, i);
                yield return new WaitForEndOfFrame();
            }
        }
        Debug.Log("terrain generation");
        terrain.terrainData.SyncHeightmap();
    }
    /*private void OnValidate()
    {
        if (Application.isPlaying)
        {
            StopAllCoroutines();
            terrain.terrainData.SyncHeightmap();
            StartCoroutine(GenerateTerrainCoroutine());
        }
    }*/

    static float sigmoid(float x) { return 1 / (1 + Mathf.Exp(-x)); }


    static float falloff(float x, float y)
    {
        float a = 3;
        float b = 2.2f;
        float dist = Mathf.Max(Mathf.Abs(x * 2 - 1), Mathf.Abs(y * 2 - 1));
        return Mathf.Pow(dist, a) / (Mathf.Pow(dist, a) + Mathf.Pow(b - b * dist, a));
    }

    float getHeight(float x, float y)
    {
        float h = 1;
        for (int i = 0; i < octaves; i++)
        {
            float xO = octaveOffsets[i].x;
            float yO = octaveOffsets[i].y;
            h += Mathf.PerlinNoise(
                x * Mathf.Pow(lacunarity,i)+xO,
                y * Mathf.Pow(lacunarity, i)+yO
            ) * Mathf.Pow(persistance, i);
        }
            
        return (h*height-0.5f)*(falloffBase + falloffScale * falloff(x,y));
    }
    public float falloffBase = 0f;
    public float falloffScale = 1f;
}
