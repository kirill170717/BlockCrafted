using Chunk.Enums;
using UnityEngine;
using static Consts.Chunk;

namespace DefaultNamespace
{
    public static class TerrainGenerator
    {
        public static BlockType[,,] GenerateTerrain(int xOffset, int yOffset)
        {
            var result = new BlockType[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];

            for (int x = 0; x < CHUNK_WIDTH; x++)
            {
                for (int z = 0; z < CHUNK_WIDTH; z++)
                {
                    var height = Mathf.PerlinNoise((x + xOffset) * 0.2f, (z + yOffset) * 0.2f) * 5 + 10;

                    for (int y = 0; y < height; y++)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                }
            }

            return result;
        }
    }
}