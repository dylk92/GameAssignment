using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyData
{
    public float SpawnTime;
    public float SpawnDelay;
    public MonsterStats stats;
    public int EXP;
}

public class EnemyManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    IEnumerator SpawnCounter;
    Timer GameTimer;

    Dictionary<int, EnemyData> EnemyDataDic = new Dictionary<int, EnemyData>();

    public List<GameObject> ActiveEnemyList = new List<GameObject>();

    EnemyData ReadyEnemy;
    public int EnemyLevel;

    bool _CanSpawn;
    public bool CanSpawn
    {
        get { return _CanSpawn; }
        set
        {
            Debug.Log(value);
            _CanSpawn = value;
            if (value)
            {
                SpawnCounter = SpawnTimer();
                StartCoroutine(SpawnCounter);
            }
        }
    }

    private void Awake()
    {
        GenerateEnemy();
        ReadyEnemy = new EnemyData();
        GameTimer = GameManager.Instance.UIMNG.GameTimer;
        SetMonsterDic();
    }
    

    public void Init()
    {
        if (SpawnCounter != null)
            StopCoroutine(SpawnCounter);

        _CanSpawn = true;
        EnemyLevel = 0;
        ReadyEnemy = EnemyDataDic[0];

        ListClear();

        SpawnCounter = SpawnTimer();
        StartCoroutine(SpawnCounter);
    }

    void ListClear()
    {
        for(int i = ActiveEnemyList.Count-1; i >= 0; i--)
        {
            ActiveEnemyList[i].GetComponent<Enemy>().Destroy();
        }
    }

    private void Update()
    {
        if (EnemyDataDic.Count - 1 != EnemyLevel)
            TimeCheck();
    }

    #region DataSetting

    void SetMonsterDic()
    {
        List<Dictionary<string, string>> DataList = GameManager.Instance.CSVRead.EnemyDataList;

        for(int i = 0; i < DataList.Count; i++)
        {
            EnemyData data = new EnemyData();
            MonsterStats stat = new MonsterStats();

            int Level = Int32.Parse(DataList[i]["Level"]);

            stat.ATK = float.Parse(DataList[i]["ATK"]);
            stat.MaxHP = float.Parse(DataList[i]["HP"]);
            stat.MoveSpeed = float.Parse(DataList[i]["MoveSpeed"]);
            stat.EXP = Int32.Parse(DataList[i]["EXP"]);

            data.SpawnTime = float.Parse(DataList[i]["SpawnTime"]);
            data.SpawnDelay = float.Parse(DataList[i]["SpawnDelay"]);
            data.stats = stat;

            EnemyDataDic.Add(Level, data);
        }
    }

    public MonsterStats GetEnemyStat() => ReadyEnemy.stats;

    #endregion

    #region LV Check

    void TimeCheck()
    {
        if (EnemyDataDic[EnemyLevel].SpawnTime <= GameTimer.GameTime)
            EnemyLevelUp();
    }

    void EnemyLevelUp()
    {
        EnemyLevel++;
        ReadyEnemy = EnemyDataDic[EnemyLevel];
    }

    #endregion

    #region EnemySpawner

    void GenerateEnemy()
    {
        if (EnemyPrefab == null)
        {
            Debug.LogWarning("EnemyPrefab is null");
            return;
        }

        GameManager.Instance.CacheSys.GenerateCache(EnemyPrefab, "Enemy", 500);
    }
    
    public void SpawnEnemy()
    {
        GameObject newEnemy = GameManager.Instance.CacheSys.TakeCache("Enemy");

        if (newEnemy == null)
            return;

        newEnemy.SetActive(true);
        newEnemy.transform.position = SetRandomPosition();
        Enemy script = newEnemy.GetComponent<Enemy>();
        script.Init(ReadyEnemy.stats);

        ActiveEnemyList.Add(newEnemy);
    }

    Vector3 SetRandomPosition()
    {
        Vector3 vec = new Vector3();
        Vector3 PlayerLoc = GameManager.Instance.PlayerObject.transform.position;

        float x = 30;
        float y = 20;

        int WH = UnityEngine.Random.Range(0, 2);
        int SE = UnityEngine.Random.Range(0, 2);

        if(WH == 0)
        {
            if(SE == 0)
            {
                vec.x = PlayerLoc.x + x;
                vec.y = PlayerLoc.y + UnityEngine.Random.Range(-y, y);
            }
            else
            {
                vec.x = PlayerLoc.x - x;
                vec.y = PlayerLoc.y + UnityEngine.Random.Range(-y, y);
            }
        }
        else
        {
            if (SE == 0)
            {
                vec.x = PlayerLoc.x + UnityEngine.Random.Range(-x, x);
                vec.y = PlayerLoc.y + y;
            }
            else
            {
                vec.x = PlayerLoc.x + UnityEngine.Random.Range(-x , x );
                vec.y = PlayerLoc.y - y;
            }
        }

        return vec;
    }

    IEnumerator SpawnTimer()
    {
        while (CanSpawn)
        {
            SpawnEnemy();
            yield return GameManager.Instance.CoroutineMNG.GetWFS(ReadyEnemy.SpawnDelay);
        }
    }

    #endregion
}
