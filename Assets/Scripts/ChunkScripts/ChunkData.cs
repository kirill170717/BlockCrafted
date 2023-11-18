using ChunkScripts.Enums;
using UnityEngine;

namespace ChunkScripts
{
    public class ChunkData
    {
        public ChunkDataState State;
        public Vector2Int Position;
        public BlockType[] Blocks;
        public ChunkRenderer Renderer;
        public ChunkData LeftChunk;
        public ChunkData RightChunk;
        public ChunkData ForwardChunk;
        public ChunkData BackChunk;
    }
}