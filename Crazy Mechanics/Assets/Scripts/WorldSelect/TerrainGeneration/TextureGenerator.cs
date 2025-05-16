using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    //crea una textura a partir de un mapa de colores.
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height); // Crea una textura con las dimensiones especificadas.
        texture.filterMode = FilterMode.Point;// Configura el modo de filtrado para evitar borrosidad (píxeles definidos).
        texture.wrapMode = TextureWrapMode.Clamp;// Configura cómo se manejan los bordes de la textura (sin repetir).
        texture.SetPixels(colorMap);// Asigna los colores a los píxeles de la textura.
        texture.Apply();
        return texture;
    }
    //crea una textura a partir de heightMap.
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        // Obtiene las dimensiones del array de heightMap.
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // Crea un array para almacenar los colores de cada píxel.
        Color[] colorMap = new Color[width * height];
        // Recorre todo el heightMap para convertirlo a colores (solo escala de grises).
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        // Usa el método anterior para crear la textura a partir del mapa de colores.
        return TextureFromColorMap(colorMap, width, height);
    }
}
