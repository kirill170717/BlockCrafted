using Chunk;
using UnityEngine;
using static Consts.Chunk;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        public Camera MainCamera;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                MainCamera = Camera.main;
            }
        }

        private void Update()
        {
            CheckMouseInput();
        }

        private void CheckMouseInput()
        {
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return;

            var isDestroying = Input.GetMouseButtonDown(0);
            var ray = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out var hit))
            {
                var center = hit.normal * (BLOCK_SCALE / 2);
                var blockCenter = isDestroying ? hit.point - center : hit.point + center;
                var blockWorldPosition = Vector3Int.FloorToInt(blockCenter / BLOCK_SCALE);
                var chunkPosition = ChunkExtensions.GetChunkContainingBlock(blockWorldPosition);

                if (!GameInitialization.Instance.Chunks.TryGetValue(chunkPosition, out var chunkData)) return;

                var chunkOrigin = new Vector3Int(chunkPosition.x, 0, chunkPosition.y) * CHUNK_WIDTH;

                if (isDestroying)
                {
                    chunkData.ChunkRenderer.RemoveBlock(blockWorldPosition - chunkOrigin);
                }
                else
                {
                    chunkData.ChunkRenderer.SpawnBlock(blockWorldPosition - chunkOrigin);
                }
            }
        }
    }
}