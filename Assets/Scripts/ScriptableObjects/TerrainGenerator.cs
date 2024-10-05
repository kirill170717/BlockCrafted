using UnityEngine;
using Blocks.Enums;
using ScriptableObjects.Noises;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Biomes/TerrainGenerator", fileName = "TerrainGenerator")]
    public class TerrainGenerator : ScriptableObject
    {
        [SerializeField] private float _baseHeight = 8f;
        [SerializeField] private NoiseOctaveSettings[] _octaves;
        [SerializeField] private NoiseOctaveSettings _domainWarp;

        private FastNoiseLite[] _octavesNoises;
        private FastNoiseLite _warpNoise;

        public void InitializationNoises(int worldSeed)
        {
            _octavesNoises = new FastNoiseLite[_octaves.Length];

            for (var i = 0; i < _octaves.Length; i++)
            {
                _octavesNoises[i] = new FastNoiseLite(worldSeed);
                _octavesNoises[i].SetNoiseType(_octaves[i].NoiseType);
                _octavesNoises[i].SetFrequency(_octaves[i].Frequency);
            }

            _warpNoise = new FastNoiseLite(worldSeed);
            _warpNoise.SetNoiseType(_domainWarp.NoiseType);
            _warpNoise.SetFrequency(_domainWarp.Frequency);
            _warpNoise.SetDomainWarpAmp(_domainWarp.Amplitude);
        }

        public BlockType[,,] GenerateTerrain(float xOffset, float zOffset)
        {
            if (_octavesNoises.Length == 0)
            {
                Debug.LogError("Noises are not initialized!!!");
                return null;
            }

            var result = new BlockType
            [
                Constants.Chunk.CHUNK_WIDTH,
                Constants.Chunk.CHUNK_HEIGHT,
                Constants.Chunk.CHUNK_WIDTH
            ];

            for (var x = 0; x < Constants.Chunk.CHUNK_WIDTH; x++)
            {
                for (var z = 0; z < Constants.Chunk.CHUNK_WIDTH; z++)
                {
                    var height = GetHeight(x * Constants.Block.BLOCK_SCALE + xOffset,
                        z * Constants.Block.BLOCK_SCALE + zOffset);
                    var calculatedHeight = height / Constants.Block.BLOCK_SCALE;

                    for (var y = 0; y < calculatedHeight; y++)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                }
            }

            return result;
        }

        private float GetHeight(float x, float y)
        {
            _warpNoise.DomainWarp(ref x, ref y);
            var result = _baseHeight;

            for (var i = 0; i < _octaves.Length; i++)
            {
                var noise = _octavesNoises[i].GetNoise(x, y);
                result += noise * _octaves[i].Amplitude / 2;
            }

            return result;
        }
    }
}