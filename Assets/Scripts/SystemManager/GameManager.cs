using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variable

    static GameManager instance = null;

    [SerializeField] GameObject _PlayerObject;
    [SerializeField] PlayerManager _PlayerMNG;
    [SerializeField] InputManager _InputMNG;
    [SerializeField] EnemyManager _EnemyMNG;
    [SerializeField] PlayerItemManager _PlayerItemMNG;
    [SerializeField] TilemapController Tilemap;
    [SerializeField] UIManager _UIMNG;
    CoroutineManager _CoroutineMNG;
    CSVReader _CSVReader;
    CacheSystem _CacheSys;

    private bool _isGame = true;
    private int _Score = 0;
    private int _PlayerEXP = 0;

    #endregion

    #region Property
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public CoroutineManager CoroutineMNG
    {
        get { return _CoroutineMNG; }
    }

    public GameObject PlayerObject
    {
        get { return _PlayerObject; }
    }

    public PlayerManager PlayerMNG
    {
        get { return _PlayerMNG; }
    }

    public InputManager InputMNG
    {
        get { return _InputMNG; }
    }

    public EnemyManager EnemyMNG
    {
        get { return _EnemyMNG; }
    }

    public PlayerItemManager PlayerItemMNG
    {
        get { return _PlayerItemMNG; }
    }

    public UIManager UIMNG
    {
        get { return _UIMNG; }
    }

    public CSVReader CSVRead
    {
        get { return _CSVReader; }
    }

    public CacheSystem CacheSys
    {
        get { return _CacheSys; }
    }

    public bool isGame
    {
        get { return _isGame; }
        set { _isGame = value; }
    }

    public int Score
    {
        get { return _Score; }
        set { _Score = value; }
    }
    public int PlayerEXP
    {
        get { return _PlayerEXP; }
        set { _PlayerEXP = value; }
    }

    #endregion


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _CoroutineMNG = new CoroutineManager();

        _CSVReader = new CSVReader();
        CSVRead.AwakeParse();

        _CacheSys = new CacheSystem();
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        PlayerObject.GetComponent<Player>().Init();
        EnemyMNG.Init();
        PlayerMNG.Init();
        PlayerItemMNG.Init();
        PlayerItemMNG.EquipMNG.Init();
        PlayerItemMNG.SkillMNG.Init();
        Tilemap.Init();
        UIMNG.Init();

        PlayerObject.GetComponent<Player>().SetStat();
        Time.timeScale = 1;
    }
}



/* 다음 할 것 : 
 * 몬스터 행렬 만들고 시간따라 강화하기
 * 특정 시간에 이벤트 만들기(몬스터 범람, 보스 등)
 * 설명 보충하기
 */
 
