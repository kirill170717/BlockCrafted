using UnityEngine;
using static Consts.Chunk;

namespace Chunk
{
    public static class ChunkExtensions
    {
        public static Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPosition) =>
            new(blockWorldPosition.x / CHUNK_WIDTH, blockWorldPosition.z / CHUNK_WIDTH);
    }
}