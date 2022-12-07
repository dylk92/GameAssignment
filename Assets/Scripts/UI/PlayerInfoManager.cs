using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour
{
    const int MAX_COUNT = 6;

    [SerializeField] Image[] SkillImage = new Image[MAX_COUNT];
    [SerializeField] Image[] EquipImage = new Image[MAX_COUNT];
    [SerializeField] Text[] SkillText = new Text[MAX_COUNT];
    [SerializeField] Text[] EquipText = new Text[MAX_COUNT];


    SkillManager SkillMNG;
    EquipManager EquipMNG;
    UIManager UIMNG;

    void Awake()
    {
        SkillMNG = GameManager.Instance.PlayerItemMNG.SkillMNG;
        EquipMNG = GameManager.Instance.PlayerItemMNG.EquipMNG;
        UIMNG = GameManager.Instance.UIMNG;
    }

    // Start is called before the first frame update

    public void Init()
    {
        for(int i = 0; i < MAX_COUNT; i++)
        {
            SkillImage[i].sprite = null;
            EquipImage[i].sprite = null;
            SkillText[i].text = "";
            EquipText[i].text = "";
        }

        SetSkillInfo();
        SetEquipInfo();
    }

    public void SetSkillInfo()
    {
        for(int i = 0; i < MAX_COUNT; i++)
        {
            if (SkillMNG.GetHaveSkill(i))
            {
                int[] info = SkillMNG.GetSkillInfo(i);

                SkillImage[i].sprite = UIMNG.GetSkillImage(info[0]);
                SkillImage[i].color = new Color(1, 1, 1, 1);
                SkillText[i].text = "LV." + (info[1] + 1);
            }
            else
            {
                SkillImage[i].color = new Color(1, 1, 1, 0);
                SkillText[i].text = "";
            }
        }
    }

    public void SetEquipInfo()
    {
        for(int i = 0; i < MAX_COUNT; i++)
        {
            if (EquipMNG.GetHaveEquip(i))
            {
                int[] info = EquipMNG.GetSkillInfo(i);

                EquipImage[i].sprite = UIMNG.GetEquipImage(info[0]);
                EquipImage[i].color = new Color(1, 1, 1, 1);
                EquipText[i].text = "LV." + (info[1] + 1);
            }
            else
            {
                EquipImage[i].color = new Color(1, 1, 1, 0);
                EquipText[i].text = "";
            }
        }
    }
}
