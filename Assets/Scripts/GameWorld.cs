using System.Collections.Generic;
using Chunks;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    [SerializeField] private ChunkRenderer _chunkPrefab;

    public static Dictionary<Vector2Int, ChunkData> ChunkDatas = new();

    private void Start()
    {
        for (var x = 0; x < 10; x++)
        {
            for (var z = 0; z < 10; z++)
            {
                var chunkPosition = new Vector2Int(x, z);
                var xPosition = x * Constants.Chunk.CHUNK_WIDTH * Constants.Block.BLOCK_SCALE;
                var zPosition = z * Constants.Chunk.CHUNK_WIDTH * Constants.Block.BLOCK_SCALE;
                var generatedBlocks = TerrainGenerator.GenerateTerrain(xPosition, zPosition);
                var chunkData = new ChunkData
                {
                    ChunkPosition = chunkPosition,
                    Blocks = generatedBlocks
                };

                ChunkDatas.Add(chunkPosition, chunkData);

                var spawnPosition = new Vector3(xPosition, 0, zPosition);
                var chunk = Instantiate(_chunkPrefab, spawnPosition, Quaternion.identity, transform);
                chunk.name = $"Chunk {x}-{z}";
                chunk.ChunkData = chunkData;
            }
        }
    }
}