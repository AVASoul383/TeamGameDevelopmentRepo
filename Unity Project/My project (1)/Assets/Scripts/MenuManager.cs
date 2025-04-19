using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;

    GameObject[] buttons;
    int buttonPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        setActiveMenu(mainMenu);
        menuActive.SetActive(true);
        buttons = GameObject.FindGameObjectsWithTag("Menu Button");
        buttonPosition = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu Down") && buttonPosition < buttons.Length) 
        {
            buttonPosition++;
        }

    }

    public void main()
    {
        menuActive.SetActive(false);
        setActiveMenu(mainMenu);
        menuActive.SetActive(true);
    }

    public void options()
    {
        menuActive.SetActive(false);
        setActiveMenu(optionsMenu);
        menuActive.SetActive(true);
    }

    public void setActiveMenu(GameObject menu)
    {
        menuActive = menu;
        menuActive.SetActive(true);
    }
}
