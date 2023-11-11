using System.Collections.Generic;
using Chunk;
using DefaultNamespace;
using UnityEngine;
using static Consts.Chunk;

public class GameInitialization : MonoBehaviour
{
    public Dictionary<Vector2Int, ChunkData> Chunks = new Dictionary<Vector2Int, ChunkData>();
    [SerializeField] ChunkRenderer _chunkPrefab;

    private void Start()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                var xPos = (int)(x * CHUNK_WIDTH * BLOCK_SCALE);
                var yPos = (int)(y * CHUNK_WIDTH * BLOCK_SCALE);

                var blocks = TerrainGenerator.GenerateTerrain(xPos, yPos);

                var data = new ChunkData();
                data.Position = new Vector2Int(x, y);
                data.Blocks = blocks;

                Chunks.Add(new Vector2Int(x, y), data);

                var chunk = Instantiate(_chunkPrefab, new Vector3(xPos, 0, yPos), Quaternion.identity, transform);
                chunk.ChunkData = data;
                chunk.GameInitialization = this;
            }
        }
    }
}