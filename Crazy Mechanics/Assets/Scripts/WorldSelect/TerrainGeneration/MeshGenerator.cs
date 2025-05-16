using UnityEngine;

public static class MeshGenerator
{
    //genera los datos de la malla
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        //heightMap: Matriz 2D con valores de altura (proveniente del Noise.cs).
        //heightMultiplier: Factor para escalar la altura del terreno.
        //heightCurve: Curva para ajustar el perfil de alturas.
        //levelOfDetail(LOD): Controla la densidad de la malla.

        //Obtiene dimensiones del heightMap.
        int width = heightMap.GetLength(0);//Le pedimos el ancho o la cantidad de columnas dentro de la matriz de heightMap para recorrerla correctamente.
        int height = heightMap.GetLength(1);//Le pedimos el alto o la cantidad de filas dentro de la matriz de heightMap para recorrerla correctamente.
        //Calcula la posici�n de la esquina superior izquierda para centrar el mesh.
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2; //Calculo del LOD.
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1; //Calcula cu�ntos v�rtices tendr� cada l�nea de la malla.
        //Crea estructura para almacenar los datos de la malla.
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0; //lleva la cuenta de los v�rtices procesados.

        //recorre el heightMap seg�n el incremento del LOD
        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                //Posici�n XZ basada en su ubicaci�n en el grid
                //Posici�n Y(altura) obtenida del heightMap
                //heightCurve.Evaluate(): Ajusta la distribuci�n de alturas
                //heightMultiplier: Escala la altura
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);//Coordenadas UV(para texturizado) normalizadas[0 - 1]

                if (x < width - 1 && y < height - 1)
                {
                    //Crea 2 tri�ngulos por cada cuadrado del grid:
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                //Incrementa el �ndice del v�rtice procesado
                vertexIndex++;
            }
        }

        return meshData;

    }
}
//clase auxiliar almacena y organiza los datos de la malla:
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight]; //Posiciones 3D de los v�rtices
        uvs = new Vector2[meshWidth * meshHeight]; //Coordenadas de textura
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6]; //�ndices que definen los tri�ngulos
    }

    public void AddTriangle(int a, int b, int c)
    {
        //A�ade 3 v�rtices para formar un tri�ngulo
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3; //lleva la cuenta de la posici�n actual en el array
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;//Asigna v�rtices
        mesh.triangles = triangles;//Asigna tri�ngulos
        mesh.uv = uvs;//Asigna coordenadas UV
        mesh.RecalculateNormals();//Recalcula normales(para iluminaci�n correcta)
        return mesh;
    }

}