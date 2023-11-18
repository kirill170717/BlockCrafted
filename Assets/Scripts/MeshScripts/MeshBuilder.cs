using System.Collections.Generic;
using ChunkScripts;
using ChunkScripts.Enums;
using UnityEngine;
using static Consts.Chunk;

namespace MeshScripts
{
    public static class MeshBuilder
    {
        public static GeneratedMeshData GeneratedMesh(ChunkData chunkData)
        {
            var vertices = new List<GeneratedMeshVertex>();
            var maxY = 0;

            for (int y = 0; y < CHUNK_HEIGHT; y++)
            {
                for (int x = 0; x < CHUNK_WIDTH; x++)
                {
                    for (int z = 0; z < CHUNK_WIDTH; z++)
                    {
                        var isGenerated = GenerateBlock(new Vector3Int(x, y, z), vertices, chunkData);

                        if (isGenerated && maxY < y) maxY = y;
                    }
                }
            }

            var boundsSize = new Vector3(CHUNK_WIDTH, maxY, CHUNK_WIDTH) * BLOCK_SCALE;
            var mesh = new GeneratedMeshData
            {
                Vertices = vertices.ToArray(),
                Bounds = new Bounds(boundsSize / 2, boundsSize),
                ChunkData = chunkData
            };
            return mesh;
        }

        private static bool GenerateBlock(Vector3Int position, List<GeneratedMeshVertex> vertices, ChunkData chunkData)
        {
            var blockType = GetBlockAtPosition(position, chunkData);

            if (blockType == BlockType.Air) return false;

            var parameters = new SideParameters
            {
                BlockType = blockType,
                BlockSide = BlockSide.Left,
                SideType = SideType.Side,
                BlockPosition = position,
                Vertices = vertices,
            };
            GenerateSide(parameters, chunkData);
            parameters.BlockSide = BlockSide.Right;
            GenerateSide(parameters, chunkData);
            parameters.BlockSide = BlockSide.Front;
            GenerateSide(parameters, chunkData);
            parameters.BlockSide = BlockSide.Back;
            GenerateSide(parameters, chunkData);
            parameters.BlockSide = BlockSide.Up;
            parameters.SideType = SideType.Up;
            GenerateSide(parameters, chunkData);
            parameters.BlockSide = BlockSide.Down;
            parameters.SideType = SideType.Down;
            GenerateSide(parameters, chunkData);
            return true;
        }

        private static void GenerateSide(SideParameters parameters, ChunkData chunkData)
        {
            var direction = GetDirection(parameters.BlockSide);
            var blockTypeAtPosition = GetBlockAtPosition(parameters.BlockPosition + direction, chunkData);
            var isBlock = blockTypeAtPosition != BlockType.Air;
            var isBottomOfWorld = parameters.BlockSide == BlockSide.Down && parameters.BlockPosition.y == 0;

            if (isBlock || isBottomOfWorld) return;

            GenerateSideVertices(parameters);
        }

        private static Vector3Int GetDirection(BlockSide side)
        {
            return side switch
            {
                BlockSide.Left => Vector3Int.left,
                BlockSide.Right => Vector3Int.right,
                BlockSide.Front => Vector3Int.forward,
                BlockSide.Back => Vector3Int.back,
                BlockSide.Up => Vector3Int.up,
                BlockSide.Down => Vector3Int.down,
                _ => Vector3Int.zero
            };
        }

        private static void GenerateSideVertices(SideParameters parameters)
        {
            var vertex = new GeneratedMeshVertex
            {
                NormalX = sbyte.MaxValue,
                NormalY = 0,
                NormalZ = 0,
                NormalW = 1
            };
            GetUVs(parameters.BlockType, parameters.SideType, out vertex.UVx, out vertex.UVy);

            switch (parameters.BlockSide)
            {
                case BlockSide.Left:
                    vertex.Position = (new Vector3(0, 0, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(0, 0, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(0, 1, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(0, 1, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    break;
                case BlockSide.Right:
                    vertex.Position = (new Vector3(1, 0, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 1, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 0, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 1, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    break;
                case BlockSide.Front:
                    vertex.Position = (new Vector3(0, 0, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 0, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(0, 1, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 1, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    break;
                case BlockSide.Back:
                    vertex.Position = (new Vector3(0, 0, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(0, 1, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 0, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 1, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    break;
                case BlockSide.Up:
                    vertex.Position = (new Vector3(0, 1, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(0, 1, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 1, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 1, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    break;
                case BlockSide.Down:
                    vertex.Position = (new Vector3(0, 0, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 0, 0) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(0, 0, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    vertex.Position = (new Vector3(1, 0, 1) + parameters.BlockPosition) * BLOCK_SCALE;
                    parameters.Vertices.Add(vertex);
                    break;
            }
        }

        private static void GetUVs(BlockType blockType, SideType sideType, out ushort x, out ushort y)
        {
            x = 0;
            y = 0;

            switch (blockType)
            {
                case BlockType.Grass:
                    switch (sideType)
                    {
                        case SideType.Up:
                            x = 0 * 256;
                            y = 240 * 256;
                            break;
                        case SideType.Down:
                            x = 32 * 256;
                            y = 240 * 256;
                            break;
                        case SideType.Side:
                            x = 48 * 256;
                            y = 240 * 256;
                            break;
                    }

                    break;
                case BlockType.Dirt:
                    x = 32 * 256;
                    y = 240 * 256;
                    break;
                case BlockType.Gravel:
                    x = 48 * 256;
                    y = 224 * 256;
                    break;
                case BlockType.Stone:
                    x = 16 * 256;
                    y = 240 * 256;
                    break;
                case BlockType.Bedrock:
                    x = 16 * 256;
                    y = 224 * 256;
                    break;
                default:
                    x = 160 * 256;
                    y = 224 * 256;
                    break;
            }
        }

        private static BlockType GetBlockAtPosition(Vector3Int blockPosition, ChunkData chunkData)
        {
            int index;

            if (blockPosition.x >= 0 && blockPosition.x < CHUNK_WIDTH && blockPosition.y >= 0 &&
                blockPosition.y < CHUNK_HEIGHT && blockPosition.z >= 0 && blockPosition.z < CHUNK_WIDTH)
            {
                index = blockPosition.x + blockPosition.y * CHUNK_WIDTH_SQ + blockPosition.z * CHUNK_WIDTH;
                return chunkData.Blocks[index];
            }

            if (blockPosition.y < 0 || blockPosition.y >= CHUNK_HEIGHT) return BlockType.Air;

            if (blockPosition.x < 0)
            {
                if (chunkData.LeftChunk == null) return BlockType.Air;

                blockPosition.x += CHUNK_WIDTH;
                index = blockPosition.x + blockPosition.y * CHUNK_WIDTH_SQ + blockPosition.z * CHUNK_WIDTH;
                return chunkData.LeftChunk.Blocks[index];
            }

            if (blockPosition.x >= CHUNK_WIDTH)
            {
                if (chunkData.RightChunk == null) return BlockType.Air;

                blockPosition.x -= CHUNK_WIDTH;
                index = blockPosition.x + blockPosition.y * CHUNK_WIDTH_SQ + blockPosition.z * CHUNK_WIDTH;
                return chunkData.RightChunk.Blocks[index];
            }

            if (blockPosition.z < 0)
            {
                if (chunkData.BackChunk == null) return BlockType.Air;

                blockPosition.z += CHUNK_WIDTH;
                index = blockPosition.x + blockPosition.y * CHUNK_WIDTH_SQ + blockPosition.z * CHUNK_WIDTH;
                return chunkData.BackChunk.Blocks[index];
            }

            if (blockPosition.z >= CHUNK_WIDTH)
            {
                if (chunkData.ForwardChunk == null) return BlockType.Air;

                blockPosition.z -= CHUNK_WIDTH;
                index = blockPosition.x + blockPosition.y * CHUNK_WIDTH_SQ + blockPosition.z * CHUNK_WIDTH;
                return chunkData.ForwardChunk.Blocks[index];
            }

            return BlockType.Air;
        }
    }
}