using UnityEngine;
using static Consts.Chunk;

namespace ChunkScripts
{
    public static class ChunkExtensions
    {
        public static Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPosition)
        {
            var chunkPosition = new Vector2((float)blockWorldPosition.x / CHUNK_WIDTH,
                (float)blockWorldPosition.z / CHUNK_WIDTH);
            return Vector2Int.FloorToInt(chunkPosition);
        }
    }
}