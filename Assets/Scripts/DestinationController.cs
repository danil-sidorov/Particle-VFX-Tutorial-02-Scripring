using UnityEngine;

public class DestinationController : MonoBehaviour
{

    public GameObject Destination;
    public float maxDistance = 6.0f;
    public float minDistance = 2.5f;
    public float speed = 3.0f;
    public float rotationSpeed = 50.0f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        Destination.transform.RotateAround(transform.position, Vector3.up, rotation * Time.deltaTime);
        float translation = Input.GetAxis("Vertical") * speed;
        if (
            (translation < 0 && Vector3.Distance(Destination.transform.position, transform.position) < minDistance) ||
            (translation > 0 && Vector3.Distance(Destination.transform.position, transform.position) > maxDistance)
        )
        {
            return;
        }
        translation *= Time.deltaTime;
        Vector3 direction = (Destination.transform.position - transform.position).normalized;
        Destination.transform.Translate(direction * translation, transform);
    }
}
