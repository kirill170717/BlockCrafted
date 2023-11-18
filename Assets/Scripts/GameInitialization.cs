using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChunkScripts;
using ChunkScripts.Enums;
using MeshScripts;
using Player;
using ScriptableObjects;
using UnityEngine;
using static Consts.Chunk;
using Random = UnityEngine.Random;

public class GameInitialization : MonoBehaviour
{
    public static GameInitialization Instance;

    public BlockDatabase Blocks;

    [Tooltip("Set WorldSeed to 0 if you want to use random Seed"), SerializeField]
    private int _worldSeed = 0;

    [SerializeField] private TerrainGenerator _terrainGenerator;
    [SerializeField] private ChunkRenderer _chunkPrefab;
    [SerializeField] private int _viewRadius = 8;
    public int MaxDistanceForInteraction = 4;

    public Dictionary<Vector2Int, ChunkData> Chunks = new();

    private Transform PlayerTransform => PlayerController.Instance.transform;

    private Vector2Int _currentPlayerChunk;
    private ConcurrentQueue<GeneratedMeshData> _meshingResults = new();

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
        if (_worldSeed == 0) _worldSeed = Random.Range(int.MinValue, int.MaxValue);

        ChunkRenderer.InitTriangles();
        _terrainGenerator.InitNoises(_worldSeed);
        StartCoroutine(Generate(false));
    }

    private void Update()
    {
        var playerWorldPosition = Vector3Int.FloorToInt(PlayerTransform.position / BLOCK_SCALE);
        var playerChunk = ChunkExtensions.GetChunkContainingBlock(playerWorldPosition);

        if (_currentPlayerChunk != playerChunk)
        {
            _currentPlayerChunk = playerChunk;
            StartCoroutine(Generate(true));
        }

        while (_meshingResults.TryDequeue(out var meshData))
        {
            var xPos = meshData.ChunkData.Position.x * CHUNK_WIDTH * BLOCK_SCALE;
            var zPos = meshData.ChunkData.Position.y * CHUNK_WIDTH * BLOCK_SCALE;
            var chunk = Instantiate(_chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
            chunk.ChunkData = meshData.ChunkData;
            chunk.SetMesh(meshData);
            meshData.ChunkData.Renderer = chunk;
            meshData.ChunkData.State = ChunkDataState.SpawnedInWorld;
        }
    }

    private IEnumerator Generate(bool wait)
    {
        var loadRadius = _viewRadius + 1;
        var center = _currentPlayerChunk;
        var loadingChunks = new List<ChunkData>();

        for (int x = center.x - loadRadius; x <= center.x + loadRadius; x++)
        {
            for (int y = center.y - loadRadius; y <= center.y + loadRadius; y++)
            {
                var chunkPosition = new Vector2Int(x, y);

                if (Chunks.ContainsKey(chunkPosition)) continue;

                loadingChunks.Add(LoadChunkAt(chunkPosition));

                if (wait) yield return null;
            }
        }

        yield return new WaitWhile(() =>
        {
            return loadingChunks.Any(data => data.State == ChunkDataState.StartedLoading);
        });

        for (int x = center.x - _viewRadius; x <= center.x + _viewRadius; x++)
        {
            for (int y = center.y - _viewRadius; y <= center.y + _viewRadius; y++)
            {
                var chunkPosition = new Vector2Int(x, y);
                var chunkData = Chunks[chunkPosition];

                if (chunkData.Renderer != null) continue;

                SpawnChunkRenderer(chunkData);

                if (wait) yield return null;
            }
        }
    }

    private ChunkData LoadChunkAt(Vector2Int chunkPosition)
    {
        var xPos = chunkPosition.x * CHUNK_WIDTH * BLOCK_SCALE;
        var yPos = chunkPosition.y * CHUNK_WIDTH * BLOCK_SCALE;
        var chunkData = new ChunkData
        {
            State = ChunkDataState.StartedLoading,
            Position = chunkPosition
        };
        Chunks.Add(chunkPosition, chunkData);

        Task.Factory.StartNew(() =>
        {
            chunkData.Blocks = _terrainGenerator.GenerateTerrain(xPos, yPos);
            chunkData.State = ChunkDataState.Loaded;
        });

        return chunkData;
    }

    private void SpawnChunkRenderer(ChunkData chunkData)
    {
        Chunks.TryGetValue(chunkData.Position + Vector2Int.left, out chunkData.LeftChunk);
        Chunks.TryGetValue(chunkData.Position + Vector2Int.right, out chunkData.RightChunk);
        Chunks.TryGetValue(chunkData.Position + Vector2Int.up, out chunkData.ForwardChunk);
        Chunks.TryGetValue(chunkData.Position + Vector2Int.down, out chunkData.BackChunk);

        chunkData.State = ChunkDataState.StartedMeshing;

        Task.Factory.StartNew(() =>
        {
            var meshData = MeshBuilder.GeneratedMesh(chunkData);
            _meshingResults.Enqueue(meshData);
        });
    }

    [ContextMenu("Regenerate terrain")]
    public void RegenerateTerrain()
    {
        _terrainGenerator.InitNoises(_worldSeed);

        foreach (var chunk in Chunks)
        {
            Destroy(chunk.Value.Renderer.gameObject);
        }

        Chunks.Clear();
        StartCoroutine(Generate(false));
    }
}