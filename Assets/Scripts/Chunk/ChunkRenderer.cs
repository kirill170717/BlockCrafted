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

        private List<Vector3> _verticies = new List<Vector3>();
        private List<int> _triangles = new List<int>();

        private void Start()
        {
            GenerateChunk();
        }

        private void GenerateChunk()
        {
            var mesh = new Mesh();

            for (int x = 0; x < CHUNK_WIDTH; x++)
            {
                for (int y = 0; y < CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < CHUNK_WIDTH; z++)
                    {
                        GenerateBlock(x, y, z);
                    }
                }
            }

            mesh.vertices = _verticies.ToArray();
            mesh.triangles = _triangles.ToArray();

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        private void GenerateBlock(int x, int y, int z)
        {
            var position = new Vector3Int(x, y, z);

            if (GetBlockAtPosition(position) == 0) return;

            if (GetBlockAtPosition(position + Vector3Int.left) == 0) GenerateSide(BlockSide.Left, position);
            if (GetBlockAtPosition(position + Vector3Int.right) == 0) GenerateSide(BlockSide.Right, position);
            if (GetBlockAtPosition(position + Vector3Int.forward) == 0) GenerateSide(BlockSide.Front, position);
            if (GetBlockAtPosition(position + Vector3Int.back) == 0) GenerateSide(BlockSide.Back, position);
            if (GetBlockAtPosition(position + Vector3Int.up) == 0) GenerateSide(BlockSide.Up, position);
            if (GetBlockAtPosition(position + Vector3Int.down) == 0) GenerateSide(BlockSide.Down, position);
        }

        private void GenerateSide(BlockSide side, Vector3Int blockPosition)
        {
            switch (side)
            {
                case BlockSide.Left:
                    _verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Right:
                    _verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Front:
                    _verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Back:
                    _verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Up:
                    _verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);
                    break;
                case BlockSide.Down:
                    _verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
                    _verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
                    break;
            }

            _triangles.Add(_verticies.Count - 4);
            _triangles.Add(_verticies.Count - 3);
            _triangles.Add(_verticies.Count - 2);

            _triangles.Add(_verticies.Count - 3);
            _triangles.Add(_verticies.Count - 1);
            _triangles.Add(_verticies.Count - 2);
        }

        private BlockType GetBlockAtPosition(Vector3Int position)
        {
            if (position.x >= 0 && position.x < CHUNK_WIDTH &&
                position.y >= 0 && position.y < CHUNK_HEIGHT &&
                position.z >= 0 && position.z < CHUNK_WIDTH)
            {
                return ChunkData.Blocks[position.x, position.y, position.z];
            }
            else
            {
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
}