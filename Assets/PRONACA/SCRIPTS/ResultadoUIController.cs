using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultadoUIController : MonoBehaviour
{
    [Header("Refs UI")]
    [SerializeField] private GameObject panelResultado;
    [SerializeField] private TMP_Text titulo;
    [SerializeField] private TMP_Text subtitulo;
    [SerializeField] private Image imagenColor;

    [Header("Apariencia")]
    [SerializeField] private Color colorExito = Color.green;
    [SerializeField] private Color colorFallo = Color.red;
    [SerializeField] private string tituloExito = "Lo lograste";
    [SerializeField] private string subtituloExito = "Vuelve a jugar cuando quieras";
    [SerializeField] private string tituloFallo = "Estuviste cerca";
    [SerializeField] private string subtituloFallo = "Vuelve a intentarlo";

    private void Awake()
    {
        if (panelResultado != null)
            panelResultado.SetActive(false);
    }

    public void MostrarExito()
    {
        if (panelResultado != null) panelResultado.SetActive(true);
        if (titulo != null) titulo.text = tituloExito;
        if (subtitulo != null) subtitulo.text = subtituloExito;
        if (imagenColor != null) imagenColor.color = colorExito;
    }

    public void MostrarFallo()
    {
        if (panelResultado != null) panelResultado.SetActive(true);
        if (titulo != null) titulo.text = tituloFallo;
        if (subtitulo != null) subtitulo.text = subtituloFallo;
        if (imagenColor != null) imagenColor.color = colorFallo;
    }

    public void Ocultar()
    {
        if (panelResultado != null)
            panelResultado.SetActive(false);
    }
}
