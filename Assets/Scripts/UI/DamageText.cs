using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    Text text;
    Outline ol;

    public float Speed = 200;
    bool isMaximum = false;

    public void Init()
    {
        text = GetComponent<Text>();
        ol = GetComponent<Outline>();

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        ol.effectColor = new Color(ol.effectColor.r, ol.effectColor.g, ol.effectColor.b, 1);

        isMaximum = false;
        StartCoroutine(SetMaximum());
    }

    IEnumerator SetMaximum()
    {
        yield return GameManager.Instance.CoroutineMNG.GetWFS(0.1f);
        isMaximum = true;
    }
    

    void Update()
    {
        if(!isMaximum)
            transform.Translate(Vector3.up * 10 * Time.deltaTime);
        else
        {
            float a = text.color.a - (Time.deltaTime * 2);

            if (a <= 0)
                Destroy(gameObject);

            text.color = new Color(text.color.r, text.color.g, text.color.b, a);
            ol.effectColor = new Color(ol.effectColor.r, ol.effectColor.g, ol.effectColor.b, a);
        }
    }
}
