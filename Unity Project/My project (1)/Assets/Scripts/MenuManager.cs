using System.Collections.Generic;
using TMPro;
using Unity.Profiling.LowLevel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject continueMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject prompt; 

    GameObject prevMenu;
    GameObject[] buttons;
    int buttonPosition;
    float canBePressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            setActiveMenu(mainMenu);
            menuActive.SetActive(true);
            findButtons();
            buttonPosition = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canBePressed == 0 && menuActive != null)
        {
            if (Input.GetAxis("Vertical") < 0)buttonPosition++;
            else if (buttonPosition < 0) buttonPosition = buttons.Length - 1;
            else if (Input.GetAxis("Vertical") > 0) buttonPosition--;
            else if (buttonPosition > buttons.Length - 1) buttonPosition = 0;

            Selected();
        }
        
        canBePressed = Input.GetAxis("Vertical");

        if ((Input.GetButtonDown("Submit")) && menuActive != null)
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
        menuActive.SetActive(true);
    }

    private void findButtons()
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
        {
            menuActive.SetActive(true);
            findButtons();
            Selected();
        }
            
    }

    public void prevMenuCall()
    {
        menuActive.SetActive(false);
        setActiveMenu(prevMenu);
        menuActive.SetActive(true);
        findButtons();
    }

    public void promptInput()
    {
        TMP_Text line= prompt.GetComponentInChildren<TMP_Text>();
        line.text = "Press Enter to Continue";
        prompt.SetActive(true);
    }

    private void Selected()
    {
        if(buttonPosition < buttons.Length && buttonPosition >= 0)
        {
            if (buttons[buttonPosition] != null)
            {
                EventSystem.current.SetSelectedGameObject(buttons[buttonPosition], new BaseEventData(EventSystem.current));
            }
        }
    }
}
