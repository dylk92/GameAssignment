using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobSelecter : MonoBehaviour
{
    const int MAX_SELECT_COUNT = 2;

    [SerializeField] RectTransform Box;
    [SerializeField] Image[] JobImage = new Image[MAX_SELECT_COUNT];
    [SerializeField] Text[] JobNameText = new Text[MAX_SELECT_COUNT];
    [SerializeField] Text[] JobInfoText = new Text[MAX_SELECT_COUNT];

    UIManager UIMNG;
    PlayerManager PlayerMNG;

    public string[] newJob;


    private void Awake()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        UIMNG = GameManager.Instance.UIMNG;
        PlayerMNG = GameManager.Instance.PlayerMNG;
    }


    public void GetNewBox()
    {
        int index = 0;
        for (int i = 1; i < 3; i++) {
            if (GameManager.Instance.PlayerMNG.ClassJob[i] == "")
            {
                index = i;
                break;
            }
        }
        newJob = PlayerMNG.GetRandJob(index);

        for(int i = 0; i < MAX_SELECT_COUNT; i++)
        {
            //JobImage[i].sprite = "";
            JobNameText[i].text = PlayerMNG.CharacterInfo[newJob[i]].Name;
            JobInfoText[i].text = PlayerMNG.CharacterInfo[newJob[i]].Text.Replace("~", "\n");
        }
    }

    public void JobSelecterAppear()
    {
        Box.anchoredPosition = new Vector2(0, Box.rect.size.y);

        StartCoroutine(MoveUI());
    }

    IEnumerator MoveUI()
    {
        while (true)
        {
            Box.anchoredPosition += new Vector2(0, -36);

            if (Box.anchoredPosition.y <= 0)
            {
                Box.anchoredPosition = new Vector2(0, 0);
                break;
            }
            yield return null;
        }
    }
}
