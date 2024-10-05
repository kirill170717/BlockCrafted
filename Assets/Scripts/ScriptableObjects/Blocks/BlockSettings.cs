using Blocks.Enums;
using UnityEngine;

namespace ScriptableObjects.Blocks
{
    [CreateAssetMenu(menuName = "Blocks/BlockSettings", fileName = "BlockSettings")]
    public class BlockSettings : ScriptableObject
    {
        [field: SerializeField] public BlockType BlockType { get; private set; }
        [field: SerializeField] public AudioClip StepSound { get; private set; }
        [field: SerializeField] public float TimeToBreak { get; private set; }

        [SerializeField] private Vector2 _textureOffset;

        public virtual Vector2 GetTextureOffset(Vector3Int normal) => _textureOffset;
    }
}