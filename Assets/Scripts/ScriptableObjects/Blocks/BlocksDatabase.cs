using System.Collections.Generic;
using Blocks.Enums;
using UnityEngine;

namespace ScriptableObjects.Blocks
{
    [CreateAssetMenu(menuName = "Blocks/BlockDatabase", fileName = "BlockDatabase")]
    public class BlocksDatabase : ScriptableObject
    {
        [SerializeField] private BlockSettings[] _blocksSettings;

        private readonly Dictionary<BlockType, BlockSettings> _blockSettingsMap = new();

        private void OnEnable()
        {
            _blockSettingsMap.Clear();

            foreach (var blockSettings in _blocksSettings)
            {
                _blockSettingsMap.Add(blockSettings.BlockType, blockSettings);
            }
        }

        public BlockSettings GetBlockSettings(BlockType blockType) => _blockSettingsMap.GetValueOrDefault(blockType);
    }
}