using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISkillSelectionTrigger : EventTrigger
{
    GameObject HighlightObject;
    PointerEventData ped;
    List<GameObject> GOList = new List<GameObject>();
    
    private void Update()
    {
        if (Time.timeScale == 0)
        {
            if (ped != null)
                GOList = ped.hovered;
            else
                GOList.Clear();
            RayCheck();
        }
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
        go.GetComponent<Image>().color = new Color(1, 0.925f, 0.243f, 0.78f);
    }
    void ExitColor(GameObject go)
    {
        go.GetComponent<Image>().color = new Color(0.24f, 0.66f, 0.72f, 0.78f);
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        ped = data;
        HighlightObject = GetEnterObject(data.pointerEnter);
        
    }

    public override void OnPointerClick(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            GameObject SelectedBox = GetEnterObject(data.pointerCurrentRaycast.gameObject);

            if (SelectedBox != null)
            {
                string[] name = SelectedBox.name.Split('_');
                
                int index = Int32.Parse(name[1]);

                switch (name[0])
                {
                    case "ItemSelectBox":
                        GameManager.Instance.PlayerItemMNG.InputItem(index);
                        break;
                    case "JobSelectBox":
                        GameManager.Instance.PlayerMNG.JobChange(index);
                        break;
                }

                ExitColor(HighlightObject);
                HighlightObject = null;
            }
        }
    }

    GameObject GetEnterObject(GameObject go)
    {
        if (go.CompareTag("SelectionBox"))
            return go;
        else
            return go.transform.parent.gameObject;
    }
}
