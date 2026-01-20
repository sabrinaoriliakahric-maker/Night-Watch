using UnityEngine;
using static GameManager;

public class ShipController : MonoBehaviour
{
    public float speed = 5f;
    public float width;

    void Update()
    {
        if (GameManager.Instance.CurrentPhase != GamePhase.Ship) return;

        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GateFail"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
