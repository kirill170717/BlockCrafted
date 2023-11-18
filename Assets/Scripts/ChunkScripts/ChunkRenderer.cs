using ChunkScripts.Enums;
using MeshScripts;
using UnityEngine;
using UnityEngine.Rendering;

namespace ChunkScripts
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        public ChunkData ChunkData;

        private Mesh _chunkMesh;

        private static int[] _triangles;

        private void Awake()
        {
            _chunkMesh = new Mesh();
            _meshFilter.mesh = _chunkMesh;
        }

        public static void InitTriangles()
        {
            _triangles = new int[65536 * 6 / 4];

            var vertexNum = 4;

            for (int i = 0; i < _triangles.Length; i += 6)
            {
                _triangles[i] = vertexNum - 4;
                _triangles[i + 1] = vertexNum - 3;
                _triangles[i + 2] = vertexNum - 2;
                _triangles[i + 3] = vertexNum - 3;
                _triangles[i + 4] = vertexNum - 1;
                _triangles[i + 5] = vertexNum - 2;
                vertexNum += 4;
            }
        }

        public void SetMesh(GeneratedMeshData meshData)
        {
            var layout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.SNorm8, 4),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.UNorm16, 2)
            };

            _chunkMesh.SetVertexBufferParams(meshData.Vertices.Length, layout);
            _chunkMesh.SetVertexBufferData(meshData.Vertices, 0, 0, meshData.Vertices.Length, 0,
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices |
                MeshUpdateFlags.DontResetBoneBounds);
            
            var trianglesCount = meshData.Vertices.Length * 6 / 4;

            _chunkMesh.SetIndexBufferParams(trianglesCount, IndexFormat.UInt32);
            _chunkMesh.SetIndexBufferData(_triangles, 0, 0, trianglesCount,
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices |
                MeshUpdateFlags.DontResetBoneBounds);
            
            _chunkMesh.subMeshCount = 1;
            _chunkMesh.SetSubMesh(0, new SubMeshDescriptor(0, trianglesCount));
            _chunkMesh.bounds = meshData.Bounds;
            _meshCollider.sharedMesh = _chunkMesh;
        }

        public void SpawnBlock(int index) => ChunkData.Blocks[index] = BlockType.Stone;

        public void RemoveBlock(int index) => ChunkData.Blocks[index] = BlockType.Air;
    }
}