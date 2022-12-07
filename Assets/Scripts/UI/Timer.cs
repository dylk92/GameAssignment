using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Text timeText;
    public float GameTime;

    private void Awake()
    {
        timeText = GetComponent<Text>();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        timeText.text = "00:00";
        GameTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeSet();
    }

    void TimeSet()
    {
        GameTime += Time.deltaTime;

        int minute = (int)GameTime / 60;
        int second = (int)GameTime % 60;

        string MinText = minute.ToString();
        string SecText = second.ToString();
        if (minute < 10)
            MinText = "0" + MinText;
        if (second < 10)
            SecText = "0" + SecText;

        timeText.text = MinText + ":" + SecText;

        if (minute >= 10)
            GameClear();
    }

    void GameClear()
    {
        Time.timeScale = 0;
        GameManager.Instance.UIMNG.GameOver.GetComponent<GameOverScreen>().TextSet("GAME CLEAR");
        GameManager.Instance.UIMNG.GameOver.gameObject.SetActive(true);
    }
}
