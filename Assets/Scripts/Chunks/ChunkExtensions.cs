using UnityEngine;

namespace Chunks
{
    public static class ChunkExtensions
    {
        public static Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPosition)
        {
            var chunkPosition = new Vector2Int
            (
                blockWorldPosition.x / Constants.Chunk.CHUNK_WIDTH,
                blockWorldPosition.z / Constants.Chunk.CHUNK_WIDTH
            );

            if (blockWorldPosition.x < 0) chunkPosition.x--;

            if (blockWorldPosition.y < 0) chunkPosition.y--;
            
            return chunkPosition;
        }
    }
}