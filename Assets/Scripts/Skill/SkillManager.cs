using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 스킬 관리
// 스킬의 사이클부터 스킬 생성, 

public class SkillManager : MonoBehaviour
{
    const int MAX_SKILL_SPACE = 6;
    const int MAX_LEVEL = 4;
    readonly int[] MAX_JobCount = {3, 3, 0};

    [SerializeField] GameObject SkillPrefab;

    // 현재 직업의 모든 스킬 정보를 담아두는 리스트
    Dictionary<int, SkillData[]> SkillDataList = new Dictionary<int, SkillData[]>();
    Dictionary<int, SkillData[]> ChildSkillDataList = new Dictionary<int, SkillData[]>();
    
    // 현재 플레이어가 사용하는 스킬의 생성 클래스
    Skill[] PlayerSkillList = new Skill[MAX_SKILL_SPACE];
    // 현재 플레이어가 사용하는 스킬의 데이터
    SkillData[][] PlayerSkillDataList = new SkillData[MAX_SKILL_SPACE][];
    // 사용되고 있는 오브젝트는 이곳에서 보관
    public List<GameObject> ActiveSkillList = new List<GameObject>();

    int[] PlayerSkillID = new int[MAX_SKILL_SPACE];
    int[] PlayerSkillLevel = new int[MAX_SKILL_SPACE];
    bool[] isHaveSkill = new bool[MAX_SKILL_SPACE];
    IEnumerator[] SkillCoro = new IEnumerator[MAX_SKILL_SPACE];

    string[] JobName = new string[3];
    int[] JobCount = new int[3];

    PlayerItemManager PlayerItemMNG;
    Player PlayerScript;

    #region UnityAction

    private void Awake()
    {
        PlayerItemMNG = GameManager.Instance.PlayerItemMNG;
        GenerateSkill(); // 스킬 캐시 생성
        PlayerScript = GameManager.Instance.PlayerObject.GetComponent<Player>();
    }

    #endregion

    #region Init

    public void Init()
    {
        PlayerSkillID = new int[MAX_SKILL_SPACE];
        PlayerSkillLevel = new int[MAX_SKILL_SPACE];
        isHaveSkill = new bool[MAX_SKILL_SPACE];

        ListClear();
        SkillCoroutineStop();

        for(int i = 0; i < MAX_SKILL_SPACE; i++)
            isHaveSkill[i] = false;

        for (int i = 0; i < JobName.Length; i++)
        {
            JobName[i] = "";
            JobCount[i] = 0;
        }
        SkillDataList.Clear();
        ChildSkillDataList.Clear();
        SkillDataInput("Base");
        ChildSkillDataInput();
        
        InputPlayerSkill(0);
    }

    void ListClear()
    {
        for(int i = ActiveSkillList.Count-1; i >= 0; i--)
        {
            ActiveSkillList[i].GetComponent<SkillObject>().DestroyThis();
        }
    }

    void SkillCoroutineStop()
    {
        for(int i = 0; i< MAX_SKILL_SPACE; i++)
        {
            if (SkillCoro[i] != null)
                StopCoroutine(SkillCoro[i]);
        }
    }

    #endregion

    #region SkillGenerate


    public void SkillDataInput(string job)
    {
        List<Dictionary<string, string>> SDL = GameManager.Instance.CSVRead.SkillDataList;
        string type = GameManager.Instance.PlayerMNG.ClassType;
        SkillData skillData = new SkillData();

        for (int i = 0; i < SDL.Count; i++)
        {
            if (SDL[i]["Type"] != "") skillData.skillInfo.Type = SDL[i]["Type"];
            if (SDL[i]["Job"] != "") skillData.skillInfo.Job = SDL[i]["Job"];

            if (skillData.skillInfo.Type == type && skillData.skillInfo.Job == job)
            {
                skillData = DataParse(skillData, job, SDL[i]);

                if (!SkillDataList.ContainsKey(skillData.skillInfo.ID))
                {
                    SkillData[] TempSkill = new SkillData[5];
                    SkillDataList.Add(skillData.skillInfo.ID, TempSkill);
                    if (PlayerItemMNG.CanAppearSkill != null)
                    {
                        PlayerItemMNG.CanAppearSkill.Add(skillData.skillInfo.ID);
                    }
                }
                SkillDataList[skillData.skillInfo.ID][skillData.skillInfo.Level] = skillData;
            }
        }

        for (int i = 0; i < JobName.Length; i++)
        {
            if (JobName[i] == "")
            {
                JobName[i] = job;
                break;
            }

        }
    }

    void ChildSkillDataInput()
    {
        List<Dictionary<string, string>> SDL = GameManager.Instance.CSVRead.SkillDataList;
        string type = GameManager.Instance.PlayerMNG.ClassType;
        string job = "Child";
        SkillData skillData = new SkillData();

        for (int i = 0; i < SDL.Count; i++)
        {
            if (SDL[i]["Type"] != "") skillData.skillInfo.Type = SDL[i]["Type"];
            if (SDL[i]["Job"] != "") skillData.skillInfo.Job = SDL[i]["Job"];

            if (skillData.skillInfo.Type == type && skillData.skillInfo.Job == job)
            {
                skillData = DataParse(skillData, job, SDL[i]);

                if (!ChildSkillDataList.ContainsKey(skillData.skillInfo.ID))
                {
                    SkillData[] TempSkill = new SkillData[5];
                    ChildSkillDataList.Add(skillData.skillInfo.ID, TempSkill);
                }
                ChildSkillDataList[skillData.skillInfo.ID][skillData.skillInfo.Level] = skillData;
            }
        }
    }

    public SkillData DataParse(SkillData skillData, string job, Dictionary<string, string> data)
    {
        #region SkillInfoParse

        if (data["Name"] != "") skillData.skillInfo.Name = data["Name"];
        if (data["ID"] != "") skillData.skillInfo.ID = Int32.Parse(data["ID"]);
        skillData.skillInfo.Level = Int32.Parse(data["Level"]);

        #endregion

        #region SkillStatParse

        skillData.skillStat.Lifespan = float.Parse(data["Lifespan"]);
        skillData.skillStat.DMG = float.Parse(data["DMG"]);
        skillData.skillStat.Speed = float.Parse(data["Speed"]);
        skillData.skillStat.KnockBack = float.Parse(data["KnockBack"]);
        skillData.skillStat.Debuff = DebuffParse(data["Debuff"]);
        skillData.skillStat.Breakable = bool.Parse(data["Breakable"]);
        skillData.skillStat.ATKDelay = float.Parse(data["ATKDelay"]);

        #endregion

        #region SkillCreateDataParse

        if (data["Cooltime"] != "") skillData.skillCreateData.Cooltime = float.Parse(data["Cooltime"]);
        if (data["SpawnCount"] != "") skillData.skillCreateData.SpawnCount = Int32.Parse(data["SpawnCount"]);
        if (data["SpawnAngle"] != "") skillData.skillCreateData.SpawnAngle = Int32.Parse(data["SpawnAngle"]);
        if (data["CreateDelay"] != "") skillData.skillCreateData.CreateDelay = float.Parse(data["CreateDelay"]);
        if (data["Size"] != "") skillData.skillCreateData.Size = SizeParse(data["Size"]);
        if (data["Shape"] != "") skillData.skillCreateData.Shape = data["Shape"];
        #endregion

        if(data["ChildObject"] != "") skillData.ChildID = Int32.Parse(data["ChildObject"]);
        
        return skillData;
    }

    // num1_num2_num3~no1_no2_no3 <- 이런 식으로 오는걸 _ 와 ~ 를 구분점으로 잘라서 반환
    // └> [[0, 1, 2], [1, 3, 5]]    이런 형태로 변경, N 이면 디버프 내용이 없는 것
    List<float[]> DebuffParse(string debuff)
    {
        if (debuff == "")
            return null;

        List<float[]> list = new List<float[]>();

        string[] parse = debuff.Split('~');

        for(int i = 0; i < parse.Length; i++)
        {
            float[] debuffList = new float[3];
            string[] dump = parse[i].Split('_');

            for(int j = 0; j < dump.Length; j++)
            {
                debuffList[j] = float.Parse(dump[j]);
            }
            list.Add(debuffList);
        }

        return list;
    }

    // "sizeX_sizeY" 으로 온 데이터를 [float, float]로 바꾼다.
    float[] SizeParse(string size)
    {
        string[] dump = size.Split('_');
        float[] sizeList = new float[2];

        sizeList[0] = float.Parse(dump[0]);
        sizeList[1] = float.Parse(dump[1]);

        return sizeList;
    }

    void GenerateSkill()
    {
        GameManager.Instance.CacheSys.GenerateCache(SkillPrefab, "Skill", 400);
    }

    #endregion

    #region SkillInput

    // 스킬을 입력할 때(새로 추가 & 스킬 레벨 업)
    public void InputPlayerSkill(int index)
    {
        for (int i = 0; i < MAX_SKILL_SPACE; i++)
        {
            if (isHaveSkill[i])
            {
                if (PlayerSkillID[i] == index)
                {
                    PlayerSkillLevel[i]++;
                    MaxLevelCheck(i);
                    break;
                }
            }
            else
            {
                isHaveSkill[i] = true;
                PlayerSkillLevel[i] = 0;
                PlayerSkillID[i] = index;

                PlayerSkillList[i] = FindSkillClass(index);
                PlayerSkillDataList[i] = SkillDataList[index];

                string job = PlayerSkillDataList[i][0].skillInfo.Job;
                JobSkillCheck(job);

                StartCoroutine(CreateSkillLoop(i));

                break;
            }
        }
        GameManager.Instance.UIMNG.PlayerInfoMNG.SetSkillInfo();
    }

    void MaxLevelCheck(int index)
    {
        if(PlayerSkillLevel[index] == MAX_LEVEL)
        {
            int MaxLevelID = PlayerSkillID[index];

            PlayerItemMNG.CanAppearSkill.RemoveAll(x => x == MaxLevelID);
        }
    }

    void JobSkillCheck(string job)
    {
        for(int i = 0; i < JobName.Length; i++)
        {
            if(JobName[i] == job)
            {
                JobCount[i]++;

                if (MAX_JobCount[i] <= JobCount[i])
                {
                    PlayerItemMNG.SkillListEdit(job);
                }
                break;
            }
        }
    }

    #endregion

    #region SkillCreateLoop

    IEnumerator CreateSkillLoop(int index)
    {
        yield return GameManager.Instance.CoroutineMNG.GetWFS(0.5f);

        SkillCoro[index] = SkillLoopStart(index);
        StartCoroutine(SkillCoro[index]);
    }

    IEnumerator SkillLoopStart(int index)
    {
        while (isHaveSkill[index])
        {
            SkillData skillData = PlayerSkillDataList[index][PlayerSkillLevel[index]];
            PlayerSkillList[index].Activate(skillData);

            float DelayTime = skillData.skillCreateData.Cooltime / PlayerScript.GetStats().ATKSpeed;
            yield return GameManager.Instance.CoroutineMNG.GetWFS(DelayTime);
        }
    }

    #endregion

    public int GetSkillLevel(int ID)
    {
        int index = Array.IndexOf(PlayerSkillID, ID);

        if(index == -1)
            return 0;
        return PlayerSkillLevel[index] + 1;
    }

    public SkillData GetChildSkillData(int ID, int Level)
    {
        SkillData sd = new SkillData();
        if (ChildSkillDataList.ContainsKey(ID))
            sd = ChildSkillDataList[ID][Level];
        else if (SkillDataList.ContainsKey(ID))
            sd = SkillDataList[ID][Level];
            
        return sd;
    }

    public bool GetHaveSkill(int i) => isHaveSkill[i];

    public int[] GetSkillInfo(int i) => new int[] {PlayerSkillID[i], PlayerSkillLevel[i]};

    public string GetSkillJob(int index) => SkillDataList[index][0].skillInfo.Job;


    Skill FindSkillClass(int index)
    {
        Skill dump = null;
        switch (index)
        {
            #region Archer Base
            case 0:
                dump = new BaseArrow();
                break;
            case 1:
                dump = new ArrowDrive();
                break;
            case 2:
                dump = new ArrowBurst();
                break;
            case 3:
                dump = new MultiShot();
                break;
            case 4:
                dump = new BowSweep();
                break;
            case 5:
                dump = new ArrowRain();
                break;
            #endregion

            #region Ranger
            case 6:
                dump = new Stinger();
                break;
            case 7:
                dump = new SpreadShot();
                break;
            case 8:
                dump = new AutoFire();
                break;
            case 9:
                dump = new WhirlWind();
                break;
            case 10:
                dump = new ArrowStorm();
                break;
            #endregion

            #region Sniper
            case 11:
                dump = new ArrowBlast();
                break;
            case 12:
                dump = new Sniping();
                break;
            case 13:
                dump = new BombArrow();
                break;
            case 15:
                dump = new ArrowDrive();
                break;
            case 16:
                dump = new Frangible();
                break;
            #endregion

            #region Druid
            case 18:
                dump = new WildRoar();
                break;
            case 19:
                dump = new WoodenTurret();
                break;
            case 21:
                dump = new BirdStrike();
                break;
            case 22:
                dump = new VineChain();
                break;
            case 23:
                dump = new LeafRotation();
                break;
            #endregion
            #region HighElf
            case 24:
                dump = new RapidLaser();
                break;
            case 25:
                dump = new MagicArrow();
                dump.ExtraTarget = GameManager.Instance.PlayerObject;
                break;
            case 26:
                dump = new Meteor();
                break;
            case 28:
                dump = new ArrowRotation();
                break;
            case 29:
                dump = new ManaEruption();
                break;
            #endregion
        }
        return dump;
    }
}
