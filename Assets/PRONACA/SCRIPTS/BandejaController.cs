using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BandejaController : MonoBehaviour
{
    [Header("Identificaci�n del Slot")]
    [SerializeField] private int idBandeja = 0;

    [Header("Tiempo para adjuntar")]
    [SerializeField, Min(0f)] private float tiempoParaAdjuntar = 1.0f;

    [Header("Visual")]
    [SerializeField] private Material materialColocada;
    [SerializeField] private Material materialEnEspera;

    [Header("Snap")]
    [SerializeField] private Transform puntoDeSnap;
    [SerializeField] private bool bloquearAlColocar = true;

    private bool colocada = false;
    private Renderer rend;
    private Coroutine esperaCoroutine;
    private BandejaItem bandejaEnEspera;   
    private BandejaItem bandejaColocada;   

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        var trigger = GetComponent<Collider>();
        if (trigger) trigger.isTrigger = true;
        if (puntoDeSnap == null) puntoDeSnap = transform;
    }

    private void OnEnable()
    {
        colocada = false;
        bandejaEnEspera = null;
        bandejaColocada = null;
        if (rend && materialEnEspera) rend.material = materialEnEspera;
    }

    private void OnDisable()
    {
        CancelarTemporizador();
        colocada = false;
        bandejaEnEspera = null;
        bandejaColocada = null;
        if (rend && materialEnEspera) rend.material = materialEnEspera;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (colocada && bloquearAlColocar) return; 

        if (other.CompareTag("Bandeja"))
        {
            var item = other.GetComponent<BandejaItem>();
            if (item == null) return;

            if (bandejaEnEspera == null)
            {
                bandejaEnEspera = item;
                IniciarTemporizador();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (colocada && bloquearAlColocar) return;

        if (other.CompareTag("Bandeja"))
        {
            var item = other.GetComponent<BandejaItem>();
            if (item != null && item == bandejaEnEspera)
            {
                CancelarTemporizador();
                bandejaEnEspera = null;
            }
        }
    }

    private void IniciarTemporizador()
    {
        CancelarTemporizador();
        if (tiempoParaAdjuntar <= 0f)
        {
            AdjuntarBandeja(bandejaEnEspera);
        }
        else
        {
            esperaCoroutine = StartCoroutine(EsperarYAdjuntar(tiempoParaAdjuntar));
        }
    }

    private void CancelarTemporizador()
    {
        if (esperaCoroutine != null)
        {
            StopCoroutine(esperaCoroutine);
            esperaCoroutine = null;
        }
    }

    private IEnumerator EsperarYAdjuntar(float t)
    {
        float tiempo = 0f;
        while (tiempo < t)
        {
            if (bandejaEnEspera == null) yield break;
            tiempo += Time.deltaTime;
            yield return null;
        }

        AdjuntarBandeja(bandejaEnEspera);
    }

    private void AdjuntarBandeja(BandejaItem item)
    {
        if (item == null) return;

        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        var col = item.GetComponent<Collider>();
        if (col) col.enabled = false;

        item.transform.SetParent(puntoDeSnap, true);
        item.transform.position = puntoDeSnap.position;
        item.transform.rotation = puntoDeSnap.rotation;

        colocada = true;
        bandejaColocada = item;
        bandejaEnEspera = null;
        CancelarTemporizador();

        if (rend && materialColocada) rend.material = materialColocada;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarBandeja(idBandeja, item.idObjeto);
        }
        else
        {
            Debug.LogWarning("[BandejaController] No hay GameManager en la escena.");
        }
    }


    public void LiberarBandeja()
    {
        if (!colocada || bandejaColocada == null) return;

        var rb = bandejaColocada.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
        }
        var col = bandejaColocada.GetComponent<Collider>();
        if (col) col.enabled = true;

        bandejaColocada.transform.SetParent(null, true);
        bandejaColocada = null;
        colocada = false;
        if (rend && materialEnEspera) rend.material = materialEnEspera;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DesregistrarBandeja(idBandeja);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!bloquearAlColocar || !colocada) return;
    }
}
