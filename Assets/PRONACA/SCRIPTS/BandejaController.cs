using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BandejaController : MonoBehaviour
{
    [SerializeField] private int idBandeja = 0;
    [SerializeField] private Transform puntoDeSnap;
    [SerializeField] private bool bloquearAlColocar = true;
    [SerializeField] private Material materialColocada;
    [SerializeField] private Material materialLibre;

    private bool ocupada = false;
    private Renderer rend;
    private BandejaItem bandejaActual;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        var c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
        if (puntoDeSnap == null) puntoDeSnap = transform;
    }

    private void OnEnable()
    {
        ocupada = false;
        bandejaActual = null;
        if (rend && materialLibre) rend.material = materialLibre;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ocupada && bloquearAlColocar) return;
        if (!other.CompareTag("Bandeja")) return;

        var item = other.GetComponent<BandejaItem>();
        if (item == null) return;

        HacerSnap(item);
    }

    private void HacerSnap(BandejaItem item)
    {
        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        var colItem = item.GetComponent<Collider>();
        if (colItem) colItem.enabled = false;

        item.transform.SetParent(puntoDeSnap, true);
        item.transform.position = puntoDeSnap.position;
        item.transform.rotation = puntoDeSnap.rotation;

        ocupada = true;
        bandejaActual = item;

        if (rend && materialColocada) rend.material = materialColocada;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarBandeja(idBandeja, item.idObjeto);
        }
    }

    public void LiberarBandeja()
    {
        if (!ocupada || bandejaActual == null) return;

        var rb = bandejaActual.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        var colItem = bandejaActual.GetComponent<Collider>();
        if (colItem) colItem.enabled = true;

        bandejaActual.transform.SetParent(null, true);

        bandejaActual = null;
        ocupada = false;

        if (rend && materialLibre) rend.material = materialLibre;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DesregistrarBandeja(idBandeja);
        }
    }
}
