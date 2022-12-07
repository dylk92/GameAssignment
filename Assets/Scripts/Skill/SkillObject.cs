using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillObject : MonoBehaviour
{
    public Action MoveAction;
    public Action DamageAction;
    public Action DestroyAction;

    public SkillStat stat;
    public bool alreadyDamaged;
    public GameObject ignoreObject;
    public GameObject NextIgnoreObject;
    public Transform Target;
    BoxCollider2D BoxCol;
    CircleCollider2D CirCol;
    
    public int[] ChildInfo; // 0 : ID, 1 : Level
    Skill ChildSkill;
    public bool isSpawnEnemyLoc;
    public GameObject SpawnEnemyLoc;

    private void Awake()
    {
        BoxCol = GetComponent<BoxCollider2D>();
        CirCol = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region Init

    public void InitAction()
    {
        MoveAction = null;
        DamageAction = null;
        DestroyAction = null;
    }

    public void Init(SkillStat ss)
    {
        stat = ss;
        stat += GameManager.Instance.PlayerObject.GetComponent<Player>().GetStats();

        alreadyDamaged = false;
        ignoreObject = null;
        ChildInfo = new int[2];
        ChildSkill = null;
        isSpawnEnemyLoc = false;
        SpawnEnemyLoc = null;

        if (MoveAction != null)
            MoveAction();

        StartCoroutine(DestroyTimeout());
    }

    public void SetSize(float[] size)
    {
        float big = (size[0] > size[1]) ? size[0] : size[1];
        float ATKRange = GameManager.Instance.PlayerObject.GetComponent<Player>().GetStats().ATKRange;
        float bigsize = big * ATKRange;

        transform.localScale = new Vector3(bigsize, bigsize, 0);

        BoxCol.size = new Vector2((size[0] * ATKRange) / bigsize, (size[1] * ATKRange) / bigsize);
        CirCol.radius = 1;
    }

    public void SetChildLoc(GameObject go) => SpawnEnemyLoc = go;

    #endregion
    
    // 스킬 이동, 움직이지 않는 스킬은 설정 X
    #region Move

    public void MoveTracking()
    {
        transform.position = Target.position;
    }
    // 이 아래로 스피드 넣기
    public void MoveVector()
    {
        transform.Translate(Vector3.right * stat.Speed * Time.deltaTime);
    }

    public void MoveForwardSize()
    {
        transform.Translate(Vector3.right * transform.localScale.x * 0.5f);
    }
    public void MoveForwardSpeed()
    {
        transform.Translate(Vector3.right * stat.Speed);
    }

    public void MoveGuide()
    {
        Vector3 vec = (Target.position - transform.position).normalized;

        transform.Translate(vec * stat.Speed * Time.deltaTime);
    }

    public void MoveGuideAngle()
    {
        Vector3 vec = (Target.position - transform.position);
        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        transform.Translate(Vector3.right * stat.Speed * Time.deltaTime);
    }

    public void MoveGuideMouse()
    {
        Vector3 vec = GameManager.Instance.InputMNG.MouseVec.normalized;

        transform.Translate(vec * stat.Speed * Time.deltaTime);
    }

    public void MoveGuideMouseAngle()
    {
        Vector3 vec = GameManager.Instance.InputMNG.MouseVec;

        float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        transform.Translate(Vector3.right * stat.Speed * Time.deltaTime);
    }

    public void MoveRotation()
    {
        float vec = transform.rotation.eulerAngles.z + (stat.Speed);
        transform.rotation = Quaternion.AngleAxis(vec, Vector3.forward);
    }
    
    #endregion

    #region ObjectDestroy

    IEnumerator DestroyTimeout()
    {
        yield return GameManager.Instance.CoroutineMNG.GetWFS(stat.Lifespan);

        Destroy();
    }

    public void DestroyThis()
    {
        GetComponent<Animator>().SetInteger("ID", -1);
        transform.rotation = default;
        GameManager.Instance.PlayerItemMNG.SkillMNG.ActiveSkillList.Remove(gameObject);
        GameManager.Instance.CacheSys.RestoreCache(gameObject, "Skill");
    }

    #endregion

    #region ChildSkill
    
    public void SetChild(Skill childSkill, int childID, int Level)
    {
        ChildSkill = childSkill;
        ChildInfo[0] = childID;
        ChildInfo[1] = Level;
    }

    public void ChildLocationSkillPoint()
    {
        ChildSkill.ExtraTarget = gameObject;
    }

    public void ChildLocationSet(GameObject go)
    {
        ChildSkill.ExtraTarget = go;
    }

    public void ChildRotationSet()
    {
        ChildSkill.ExtraQuaternion = transform.rotation;
    }

    public void ChildActive()
    {
        SkillData ss = GameManager.Instance.PlayerItemMNG.SkillMNG.GetChildSkillData(ChildInfo[0], ChildInfo[1]);
        ChildSkill.Activate(ss);
    }

    public void StartChildSkillLoop()
    {
        StartCoroutine(ChildSkillLoop());
    }

    IEnumerator ChildSkillLoop()
    {
        SkillData ss = GameManager.Instance.PlayerItemMNG.SkillMNG.GetChildSkillData(ChildInfo[0], ChildInfo[1]);

        while (true)
        {
            yield return GameManager.Instance.CoroutineMNG.GetWFS(ss.skillCreateData.Cooltime);
            if (gameObject.activeSelf)
                ChildActive();
            else
                break;
        }
    }

    public void ChildAlreadyObject(GameObject go)
    {
        NextIgnoreObject = go;
    }

    public void GetNextIgnore()
    {
        if (ChildSkill != null)
        {
            ChildSkill.IgnoreObject = NextIgnoreObject;
            //Debug.Log(NextIgnoreObject);
        }
    }

    #endregion

    #region GetData

    void Move()
    {
        if(MoveAction != null)
            MoveAction();
    }

    public void Destroy()
    {
        DestroyAction();
    }

    #endregion
}