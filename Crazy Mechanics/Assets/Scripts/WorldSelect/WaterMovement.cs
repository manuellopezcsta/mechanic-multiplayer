using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    public float minY = 3.8f;
    public float maxY = 4.1f;
    public float speed = 0.1f;

    private float targetY;

    // Start is called before the first frame update
    void Start()
    {
        targetY = maxY; // Start by moving towards the maximum height
    }

    // Update is called once per frame
    void Update()
    {
        // Get current position
        Vector3 currentPosition = transform.position;

        // Calculate new Y position
        float newY = Mathf.MoveTowards(currentPosition.y, targetY, speed * Time.deltaTime);

        // Update GameObject's position
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

        // Check if the targetY has been reached and switch target
        if (Mathf.Approximately(currentPosition.y, targetY))
        {
            if (targetY == maxY)
            {
                targetY = minY;
            }
            else
            {
                targetY = maxY;
            }
        }
    }
}