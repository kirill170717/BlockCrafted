using UnityEngine;

namespace DefaultNamespace
{
    public class TerrainGenerator
    {
        public static int[,,] GenerateTerrain()
        {
            var result = new int
            [
                Constants.Chunk.CHUNK_WIDTH,
                Constants.Chunk.CHUNK_HEIGHT,
                Constants.Chunk.CHUNK_WIDTH
            ];

            for (var x = 0; x < Constants.Chunk.CHUNK_WIDTH; x++)
            {
                for (var z = 0; z < Constants.Chunk.CHUNK_WIDTH; z++)
                {
                    var height = Mathf.PerlinNoise(x * 0.2f, z * 0.2f) * 5 + 10;

                    for (var y = 0; y < height; y++)
                    {
                        result[x, y, z] = 1;
                    }
                }
            }

            return result;
        }
    }
}