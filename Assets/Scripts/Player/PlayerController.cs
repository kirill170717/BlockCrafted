using Input;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        public Camera MainCamera;

        // private GameInitialization GameInit => GameInitialization.Instance;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            MainCamera = Camera.main;
        }

        private void Update()
        {
            var breakBlock = InputSystem.InputControls.GameActionMap.BreakBlock.IsPressed();
            var placeBlock = InputSystem.InputControls.GameActionMap.PlaceBlock.IsPressed();
            CheckMouseInput(breakBlock, placeBlock);
        }

        private void CheckMouseInput(bool breakBlock, bool placeBlock)
        {
            // if (!breakBlock && !placeBlock) return;
            //
            // var ray = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            //
            // if (!Physics.Raycast(ray, out var hit)) return;
            //
            // var center = hit.normal * (Consts.Chunk.BLOCK_SCALE / 2);
            // var blockCenter = breakBlock ? hit.point - center : hit.point + center;
            // var blockWorldPosition = Vector3Int.FloorToInt(blockCenter / Consts.Chunk.BLOCK_SCALE);
            // var playerPosition = Vector3Int.FloorToInt(transform.position);
            //
            // if (Vector3Int.Distance(playerPosition, blockWorldPosition) > GameInit.MaxDistanceForInteraction) return;
            //
            // var chunkPosition = ChunkExtensions.GetChunkContainingBlock(blockWorldPosition);
            //
            // if (!GameInit.Chunks.TryGetValue(chunkPosition, out var chunkData)) return;
            //
            // var chunkOrigin = new Vector3Int(chunkPosition.x, 0, chunkPosition.y) * Consts.Chunk.CHUNK_WIDTH;
            // var blockPosition = blockWorldPosition - chunkOrigin;
            // var index = blockPosition.x + blockPosition.y * Consts.Chunk.CHUNK_WIDTH_SQ + blockPosition.z * Consts.Chunk.CHUNK_WIDTH;
            //
            // if (breakBlock) chunkData.Renderer.RemoveBlock(index);
            //
            // if (placeBlock) chunkData.Renderer.SpawnBlock(index);
        }
    }
}