using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public static class Noise
{
    // genera un mapa de ruido 2D (utiliza array de matriz)
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        //mapWidth, mapHeight: Dimensiones del mapa.
        //seed: Semilla para generación reproducible.
        //scale: Factor de zoom / ampliación del ruido.
        //octaves: Número de capas de ruido a combinar.
        //persistance: Controla la disminución de amplitud en cada octava.
        //lacunarity: Controla el aumento de frecuencia en cada octava.
        //offset: Permite desplazar el mapa generado.

        //generador de números aleatorios con la semilla dada.
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x; //valores minimos para que no explote o no funcione en absoluto.
            float offsetY = prng.Next(-100000, 100000) + offset.y; //valores minimos para que no explote o no funcione en absoluto.
            octaveOffsets[i] = new Vector2(offsetX, offsetY); //Guarda un vector de offset por octava.
        }
        //se crea la matriz que contiene el ruido
        float[,] noiseMap = new float[mapWidth, mapHeight];
        if (scale <= 0)
        {  
            scale = 0.0001f; //para evitar division por 0.
        }
        //esto se usa para definir los maximos y minimos encontrados y actualizarlos al final. Si arrancara en 0 el minimo no podria tener numeros positivos ni el maximo tener negativos.
        float maxNoiseHeight = float.MinValue; //Variable para que el numero mas alto sea el mas pequeño que tiene unity.
        float minNoiseHeight = float.MaxValue; //Variable para que el numero mas bajo sea el mas grande que tiene unity.

        float halfWidth = mapWidth / 2f; //Calcula mitades de las dimensiones para centrar el ruido.
        float halfHeight = mapHeight / 2f; //igual que arriba, basicamente es para que el zoom ocurra desde el centro del mapa y no desde el costado derecho superior.

        //recorre cada posicion del mapa
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                float amplitude = 1; //Intensidad del ruido (disminuye en cada octava)
                float frequency = 1; //Detalle del ruido (aumenta en cada octava)
                float noiseHeight = 0; //Actual valor acumulado
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x; //Calcula coordenadas de muestreo (ajustadas por escala, frecuencia y offset)
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y; //Calcula coordenadas de muestreo (ajustadas por escala, frecuencia y offset)
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //Obtiene valor de Perlin Noise (rango original [0,1], convertido a [-1,1]) si fuera exclusivamente de 0 a 1 no podriamos tener lagos, sistemas de islas o lo que sea que este por debajo del nivel del mar.
                    noiseHeight += perlinValue * amplitude; // Acumula el valor aplicando la amplitud actual

                    amplitude *= persistance; //Actualiza amplitud para la siguiente octava
                    frequency *= lacunarity;  //Actualiza frecuencia para la siguiente octava
                }
                //Actualiza los valores máximos/minimos encontrados
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight; //Almacena el valor en el mapa de ruido
            }
        }
        //normalizar todos los valores al rango [0,1], sin esto no podriamos definir bien los tipos de terreno
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //mapear linealmente los valores
            }
        }
        return noiseMap;
    }
}
