using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EXPBarManager : MonoBehaviour
{
    PlayerManager PlayerMNG;

    Image EXPGauge;
    Text LevelText;

    private void Awake()
    {
        PlayerMNG = GameManager.Instance.PlayerMNG;

        EXPGauge = transform.GetChild(1).gameObject.GetComponent<Image>();
        LevelText = transform.GetChild(2).gameObject.GetComponent<Text>();
    }

    public void Init()
    {
        EXPGauge.fillAmount = 0;
        LevelText.text = "Lv. " + 1;
    }

    public void GagueUpdate()
    {
        float gauge = (float)PlayerMNG.PlayerEXP / (float)PlayerMNG.MaxEXP;

        EXPGauge.fillAmount = gauge;
    }

    public void LevelUp(int Lv)
    {
        LevelText.text = "Lv. " + Lv;
    }
}
