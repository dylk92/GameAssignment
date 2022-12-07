using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterInfo
{
    public int Class;
    public string Name;
    public string Text;
}

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerObject;
    [SerializeField] Player PlayerScript;
    UIManager UIMNG;

    public Dictionary<string, Stats> CharacterStat = new Dictionary<string, Stats>();
    public Dictionary<string, CharacterInfo> CharacterInfo = new Dictionary<string, CharacterInfo>();


    public string ClassType;
    public string[] ClassJob;

    const int DEFAULT_EXP = 5;
    int maxEXP = 5;
    int playerEXP;
    int PlayerLevel;

    public int MaxEXP
    {
        get { return maxEXP; }
    }
    public int PlayerEXP
    {
        get { return playerEXP; }
    }

    void Awake()
    {
        UIMNG = GameManager.Instance.UIMNG;

        ClassJob = new string[3];

        Init();
        SetCharDic();
    }

    public void Init()
    {
        maxEXP = DEFAULT_EXP;
        playerEXP = 0;
        PlayerLevel = 1;
        ClassType = "Archer";
        for (int i = 0; i < ClassJob.Length; i++)
        {
            ClassJob[i] = "";
        }
        Debug.Log(PlayerLevel + ", " + PlayerEXP + ", " + MaxEXP);
    }
    
    #region Dictionary MNG

    void SetCharDic()
    {
        List<Dictionary<string, string>> CharList = GameManager.Instance.CSVRead.CharacterStatsList;

        for(int i = 0; i < CharList.Count; i++)
        {
            Stats stat = new Stats();
            CharacterInfo info = new CharacterInfo();
            if(ClassType == CharList[i]["Type"])
            {
                string job = CharList[i]["Job"];

                stat.MaxHP = float.Parse(CharList[i]["MaxHP"]);
                stat.ATK = float.Parse(CharList[i]["ATK"]);
                stat.MoveSpeed = float.Parse(CharList[i]["MoveSpeed"]);
                stat.ATKSpeed = float.Parse(CharList[i]["ATKSpeed"]);
                stat.DEF = float.Parse(CharList[i]["DEF"]);
                stat.CritPer = Int32.Parse(CharList[i]["CritPer"]);
                stat.CritDMG = float.Parse(CharList[i]["CritDMG"]);
                stat.IncreaseDMG = float.Parse(CharList[i]["IncreaseDMG"]);
                stat.ATKRange = float.Parse(CharList[i]["ATKRange"]);
                stat.AGI = float.Parse(CharList[i]["AGI"]);

                info.Class = Int32.Parse(CharList[i]["Class"]);
                info.Name = CharList[i]["Name"];
                info.Text = CharList[i]["Text"];

                CharacterStat.Add(job, stat);
                CharacterInfo.Add(job, info);
            }
        }
    }

    public String[] GetRandJob(int index)
    {
        string[] dump = new string[2];
        List<string> list = new List<string>();

        // 현재 전직할 수 있는 클래스를 추출
        foreach(KeyValuePair<string, CharacterInfo> temp in CharacterInfo)
        {
            if(temp.Value.Class == index)
            {
                list.Add(temp.Key);
            }
        }

        for(int i = 0; i < 2; i++)
        {
            int rand = UnityEngine.Random.Range(0, list.Count);

            if(i == 1)
            {
                if(list[rand] == dump[0])
                {
                    if (++rand == list.Count)
                        rand = 0;
                }
            }
            dump[i] = list[rand];
        }
        //dump[0] = "Ranger";
        return dump;
    }

    #endregion

    #region Level

    public void EXPPlus(int EXP)
    {
        playerEXP += EXP;
        GameManager.Instance.UIMNG.EXPMNG.GagueUpdate();

        if (playerEXP >= MaxEXP) // 레벨업 조건
            LevelUP();
    }

    public void LevelUP()
    {
        GameManager.Instance.UIMNG.EXPMNG.LevelUp(++PlayerLevel);

        GameManager.Instance.UIMNG.ActiveJobSelectUI(false);
        GameManager.Instance.UIMNG.ActiveSkillSelectUI(false);


        if (PlayerLevel == 10)
        {
            GameManager.Instance.UIMNG.ActiveJobSelectUI(true);
            GameManager.Instance.UIMNG.JobSelection.JobSelecterAppear();
        }
        else
        {
            GameManager.Instance.UIMNG.ActiveSkillSelectUI(true);
            GameManager.Instance.UIMNG.SkillSelection.SkillSelecterAppear();
        }

        PlayerScript.HealthRecovery(PlayerScript.GetStats().MaxHP * 0.2f);
        PlayerScript.HPGaugeSet();

        playerEXP -= MaxEXP;
        maxEXP = (int)(maxEXP * 1.2f);

        if (PlayerEXP < 0)
            playerEXP = 0;

        GameManager.Instance.UIMNG.EXPMNG.GagueUpdate();
    }

    #endregion

    #region Job
    
    void SetJob(string job)
    {
        for (int i = 0; i < ClassJob.Length; i++)
        {
            if (ClassJob[i] == "")
            {
                ClassJob[i] = job;
                GameManager.Instance.PlayerObject.GetComponent<Player>().JobChange(job);
                GameManager.Instance.PlayerItemMNG.SkillMNG.SkillDataInput(job);
                break;
            }
        }
    }

    public void JobChange(int index)
    {
        UIMNG.ActiveJobSelectUI(false);
        string newJob = UIMNG.JobSelection.newJob[index];

        SetJob(newJob);
    } 

    #endregion
}
