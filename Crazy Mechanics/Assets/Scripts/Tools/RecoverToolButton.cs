using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverToolButton : BaseCounter
{
    // Hacemos que herede de base Counter para utilizar el interact.
    [SerializeField] CarObject[] targetTool; // Marcamos la tool x ej el taladro.
    [SerializeField] GameObject[] toolSpawner;
    [SerializeField] SpriteRenderer spriteSO;

    void Start()
    {
        spriteSO.sprite = targetTool[0].GetObjectSO().sprite;
    }

    public override void Interact(Player player)
    {
        ResetPositionTools();
    }

    private void MoveToolToSpawnPosition(GameObject target, int value)
    {
        // Basandonos en el objeto en si, tratamos de ver si se encuentra en un contenedor, de ser asi lo agarramos.
        // Parent seria counterTopPoint, y el parent de eso El InvisibleHolder
        bool hasAContainer = target.transform.parent.parent.TryGetComponent<InvisibleHolder>(out InvisibleHolder container);
        //Debug.Log(hasAContainer);

        // Hacemos un checkeo de null, xq en caso de que un player la tenga en su mano, esto retornaria null y no la queremos mover.
        if (hasAContainer)
        {
            GameObject holder = container.gameObject;
            holder.transform.SetParent(null);
            holder.transform.position = toolSpawner[value].transform.position;
        }
    }
    private void ResetPositionTools()
    {
        for (int i = 0; i < targetTool.Length; i++)
        {
            MoveToolToSpawnPosition(targetTool[i].gameObject, i);
        }
    }
}
