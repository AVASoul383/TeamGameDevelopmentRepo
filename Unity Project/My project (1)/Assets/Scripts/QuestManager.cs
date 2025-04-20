using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public TMP_Text questText;

    void Start()
    {
        ShowObjective("🎯 Objective: Pick up the grenade");
    }

    public void ShowObjective(string message)
    {
        questText.gameObject.SetActive(true);
        questText.text = message;
    }

    public void HideObjective()
    {
        questText.gameObject.SetActive(false);
    }

    public void OnGrenadePickedUp()
    {
        ShowObjective("✅ Objective Complete: Picked up the grenade");
        
        Invoke("HideObjective", 3f);
    }
}
