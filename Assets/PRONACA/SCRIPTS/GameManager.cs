using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuracion")]
    [SerializeField] private int totalBandejas = 0;
    [SerializeField] private bool terminarAutoAlCompletar = true;

    public enum GameState { Idle, EnCurso, Terminado, Salido }
    [SerializeField] private GameState estadoActual = GameState.Idle;

    [Header("Progreso")]
    [SerializeField] private int bandejasColocadas = 0;

    [Header("Eventos")]
    public UnityEvent OnJuegoIniciado;
    public UnityEvent OnTodasBandejasColocadas;
    public UnityEvent OnJuegoTerminado;
    public UnityEvent OnJuegoReiniciado;
    public UnityEvent OnJuegoSalido;

    [Header("Referencias runtime")]
    [SerializeField] private BandejaController[] bandejaControllers;

    [Header("UI Resultado")]
    [SerializeField] private ResultadoUIController resultadoUI;

    private readonly Dictionary<int, int> registroColocaciones = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SetSnapEnTodas(false);
    }

    public void IniciarJuego()
    {
        if (estadoActual == GameState.Salido) return;

        bandejasColocadas = 0;
        registroColocaciones.Clear();
        estadoActual = GameState.EnCurso;

        if (resultadoUI != null)
            resultadoUI.Ocultar();

        OnJuegoIniciado?.Invoke();
    }

    public void PrepararRonda()
    {
        if (bandejaControllers != null)
        {
            for (int i = 0; i < bandejaControllers.Length; i++)
            {
                var bc = bandejaControllers[i];
                if (bc == null) continue;
                bc.ForzarDisponible();
            }
        }

        SetSnapEnTodas(true);
    }

    private void SetSnapEnTodas(bool habilitar)
    {
        if (bandejaControllers == null) return;

        for (int i = 0; i < bandejaControllers.Length; i++)
        {
            var bc = bandejaControllers[i];
            if (bc == null) continue;
            bc.SetSnapHabilitado(habilitar);
        }
    }

    public void RegistrarBandeja(int idSlot, int idObjeto)
    {
        if (estadoActual != GameState.EnCurso) return;

        bool esNueva = !registroColocaciones.ContainsKey(idSlot);
        registroColocaciones[idSlot] = idObjeto;

        if (esNueva)
        {
            bandejasColocadas++;

            if (bandejasColocadas >= totalBandejas)
            {
                OnTodasBandejasColocadas?.Invoke();

                if (terminarAutoAlCompletar)
                {
                    bool todoCorrecto = VerificarBandejas();
                    TerminarJuego(todoCorrecto);
                }
            }
        }
    }

    public void DesregistrarBandeja(int idSlot)
    {
        if (registroColocaciones.Remove(idSlot))
        {
            bandejasColocadas = Mathf.Max(0, bandejasColocadas - 1);
        }
    }

    public bool VerificarBandejas()
    {
        if (bandejasColocadas < totalBandejas)
        {
            return false;
        }

        foreach (var par in registroColocaciones)
        {
            int idSlot = par.Key;
            int idObjeto = par.Value;
            if (idSlot != idObjeto)
            {
                return false;
            }
        }

        return true;
    }

    public void TerminarJuego(bool exito)
    {
        if (estadoActual == GameState.Terminado) return;
        estadoActual = GameState.Terminado;

        OnJuegoTerminado?.Invoke();

        EvaluarBandejasVisual();

        if (resultadoUI != null)
        {
            if (exito)
            {
                resultadoUI.MostrarExito();
            }
            else
            {
                resultadoUI.MostrarFallo();
            }
        }
    }

    public void ReiniciarJuego()
    {
        estadoActual = GameState.Idle;
        OnJuegoReiniciado?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SalirJuego()
    {
        estadoActual = GameState.Salido;
        OnJuegoSalido?.Invoke();
        Application.Quit();
    }

    public void EvaluarBandejasVisual()
    {
        if (bandejaControllers == null) return;

        for (int i = 0; i < bandejaControllers.Length; i++)
        {
            var bc = bandejaControllers[i];
            if (bc == null) continue;

            int slotId = bc.GetIdBandeja();
            int objId = bc.GetIdObjetoActual();

            if (objId < 0) continue;

            if (slotId == objId)
            {
                bc.SetVisualCorrecto();
            }
            else
            {
                bc.SetVisualIncorrecto();
            }
        }
    }

    public void SetRenderersBandejas(bool activo)
    {
        if (bandejaControllers == null) return;

        for (int i = 0; i < bandejaControllers.Length; i++)
        {
            var bc = bandejaControllers[i];
            if (bc == null) continue;

            MeshRenderer[] rends = bc.gameObject.GetComponents<MeshRenderer>();
            for (int r = 0; r < rends.Length; r++)
            {
                if (rends[r] != null)
                {
                    rends[r].enabled = activo;
                }
            }
        }
    }

    public void ActivarRenderersBandejas()
    {
        SetRenderersBandejas(true);
    }

    public void DesactivarRenderersBandejas()
    {
        SetRenderersBandejas(false);
    }
}
