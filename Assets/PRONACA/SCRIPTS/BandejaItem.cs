using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BandejaItem : MonoBehaviour
{
    public int idObjeto = 0;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = false;

        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        gameObject.tag = "Bandeja";
    }
}
