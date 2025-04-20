using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;

    public List<GameObject> buttons;
    public int buttonPosition;
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
        if (Input.GetButtonDown("Menu Down") && buttonPosition < buttons.Count - 1) 
        {
            buttonPosition++;
        }
        else if(Input.GetButtonDown("Menu Up") && buttonPosition > 0)
        {
            buttonPosition--;
        }

        if (Input.GetButtonDown("Submit"))
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
        setActiveMenu(optionsMenu);
        findButtons();
        menuActive.SetActive(true);
    }

    public void findButtons()
    {
        GameObject.FindGameObjectsWithTag("Menu Button",buttons);
        buttonPosition = 0;
    }

    public void setActiveMenu(GameObject menu)
    {
        menuActive = menu;
        menuActive.SetActive(true);
    }
}
