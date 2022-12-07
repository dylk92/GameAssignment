using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Stats
{
    #region Variable
    
    public float MaxHP;
    public float ATK;
    public float MoveSpeed;
    public float ATKSpeed;
    public float DEF;
    public int CritPer;
    public float CritDMG;
    public float IncreaseDMG;
    public float ATKRange;
    public float AGI;

    #endregion

    public Stats(int HP)
    {
        MaxHP = HP;
        ATK = 1;
        MoveSpeed = ATKSpeed = 1f;
        DEF = AGI = 0;
        CritPer = 10;
        CritDMG = 1.5f;
        IncreaseDMG = IncreaseDMG = ATKRange = 1f;
    }

    public Stats(int HP, float atk) : this(HP)
    {
        ATK = atk;
    }

    public static Stats operator +(Stats A, Stats B)
    {
        A.MaxHP += B.MaxHP;
        A.ATK += B.ATK;
        A.MoveSpeed *= 1+B.MoveSpeed;
        A.ATKSpeed += B.ATKSpeed;
        A.DEF += B.DEF;
        A.CritPer += B.CritPer;
        A.CritDMG += B.CritDMG;
        A.IncreaseDMG += B.IncreaseDMG;
        A.ATKRange += B.ATKRange;
        A.AGI += B.AGI;

        return A;
    }

    public static Stats operator *(Stats A, PerStats B)
    {
        A.MaxHP *= B.MaxHP;
        A.ATK *= B.ATK;
        A.MoveSpeed *= B.MoveSpeed;
        A.ATKSpeed *= B.ATKSpeed;
        A.DEF *= B.DEF;
        A.AGI *= B.AGI;

        return A;
    }

    public static Stats operator +(Stats A, Equip B)
    {
        A.MaxHP += B.HP;
        A.ATK += B.ATK;
        A.MoveSpeed += B.MoveSpeed;
        A.ATKSpeed += B.ATKSpeed;
        A.DEF += B.DEF;
        A.CritPer += B.CritPer;
        A.CritDMG += B.CritDMG;
        A.IncreaseDMG += B.IncreaseDMG;
        A.ATKRange += B.ATKRange;
        A.AGI += B.AGI;

        return A;
    }
}

public struct MonsterStats
{
    public float MaxHP;
    public float CurHP;
    public float ATK;
    public float MoveSpeed;
    public int EXP;

    public MonsterStats(float _MaxHP, float _ATK, float _MoveSpeed, int _EXP)
    {
        MaxHP = CurHP = _MaxHP;
        ATK = _ATK;
        MoveSpeed = _MoveSpeed;
        EXP = _EXP;
    }
}

// 곱적용될 스탯
public struct PerStats
{
    public float MaxHP;
    public float ATK;
    public float MoveSpeed;
    public float ATKSpeed;
    public float DEF;
    public float AGI;


    public PerStats(int A)
    {
        MaxHP = ATK = MoveSpeed = ATKSpeed = DEF = AGI = A;
    }
}

abstract class Character
{
}

class Monster : Character
{
    public MonsterStats stat = new MonsterStats();

    private bool _IsCanATK = true;

    public bool IsCanATK
    {
        get { return _IsCanATK; }
        set { _IsCanATK = value; }
    }
    
    public void Init(MonsterStats stats)
    {
        stat = new MonsterStats(stats.MaxHP, stats.ATK, stats.MoveSpeed, stats.EXP);
    }

    public float GetMoveSpeed() => stat.MoveSpeed;
}

class PlayableCharacter : Character
{
    protected Stats stat;
    protected Stats resultStat;
    protected Stats _IncreaseStat;   // 프로퍼티 이용하기
    protected PerStats MultipleStat; // 프로퍼티 이용하기

    public float PlayerHP;

    public Stats IncreaseStat
    {
        get { return _IncreaseStat; }
        set { _IncreaseStat = value; }
    }

    public Stats ResultStat
    {
        get { return resultStat; }
    }

    public PlayableCharacter()
    {
        JobChange("Base");

        MultipleStat = new PerStats(1);
    }

    public void JobChange(string Job)
    {
        Stats Jstat = GameManager.Instance.PlayerMNG.CharacterStat[Job];

        stat.MaxHP = PlayerHP = Jstat.MaxHP;
        stat.ATK = Jstat.ATK;
        stat.MoveSpeed = Jstat.MoveSpeed;
        stat.ATKSpeed = Jstat.ATKSpeed;
        stat.DEF = Jstat.DEF;
        stat.CritPer = Jstat.CritPer;
        stat.CritDMG = Jstat.CritDMG;
        stat.IncreaseDMG = Jstat.IncreaseDMG;
        stat.ATKRange = Jstat.ATKRange;
        stat.AGI = Jstat.AGI;


        SetResultStat();
    }

    public void SetResultStat()
    {
        resultStat = (stat + IncreaseStat) * MultipleStat;
    }

    public void StatView()
    {
        Debug.Log("HP : " + resultStat.MaxHP);
        Debug.Log("ATK : " + resultStat.ATK);
        Debug.Log("MoveSpeed : " + resultStat.MoveSpeed);
        Debug.Log("ATKSpeed : " + resultStat.ATKSpeed);
        Debug.Log("DEF : " + resultStat.DEF);
        Debug.Log("CritPer : " + resultStat.CritPer);
        Debug.Log("CritDMG : " + resultStat.CritDMG);
        Debug.Log("IncreaseDMG : " + resultStat.IncreaseDMG);
        Debug.Log("ATKRange : " + resultStat.ATKRange);
        Debug.Log("AGI : " + resultStat.AGI);
    }

    public bool SubHP(float DMG)
    {
        PlayerHP -= DMG;

        if(PlayerHP <= 0)
        {
            PlayerHP = 0;

            return true;
        }
        return false;
    }

    public void PlsHP(float HP)
    {
        PlayerHP += HP;

        if(PlayerHP > stat.MaxHP)
            PlayerHP = stat.MaxHP;
    }

    public float HPper()
    {
        return PlayerHP / stat.MaxHP;
    }
}