using ChunkScripts.Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "SO's/BlocksInfoSides", fileName = "BlockSides")]
    public class BlockInfoSides : BlockInfo
    {
        public Vector2 PixelsOffsetUp;
        public Vector2 PixelsOffsetDown;

        public override Vector2 GetPixelOffset(SideType sideType)
        {
            return sideType switch
            {
                SideType.Up => PixelsOffsetUp,
                SideType.Down => PixelsOffsetDown,
                _ => base.GetPixelOffset(sideType)
            };
        }
    }
}