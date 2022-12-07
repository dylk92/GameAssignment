using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region SkillData

public struct SkillData
{
    public SkillInfo skillInfo;
    public SkillStat skillStat;
    public SkillCreateData skillCreateData;
    public int ChildID;
}

public struct SkillInfo
{
    public string Type;
    public string Job;
    public string Name;
    public int ID;
    public int Level;
}

[Serializable]
public struct SkillStat
{
    public float Lifespan;
    public float DMG;
    public int CritPer;
    public float CritDMG;
    public float Speed;
    public float KnockBack;
    public List<float[]> Debuff; // [타입, 시간, 파워]
    public bool Breakable;
    public float ATKDelay;

    public static SkillStat operator +(SkillStat A, Stats B)
    {
        A.DMG *= B.ATK * B.IncreaseDMG;
        A.CritPer = B.CritPer;
        A.CritDMG = B.CritDMG;

        return A;
    }
}

[Serializable]
public struct SkillCreateData
{
    public float Cooltime;
    public int SpawnCount;
    public float SpawnAngle;
    public float CreateDelay;
    public float[] Size;
    public string Shape;
}

#endregion

[Serializable]
public class Skill
{
    #region Variable
    
    protected GameObject[] Skills;

    protected GameObject player;
    public GameObject ExtraTarget;
    public Quaternion ExtraQuaternion;
    public GameObject IgnoreObject;
    protected Skill ChildSkill;
    protected SkillData skilldata;
    public Transform SpawnTargetTransform = null;
    #endregion

    /// <summary>
    /// 스킬을 실행 할 몸통. 이곳에 구동부를 입력하여 스킬 가동
    /// </summary>
    public virtual void Activate(SkillData ss)
    {
        SetData(ss);
        player = GameManager.Instance.PlayerObject;
    }


    // 스킬 구동 단계를 분리, 각 단계마다 최대 하나의 메서드를 참조
    // [준비 - 생성 - 생성위치 지정 - 이동위치 지정 - 이동 - 피격 - 파괴]의 단계


    // 스킬의 데이터 받기
    public void SetData(SkillData ss) => skilldata = ss;

    #region Active
    // 스킬 생성 전, 몇개를 어떻게 생성할지 받기
    #region Prepare
        
    protected void Prepare()
    {
        int count = skilldata.skillCreateData.SpawnCount;
        float angle = skilldata.skillCreateData.SpawnAngle;
        skilldata.skillCreateData.SpawnCount = count;
        skilldata.skillCreateData.SpawnAngle = angle;
        Skills = new GameObject[count];
    }

    #endregion
    
    // 스킬 생성 시, 하나 이상이라면, 딜레이를 어떻게 줄지
    #region Create
        
    protected void CreateOnce()
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i] = SpawnSkill();
            Skills[i].SetActive(true);
            SkillActionInit(Skills[i]);
            SkillInit(Skills[i]);
        }
        SetImage();
    }

    protected void CreateDelay()
    {
        float delay = skilldata.skillCreateData.CreateDelay;

        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i] = SpawnSkill();
            SkillActionInit(Skills[i]);
        }
        SetImage();
        CoroutineHandler.Start_Coroutine(ActiveWithDelay(delay), delay * skilldata.skillCreateData.SpawnCount);
    }
    IEnumerator ActiveWithDelay(float delay)
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i].SetActive(true);
            if (SpawnTargetTransform != null)
                Skills[i].transform.position = SpawnTargetTransform.position;
            SkillInit(Skills[i]);
            yield return GameManager.Instance.CoroutineMNG.GetWFS(delay);
        }
    }

    protected void CreateDelayWithAngle()
    {
        float delay = skilldata.skillCreateData.CreateDelay;

        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i] = SpawnSkill();
            SkillActionInit(Skills[i]);
        }
        SetImage();
        CoroutineHandler.Start_Coroutine(ActiveWithDelayAngle(delay), delay * skilldata.skillCreateData.SpawnCount);
    }
    IEnumerator ActiveWithDelayAngle(float delay)
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i].SetActive(true);
            if (SpawnTargetTransform != null)
                Skills[i].transform.position = SpawnTargetTransform.position;
            SkillInit(Skills[i]);

            Vector3 vec = GameManager.Instance.InputMNG.MouseVec;
            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Skills[i].transform.rotation = rotation;
            Skills[i].transform.Translate(Vector3.right * skilldata.skillCreateData.Size[0] * 0.5f);

            yield return GameManager.Instance.CoroutineMNG.GetWFS(delay);
        }
    }

    #endregion

    #region Set Shape

    void SetImage()
    {
        bool rect = false;
        bool circle = false;

        switch (skilldata.skillCreateData.Shape)
        {
            case "Rect":
                rect = true;
                break;
            case "Circle":
                circle = true;
                break;
        }
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i].GetComponent<BoxCollider2D>().enabled = rect;
            Skills[i].GetComponent<CircleCollider2D>().enabled = circle;

            Animator anim = Skills[i].GetComponent<Animator>();

            
            if (anim != null)
            {
                //Skills[i].GetComponent<Animator>().runtimeAnimatorController;
            }
        }
    }

    #endregion

    // 스킬의 등장위치 지정
    #region Set SpawnLocation

    protected void SpawnMouseLocation()
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Vector3 vec = GameManager.Instance.InputMNG.GetMouseLocation();
            vec += GameManager.Instance.PlayerObject.transform.position;
            Skills[i].transform.position = vec;
        }
    }

    protected void SpawnTargetLocation(GameObject target)
    {
        for(int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i].transform.position = target.transform.position;
        }
    }

    #endregion
    
    // 이동할 위치 지정, 움직이지 않는 스킬은 설정 X
    #region Set MoveLocation

    protected void MoveMouseLocation()
    {
        Vector3 vec = GameManager.Instance.InputMNG.MouseVec;

        SetLotationVector(vec);
    }

    protected void MoveTargetLocation(GameObject target)
    {
        if(target == null)
        {
            Debug.LogWarning("target is null");

            target = player;
        }
        Vector3 vec = target.transform.position - Skills[0].transform.position;

        SetLotationVector(vec);
    }

    protected void MoveTargetTracking(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("target is null");
        }
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i].GetComponent<SkillObject>().Target = target.transform;
        }
    }

    public void SetLotationVector(Vector3 vec)
    {
        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;

        SetQuaternion(angle);
    }

    public void SetLotationQuaternion(Quaternion rotation)
    {
        float angle = rotation.eulerAngles.z;

        SetQuaternion(angle);
    }

    public void SetQuaternion(float angle)
    {
        float multiAngle = 0;

        if (skilldata.skillCreateData.SpawnAngle != 0)
        {
            multiAngle = skilldata.skillCreateData.SpawnAngle / (skilldata.skillCreateData.SpawnCount - 1);
            angle -= skilldata.skillCreateData.SpawnAngle * 0.5f;
        }

        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            if (skilldata.skillCreateData.SpawnAngle == 0)
            {
                var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                Skills[i].transform.rotation = rotation;
                Skills[i].transform.Translate(Vector3.right * skilldata.skillCreateData.Size[0] * 0.5f);
            }
            else
            {
                var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                angle += multiAngle;

                Skills[i].transform.rotation = rotation;
                Skills[i].transform.Translate(Vector3.right * skilldata.skillCreateData.Size[0] * 0.5f);
            }
        }
    }

    #endregion

    #region Move

    /// <summary>
    /// 이동할 함수 지정
    /// 0 : MoveVector,  1 : MoveTracking, 2 : MoveForwardSize
    /// 3 : MoveForwardSpeed, 4 : MoveGuide, 5 : MoveGuideAngle
    /// 6 : MoveGuideMouse, 7 : MoveGuideMouseAngle
    /// </summary>
    protected void SetMoveAction(int num = 0)
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Action action = null;
            switch (num)
            {
                case 0:
                    action += Skills[i].GetComponent<SkillObject>().MoveVector;
                    break;
                case 1:
                    action += Skills[i].GetComponent<SkillObject>().MoveTracking;
                    break;
                case 2:
                    action += Skills[i].GetComponent<SkillObject>().MoveForwardSize;
                    break;
                case 3:
                    action += Skills[i].GetComponent<SkillObject>().MoveForwardSpeed;
                    break;
                case 4:
                    action += Skills[i].GetComponent<SkillObject>().MoveGuide;
                    break;
                case 5:
                    action += Skills[i].GetComponent<SkillObject>().MoveGuideAngle;
                    break;
                case 6:
                    action += Skills[i].GetComponent<SkillObject>().MoveGuideMouse;
                    break;
                case 7:
                    action += Skills[i].GetComponent<SkillObject>().MoveGuideMouseAngle;
                    break;
                case 8:
                    action += Skills[i].GetComponent<SkillObject>().MoveRotation;
                    break;
            }
            Skills[i].GetComponent<SkillObject>().MoveAction += action;
        }
    }

    #endregion

    // 적에게 데미지를 줄 때, 다단히트인지 아닌지 체크
    #region Damage
    // 지금은 일단 Enemy에서 처리하는 걸로
    protected void DamageOnce() { }

    protected void DamageMulti() { }

    #endregion

    // 적중 시 적에게 가할 효과, 없으면 설정 X
    #region HitEffect

    protected void EffectKnockback()
    {
        if (skilldata.skillStat.KnockBack <= 0)
            return;
        // Enemy에서 연동 되도록 설계하기
    }

    protected void EffectDebuff()
    {
        if (skilldata.skillStat.Debuff.Count <= 0)
            return;
        // Enemy와 연동되도록
    }

    #endregion

    #region ObjectDestroy

    /// <summary>
    /// 파괴 형식 지정
    /// 0 : DestroyThis, 1 : ChildActve
    /// </summary>
    protected void SetDestroyAction(int num = 0)
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Action action = null;
            switch (num)
            {
                case 0:
                    action += Skills[i].GetComponent<SkillObject>().DestroyThis;
                    break;
                case 1:
                    action += Skills[i].GetComponent<SkillObject>().ChildActive;
                    break;

            }

            Skills[i].GetComponent<SkillObject>().DestroyAction += action;
        }
    }

    protected void SetDestroyAction(Action action)
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i].GetComponent<SkillObject>().DestroyAction += action;
        }
    }

    #endregion

    #region Child

    /// <summary>
    ///  0 : SetChild, 1 : ChildSpawnLoc, 2 : ChildRotation, 3 : ChildSpawnEnemyLoc
    /// </summary>
    protected void SetChild(int num = 0)
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            switch (num)
            {
                case 0:
                    Skills[i].GetComponent<SkillObject>().SetChild(ChildSkill, skilldata.ChildID, skilldata.skillInfo.Level);
                    break;
                case 1:
                    Skills[i].GetComponent<SkillObject>().ChildLocationSkillPoint();
                    break;
                case 2:
                    Skills[i].GetComponent<SkillObject>().ChildRotationSet();
                    break;
                case 3:
                    Skills[i].GetComponent<SkillObject>().isSpawnEnemyLoc = true;
                    break;
            }
        }
    }
    // 자식스킬 생성루프 시작 함수
    protected void ChildCreateLoop()
    {
        for (int i = 0; i < skilldata.skillCreateData.SpawnCount; i++)
        {
            Skills[i].GetComponent<SkillObject>().StartChildSkillLoop();
        }
    }

    #endregion

    #endregion

    GameObject SpawnSkill() => GameManager.Instance.CacheSys.TakeCache("Skill");

    void SkillActionInit(GameObject skill)
    {
        skill.GetComponent<SkillObject>().InitAction();

    }

    void SkillInit(GameObject skill)
    {
        SkillObject script = skill.GetComponent<SkillObject>();
        Animator anim = skill.GetComponent<Animator>();
        
        GameManager.Instance.PlayerItemMNG.SkillMNG.ActiveSkillList.Add(skill);

        script.Init(skilldata.skillStat);
        script.SetSize(skilldata.skillCreateData.Size);
        if (IgnoreObject != null)
            script.ignoreObject = IgnoreObject;
        anim.SetInteger("ID", skilldata.skillInfo.ID);
    }

    protected GameObject FindNearEnemy(GameObject pivot)
    {
        GameObject NearEnemy = null;
        float NearDistance = Mathf.Infinity;

        foreach (GameObject go in GameManager.Instance.EnemyMNG.ActiveEnemyList)
        {
            if (go.activeSelf && go != pivot)
            {
                float distance = (pivot.transform.position - go.transform.position).sqrMagnitude;
                if(distance < NearDistance)
                {
                    NearDistance = distance;
                    NearEnemy = go;
                }
            }
        }
        
        return NearEnemy;
    }
}