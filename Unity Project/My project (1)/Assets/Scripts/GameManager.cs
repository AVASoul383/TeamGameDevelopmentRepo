using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] GameObject continueMenu;



    public Image playerHPBar;
    public Image playerExpBar;
    public GameObject playerDamageScreen;
    public GameObject playerHealthScreen;
    public GameObject player;
    public playerController playerScript;

    [SerializeField] TMP_Text item1CountText;
    [SerializeField] TMP_Text item2CountText;
    [SerializeField] TMP_Text item3CountText;
    [SerializeField] TMP_Text item4CountText;
    [SerializeField] TMP_Text moneyCountText;
    public GameObject playerDamageBoostScreen;
    public GameObject playerSpeedBoostScreen;
    public GameObject playerJumpBoostScreen;

    public bool isPaused;

    
    
    
    

    int goalCount;
    int moneyCount;
    int waves;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel") && !isPaused)
        {
            if(menuActive == null)
            {
                statePause();
                setActiveMenu(menuPause);
            }
            else if(menuActive == menuPause)
            {
                stateUnpause();
            }
           
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");

        if(goalCount <= 0)
        {
            waves += 1;
            if(waves == 1)
            {
                statePause();
                setActiveMenu(continueMenu);
            }
            else if(waves > 1)
            {
                statePause();
                setActiveMenu(menuWin);
            }
            
        }
    }

    public void updateMoneyUI() => moneyCountText.text = playerScript.currency.ToString("F0");
    public void updateItemCount1() => item1CountText.text = playerScript.item1Count.ToString("F0");
    public void updateItemCount2() => item2CountText.text = playerScript.item2Count.ToString("F0");
    public void updateItemCount3() => item3CountText.text = playerScript.item3Count.ToString("F0");
    public void updateItemCount4() => item4CountText.text = playerScript.item4Count.ToString("F0");

    public void youLose()
    {
        statePause();
        setActiveMenu(menuLose);
    }

    public void setActiveMenu(GameObject menu)
    {
        menuActive = menu;
        menuActive.SetActive(true);
    }
}
