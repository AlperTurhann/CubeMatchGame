using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    const string moneyFirst = "Para: ";

    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI moneyText;

    public void ActivateResultPanel(bool win)
    {
        resultPanel.SetActive(true);

        if (win) resultText.text = "Tebrikler Kazandınız!";
        else resultText.text = "Maalesef Kaybettiniz!";
    }

    public void ChangeMoneyTxt(string money)
    {
        moneyText.text = moneyFirst + money;
    }

    public void ReplayButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
