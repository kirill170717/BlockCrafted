using Blocks.Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Blocks/BlockSettings", fileName = "BlockSettings")]
    public class BlockSettings : ScriptableObject
    {
        public BlockType BlockType;
        public Vector2 TextureOffset;
        public AudioClip StepSound;
        public float TimeToBreak;
    }
}