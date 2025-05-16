using UnityEngine;
using UnityEngine.Analytics;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, mesh };// Modos de visualización
    public DrawMode drawMode;// Selección actual del modo

    const int mapChunkSize = 241;// Tamaño fijo del mapa (241x241, ideal para LOD por tema de divisiones)
    [Range(0,6)]
    public int levelOfDetail;// Nivel de detalle (simplificación de malla)
    public float noiseScale;// Escala del ruido Perlin (zoom)

    public int octaves;// Capas de ruido superpuestas
    [Range (0,1)]
    public float persistance;// Controla la influencia de cada octava
    public float lacunarity;// Aumento de detalle en cada octava

    public int seed;// Semilla para generación procedural
    public Vector2 offset;// Desplazamiento del mapa

    public float meshHeightMultiplier;// Escala vertical
    public AnimationCurve meshHeightCurve;// Curva para perfiles de altura

    public bool autoUpdate;//Regenerar automáticamente al editar en el Inspector

    public TerrainType[] regions;// Definición de biomas/colores por altura

    void Awake()
    {
        GenerateMap(); //El objeto se inicializa y genera el mapa al iniciar la escena.
    }

    public void GenerateMap()
    {
        //Genera el mapa de ruido(matriz de valores entre 0 y 1)
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance,lacunarity, offset);
        //Crea el mapa de colores basado en las regiones definidas
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                // Asigna un color según la altura y las regiones configuradas
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;//si no cortamos inmediatamente, cualquier numero de region menor a 1 y mayor a 0.3 podria cumplir con la condicion y se pintaria de todos los colores quedando en el ultimo del recorrido y seria un desastre.
                    }
                }
            }
        }
        //Visualiza el resultado según el modo seleccionado
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            // Muestra el ruido en escala de grises
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            // Muestra el mapa de colores de regiones
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.mesh)
        {
            // Genera y muestra el terreno 3D con textura
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }
    //Validación de Parámetros (se ejecuta al cambiar valores en el Inspector)
    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;// Lacunarity no puede ser < 1
        }
        if (octaves < 0)
        {
            octaves = 0;// Mínimo 0 octavas
        }
    }
}
[System.Serializable]
public struct TerrainType //para que el inspector lo reconozca como un objeto serializable y pueda ser editado desde el editor de unity.
{
    public string name;// Nombre del bioma (ej: "Agua", "Montaña")
    public float height;// Altura máxima para esta región (0 a 1)
    public Color color;// Color asociado a la región
}