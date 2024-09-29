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

        private readonly List<Vector3> verticies = new();
        private readonly List<int> triangles = new();

        private void Start()
        {
            var mesh = new Mesh();

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

            mesh.vertices = verticies.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
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
                verticies.Add((vertex + blockPosition) * Constants.Block.BLOCK_SCALE);
            }

            AddLastVerticiesSquare();
        }

        private void AddLastVerticiesSquare()
        {
            triangles.Add(verticies.Count - 4);
            triangles.Add(verticies.Count - 3);
            triangles.Add(verticies.Count - 2);

            triangles.Add(verticies.Count - 3);
            triangles.Add(verticies.Count - 1);
            triangles.Add(verticies.Count - 2);
        }
    }
}