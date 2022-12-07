using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Item
{
    Skill,
    Equip
}

public class PlayerItemManager : MonoBehaviour
{
    [SerializeField] SkillManager _SkillMNG;
    [SerializeField] EquipManager _EquipMNG;
    
    public SkillManager SkillMNG
    {
        get { return _SkillMNG; }
    }
    public EquipManager EquipMNG
    {
        get { return _EquipMNG; }
    }

    public List<int> CanAppearSkill = new List<int>();
    public List<int> CanAppearEquip = new List<int>();
    

    public void Init()
    {
        CanAppearSkill.Clear();
        CanAppearEquip.Clear();
    }

    public List<int[]> GetSelectList()
    {
        List<int[]> SendData;
        SendData = new List<int[]>();

        int SkillLength = CanAppearSkill.Count;
        int EquipLength = CanAppearEquip.Count;

        int SkillCount = 0;
        int EquipCount = 0;

        for (int i = 0; i < 3; i++)
        {
            int randType = Random.Range(0, 2);
            int randID;

            if (randType == 0 && SkillLength <= SkillCount)
            {
                if (EquipLength <= EquipCount)
                    randType = 2;
                else
                    randType = 1;
            }
            else if (randType == 1 && EquipLength <= EquipCount)
            {
                if (SkillLength <= SkillCount)
                    randType = 2;
                else
                    randType = 0;
            }

            if (randType == 0)
            {
                randID = Random.Range(0, SkillLength);
                SkillCount++;
            }
            else // if(randType == 1)
            {
                randID = Random.Range(0, EquipLength);
                EquipCount++;
            }
            //else
            //{
            //    // 체력 회복이라도 두자
            //}

            int[] dump = new int[2] { randType, randID };
            SendData.Add(dump);
        }

        for (int i = 1; i < SendData.Count; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (SendData[i][0] == SendData[j][0])
                {
                    if (SendData[i][1] == SendData[j][1])
                    {
                        SendData[i][1]++;
                        if (SendData[i][0] == 0 && SendData[i][1] == SkillLength)
                            SendData[i][1] = 0;
                        else if (SendData[i][0] == 1 && SendData[i][1] == EquipLength)
                            SendData[i][1] = 0;

                        i--;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < SendData.Count; i++)
        {
            switch (SendData[i][0])
            {
                case 0:
                    SendData[i][1] = CanAppearSkill[SendData[i][1]];
                    break;
                case 1:
                    SendData[i][1] = CanAppearEquip[SendData[i][1]];
                    break;
            }
        }

        return SendData;
    }

    // UI에서 정보를 받으면, 여기에서 처리
    // 처리하면서 스탯에 변동이 있으면 플레이어 스탯도 재설정
    public void InputItem(int index)
    {
        int[] Item = GameManager.Instance.UIMNG.SkillSelection.GetSelectedItem(index);

        if(Item[0] == 0)
            SkillMNG.InputPlayerSkill(Item[1]);
        else if(Item[0] == 1)
            EquipMNG.InputPlayerEquip(Item[1]);

        GameManager.Instance.UIMNG.ActiveSkillSelectUI(false);
    }

    public void SkillListEdit(string job)
    {
        CanAppearSkill.RemoveAll(x => SkillMNG.GetSkillJob(x) == job);
        for(int i = 0; i < 6; i++)
        {
            if (!SkillMNG.GetHaveSkill(i))
                break;
            int ID = SkillMNG.GetSkillInfo(i)[0];
            if (SkillMNG.GetSkillJob(ID) == job && SkillMNG.GetSkillLevel(ID) != 5)
                CanAppearSkill.Add(ID);
        }
    }

    public void EquipListEdit()
    {
        CanAppearEquip.Clear();
        for (int i = 0; i < 6; i++)
        {
            int ID = EquipMNG.GetEquipID(i);
            if(EquipMNG.GetEquipLevel(ID) != 4)
                CanAppearEquip.Add(ID);
        }
    }
}
