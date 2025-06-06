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
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] TMP_Text npcName;
    [SerializeField] GameObject interactionPrompt;


    public GameObject playerSpawnPos;
    public GameObject[] advancementPlatforms;
    public Image playerHPBar;
    public Image playerExpBar;
    public GameObject playerDamageScreen;
    public GameObject playerHealthScreen;
    public GameObject player;
    public playerController playerScript;
    public GameObject checkpointPopup;
    public TMP_Text ammoAmt;

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

        // Try to find player
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found.");
        }

        // Try to find player spawn position
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        if (playerSpawnPos == null)
            Debug.LogWarning("Player Spawn Pos not found.");

        // Safe way to find triggers
        TrySetupTrigger("F1 Trigger", ref trigger1);
        TrySetupTrigger("F2 Trigger", ref trigger2);
        TrySetupTrigger("F3 Trigger", ref trigger3);
        TrySetupTrigger("Boss Trigger", ref trigger4);

        MusicManager.instance.playGameplayMusic();
    }

    void TrySetupTrigger(string tag, ref TurnOnOff trigger)
    {
        GameObject obj = GameObject.FindWithTag(tag);
        if (obj != null)
        {
            trigger = obj.GetComponent<TurnOnOff>();
            if (trigger != null)
            {
                foreach (var item in trigger.levelItem)
                    item.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"GameObject with tag '{tag}' found but missing TurnOnOff script.");
            }
        }
        else
        {
            Debug.LogWarning($"GameObject with tag '{tag}' not found.");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !isPaused)
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
        if (dialogueBox == null)
        {
            Debug.Log("Dialogue box is null");  
        }
        if (dialogueBox.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E key pressed");
                dialogueBox.SetActive(false);
            }
        }
        openArea();
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        MusicManager.instance.playMenuMusic();
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        MusicManager.instance.playGameplayMusic();
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

    public void openArea()
    {
        if(goalCount == 0)
        {
            for (int i = 0; i < advancementPlatforms.Length; i++)
            {
                advancementPlatforms[i].SetActive(false);
            }
        }
        else if(goalCount > 0)
        {
            for (int i = 0; i < advancementPlatforms.Length; i++)
            {
                advancementPlatforms[i].SetActive(true);
            }
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

    public void showDialogue(string text, string name)
    {
        dialogueBox.SetActive(true);
        npcName.text = name;
        dialogueText.text = text;
    }

    public void hideDialogue()
    {
        
            dialogueBox.SetActive(false);
        
        
    }

    public void showInteractionPrompt()
    {
        interactionPrompt.SetActive(true);
    }

    public void hideInteractionPrompt()
    {
        interactionPrompt.SetActive(false);
    }
}
