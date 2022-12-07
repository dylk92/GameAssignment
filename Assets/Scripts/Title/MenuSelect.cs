using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuSelect : EventTrigger
{
    GameObject HighlightObject;
    PointerEventData ped;
    List<GameObject> GOList = new List<GameObject>();

    void Update()
    {
        if (ped != null)
            GOList = ped.hovered;
        else
            GOList.Clear();
        RayCheck();
    }

    void RayCheck()
    {
        if (GOList.Count != 0)
        {
            for (int i = 0; i < GOList.Count; i++)
            {
                GameObject go = GOList[i].gameObject;
                if (go.CompareTag("SelectionBox"))
                {
                    EnterColor(go);
                    if (!System.Object.ReferenceEquals(HighlightObject, go))
                    {
                        if(HighlightObject != null)
                            ExitColor(HighlightObject);
                        HighlightObject = go;
                    }

                    return;
                }
            }
        }

        if (HighlightObject != null)
        {
            ExitColor(HighlightObject);
            ped = null;
            HighlightObject = null;
        }
    }

    void EnterColor(GameObject go)
    {
        go.GetComponent<Image>().color = new Color(0.42f, 0.95f, 0.7f, 1);
    }
    void ExitColor(GameObject go)
    {
        go.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        ped = data;
        HighlightObject = GetEnterObject(data.pointerEnter);
    }
    GameObject GetEnterObject(GameObject go)
    {
        if (go.CompareTag("SelectionBox"))
            return go;
        else
            return null;
    }

    public override void OnPointerClick(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            GameObject SelectedBox = GetEnterObject(data.pointerCurrentRaycast.gameObject);
            
            if (SelectedBox != null)
            {
                string[] name = SelectedBox.name.Split('_');

                switch (name[1])
                {
                    case "0":
                        SceneManager.LoadScene("PlayGameScene");
                        break;
                    case "1":
                        TitleManager.Instance.HowToPlay.SetActive(true);
                        break;
                    case "2":
                        //UnityEditor.EditorApplication.isPlaying = false;
                        Application.Quit();
                        break;
                }

                ExitColor(HighlightObject);
                HighlightObject = null;
            }
        }
    }
}
