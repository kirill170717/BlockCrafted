using System.Collections.Generic;
using ChunkScripts.Enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "SO's/BLockDatabase", fileName = "BlockDatabase")]
    public class BlockDatabase : ScriptableObject
    {
        [SerializeField] private BlockInfo[] _blockInfos;

        private Dictionary<BlockType, BlockInfo> _blocksCached = new();

        private void Init()
        {
            _blocksCached.Clear();
            
            foreach (var blockInfo in _blockInfos)
            {
                _blocksCached.Add(blockInfo.BlockType, blockInfo);
            }
        }

        public BlockInfo GetBlockInfo(BlockType blockType)
        {
            if(_blocksCached.Count == 0) Init();
            
            return _blocksCached.TryGetValue(blockType, out var blockInfo) ? blockInfo : null;
        }
    }
}