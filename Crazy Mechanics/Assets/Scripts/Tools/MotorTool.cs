using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class MotorTool : BaseCounter, IHasProgress
{

    [Header("SparkPlug Task Data")]
    //Task type 1

    [SerializeField] private Sprite iconSparkPlug;
    [SerializeField] private Sprite iconCricket;
    [SerializeField] private ObjectsSO fixingToolCricket;
    [SerializeField] private ObjectsSO sparkPlug;

    public static event EventHandler OnCricketUsed;

    [Header("Piston Task Data")]
    //Task type 2

    [SerializeField] private Sprite iconPiston;
    [SerializeField] private Sprite iconDrill;
    [SerializeField] private ObjectsSO fixingToolDrill;
    [SerializeField] private ObjectsSO piston;
    public static event EventHandler OnDrillUsed;

    [Header("Generated Task Values")]

    //Estos seran los objetos con los que trabajaremos en el codigo, los cuales se setearan en el start segun la tarea pertinente
    [SerializeField] private ObjectsSO fixingTool;
    [SerializeField] private ObjectsSO objectToFix;
    [SerializeField] private Sprite iconTool;
    [SerializeField] GameObject[] motorPiecesVisual;

    [SerializeField] private bool motorFixed = false;
    [SerializeField] private ObjectsSO motorFixedSO;



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
        // Elegimos que tarea va a contener el motor.
        ChooseRandomTaskForMotor();
    }

    void HideMotor()
    {
        foreach (GameObject piece in motorPiecesVisual)
        {
            piece.SetActive(false);
        }
    }



    //Reseteamos los valores para volver a comenzar, si se coloca otro motor;
    void ResetTaskValues(){
        fixingTool = null;
        objectToFix = null;
    }

    public void FinishFixing(){
        HideMotor();
        motorFixed = false;
        ResetTaskValues();
    }

    private void ChooseRandomTaskForMotor(){
        //Si sale 1 se realizara la tarea del sparkplug y si da 0 la del piston
        int randomTask = UnityEngine.Random.Range(0,2);

        // Seteamos los valores para el cricket o el drill.
        if(randomTask != 0){
            fixingTool = fixingToolCricket;
            objectToFix = sparkPlug;
            GetCarObject().gameObject.GetComponentInChildren<Image>().sprite = iconSparkPlug;
            iconTool = iconCricket;
        }else{
            fixingTool = fixingToolDrill;
            objectToFix = piston;
            GetCarObject().gameObject.GetComponentInChildren<Image>().sprite = iconPiston;
            iconTool = iconDrill;
        }
    }


    private void TryFixMotor(Player player)
    {
        if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == objectToFix)
        {
            // Le ponemos la bugia y la borramos visualmente.
            player.GetCarObject().DestroySelf();
            needsWhacking = true;
            fixingProgress = 0;
            GetCarObject().gameObject.GetComponentInChildren<Image>().sprite = iconTool;
        }

        if(player.HasCarObject() && player.GetCarObject().GetObjectSO() == fixingTool  && needsWhacking) {
            fixingProgress ++;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
            progressNormalized = (float) fixingProgress / fixingProgressMax
        });

        // Invocamos el evento de sonido que corresponde.
        if(fixingTool == fixingToolCricket) {
            OnCricketUsed?.Invoke(this, EventArgs.Empty);
        } else {
            OnDrillUsed?.Invoke(this, EventArgs.Empty);
        }

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
