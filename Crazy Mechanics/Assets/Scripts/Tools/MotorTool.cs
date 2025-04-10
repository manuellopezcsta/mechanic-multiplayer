using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorTool : BaseCounter
{
    [SerializeField] private bool motorFixed = false;
    [SerializeField] private ObjectsSO motorFixedSO;
    [SerializeField] private ObjectsSO fixingTool;

    [SerializeField] GameObject[] motorPieces;

    void Start()
    {
        HideMotor();
    }

    public override void Interact(Player player)
    {
        if (!motorFixed)
        {
            TryFixMotor(player);

        }
    }

    public void ShowMotor()
    {
        foreach (GameObject piece in motorPieces)
        {
            piece.SetActive(true);
        }
    }

    void HideMotor()
    {
        foreach (GameObject piece in motorPieces)
        {
            piece.SetActive(false);
        }

    }


    public void FinishFixing(){
        HideMotor();
        motorFixed = false;
    }

    private void TryFixMotor(Player player)
    {

        if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool)
        {
            Transform motorFixedPreFab = Instantiate(motorFixedSO.prefab);
            Destroy(GetCarObject().gameObject);
            ClearCarObject();
            motorFixedPreFab.transform.position = GetCarObjectFollowTransform().position;
            motorFixedPreFab.GetComponent<CarObject>().SetCarObjectParent(this);
            Debug.Log("Arreglo el motor");
            motorFixed = true;
        }
    }
}
