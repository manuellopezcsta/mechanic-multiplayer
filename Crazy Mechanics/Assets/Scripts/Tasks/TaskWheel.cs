using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskWheel : BaseCounter
{
    [SerializeField] private ObjectsSO balancedWheel;
    [SerializeField] private CarObject wheel;

    [SerializeField] private bool taskComplete;

    private void Start() {
        SetCarObject(wheel);
        int random = Random.Range(0,2);
        if(random == 1){
           SetCarObject(wheel);
           wheel.transform.position = transform.parent.gameObject.transform.position;
        }else{
            ClearCarObject();
            wheel.gameObject.SetActive(false);
            Destroy(wheel.gameObject);
        }
        transform.parent.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
    public override void Interact(Player player)
    {
        Debug.Log("EntroAlInteract");
        // Logica para dejar objetos
        if (!HasCarObject()) {
            // There is no obj here and check if they are the same object
            if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == balancedWheel) {
                // El player tiene algo en la mano
                player.GetCarObject().SetCarObjectParent(this);
                taskComplete = true;
            } else {
                // Player no tiene nada en la mano

            }
        } else {
            if(!player.HasCarObject() && !taskComplete) {
                GetCarObject().SetCarObjectParent(player);
            } 
        }
    }
}
