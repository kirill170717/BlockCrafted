using System;
using System.Collections.Generic;
using Chunk.Enums;
using UnityEngine;
using static Consts.Chunk;

namespace Chunk
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        public ChunkData ChunkData;
        public GameInitialization GameInitialization;

        private Mesh _chunkMesh;

        private List<int> _triangles = new();
        private List<Vector2> _uvs = new();
        private List<Vector3> _vertices = new();

        private void Start()
        {
            _chunkMesh = new Mesh();
            RegenerateMesh();
            _meshFilter.mesh = _chunkMesh;
        }

        private void RegenerateMesh()
        {
            _vertices.Clear();
            _uvs.Clear();
            _triangles.Clear();

            for (int x = 0; x < CHUNK_WIDTH; x++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < CHUNK_WIDTH; z++)
                    {
                        GenerateBlock(new Vector3Int(x, y, z));
                    }
                }
            }

            _chunkMesh.triangles = Array.Empty<int>();
            _chunkMesh.vertices = _vertices.ToArray();
            _chunkMesh.uv = _uvs.ToArray();
            _chunkMesh.triangles = _triangles.ToArray();

            _chunkMesh.Optimize();
            _chunkMesh.RecalculateNormals();
            _chunkMesh.RecalculateBounds();

            _meshCollider.sharedMesh = _chunkMesh;
        }

        public void SpawnBlock(Vector3Int position)
        {
            ChunkData.Blocks[position.x, position.y, position.z] = BlockType.Stone;
            RegenerateMesh();
        }

        public void RemoveBlock(Vector3Int position)
        {
            ChunkData.Blocks[position.x, position.y, position.z] = BlockType.Air;
            RegenerateMesh();
        }

        private void GenerateBlock(Vector3Int position)
        {
            if (GetBlockAtPosition(position) == 0) return;

            GenerateSide(position, Vector3Int.left, BlockSide.Left);
            GenerateSide(position, Vector3Int.right, BlockSide.Right);
            GenerateSide(position, Vector3Int.forward, BlockSide.Front);
            GenerateSide(position, Vector3Int.back, BlockSide.Back);
            GenerateSide(position, Vector3Int.up, BlockSide.Up);
            GenerateSide(position, Vector3Int.down, BlockSide.Down);
        }

        private void GenerateSide(Vector3Int position, Vector3Int direction, BlockSide blockSide)
        {
            if (GetBlockAtPosition(position + direction) != 0) return;

            GenerateSideVertices(blockSide, position);
            AddUVs(GetBlockAtPosition(position), GetSideType(blockSide));
        }

        private SideType GetSideType(BlockSide blockSide)
        {
            var sideType = blockSide switch
            {
                BlockSide.Up => SideType.Up,
                BlockSide.Down => SideType.Down,
                _ => SideType.Side
            };
            return sideType;
        }

        private void GenerateSideVertices(BlockSide side, Vector3Int blockPosition)
        {
            switch (side)
            {
                case BlockSide.Left:
                    _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Right:
                    _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Front:
                    _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Back:
                    _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Up:
                    _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Down:
                    _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
                    break;
            }

            AddLastVerticesSquare();
        }

        private void AddLastVerticesSquare()
        {
            _triangles.Add(_vertices.Count - 4);
            _triangles.Add(_vertices.Count - 3);
            _triangles.Add(_vertices.Count - 2);
            _triangles.Add(_vertices.Count - 3);
            _triangles.Add(_vertices.Count - 1);
            _triangles.Add(_vertices.Count - 2);
        }

        private void AddUVs(BlockType blockType, SideType sideType)
        {
            var uv = GetBlockUV(blockType, sideType);

            for (int i = 0; i < 4; i++)
            {
                _uvs.Add(uv);
            }
        }

        private Vector2 GetBlockUV(BlockType blockType, SideType sideType)
        {
            return blockType switch
            {
                BlockType.Grass => sideType switch
                {
                    SideType.Up => new Vector2(0f / 256f, 240f / 256f),
                    SideType.Down => new Vector2(32f / 256f, 240f / 256f),
                    _ => new Vector2(48f / 256f, 240f / 256f)
                },
                BlockType.Dirt => new Vector2(32f / 256f, 240f / 256f),
                BlockType.Gravel => new Vector2(48f / 256f, 224f / 256f),
                BlockType.Stone => new Vector2(16f / 256f, 240f / 256f),
                BlockType.Bedrock => new Vector2(16f/256f, 224f/256f),
                _ => new Vector2(160f / 256f, 224f / 256f)
            };
        }

        private BlockType GetBlockAtPosition(Vector3Int position)
        {
            if (position.x >= 0 && position.x < CHUNK_WIDTH &&
                position.y >= 0 && position.y < CHUNK_HEIGHT &&
                position.z >= 0 && position.z < CHUNK_WIDTH)
            {
                return ChunkData.Blocks[position.x, position.y, position.z];
            }

            if (position.y < 0 || position.y >= CHUNK_HEIGHT) return BlockType.Air;

            var adjacentChunkPosition = ChunkData.Position;

            if (position.x < 0)
            {
                adjacentChunkPosition.x--;
                position.x += CHUNK_WIDTH;
            }
            else if (position.x >= CHUNK_WIDTH)
            {
                adjacentChunkPosition.x++;
                position.x -= CHUNK_WIDTH;
            }

            if (position.z < 0)
            {
                adjacentChunkPosition.y--;
                position.z += CHUNK_WIDTH;
            }
            else if (position.z >= CHUNK_WIDTH)
            {
                adjacentChunkPosition.y++;
                position.z -= CHUNK_WIDTH;
            }

            if (GameInitialization.Chunks.TryGetValue(adjacentChunkPosition, out var adjacentChunk))
            {
                return adjacentChunk.Blocks[position.x, position.y, position.z];
            }
            else
            {
                return BlockType.Air;
            }
        }
    }
}