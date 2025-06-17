using System.Collections;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public TextMeshProUGUI TargetText;
    public int Seconds;

    private void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        StartCoroutine(DecreaseTime());
    }

    public IEnumerator DecreaseTime()
    {
        int minutes = 0;
        int seconds = 0;
        if (Seconds < 60)
        {
            seconds = Seconds;
        }
        else
        {
            seconds = Seconds % 60;
            minutes = Seconds / 60;
        }

        if (seconds < 10)
        {
            TargetText.text = minutes + ":0" + seconds;
        }
        else
        {
            TargetText.text = minutes + ":" + seconds;
        }

        yield return new WaitForSeconds(1);
        Seconds--;
        if (Seconds == -1)
        {
            TimeEnded();
        }
        else
        {
            StartCoroutine(DecreaseTime());
        }
    }

    public void TimeEnded()
    {
        GameObject.FindGameObjectWithTag("TicketContent").GetComponent<TicketInfo>().RevealGameOver();
    }
}
