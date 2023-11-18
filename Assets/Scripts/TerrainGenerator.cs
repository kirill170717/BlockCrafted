using System;
using ChunkScripts.Enums;
using UnityEngine;
using static Consts.Chunk;

[CreateAssetMenu(menuName = "Terrain/TerrainGenerator", fileName = "Terrain")]
public class TerrainGenerator : ScriptableObject
{
    [Serializable]
    private class NoiseOctaveSetting
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    [SerializeField] private float _baseHeight = 128f;
    [SerializeField] private NoiseOctaveSetting[] _noiseOctaves;
    [SerializeField] private NoiseOctaveSetting _domainWarp;

    private FastNoiseLite[] _octaveNoises;
    private FastNoiseLite _warpNoise;

    public void InitNoises(int seed)
    {
        _warpNoise = new FastNoiseLite(seed);
        _warpNoise.SetNoiseType(_domainWarp.NoiseType);
        _warpNoise.SetFrequency(_domainWarp.Frequency);
        _warpNoise.SetDomainWarpAmp(_domainWarp.Amplitude);

        _octaveNoises = new FastNoiseLite[_noiseOctaves.Length];

        for (int i = 0; i < _noiseOctaves.Length; i++)
        {
            _octaveNoises[i] = new FastNoiseLite(seed);
            _octaveNoises[i].SetNoiseType(_noiseOctaves[i].NoiseType);
            _octaveNoises[i].SetFrequency(_noiseOctaves[i].Frequency);
        }
    }

    public BlockType[] GenerateTerrain(float xOffset, float zOffset)
    {
        var result = new BlockType[CHUNK_WIDTH * CHUNK_HEIGHT * CHUNK_WIDTH];

        for (int x = 0; x < CHUNK_WIDTH; x++)
        {
            for (int z = 0; z < CHUNK_WIDTH; z++)
            {
                var worldX = x * BLOCK_SCALE + xOffset;
                var worldZ = z * BLOCK_SCALE + zOffset;
                var height = GetHeight(worldX, worldZ);
                var bedrockLayerHeight = 1f + _octaveNoises[0].GetNoise(worldX, worldZ);

                for (int y = 0; y < height / BLOCK_SCALE; y++)
                {
                    var index = x + y * CHUNK_WIDTH_SQ + z * CHUNK_WIDTH;

                    if (height - y * BLOCK_SCALE < GRASS_LAYER_HEIGHT)
                    {
                        result[index] = BlockType.Grass;
                    }
                    else if (y * BLOCK_SCALE < bedrockLayerHeight)
                    {
                        result[index] = BlockType.Bedrock;
                    }
                    else
                    {
                        result[index] = BlockType.Dirt;
                    }
                }
            }
        }

        return result;
    }

    private float GetHeight(float x, float y)
    {
        _warpNoise.DomainWarp(ref x, ref y);
        var result = _baseHeight;

        for (int i = 0; i < _noiseOctaves.Length; i++)
        {
            var noise = _octaveNoises[i].GetNoise(x, y);
            result += noise * _noiseOctaves[i].Amplitude / 2;
        }

        return result;
    }
}