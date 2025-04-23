using UnityEngine;

public class OverWorldCamera : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 newPosition;

    public  GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        offset=player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //newPosition=transform.position;
        //newPosition.x=player.transform.position.x-offset.x;
        //newPosition.z=player.transform.position.z-offset.z;
        transform.position = player.transform.position - offset;
    }
}
