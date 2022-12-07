using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct ItemInfo
{
    public int ID;
    public string Name;
    public string Text;
}

public class SkillSelecter : MonoBehaviour
{
    [SerializeField] GameObject[] SelectionBox = new GameObject[3];
    [SerializeField] Image[] ImageBox = new Image[3];
    [SerializeField] RectTransform StatObject;
    [SerializeField] RectTransform SelectObject;
    
    UIManager UIMNG;
    PlayerItemManager PlayerItemMNG;

    Text[,] TextBox = new Text[3, 3];
    List<int[]> ItemBoxList;

    Dictionary<int, ItemInfo[]> SkillInfoList = new Dictionary<int, ItemInfo[]>();
    Dictionary<int, ItemInfo[]> EquipInfoList = new Dictionary<int, ItemInfo[]>();

    private void Awake()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        PlayerItemMNG = GameManager.Instance.PlayerItemMNG;
        UIMNG = GameManager.Instance.UIMNG;
        GenerateItemList(GameManager.Instance.CSVRead.SkillInfoList, ref SkillInfoList);
        GenerateItemList(GameManager.Instance.CSVRead.EquipInfoList, ref EquipInfoList);
        GetTextBox();
    }

    private void Start()
    {
        GetNewBox();
    }


    void GenerateItemList(List<Dictionary<string, string>> InfoList, ref Dictionary<int, ItemInfo[]> InfoDic)
    {
        string PlayerType = GameManager.Instance.PlayerMNG.ClassType;

        string ItemType = null;
        ItemInfo ItemData = new ItemInfo();

        for (int i = 0; i < InfoList.Count; i++)
        {
            if(InfoList[i]["Type"] != "") ItemType = InfoList[i]["Type"];
            if (ItemType == PlayerType)
            {
                if(InfoList[i]["ID"]  != "") ItemData.ID = Int32.Parse(InfoList[i]["ID"]);
                int Item_Level = Int32.Parse(InfoList[i]["Level"]);

                if(InfoList[i]["Name"] != "") ItemData.Name = InfoList[i]["Name"];
                ItemData.Text = InfoList[i]["Text"].Replace("_", ",");
                

                if (!InfoDic.ContainsKey(ItemData.ID))
                {
                    ItemInfo[] TempInfo = new ItemInfo[5];

                    InfoDic.Add(ItemData.ID, TempInfo);
                    InfoDic[ItemData.ID][Item_Level] = ItemData;
                }
                else
                {
                    if(InfoDic[ItemData.ID][Item_Level].Name == null)
                    {
                        InfoDic[ItemData.ID][Item_Level] = ItemData;
                    }
                }
            }
        }
    }

    void GetTextBox()
    {
        for (int i = 0; i < 3; i++)
        {

            TextBox[i, 0] = SelectionBox[i].transform.GetChild(0).gameObject.GetComponent<Text>();
            TextBox[i, 1] = SelectionBox[i].transform.GetChild(1).gameObject.GetComponent<Text>();
            TextBox[i, 2] = SelectionBox[i].transform.GetChild(2).gameObject.GetComponent<Text>();
        }
    }

    public void GetNewBox()
    {
        ItemBoxList = PlayerItemMNG.GetSelectList();
        
        BoxSetting();
    }
    

    // 레벨 업 신호를 받았을 시, 선택 상자를 세팅하는 함수
    void BoxSetting()
    {
        for (int i = 0; i < 3; i++)
        {
            int ID = ItemBoxList[i][1];
            int Level;

            if (ItemBoxList[i][0] == 0)
            {
                Level = PlayerItemMNG.SkillMNG.GetSkillLevel(ID);

                TextBox[i, 0].text = SkillInfoList[ID][Level].Name + "  Lv. " + (Level+1);
                TextBox[i, 1].text = SkillInfoList[ID][Level].Text;
                TextBox[i, 2].text = "Skill";
                ImageBox[i].sprite = UIMNG.GetSkillImage(ID);
            }
            else // if(ItemList[i][0] == 1)
            {
                Level = PlayerItemMNG.EquipMNG.GetEquipLevel(ID);

                TextBox[i, 0].text = EquipInfoList[ID][Level].Name + "  Lv. " + (Level+1);
                TextBox[i, 1].text = EquipInfoList[ID][Level].Text;
                TextBox[i, 2].text = "Equip";
                ImageBox[i].sprite = UIMNG.GetEquipImage(ID);
            }
        }
    }

    public void SkillSelecterAppear()
    {
        float StatSize = StatObject.rect.size.x;
        float SelectSize = SelectObject.rect.size.x;

        StatObject.anchoredPosition = new Vector2(-StatSize, 0);
        SelectObject.anchoredPosition = new Vector2(SelectSize, 0);

        StartCoroutine(MoveUI());
    }

    IEnumerator MoveUI()
    {
        while (true)
        {
            StatObject.anchoredPosition += new Vector2(16, 0);
            SelectObject.anchoredPosition += new Vector2(-48, 0);

            if(StatObject.anchoredPosition.x >= 0 && SelectObject.anchoredPosition.x <= 0)
            {
                StatObject.anchoredPosition = new Vector2(0, 0);
                SelectObject.anchoredPosition = new Vector2(0, 0);
                break;
            }
            yield return null;
        }
    }


    // 세팅하면서 세팅한 값을 변수로 저장, 선택된 스킬을 스킬매니저에 건네주기
    // 상자를 클릭했을 때, 상자의 정보 받아오는 함수
    // 예외처리 하기
    public int[] GetSelectedItem(int num)
    {
        GameManager.Instance.UIMNG.EXPMNG.GagueUpdate();

        return ItemBoxList[num];
    }
}
