using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const int DIFFICULTY_COUNT = 3;
    const int MONEY_RATE = 10;

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public enum GameState
    {
        Continue,
        Win,
        Lose
    }

    public Difficulty difficulty;
    public GameObject[] cubes = new GameObject[DIFFICULTY_COUNT];
    public List<GameObject> placeHolders;
    public List<bool> placeHoldersSituation;

    int level = 21; //Save dosyası yapılırsa oradan oyuncunun bulunduğu seviye çekilebilir
    int money = 0; //Save dosyası yapılırsa oradan oyuncunun sahip olduğu para çekilebilir
    int cubeIndex;
    GameObject mainCube;
    GameState gameSta;
    int cubeCount;
    MenuManager menuManager;

    public int Money
    {
        get { return money; }
        set { money = value; }
    }
    public int CubeIndex { get { return cubeIndex; } }
    public int CubeCount
    {
        get { return cubeCount; }
        set { cubeCount = value; }
    }

    void Awake()
    {
        menuManager = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
        menuManager.ChangeMoneyTxt(money.ToString());
        gameSta = GameState.Continue;

        //Bulunduğu seviyeye göre zorluğu ayarlama
        if (level <= 20) difficulty = Difficulty.Easy;
        else if (level <= 40) difficulty = Difficulty.Normal;
        else difficulty = Difficulty.Hard;

        //Zorluğa göre ana kübü oluşturma
        mainCube = Instantiate(cubes[Convert.ToInt32(difficulty)]) as GameObject;
        mainCube.name = mainCube.name.Replace("(Clone)", String.Empty);
        switch (difficulty)
        {
            case Difficulty.Easy:
                cubeIndex = 4;
                break;
            case Difficulty.Normal:
                cubeIndex = 6;
                break;
            case Difficulty.Hard:
                cubeIndex = 8;
                break;
        }

        //Placeholderları ayarlama
        Transform[] phs = mainCube.GetComponentsInChildren<Transform>();
        foreach (Transform ph in phs)
        {
            placeHolders.Add(ph.gameObject);
            placeHoldersSituation.Add(false);
        }
        placeHolders.RemoveAt(0);
        placeHoldersSituation.RemoveAt(0);
    }

    void Update()
    {
        //Oyun durum kontrolü
        if (gameSta == GameState.Win) menuManager.ActivateResultPanel(true); //Kazanma
        else if (gameSta == GameState.Lose) menuManager.ActivateResultPanel(false); //Kaybetme
    }

    public void CubeDestroyer(GameObject cube1) //Tekli küp yok etme
    {
        Destroy(cube1);
        cubeCount--;
        money += MONEY_RATE;

        CheckAndChangeGameState();
    }

    public void CubeDestroyer(GameObject cube1, GameObject cube2) //Eşli küp yok etme
    {
        Destroy(cube1);
        Destroy(cube2);
        cubeCount -= 2;
        money += MONEY_RATE * 2;

        CheckAndChangeGameState();
    }

    void CheckAndChangeGameState()
    {
        menuManager.ChangeMoneyTxt(money.ToString());
        if (cubeCount <= 0) gameSta = GameState.Win; //Tüm küpler yok edilmişse
    }
}
