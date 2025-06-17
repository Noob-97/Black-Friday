using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TicketInfo : MonoBehaviour
{
    public GameObject TextPrefab;

    public void RevealGameOver()
    {
        LoadTicket();
        GameObject.FindGameObjectWithTag("GameOver").GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.OutExpo);
        GameObject.FindGameObjectWithTag("GameOver").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.FindGameObjectWithTag("GameOver").GetComponent<CanvasGroup>().interactable = true;
        GameObject.FindGameObjectWithTag("GameOver").GetComponentInChildren<VerticalLayoutGroup>().padding.right = 1;
    }
    public void LoadTicket()
    {
        string minutes = "";
        if (DateTime.Now.Minute < 10)
        {
            minutes = "0" + DateTime.Now.Minute;
        }
        else
        {
            minutes = DateTime.Now.Minute.ToString();
        }
        WriteText(DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + (DateTime.Today.Year - 2000) + " " + DateTime.Now.Hour + ":" + minutes);
        int runs = 0;
        if (PlayerPrefs.HasKey("Runs"))
        {
            PlayerPrefs.SetInt("Runs", PlayerPrefs.GetInt("Runs") + 1);
            runs = PlayerPrefs.GetInt("Runs");
        }
        else
        {
            PlayerPrefs.SetInt("Runs", 1);
            runs = 1;
        }

        WriteText("-----------------");

        WriteText("<u>Product           Cost");
        foreach (ItemObject item in GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().ItemsPurchased)
        {
            string cost = "";
            if (item.ItemCost == ItemObject.CostCategory.Low)
            {
                cost = "$";
            }
            if (item.ItemCost == ItemObject.CostCategory.Medium)
            {
                cost = "$$";
            }
            if (item.ItemCost == ItemObject.CostCategory.High)
            {
                cost = "$$$";
            }
            if (item.ItemCost == ItemObject.CostCategory.Expensive)
            {
                cost = "$$$$";
            }
            if (item.ItemCost == ItemObject.CostCategory.Priceless)
            {
                cost = "$$$$$";
            }
            WriteText(item.ItemName + ".........." + cost);
        }

        WriteText("-----------------");

        WriteText("<u>Subject        Points");
        foreach (PointGain gain in GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().PointsGained)
        {
            WriteText(gain.Subtitle + "...." + gain.Points);
        }

        WriteText("-----------------");

        WriteText("Items Bought: " + GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().ItemsPurchased.Count);

        int average = 0;
        foreach (ItemObject item in GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().ItemsPurchased)
        {
            if (item.ItemCost == ItemObject.CostCategory.Low)
            {
                average += 1;
            }
            if (item.ItemCost == ItemObject.CostCategory.Medium)
            {
                average += 2;
            }
            if (item.ItemCost == ItemObject.CostCategory.High)
            {
                average += 3;
            }
            if (item.ItemCost == ItemObject.CostCategory.Expensive)
            {
                average += 4;
            }
            if (item.ItemCost == ItemObject.CostCategory.Priceless)
            {
                average += 5;
            }
        }
        average = Mathf.RoundToInt(average / GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().ItemsPurchased.Count);
        if (average == 1)
        {
            WriteText("Average Cost: $");
        }
        if (average == 2)
        {
            WriteText("Average Cost: $$");
        }
        if (average == 3)
        {
            WriteText("Average Cost: $$$");
        }
        if (average == 4)
        {
            WriteText("Average Cost: $$$$");
        }
        if (average == 5)
        {
            WriteText("Average Cost: $$$$$");
        }

        int points = 0;
        foreach (PointGain gain in GameObject.FindGameObjectWithTag("Circle").GetComponent<PlayerMovement>().PointsGained)
        {
            points += gain.Points;
        }
        WriteText("Points Gained: " + points);
        GameObject.FindGameObjectWithTag("GameOver").transform.Find("ScoreGain").GetComponentInChildren<TextMeshProUGUI>().text = points + "<size=40>p";

        WriteText("-----------------");

        int TotalPoints = 0;
        if (average == 1)
        {
            WriteText(points + "p - 0% (Cost Penalty)");
            TotalPoints = points;
            GameObject.FindGameObjectWithTag("GameOver").transform.Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = "-0% (Cost Penalty)";
        }
        if (average == 2)
        {
            WriteText(points + "p - 10% (Cost Penalty)");
            TotalPoints = Mathf.RoundToInt(points * 0.9f);
            GameObject.FindGameObjectWithTag("GameOver").transform.Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = "-10% (Cost Penalty)";

        }
        if (average == 3)
        {
            WriteText(points + "p - 20% (Cost Penalty)");
            TotalPoints = Mathf.RoundToInt(points * 0.8f);
            GameObject.FindGameObjectWithTag("GameOver").transform.Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = "-20% (Cost Penalty)";
        }
        if (average == 4)
        {
            WriteText(points + "p - 30% (Cost Penalty)");
            TotalPoints = Mathf.RoundToInt(points * 0.7f);
            GameObject.FindGameObjectWithTag("GameOver").transform.Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = "-30% (Cost Penalty)";
        }
        if (average == 5)
        {
            WriteText(points + "p - 40% (Cost Penalty)");
            TotalPoints = Mathf.RoundToInt(points * 0.6f);
            GameObject.FindGameObjectWithTag("GameOver").transform.Find("Text (TMP) (1)").GetComponent<TextMeshProUGUI>().text = "-40% (Cost Penalty)";
        }

        WriteText("TOTAL SCORE: " + TotalPoints);
        GameObject.FindGameObjectWithTag("GameOver").transform.Find("Text (TMP) (2)").GetComponent<TextMeshProUGUI>().text = TotalPoints + "<size=50>p";

        WriteText("-----------------");

        WriteText("Thanks for playing Black Friday!");
        WriteText("Made for the Brackeys Game Jam 2024.2");

        WriteText("-----------------");

        WriteText("Credits");

        WriteText("Tween System by Demigiant on DOTween");
        WriteText("Music by Vitaly Vakulenko and Cookrate Eli, from Pixabay.");

        WriteText(" ");

    }

    public void WriteText(string Text)
    {
        GameObject text = Instantiate(TextPrefab, GameObject.FindGameObjectWithTag("TicketContent").transform);
        text.transform.localScale = Vector3.one;

        text.GetComponent<TextMeshProUGUI>().text = Text;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }
}
