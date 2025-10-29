using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BandejaController : MonoBehaviour
{
    [Header("Identidad / Config")]
    [SerializeField] private int idBandeja = 0;
    [SerializeField] private Transform puntoDeSnap;
    [SerializeField] private bool bloquearAlColocar = true;

    [Header("Materiales")]
    [SerializeField] private Material materialEspera;
    [SerializeField] private Material materialColocada;
    [SerializeField] private Material materialCorrecto;
    [SerializeField] private Material materialIncorrecto;

    [Header("Runtime")]
    [SerializeField] private bool snapHabilitado = false;

    public bool ocupada = false;

    private MeshRenderer[] rends;
    private BandejaItem bandejaActual;
    private bool visualBloqueada = false;

    private void Awake()
    {
        rends = GetComponents<MeshRenderer>();

        var c = GetComponent<Collider>();
        if (c) c.isTrigger = true;

        if (puntoDeSnap == null)
            puntoDeSnap = transform;
    }

    private void OnEnable()
    {
        ocupada = false;
        bandejaActual = null;
        visualBloqueada = false;

        SetMaterial(materialEspera);

        snapHabilitado = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!snapHabilitado) return;
        if (!other.CompareTag("Bandeja")) return;

        var item = other.GetComponent<BandejaItem>();
        if (item == null) return;

        if (bloquearAlColocar && ocupada && bandejaActual != item) return;

        HacerSnap(item);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Bandeja")) return;

        var item = other.GetComponent<BandejaItem>();
        if (item == null) return;

        if (ocupada && bandejaActual == item)
        {
            LiberarBandeja();
        }
    }

    private void HacerSnap(BandejaItem item)
    {
        item.transform.SetParent(puntoDeSnap, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        ocupada = true;
        bandejaActual = item;

        if (!visualBloqueada)
            SetMaterial(materialColocada);

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
        visualBloqueada = false;

        SetMaterial(materialEspera);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DesregistrarBandeja(idBandeja);
        }
    }

    public void SetSnapHabilitado(bool value)
    {
        snapHabilitado = value;
    }

    public void ForzarDisponible()
    {
        ocupada = false;
        bandejaActual = null;
        visualBloqueada = false;

        SetMaterial(materialEspera);
    }

    public int GetIdBandeja()
    {
        return idBandeja;
    }

    public int GetIdObjetoActual()
    {
        if (bandejaActual == null) return -1;
        return bandejaActual.idObjeto;
    }

    public void SetVisualCorrecto()
    {
        visualBloqueada = true;
        SetMaterial(materialCorrecto);
    }

    public void SetVisualIncorrecto()
    {
        visualBloqueada = true;
        SetMaterial(materialIncorrecto);
    }

    public void SetVisualColocada()
    {
        if (visualBloqueada) return;
        SetMaterial(materialColocada);
    }

    public void SetVisualEspera()
    {
        if (visualBloqueada) return;
        SetMaterial(materialEspera);
    }

    private void SetMaterial(Material m)
    {
        if (m == null || rends == null) return;
        for (int i = 0; i < rends.Length; i++)
        {
            if (rends[i] != null)
                rends[i].material = m;
        }
    }
}
