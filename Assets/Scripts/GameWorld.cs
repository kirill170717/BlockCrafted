using System.Collections;
using System.Collections.Generic;
using Chunks;
using Player;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private ChunkRenderer _chunkPrefab;
    [SerializeField] private TerrainGenerator _terrainGenerator;
    [SerializeField] private int _renderDistance = 6;

    public static Dictionary<Vector2Int, ChunkData> ChunkDatas = new();

    private Camera _mainCamera;
    private Vector2Int _currentPlayerChunk;

    private void Start()
    {
        StartCoroutine(GenerateChunks(false));

        var playerController = Instantiate(_player, new Vector3(10f, 20f, 10f), Quaternion.identity);
        _mainCamera = playerController.MainCamera;
    }

    private void Update()
    {
        if (_mainCamera == null) return;

        var playerWorldPosition = Vector3Int.FloorToInt(_mainCamera.transform.position / Constants.Block.BLOCK_SCALE);
        var playerChunk = ChunkExtensions.GetChunkContainingBlock(playerWorldPosition);

        if (playerChunk != _currentPlayerChunk)
        {
            _currentPlayerChunk = playerChunk;
            StartCoroutine(GenerateChunks(true));
        }
    }

    private IEnumerator GenerateChunks(bool wait)
    {
        for (var x = _currentPlayerChunk.x - _renderDistance; x < _currentPlayerChunk.x + _renderDistance; x++)
        {
            for (var z = _currentPlayerChunk.y - _renderDistance; z < _currentPlayerChunk.y + _renderDistance; z++)
            {
                var chunkPosition = new Vector2Int(x, z);

                if (ChunkDatas.ContainsKey(chunkPosition)) continue;

                GenerateChunk(chunkPosition);

                if (wait) yield return new WaitForSecondsRealtime(0.2f);
            }
        }
    }

    private void GenerateChunk(Vector2Int chunkPosition)
    {
        var xPosition = chunkPosition.x * Constants.Chunk.CHUNK_WIDTH * Constants.Block.BLOCK_SCALE;
        var zPosition = chunkPosition.y * Constants.Chunk.CHUNK_WIDTH * Constants.Block.BLOCK_SCALE;
        var generatedBlocks = _terrainGenerator.GenerateTerrain(xPosition, zPosition);
        var chunkData = new ChunkData
        {
            ChunkPosition = chunkPosition,
            Blocks = generatedBlocks
        };

        ChunkDatas.Add(chunkPosition, chunkData);

        var spawnPosition = new Vector3(xPosition, 0, zPosition);
        var chunkRenderer = Instantiate(_chunkPrefab, spawnPosition, Quaternion.identity, transform);
        chunkRenderer.name = $"Chunk {chunkPosition.x}-{chunkPosition.y}";
        chunkRenderer.ChunkData = chunkData;
    }

    [ContextMenu("Regenerate world")]
    public void RegenerateWorld()
    {
        _terrainGenerator.InitializationNoises();
        
        foreach (var chunkData in ChunkDatas)
        {
            Destroy(chunkData.Value.ChunkRenderer.gameObject);
        }

        ChunkDatas.Clear();
        StartCoroutine(GenerateChunks(false));
    }
}