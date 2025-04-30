using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static void CopyColliderValues(Collider source, Collider target)
    {
        if (source == null || target == null)
        {
            Debug.LogWarning("Source o Target no están asignados.");
            return;
        }

        // Copiar propiedades comunes
        target.isTrigger = source.isTrigger;
        target.material = source.material;

        // Copiar valores específicos según el tipo de Collider
        if (source is BoxCollider && target is BoxCollider)
        {
            BoxCollider sourceBox = (BoxCollider)source;
            BoxCollider targetBox = (BoxCollider)target;
            targetBox.center = sourceBox.center;
            targetBox.size = sourceBox.size;
        } else
        {
            Debug.LogWarning("Los tipos de Collider no son compatibles.");
        }
    }

    public static int GetOneOrMinusOne() {
        int output = Random.Range(0, 2) == 0 ? -1 : 1;
        return output;
    }

    public static Vector3 GetRandomDirection()
    {
        // Generar un vector aleatorio en el plano XZ
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        return randomDirection.normalized; // Normalizar para que tenga magnitud 1
    }

    public static List<int> GetShuffledIndexes(int size)
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < size; i++)
        {
            indexes.Add(i);
        }

        // Mezcla la lista
        for (int i = 0; i < indexes.Count; i++)
        {
            int randomIndex = Random.Range(0, indexes.Count);
            (indexes[i], indexes[randomIndex]) = (indexes[randomIndex], indexes[i]); // Intercambio de posiciones
        }

        return indexes;
    }


}
