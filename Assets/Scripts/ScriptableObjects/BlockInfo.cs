using ChunkScripts.Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "SO's/BlocksInfo", fileName = "Block")]
    public class BlockInfo : ScriptableObject
    {
        public BlockType BlockType;
        public Vector2 PixelsOffset;
        public float TimeToBreak = 1f;
        public AudioClip StepSound;

        public virtual Vector2 GetPixelOffset(SideType sideType)
        {
            return PixelsOffset;
        }
    }
}