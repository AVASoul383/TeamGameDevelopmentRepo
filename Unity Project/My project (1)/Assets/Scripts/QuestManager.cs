using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public TMP_Text questText;

    private bool grenadePickedUp = false;

    public void ShowPressGPrompt()
    {
        if (!grenadePickedUp)
        {
            questText.text = "Press <b>G</b> to pick up the grenade";
        }
    }

    public void OnGrenadePickedUp()
    {
        grenadePickedUp = true;


        questText.text = "<color=#888888><i>Picked up grenade — Press T to throw</i></color>";
    }

    public void ClearText()
    {
        if (!grenadePickedUp)
        {
            questText.text = "";
        }
    }
}
