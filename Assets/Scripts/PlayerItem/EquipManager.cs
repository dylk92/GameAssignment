using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Equip
{
    public float HP;
    public float ATK;
    public float MoveSpeed;
    public float ATKSpeed;
    public float DEF;
    public int CritPer;
    public float CritDMG;
    public float IncreaseDMG;
    public float ATKRange;
    public float AGI;
}

public class EquipManager : MonoBehaviour
{
    Player PlayerScript;

    const int MAX_EQUIP_SPACE = 6;
    const int MAX_LEVEL = 4;
    
    public static Dictionary<int, Equip[]> EquipDataList = new Dictionary<int, Equip[]>();

    [SerializeField]
    Equip[][] PlayerEquipList = new Equip[MAX_EQUIP_SPACE][];
    int[] PlayerEquipID = new int[MAX_EQUIP_SPACE];
    int[] PlayerEquipLevel = new int[MAX_EQUIP_SPACE];
    bool[] isHaveEquip = new bool[MAX_EQUIP_SPACE];


    private void Start()
    {
        PlayerScript = GameManager.Instance.PlayerObject.GetComponent<Player>();
        EquipDataInput();
    }

    public void Init()
    {
        for (int i = 0; i < MAX_EQUIP_SPACE; i++)
        {
            PlayerEquipID[i] = -1;
            PlayerEquipLevel[i] = -1;
            isHaveEquip[i] = false;
        }

        EquipDataList.Clear();
        EquipDataInput();
    }

    #region DataManage

    void EquipDataInput()
    {
        string PlayerType = GameManager.Instance.PlayerMNG.ClassType;
        List<Dictionary<string, string>> EDL = GameManager.Instance.CSVRead.EquipDataList;

        Equip EquipData = new Equip();
        string Type = null;
        int ID = -1, Level = -1;

        for (int i = 0; i < EDL.Count; i++)
        {
            if (EDL[i]["Type"] != "") Type = EDL[i]["Type"];

            // 현재 직업의 스킬들만 추출하기
            if (Type == PlayerType)
            {
                if(EDL[i]["ID"] != "") ID = Int32.Parse(EDL[i]["ID"]);
                Level = Int32.Parse(EDL[i]["Level"]);
                
                if(EDL[i]["HP"] != "") EquipData.HP = float.Parse(EDL[i]["HP"]);
                if (EDL[i]["ATK"] != "")  EquipData.ATK = float.Parse(EDL[i]["ATK"]);
                if (EDL[i]["MoveSpeed"] != "") EquipData.MoveSpeed = float.Parse(EDL[i]["MoveSpeed"]);
                if (EDL[i]["ATKSpeed"] != "") EquipData.ATKSpeed = float.Parse(EDL[i]["ATKSpeed"]);
                if (EDL[i]["DEF"] != "") EquipData.DEF = float.Parse(EDL[i]["DEF"]);
                if (EDL[i]["CritPer"] != "") EquipData.CritPer = Int32.Parse(EDL[i]["CritPer"]);
                if (EDL[i]["CritDMG"] != "") EquipData.CritDMG = float.Parse(EDL[i]["CritDMG"]);
                if (EDL[i]["IncreaseDMG"] != "") EquipData.IncreaseDMG = float.Parse(EDL[i]["IncreaseDMG"]);
                if (EDL[i]["ATKRange"] != "") EquipData.ATKRange = float.Parse(EDL[i]["ATKRange"]);
                if (EDL[i]["AGI"] != "") EquipData.AGI = float.Parse(EDL[i]["AGI"]);

                if (!EquipDataList.ContainsKey(ID))
                {
                    Equip[] TempEquip = new Equip[5];
                    EquipDataList.Add(ID, TempEquip);

                    List<int> CanEquip = GameManager.Instance.PlayerItemMNG.CanAppearEquip;
                    if (GameManager.Instance.PlayerItemMNG.CanAppearEquip != null)
                    {
                        CanEquip.Add(ID);
                    }
                }
                EquipDataList[ID][Level] = EquipData;
            }
        }
    }

    // 장비를 새로 입력할 때
    public void InputPlayerEquip(int index)
    {
        for(int i = 0; i < MAX_EQUIP_SPACE; i++)
        {
            if (isHaveEquip[i])
            {
                if (PlayerEquipID[i] == index)
                {
                    PlayerEquipLevel[i]++;
                    MaxLevelCheck(i);
                    break;
                }
            }
            else
            {
                isHaveEquip[i] = true;
                PlayerEquipLevel[i] = 0;
                PlayerEquipID[i] = index;

                PlayerEquipList[i] = EquipDataList[index];

                if (i == 5)
                    GameManager.Instance.PlayerItemMNG.EquipListEdit();

                break;
            }
        }
        PlayerScript.SetStat();
        GameManager.Instance.UIMNG.PlayerInfoMNG.SetEquipInfo();
    }

    void MaxLevelCheck(int index)
    {
        if (PlayerEquipLevel[index] == MAX_LEVEL)
        {
            PlayerItemManager PM = GameManager.Instance.PlayerItemMNG;
            int MaxLevelID = PlayerEquipID[index];

            PM.CanAppearEquip.RemoveAll(x => x == MaxLevelID);
        }
    }

    #endregion

    #region ValueManage

    public Stats EquipStatResult()
    {
        Stats result = new Stats();

        for(int i = 0; i < MAX_EQUIP_SPACE; i++)
        {
            if (isHaveEquip[i])
            {
                result += PlayerEquipList[i][PlayerEquipLevel[i]];
            }
            else
                break;
        }

        return result;
    }

    public int GetEquipLevel(int ID)
    {
        int index = Array.IndexOf(PlayerEquipID, ID);

        if (index == -1)
            return 0;
        return PlayerEquipLevel[index] + 1;
    }

    public int GetEquipID(int index) => PlayerEquipID[index];

    public bool GetHaveEquip(int i) => isHaveEquip[i];

    public int[] GetSkillInfo(int i) => new int[] { PlayerEquipID[i], PlayerEquipLevel[i] };
    
    #endregion
}
