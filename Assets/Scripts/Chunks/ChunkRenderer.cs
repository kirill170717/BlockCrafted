using System;
using System.Collections.Generic;
using Blocks.Enums;
using UnityEngine;

namespace Chunks
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        public ChunkData ChunkData;

        private Mesh _mesh;

        private readonly List<Vector3> _verticies = new();
        private readonly List<Vector2> _uvs = new();
        private readonly List<int> _triangles = new();

        private void Start()
        {
            _mesh = new Mesh();

            RegenerateMesh();

            _meshFilter.mesh = _mesh;
            _meshCollider.sharedMesh = _mesh;
        }

        public void PlaceBlock(int blockId)
        {
            // ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Stone;
            RegenerateMesh();
        }

        public void RemoveBlock(int blockId)
        {
            // ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
            RegenerateMesh();
        }

        private void RegenerateMesh()
        {
            _verticies.Clear();
            _uvs.Clear();
            _triangles.Clear();

            for (var y = 0; y < Constants.Chunk.CHUNK_HEIGHT; y++)
            {
                for (var x = 0; x < Constants.Chunk.CHUNK_WIDTH; x++)
                {
                    for (var z = 0; z < Constants.Chunk.CHUNK_WIDTH; z++)
                    {
                        var blockPosition = new Vector3Int(x, y, z);
                        GenerateBlock(blockPosition);
                    }
                }
            }

            _mesh.triangles = Array.Empty<int>();
            _mesh.vertices = _verticies.ToArray();
            _mesh.uv = _uvs.ToArray();
            _mesh.triangles = _triangles.ToArray();

            _mesh.Optimize();
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        private void GenerateBlock(Vector3Int blockPosition)
        {
            if (GetBlockAtPosition(blockPosition) == BlockType.Air) return;

            var up = blockPosition + Vector3Int.up;
            var down = blockPosition + Vector3Int.down;
            var left = blockPosition + Vector3Int.left;
            var right = blockPosition + Vector3Int.right;
            var forward = blockPosition + Vector3Int.forward;
            var back = blockPosition + Vector3Int.back;

            var upBlock = GetBlockAtPosition(up);
            var downBlock = GetBlockAtPosition(down);
            var leftBlock = GetBlockAtPosition(left);
            var rightBlock = GetBlockAtPosition(right);
            var forwardBlock = GetBlockAtPosition(forward);
            var backBlock = GetBlockAtPosition(back);

            if (upBlock == BlockType.Air) GenerateBlockSide(Constants.Chunk.TopVerticies, blockPosition);

            if (downBlock == BlockType.Air) GenerateBlockSide(Constants.Chunk.BottomVerticies, blockPosition);

            if (leftBlock == BlockType.Air) GenerateBlockSide(Constants.Chunk.LeftVerticies, blockPosition);

            if (rightBlock == BlockType.Air) GenerateBlockSide(Constants.Chunk.RightVerticies, blockPosition);

            if (forwardBlock == BlockType.Air) GenerateBlockSide(Constants.Chunk.FrontVerticies, blockPosition);

            if (backBlock == BlockType.Air) GenerateBlockSide(Constants.Chunk.BackVerticies, blockPosition);
        }

        private BlockType GetBlockAtPosition(Vector3Int blockPosition)
        {
            if (blockPosition.x is >= 0 and < Constants.Chunk.CHUNK_WIDTH &&
                blockPosition.y is >= 0 and < Constants.Chunk.CHUNK_HEIGHT &&
                blockPosition.z is >= 0 and < Constants.Chunk.CHUNK_WIDTH)
            {
                return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }

            if (blockPosition.y is < 0 or >= Constants.Chunk.CHUNK_WIDTH) return BlockType.Air;

            var adjacentChunkPosition = ChunkData.ChunkPosition;

            if (blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += Constants.Chunk.CHUNK_WIDTH;
            }
            else if (blockPosition.x >= Constants.Chunk.CHUNK_WIDTH)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= Constants.Chunk.CHUNK_WIDTH;
            }

            if (blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += Constants.Chunk.CHUNK_WIDTH;
            }
            else if (blockPosition.z >= Constants.Chunk.CHUNK_WIDTH)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= Constants.Chunk.CHUNK_WIDTH;
            }

            return GameWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out var adjacentChunk)
                ? adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z]
                : BlockType.Air;
        }

        private void GenerateBlockSide(Vector3[] sideVerticies, Vector3Int blockPosition)
        {
            foreach (var vertex in sideVerticies)
            {
                var calculatedVertex = (vertex + blockPosition) * Constants.Block.BLOCK_SCALE;
                _verticies.Add(calculatedVertex);
            }

            AddLastVerticiesSquare();
        }

        private void AddLastVerticiesSquare()
        {
            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(0, 1));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(1, 1));

            _triangles.Add(_verticies.Count - 4);
            _triangles.Add(_verticies.Count - 3);
            _triangles.Add(_verticies.Count - 2);

            _triangles.Add(_verticies.Count - 3);
            _triangles.Add(_verticies.Count - 1);
            _triangles.Add(_verticies.Count - 2);
        }
    }
}