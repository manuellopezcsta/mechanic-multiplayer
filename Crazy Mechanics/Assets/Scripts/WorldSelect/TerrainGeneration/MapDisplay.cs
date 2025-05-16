using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    //dibuja una textura 2D (mapa de ruido o mapa de colores)
    // - Mostrar el mapa de ruido en escala de grises
    // - Mostrar el mapa de colores de las regiones
    // No genera la textura, solo la muestra
    public void DrawTexture(Texture2D texture)
    {
        // Asigna la textura generada al material del Renderer
        // Usamos sharedMaterial para afectar todas las instancias del material
        textureRender.sharedMaterial.mainTexture = texture;
        // Ajusta la escala del objeto para que coincida con las dimensiones de la textura
        // - Ancho (X): según el ancho de la textura
        // - Altura (Y): 1 (porque es un plano)
        // - Profundidad (Z): según la altura de la textura
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
    //dibuja una malla 3D (mesh) con una textura
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.mesh = meshData.CreateMesh();// Crea y asigna la malla a partir de los datos generados
        meshRenderer.sharedMaterial.mainTexture = texture;// Aplica la textura de colores al material del terreno 3D
    }
}
