using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeGenerator : MonoBehaviour
{

    [SerializeField] Terrain terrain;
    // Start is called before the first frame update
    public float lakeSize = 20.0f;
    public float lakeDepth = 5.0f;

    void Start()
    {
        GenerateLakeOnTerrain();
    }

    void GenerateLakeOnTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        int centerX = heightmapWidth / 2;
        int centerY = heightmapHeight / 2;

        // Get the current heights from the terrain
        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        // Modify the heights to create a depression (lake)
        for (int x = 0; x < heightmapWidth; x++)
        {
            for (int y = 0; y < heightmapHeight; y++)
            {
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                if (distanceToCenter < lakeSize)
                {
                    // Calculate the depth based on distance to the center
                    float depth = lakeDepth * (1.0f - (distanceToCenter / lakeSize));
                    heights[x, y] -= depth;
                }
            }
        }

        // Apply the modified heights to the terrain
        terrainData.SetHeights(0, 0, heights);
    }
}
