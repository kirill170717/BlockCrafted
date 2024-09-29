using UnityEngine;
using Blocks.Enums;

public class TerrainGenerator
{
    public static BlockType[,,] GenerateTerrain(float xOffset, float zOffset)
    {
        var result = new BlockType
        [
            Constants.Chunk.CHUNK_WIDTH,
            Constants.Chunk.CHUNK_HEIGHT,
            Constants.Chunk.CHUNK_WIDTH
        ];

        for (var x = 0; x < Constants.Chunk.CHUNK_WIDTH; x++)
        {
            for (var z = 0; z < Constants.Chunk.CHUNK_WIDTH; z++)
            {
                var height = Mathf.PerlinNoise((x + xOffset) * 0.2f, (z + zOffset) * 0.2f) * 5 + 10;

                for (var y = 0; y < height; y++)
                {
                    result[x, y, z] = BlockType.Grass;
                }
            }
        }

        return result;
    }
}