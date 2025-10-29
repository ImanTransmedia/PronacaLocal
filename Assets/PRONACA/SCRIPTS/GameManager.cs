using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración")]
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

    private readonly Dictionary<int, int> registroColocaciones = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void IniciarJuego()
    {
        if (estadoActual == GameState.Salido) return;
        bandejasColocadas = 0;
        registroColocaciones.Clear();
        estadoActual = GameState.EnCurso;
        OnJuegoIniciado?.Invoke();
        Debug.Log("[GameManager] Juego iniciado.");
    }


    public void RegistrarBandeja(int idSlot, int idObjeto)
    {
        if (estadoActual != GameState.EnCurso) return;

        bool esNueva = !registroColocaciones.ContainsKey(idSlot);
        registroColocaciones[idSlot] = idObjeto;

        if (esNueva)
        {
            bandejasColocadas++;
            Debug.Log($"[GameManager] Colocada: Slot {idSlot} <- Objeto {idObjeto} | Total: {bandejasColocadas}/{totalBandejas}");

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
        else
        {
            Debug.Log($"[GameManager] Slot {idSlot} actualizado con Objeto {idObjeto} (reemplazo).");
        }
    }


    public void DesregistrarBandeja(int idSlot)
    {
        if (registroColocaciones.Remove(idSlot))
        {
            bandejasColocadas = Mathf.Max(0, bandejasColocadas - 1);
            Debug.Log($"[GameManager] Bandeja retirada del Slot {idSlot}. Total: {bandejasColocadas}/{totalBandejas}");
        }
    }


    public bool VerificarBandejas()
    {
        if (bandejasColocadas < totalBandejas)
        {
            Debug.LogWarning("[GameManager] Aún no están todas colocadas, no se verifica.");
            return false;
        }

        foreach (var par in registroColocaciones)
        {
            int idSlot = par.Key;
            int idObjeto = par.Value;
            if (idSlot != idObjeto)
            {
                Debug.Log($"[GameManager] Mismatch: Slot {idSlot} != Objeto {idObjeto}");
                return false;
            }
        }

        Debug.Log("[GameManager] Todas las bandejas coinciden correctamente.");
        return true;
    }

    public void TerminarJuego(bool exito)
    {
        if (estadoActual == GameState.Terminado) return;
        estadoActual = GameState.Terminado;
        OnJuegoTerminado?.Invoke();
        Debug.Log($"[GameManager] Juego terminado. Éxito: {exito}");
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
}
