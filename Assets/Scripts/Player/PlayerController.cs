using Chunks;
using Input;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public Camera MainCamera { get; private set; }

        [SerializeField] private int _maxDistanceForInteraction = 5;

        private readonly Vector3 _centerPosition = new(0.5f, 0.5f);

        private void Update()
        {
            var breakBlock = InputSystem.InputControls.GameActionMap.RemoveBlock.IsPressed();
            var placeBlock = InputSystem.InputControls.GameActionMap.PlaceBlock.IsPressed();
            CheckMouseInput(breakBlock, placeBlock);
        }

        private void CheckMouseInput(bool breakBlock, bool placeBlock)
        {
            if (!breakBlock && !placeBlock) return;

            if (GetBlockIndex(breakBlock, out var chunkData, out var index)) return;

            if (placeBlock)
            {
                chunkData.ChunkRenderer.PlaceBlock(index);
            }
            else if (breakBlock)
            {
                chunkData.ChunkRenderer.RemoveBlock(index);
            }
        }

        private bool GetBlockIndex(bool breakBlock, out ChunkData chunkData, out int index)
        {
            var ray = MainCamera.ViewportPointToRay(_centerPosition);

            if (!Physics.Raycast(ray, out var hit))
            {
                chunkData = null;
                index = -1;
                return true;
            }

            var blockWorldPosition = GetBlockWorldPosition(breakBlock, hit);
            var playerPosition = Vector3Int.FloorToInt(transform.position);

            if (Vector3Int.Distance(playerPosition, blockWorldPosition) > _maxDistanceForInteraction)
            {
                chunkData = null;
                index = -1;
                return true;
            }

            var chunkPosition = ChunkExtensions.GetChunkContainingBlock(blockWorldPosition);

            if (!GameWorld.ChunkDatas.TryGetValue(chunkPosition, out chunkData))
            {
                index = -1;
                return true;
            }

            var chunkOrigin = new Vector3Int(chunkPosition.x, 0, chunkPosition.y) * Constants.Chunk.CHUNK_WIDTH;
            var blockPosition = blockWorldPosition - chunkOrigin;
            index = blockPosition.x + blockPosition.y * Constants.Chunk.CHUNK_WIDTH_SQ +
                    blockPosition.z * Constants.Chunk.CHUNK_WIDTH;
            return false;
        }

        private Vector3Int GetBlockWorldPosition(bool breakBlock, RaycastHit hit)
        {
            var center = hit.normal * (Constants.Block.BLOCK_SCALE / 2);
            var blockCenter = breakBlock ? hit.point - center : hit.point + center;
            var blockWorldPosition = Vector3Int.FloorToInt(blockCenter / Constants.Block.BLOCK_SCALE);
            return blockWorldPosition;
        }
    }
}