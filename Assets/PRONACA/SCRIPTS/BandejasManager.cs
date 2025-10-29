using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BandejasManager : MonoBehaviour
{
    [Header("Bandejas")]
    [SerializeField] private GameObject[] bandejas;

    [Header("Ubicaciones destino (pivotes)")]
    [SerializeField] private Transform[] pivotesFinales;

    [Header("Animación mezcla")]
    public float mixDuration = 3f;
    public float stepIntervalStart = 0.3f;
    public float stepIntervalEnd = 0.06f;

    [Header("Eventos")]
    public UnityEvent OnStep;
    public UnityEvent OnMixFinished;

    private Coroutine rutinaMezcla;
    private int[] asignacionFinal;

    public void HandleJuegoIniciado()
    {
        if (rutinaMezcla != null) StopCoroutine(rutinaMezcla);
        rutinaMezcla = StartCoroutine(SecuenciaInicio());
    }

    private void HandleJuegoTerminado()
    {
        ActivarBandejas();
    }

    private IEnumerator SecuenciaInicio()
    {
        if (!DatosValidos()) yield break;

        ActivarBandejas();
        GenerarAsignacionFinal();
        yield return StartCoroutine(MezclarBandejasTiempoReal());
        rutinaMezcla = null;
    }

    private bool DatosValidos()
    {
        if (bandejas == null || pivotesFinales == null) return false;
        if (bandejas.Length == 0) return false;
        if (pivotesFinales.Length != bandejas.Length) return false;
        for (int i = 0; i < bandejas.Length; i++)
            if (bandejas[i] == null || pivotesFinales[i] == null)
                return false;
        return true;
    }

    public void ActivarBandejas()
    {
        for (int i = 0; i < bandejas.Length; i++)
            bandejas[i].SetActive(true);
    }

    public void DesactivarBandejas()
    {
        for (int i = 0; i < bandejas.Length; i++)
            bandejas[i].SetActive(false);
    }

    private void GenerarAsignacionFinal()
    {
        int n = bandejas.Length;
        asignacionFinal = new int[n];

        for (int i = 0; i < n; i++)
            asignacionFinal[i] = i;

        for (int i = 0; i < n; i++)
        {
            int j = Random.Range(i, n);
            int tmp = asignacionFinal[i];
            asignacionFinal[i] = asignacionFinal[j];
            asignacionFinal[j] = tmp;
        }
    }

    public IEnumerator MezclarBandejasTiempoReal()
    {
        int n = bandejas.Length;

        int[] asignacionActual = new int[n];
        for (int i = 0; i < n; i++)
            asignacionActual[i] = i;

        float elapsed = 0f;

        while (elapsed < mixDuration)
        {
            int a = Random.Range(0, n);
            int b = Random.Range(0, n);
            while (b == a && n > 1)
                b = Random.Range(0, n);

            int tempIdx = asignacionActual[a];
            asignacionActual[a] = asignacionActual[b];
            asignacionActual[b] = tempIdx;

            float tNorm = Mathf.Clamp01(elapsed / mixDuration);
            float currentStepDuration = Mathf.Lerp(stepIntervalStart, stepIntervalEnd, tNorm);

            Vector3[] startPos = new Vector3[n];
            Quaternion[] startRot = new Quaternion[n];
            Vector3[] endPos = new Vector3[n];
            Quaternion[] endRot = new Quaternion[n];

            for (int i = 0; i < n; i++)
            {
                startPos[i] = bandejas[i].transform.position;
                startRot[i] = bandejas[i].transform.rotation;

                Transform destino = pivotesFinales[asignacionActual[i]];
                endPos[i] = destino.position;
                endRot[i] = destino.rotation;
            }

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(currentStepDuration, 0.0001f);
                float tt = Mathf.Clamp01(t);

                for (int i = 0; i < n; i++)
                {
                    bandejas[i].transform.position = Vector3.Lerp(startPos[i], endPos[i], tt);
                    bandejas[i].transform.rotation = Quaternion.Slerp(startRot[i], endRot[i], tt);
                }

                yield return null;
            }

            for (int i = 0; i < n; i++)
            {
                bandejas[i].transform.position = endPos[i];
                bandejas[i].transform.rotation = endRot[i];
            }

            OnStep?.Invoke();
            elapsed += currentStepDuration;
        }

        {
            Vector3[] startPos = new Vector3[n];
            Quaternion[] startRot = new Quaternion[n];
            Vector3[] endPos = new Vector3[n];
            Quaternion[] endRot = new Quaternion[n];

            for (int i = 0; i < n; i++)
            {
                startPos[i] = bandejas[i].transform.position;
                startRot[i] = bandejas[i].transform.rotation;

                int destinoIndex = asignacionFinal[i];
                Transform destino = pivotesFinales[destinoIndex];

                endPos[i] = destino.position;
                endRot[i] = destino.rotation;
            }

            float settleT = 0f;
            float settleDuration = stepIntervalEnd;
            while (settleT < 1f)
            {
                settleT += Time.deltaTime / Mathf.Max(settleDuration, 0.0001f);
                float tt = Mathf.Clamp01(settleT);

                for (int i = 0; i < n; i++)
                {
                    bandejas[i].transform.position = Vector3.Lerp(startPos[i], endPos[i], tt);
                    bandejas[i].transform.rotation = Quaternion.Slerp(startRot[i], endRot[i], tt);
                }

                yield return null;
            }

            for (int i = 0; i < n; i++)
            {
                bandejas[i].transform.position = endPos[i];
                bandejas[i].transform.rotation = endRot[i];
            }
        }

        OnMixFinished?.Invoke();
    }

}
