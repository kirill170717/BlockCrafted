using System.Collections.Generic;
using Chunk;
using DefaultNamespace;
using UnityEngine;
using static Consts.Chunk;

public class GameInitialization : MonoBehaviour
{
    [SerializeField] private ChunkRenderer _chunkPrefab;

    public Dictionary<Vector2Int, ChunkData> Chunks = new Dictionary<Vector2Int, ChunkData>();

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;

        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 20; y++)
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

                data.ChunkRenderer = chunk;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            var isDestroying = Input.GetMouseButtonDown(0);
            var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out var hit))
            {
                var center = hit.normal * (BLOCK_SCALE / 2);
                var blockCenter = isDestroying ? hit.point - center : hit.point + center;

                var blockWorldPosition = Vector3Int.FloorToInt(blockCenter / BLOCK_SCALE);
                var chunkPosition = GetChunkContainingBlock(blockWorldPosition);

                if (Chunks.TryGetValue(chunkPosition, out var chunkData))
                {
                    var chunkOrigin = new Vector3Int(chunkPosition.x, 0, chunkPosition.y) * CHUNK_WIDTH;

                    if (isDestroying)
                    {
                        chunkData.ChunkRenderer.RemoveBlock(blockWorldPosition - chunkOrigin);
                    }
                    else
                    {
                        chunkData.ChunkRenderer.SpawnBlock(blockWorldPosition - chunkOrigin);
                    }
                }
            }
        }
    }

    private Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPosition)
    {
        return new Vector2Int(blockWorldPosition.x / CHUNK_WIDTH, blockWorldPosition.z / CHUNK_WIDTH);
    }
}