using System.Collections;
using System.Collections.Generic;
using Chunk;
using Player;
using UnityEngine;
using static Consts.Chunk;

public class GameInitialization : MonoBehaviour
{
    public static GameInitialization Instance;

    [SerializeField] private TerrainGenerator _terrainGenerator;
    [SerializeField] private ChunkRenderer _chunkPrefab;
    [SerializeField] private int _viewRadius = 8;

    public Dictionary<Vector2Int, ChunkData> Chunks = new();

    private Vector2Int _currentPlayerChunk;
    private Transform _playerTransform => PlayerController.Instance.transform;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(Generate(false));
    }

    private void Update()
    {
        var playerWorldPosition = Vector3Int.FloorToInt(_playerTransform.position / BLOCK_SCALE);
        var playerChunk = ChunkExtensions.GetChunkContainingBlock(playerWorldPosition);

        if (_currentPlayerChunk == playerChunk) return;

        _currentPlayerChunk = playerChunk;
        
        StartCoroutine(Generate(true));
    }

    private IEnumerator Generate(bool wait)
    {
        for (int x = _currentPlayerChunk.x - _viewRadius; x < _currentPlayerChunk.x + _viewRadius; x++)
        {
            for (int y = _currentPlayerChunk.y - _viewRadius; y < _currentPlayerChunk.y + _viewRadius; y++)
            {
                var chunkPosition = new Vector2Int(x, y);
                
                if (Chunks.ContainsKey(chunkPosition)) continue;

                LoadChunk(chunkPosition);
                
                if(wait)
                {
                    yield return new WaitForSecondsRealtime(0.2f);
                }
            }
        }
    }

    private void LoadChunk(Vector2Int chunkPosition)
    {
        var xPos = (int)(chunkPosition.x * CHUNK_WIDTH * BLOCK_SCALE);
        var yPos = (int)(chunkPosition.y * CHUNK_WIDTH * BLOCK_SCALE);
        var blocks = _terrainGenerator.GenerateTerrain(xPos, yPos);
        
        var data = new ChunkData
        {
            Position = chunkPosition,
            Blocks = blocks
        };

        Chunks.Add(chunkPosition, data);
        var chunk = Instantiate(_chunkPrefab, new Vector3(xPos, 0, yPos), Quaternion.identity, transform);
        chunk.ChunkData = data;
        chunk.GameInitialization = this;
        data.ChunkRenderer = chunk;
    }
    
    [ContextMenu("Regenerate terrain")]
    public void RegenerateTerrain()
    {
        _terrainGenerator.InitNoises();
        
        foreach (var chunk in Chunks)
        {
            Destroy(chunk.Value.ChunkRenderer.gameObject);
        }
        
        Chunks.Clear();
        StartCoroutine(Generate(false));
    }
}