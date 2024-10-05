using Blocks.Enums;
using UnityEngine;

namespace Chunks
{
    public class ChunkData
    {
        public Vector2Int ChunkPosition;
        public ChunkRenderer ChunkRenderer;
        public BlockType[,,] Blocks;
    }
}