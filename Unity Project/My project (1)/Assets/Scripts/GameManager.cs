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
    [SerializeField] GameObject menuShop;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] GameObject continueMenu;

    public GameObject playerSpawnPos;
    public Image playerHPBar;
    public Image playerExpBar;
    public GameObject playerDamageScreen;
    public GameObject playerHealthScreen;
    public GameObject player;
    public playerController playerScript;
    public GameObject checkpointPopup;

    [Header("----- Hotbar Menu -----")]
    [SerializeField] TMP_Text item1CountText;
    [SerializeField] TMP_Text item2CountText;
    [SerializeField] TMP_Text item3CountText;
    [SerializeField] TMP_Text item4CountText;
    [SerializeField] TMP_Text moneyCountText;
    public GameObject playerDamageBoostScreen;
    public GameObject playerSpeedBoostScreen;
    public GameObject playerDefenseBoostScreen;

    public bool isPaused;

    int goalCount;
    int bossCount;
    public int moneyCount;
    int waves;
    TurnOnOff trigger1;
    TurnOnOff trigger2;
    TurnOnOff trigger3;
    TurnOnOff trigger4;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        trigger1 = GameObject.FindWithTag("F1 Trigger").GetComponent<TurnOnOff>();
        trigger2 = GameObject.FindWithTag("F2 Trigger").GetComponent<TurnOnOff>();
        trigger3 = GameObject.FindWithTag("F3 Trigger").GetComponent<TurnOnOff>();
        trigger4 = GameObject.FindWithTag("Boss Trigger").GetComponent<TurnOnOff>();  
        for (int i = 0; i < trigger1.levelItem.Length; i++)
        {
            trigger1.levelItem[i].SetActive(false);
        }
        for (int i = 0; i < trigger2.levelItem.Length; i++)
        {
            trigger2.levelItem[i].SetActive(false);
        }
        for (int i = 0; i < trigger3.levelItem.Length; i++)
        {
            trigger3.levelItem[i].SetActive(false);
        }
        for (int i = 0; i < trigger4.levelItem.Length; i++)
        {
            trigger4.levelItem[i].SetActive(false);
        }   
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

    public void openShop()
    {
        statePause();
        setActiveMenu(menuShop);
    }

    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");

    }

    public void bossFight(int amount)
    {
        bossCount += amount;

        if(bossCount <= 0)
        {
            statePause();
            setActiveMenu(menuWin);
        }
    }

    public void updateMoneyCount(int amount)
    {
        moneyCount += amount;
        moneyCountText.text = moneyCount.ToString("F0");
    }
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
