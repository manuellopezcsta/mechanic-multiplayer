using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MotorTool : BaseCounter, IHasProgress
{
    [SerializeField] private bool motorFixed = false;
    [SerializeField] private ObjectsSO motorFixedSO;
    [SerializeField] private ObjectsSO fixingTool;
    [SerializeField] private ObjectsSO sparkPlug;
    [SerializeField] GameObject[] motorPiecesVisual;

    // Cosas para la visual de cuando le pegas.
    bool needsWhacking = false;
    private int fixingProgress;
    [SerializeField] private int fixingProgressMax;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

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
        foreach (GameObject piece in motorPiecesVisual)
        {
            piece.SetActive(true);
        }
    }

    void HideMotor()
    {
        foreach (GameObject piece in motorPiecesVisual)
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
        if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == sparkPlug)
        {
            // Le ponemos la bugia y la borramos visualmente.
            player.GetCarObject().DestroySelf();
            needsWhacking = true;
            fixingProgress = 0;
        }

        if(player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool  && needsWhacking) {
            fixingProgress ++;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
            progressNormalized = (float) fixingProgress / fixingProgressMax
        });

        if(fixingProgress == fixingProgressMax ) {
            // Se termino el arreglo.
            GetCarObject().DestroySelf();
            // Spawneamos el nuevo
            Transform motorFixedPreFab = Instantiate(motorFixedSO.prefab);
            motorFixedPreFab.transform.position = GetCarObjectFollowTransform().position;
            motorFixedPreFab.GetComponent<CarObject>().SetCarObjectParent(this);
            Debug.Log("Arreglo el motor");
            // Seteamos las variables.
            motorFixed = true;
            needsWhacking = false;
        }
        }
    }
}
