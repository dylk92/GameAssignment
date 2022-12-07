using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HowToPlay : EventTrigger
{
    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        gameObject.SetActive(false);
    }

    public override void OnPointerClick(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            gameObject.SetActive(false);
        }
    }
}
