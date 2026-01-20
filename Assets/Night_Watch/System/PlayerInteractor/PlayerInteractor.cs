using System;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float radius = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    void Interact()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
                break;
            }
        }
    }
}

internal class Interactable
{
    internal void Interact()
    {
        throw new NotImplementedException();
    }
}