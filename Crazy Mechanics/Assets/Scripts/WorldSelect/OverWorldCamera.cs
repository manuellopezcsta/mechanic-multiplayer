using UnityEngine;

public class OverWorldCamera : MonoBehaviour
{
    const int OFFSET_X = 2;
    const int OFFSET_Y = -12;
    const int OFFSET_Z = 12;
    private Vector3 offset = new Vector3 (OFFSET_X,OFFSET_Y,OFFSET_Z);
    public  GameObject player;
    void Update()
    {
        transform.position = player.transform.position - offset; //Cambia posicion de la camara para seguir al personaje
    }
}
