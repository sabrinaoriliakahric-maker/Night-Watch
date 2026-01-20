using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    private Vector3 input;

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.z = Input.GetAxis("Vertical");

        transform.position += input.normalized * speed * Time.deltaTime;
    }
}
