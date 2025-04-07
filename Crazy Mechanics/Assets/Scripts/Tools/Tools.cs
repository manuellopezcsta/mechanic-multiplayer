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
}
