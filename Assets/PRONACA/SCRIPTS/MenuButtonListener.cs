using UnityEngine;

public class MenuButtonListener : MonoBehaviour
{

    public GameObject menuInicio;
    public GameObject Button1;
    public GameObject Button2;
    public bool hideButtons = false;

    void Update()
    {
        if (hideButtons)
        {
            Button1.SetActive(false);
            Button2.SetActive(false);
        }

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            OnMenuButtonPressed();
        }
    }

    private void OnMenuButtonPressed()
    {
        Debug.Log("Menu button pressed");
        if (menuInicio != null)
        {
            bool isActive = menuInicio.activeSelf;
            menuInicio.SetActive(!isActive);
        }
    }
}
