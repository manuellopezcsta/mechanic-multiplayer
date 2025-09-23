using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleHolder : BaseCounter
{
    [SerializeField] private CarObject tool;
    public Player thrownBy;
    public bool flying;
    private Rigidbody rb;
    [SerializeField] private Vector3 spawn;
    private bool inTutorial = false;
    const string EXIT_MAP_TAG = "ExitMap";
    void Start()
    {
        if (tool != null)
        {
            CreateTool();
            //inTutorial es true cuando esta activado el script de tutorial y el objeto es Aceite
            inTutorial = TutorialManagerOilChange.Instance != null && tool.GetObjectSO().name == "Aceite";
        }
        rb = GetComponent<Rigidbody>();
        spawn = GameManager.Instance.GetLostAndFoundPosition().position;
        
    }
    void Update()
    {
        if(rb.velocity == Vector3.zero && flying){
            flying = false;
        }

    }
    public override void Interact(Player player)
    {
        // There is a car obj here already.
        if (player.HasCarObject())
        {
            // Player is carrying something
        }
        else
        {
            // Player is not carrying anything.
            GetCarObject().SetCarObjectParent(player);
            if (inTutorial)
            {
                TutorialManagerOilChange.Instance.StateChange(TutorialManagerOilChange.StateTutorial.OilSpawner, TutorialManagerOilChange.StateTutorial.Task);
            }
            //Borramos el objeto invisible.
                Destroy(gameObject);
        }
    }

    public void FixColliderSize()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        Tools.CopyColliderValues(GetCarObject().GetComponent<BoxCollider>(), collider);

        collider.enabled = true;

    }

    public void CreateTool()
    {
        SetCarObject(tool);
    }

    public IEnumerator SkipLaunchWindow(){
        flying = false;
        yield return new WaitForSeconds(0.1f);
        flying = true;
    }
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(EXIT_MAP_TAG)){
            //Debug.Log(thrownBy.GetLastInteractDir());
            //transform.position = thrownBy.transform.position - (thrownBy.GetLastInteractDir() * 2);
            transform.position = spawn;
            rb.velocity = Vector3.zero;
            Debug.Log("Salio del mapa y se reinio la posicion");
        }
    }
}
