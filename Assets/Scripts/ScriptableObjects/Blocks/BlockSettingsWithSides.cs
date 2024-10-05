using UnityEngine;

namespace ScriptableObjects.Blocks
{
    [CreateAssetMenu(menuName = "Blocks/BlockSettingsWithSides", fileName = "BlockSettingsWithSides")]
    public class BlockSettingsWithSides : BlockSettings
    {
        [SerializeField] private Vector2 _textureOffsetUp;
        [SerializeField] private Vector2 _textureOffsetDown;
        [SerializeField] private Vector2 _textureOffsetLeft;
        [SerializeField] private Vector2 _textureOffsetRight;
        [SerializeField] private Vector2 _textureOffsetForward;
        [SerializeField] private Vector2 _textureOffsetBack;

        public override Vector2 GetTextureOffset(Vector3Int normal)
        {
            if (normal == Vector3Int.up) return _textureOffsetUp;
            if (normal == Vector3Int.down) return _textureOffsetDown;
            if (normal == Vector3Int.left) return _textureOffsetLeft;
            if (normal == Vector3Int.right) return _textureOffsetRight;
            if (normal == Vector3Int.forward) return _textureOffsetForward;
            if (normal == Vector3Int.back) return _textureOffsetBack;
            
            return base.GetTextureOffset(normal);
        }
    }
}