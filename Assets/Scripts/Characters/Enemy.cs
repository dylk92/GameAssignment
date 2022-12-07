using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Dictionary<SkillObject, bool> HitSkill;

    Monster Stat;
    Transform TargetTransform;
    SpriteRenderer spriteRenderer;
    
    int isKnockBack;
    float KnockBackPower;
    float[] DebuffPower;

    IEnumerator[] DebuffCoro;

    private void Awake()
    {
        HitSkill = new Dictionary<SkillObject, bool>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        DebuffCoro = new IEnumerator[2];
    }

    void FixedUpdate()
    {
        Move();
    }

    #region Init

    public void Init(MonsterStats stats)
    {
        Stat = new Monster();

        TargetTransform = GameManager.Instance.PlayerObject.transform;
        HitSkill.Clear();

        isKnockBack = 0;
        KnockBackPower = 0;
        DebuffPower = new float[2] {0, 0};

        DebuffClear();

        Stat.Init(stats);
    }

    #endregion

    #region Move

    void Move()
    {
        Vector3 targetVec = TargetTransform.position - transform.position;
        targetVec = targetVec.normalized;
        Vector3 EnemySpeed;

        EnemySpeed = targetVec * Stat.GetMoveSpeed() * Time.deltaTime;

        if (0 < EnemySpeed.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;

        if (isKnockBack > 0)
        {
            EnemySpeed = EnemySpeed * -KnockBackPower;
        }
        else
        {
            EnemySpeed -= EnemySpeed * DebuffPower[0];
        }

        transform.Translate(EnemySpeed);
    }

    #endregion

    #region CrashCheck

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!gameObject.activeSelf)
            return;
        if (col.gameObject.layer == LayerMask.NameToLayer("Skill"))
        {
            SkillObject skill = col.gameObject.GetComponent<SkillObject>();
            if (skill.stat.Breakable)
            {
                if (skill.alreadyDamaged)
                    return;
                else if (skill.ignoreObject == gameObject)
                    return;
                skill.alreadyDamaged = true;
                skill.ChildAlreadyObject(gameObject);
                OnHit(skill);
                if (skill.SpawnEnemyLoc)
                    skill.SetChildLoc(gameObject);
                skill.Destroy();
            }
            else
            {
                if (!HitSkill.ContainsKey(skill))
                {
                    HitSkill.Add(skill, true);
                    StartCoroutine(HitCoroutine(skill));
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Skill"))
        {
            SkillObject skill = col.gameObject.GetComponent<SkillObject>();
            if (!skill.stat.Breakable)
            {
                HitSkill[skill] = false;
            }
        }
    }

    #endregion

    #region Hit&Destroy

    IEnumerator HitCoroutine(SkillObject SO)
    {
        if (HitSkill.ContainsKey(SO))
        {
            while (HitSkill[SO])
            {
                OnHit(SO);
                yield return GameManager.Instance.CoroutineMNG.GetWFS(SO.stat.ATKDelay);
            }
            HitSkill.Remove(SO);
        }
    }

    public void OnHit(SkillObject SO)
    {
        if (SO.stat.KnockBack != 0)
        {
            if(gameObject.activeSelf)
                StartCoroutine(GetKnockBack(SO.stat.KnockBack));
        }

        float DMG = SO.stat.DMG;
        bool isCrit = false;

        if (SO.stat.CritPer > 0) // 크리티컬 히트 체크
        {
            if (SO.stat.CritPer >= Random.Range(1, 101))
            {
                DMG *= SO.stat.CritDMG;
                isCrit = true;
            }
        }
        if (SO.stat.Debuff != null)
            GetDebuff(SO.stat.Debuff);

        GetDamage(DMG, isCrit);
    }

    void GetDamage(float DMG, bool Crit)
    {
        Stat.stat.CurHP -= DMG;

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos = Camera.main.ScreenToWorldPoint(pos);

        GameManager.Instance.UIMNG.DMGController.CreateDamageText(pos, (int)DMG, Crit);

        DeadCheck();
    }
    
    IEnumerator GetKnockBack(float power)
    {
        isKnockBack++;
        KnockBackPower = power;
        yield return GameManager.Instance.CoroutineMNG.GetWFS(0.1f);

        if (--isKnockBack == 0)
            KnockBackPower = 0;

    }

    void DeadCheck()
    {
        if (Stat.stat.CurHP <= 0)
        {
            Stat.stat.CurHP = 0;

            Destroy();
        }
    }

    public void Destroy()
    {
        GameManager.Instance.PlayerMNG.EXPPlus(Stat.stat.EXP);
        GameManager.Instance.CacheSys.RestoreCache(gameObject, "Enemy");
        GameManager.Instance.EnemyMNG.ActiveEnemyList.Remove(gameObject);
    }

    #endregion

    #region Debuff
    
    void GetDebuff(List<float[]> debuff)
    {
        // [타입, 시간, 파워]
        // 타입 0 : 슬로우, 1 : 독(0.5초마다 데미지)
        for (int i = 0; i < debuff.Count; i++)
        {
            if (debuff[i][0] == 0)
            {
                if (DebuffCoro[0] != null)
                    StopCoroutine(DebuffCoro[0]);
                DebuffCoro[0] = StartSlow(debuff[i][1], debuff[i][2]);
                StartCoroutine(DebuffCoro[0]);
            }
            else if (debuff[i][0] == 1)
            {
                if (DebuffCoro[1] != null)
                    StopCoroutine(DebuffCoro[1]);
                DebuffCoro[1] = StartDOT(debuff[i][1], debuff[i][2]);
                StartCoroutine(DebuffCoro[1]);
            }
        }
    }

    IEnumerator StartSlow(float time, float power)
    {
        DebuffPower[0] = power;
        yield return GameManager.Instance.CoroutineMNG.GetWFS(time);
        DebuffPower[0] = 0;
    }

    IEnumerator StartDOT(float time, float power)
    {
        DebuffPower[1] = power;
        for (int i = 0; i < time * 2; i++)
        {
            yield return GameManager.Instance.CoroutineMNG.GetWFS(0.5f);
            GetDamage(DebuffPower[1], false);
        }
    }

    void DebuffClear()
    {
        for (int i = 0; i < DebuffCoro.Length; i++)
        {
            if (DebuffCoro[i] != null)
                StopCoroutine(DebuffCoro[i]);
        }
    }

    #endregion

    #region Damage

    public float getDamage()
    {
        if(Stat.IsCanATK)
        {
            StartCoroutine(SetCanATK());
            return Stat.stat.ATK;
        }
        return 0;
    }

    IEnumerator SetCanATK()
    {
        Stat.IsCanATK = false;
        yield return GameManager.Instance.CoroutineMNG.GetWFS(1);
        Stat.IsCanATK = true;
    }

    #endregion
}
