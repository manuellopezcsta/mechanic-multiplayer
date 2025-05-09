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
    void Start()
    {
        if (tool != null)
        {
            CreateTool();
        }
        rb= GetComponent<Rigidbody>();
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
        if(other.CompareTag("ExitMap")){
            Debug.Log(thrownBy.GetLastInteractDir());
            //transform.position = thrownBy.transform.position - (thrownBy.GetLastInteractDir() * 2);
            transform.position = spawn;
            rb.velocity = Vector3.zero;
            Debug.Log("Salio del mapa y se reinio la posicion");
        }
    }
}
