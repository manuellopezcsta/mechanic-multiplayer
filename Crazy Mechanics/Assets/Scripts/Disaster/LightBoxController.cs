using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBoxController : BaseCounter
{
    [SerializeField] ObjectsSO fixingTool;
    [SerializeField] bool isPowerDown = false;


    public void CutDownPower() {
        isPowerDown = true;
        // Alguna animacion x aca ? ..
    }

    public bool IsPowerDown() {
        return isPowerDown;
    }

    public override void Interact(Player player)
    {
        // Si el player esta holdeando la fixing tool
        if(player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool && isPowerDown) {
            Debug.Log("Se empezo a arreglar la luz");
            StartCoroutine(TimeToRequest(5));
        }
    }

    IEnumerator TimeToRequest(float timeRequest)
    {
        yield return new WaitForSeconds(timeRequest);
        isPowerDown = false;
        Debug.Log("Luz Arreglada");
    }
}
