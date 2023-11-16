using System;
using Chunk.Enums;
using UnityEngine;
using static Consts.Chunk;

public class TerrainGenerator : MonoBehaviour
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
    
    private void Awake()
    {
        InitNoises();
    }

    public void InitNoises()
    {
        _warpNoise = new FastNoiseLite();
        _warpNoise.SetNoiseType(_domainWarp.NoiseType);
        _warpNoise.SetFrequency(_domainWarp.Frequency);
        _warpNoise.SetDomainWarpAmp(_domainWarp.Amplitude);

        _octaveNoises = new FastNoiseLite[_noiseOctaves.Length];
        
        for (int i = 0; i < _noiseOctaves.Length; i++)
        {
            _octaveNoises[i] = new FastNoiseLite();
            _octaveNoises[i].SetNoiseType(_noiseOctaves[i].NoiseType);
            _octaveNoises[i].SetFrequency(_noiseOctaves[i].Frequency);
        }
    }

    public BlockType[,,] GenerateTerrain(int xOffset, int yOffset)
    {
        var fastNoise = new FastNoiseLite();
        var result = new BlockType[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_WIDTH];

        for (int x = 0; x < CHUNK_WIDTH; x++)
        {
            for (int z = 0; z < CHUNK_WIDTH; z++)
            {
                var height = GetHeight((x * BLOCK_SCALE + xOffset), (z * BLOCK_SCALE + yOffset));
                
                for (int y = 0; y < height / BLOCK_SCALE; y++)
                {
                    if (height - y * BLOCK_SCALE < GRASS_LAYER_HEIGHT)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                    else if(y * BLOCK_SCALE < BEDROCK_LAYER_HEIGHT)
                    {
                        result[x, y, z] = BlockType.Bedrock;
                    }else
                    {
                        result[x, y, z] = BlockType.Dirt;
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