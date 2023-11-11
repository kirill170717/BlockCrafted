using Chunk.Enums;
using UnityEngine;

namespace Chunk
{
    public class ChunkData
    {
        public Vector2Int Position;
        public BlockType[,,] Blocks;
        public ChunkRenderer ChunkRenderer;
    }
}