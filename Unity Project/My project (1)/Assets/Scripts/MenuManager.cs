using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;

    GameObject prevMenu;
    GameObject[] buttons;
    int buttonPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        
        setActiveMenu(mainMenu);
        menuActive.SetActive(true);
        findButtons();
        buttonPosition = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu Down") && buttonPosition < buttons.Length - 1) 
        {
            buttonPosition++;
        }
        else if(Input.GetButtonDown("Menu Up") && buttonPosition > 0)
        {
            buttonPosition--;
        }

        if (Input.GetButtonDown("Submit") && menuActive != null)
        {
            buttons[buttonPosition].GetComponent<Button>().onClick.Invoke();
        }

    }

    public void main()
    {
        menuActive.SetActive(false);
        setActiveMenu(mainMenu);
        findButtons();
        menuActive.SetActive(true);
    }

    public void options()
    {
        menuActive.SetActive(false);
        prevMenu = menuActive;
        setActiveMenu(optionsMenu);
        findButtons();
        menuActive.SetActive(true);
    }

    public void findButtons()
    {
        buttons = GameObject.FindGameObjectsWithTag("Menu Button");
        buttonPosition = 0;
    }

    public void setActiveMenu(GameObject menu)
    {
        if (menu == null)
            menuActive.SetActive(false);

        menuActive = menu;
        
        if (menuActive != null) 
            menuActive.SetActive(true);
        
            
    }

    public void prevMenuCall()
    {
        menuActive.SetActive(false);
        setActiveMenu(prevMenu);
        menuActive.SetActive(true);
        findButtons();
    }
}
