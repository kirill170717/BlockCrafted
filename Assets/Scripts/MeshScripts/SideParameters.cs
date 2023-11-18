using System.Collections.Generic;
using ChunkScripts.Enums;
using UnityEngine;

namespace MeshScripts
{
    public struct SideParameters
    {
        public BlockType BlockType;
        public BlockSide BlockSide;
        public SideType SideType;
        public Vector3Int BlockPosition;
        public List<GeneratedMeshVertex> Vertices;
    }
}