using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] SkillSelecter _SkillSelection;
    [SerializeField] JobSelecter _JobSelection;
    [SerializeField] EXPBarManager _EXPMNG;
    [SerializeField] StatViewer _StatViewer;
    [SerializeField] DamageTextController _DMGController;
    [SerializeField] PlayerInfoManager _PlayerInfoMNG;
    [SerializeField] Timer _GameTimer;
    [SerializeField] GameOverScreen _GameOver;
    [SerializeField] GamePause _GamePause;

    public SkillSelecter SkillSelection => _SkillSelection;
    public JobSelecter JobSelection => _JobSelection;
    public EXPBarManager EXPMNG => _EXPMNG;
    public StatViewer StatViewer => _StatViewer;
    public DamageTextController DMGController => _DMGController;
    public PlayerInfoManager PlayerInfoMNG => _PlayerInfoMNG;
    public Timer GameTimer => _GameTimer;
    public GameOverScreen GameOver => _GameOver;
    public GamePause GamePause => _GamePause;

    [NonSerialized] public Sprite[] SkillIcon;
    [NonSerialized] public Sprite[] EquipIcon;

    void Awake()
    {
        SkillIcon = Resources.LoadAll<Sprite>("Image/SkillIcon");
        EquipIcon = Resources.LoadAll<Sprite>("Image/EquipIcon");
    }
    // Start is called before the first frame update
    void Start()
    {
        ActiveSkillSelectUI(false);
        ActiveJobSelectUI(false);
    }
    

    public void Init()
    {
        EXPMNG.Init();
        GameTimer.Init();
        PlayerInfoMNG.Init();
        SkillSelection.gameObject.SetActive(false);
        JobSelection.gameObject.SetActive(false);
        GameOver.gameObject.SetActive(false);
        GamePause.gameObject.SetActive(false);
    }

    public void ActiveSkillSelectUI(bool active)
    {
        Time.timeScale = active ? 0 : 1;

        if(active)
            SkillSelection.GetNewBox();

        StatViewer.SetStat();
        SkillSelection.gameObject.SetActive(active);
    }

    public void ActiveJobSelectUI(bool active)
    {
        Time.timeScale = active ? 0 : 1;

        if (active)
            JobSelection.GetNewBox();
        
        JobSelection.gameObject.SetActive(active);
    }

    public void Pause()
    {
        if (Time.timeScale == 0)
            return;
        Time.timeScale = 0;
        GamePause.gameObject.SetActive(true);
    }

    public Sprite GetSkillImage(int ID)
    {
        for (int i = 0; i < SkillIcon.Length; i++)
        {
            string[] image = SkillIcon[i].name.Split('_');
            if (image[1] == ID.ToString())
            {
                return SkillIcon[i];
            }
        }
        return null;
    }

    public Sprite GetEquipImage(int ID)
    {
        for (int i = 0; i < EquipIcon.Length; i++)
        {
            string[] image = EquipIcon[i].name.Split('_');
            if (image[1] == ID.ToString())
            {
                return EquipIcon[i];
            }
        }
        return null;
    }
}
