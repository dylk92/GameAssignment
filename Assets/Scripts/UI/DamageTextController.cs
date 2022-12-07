using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextController : MonoBehaviour
{
    public Canvas Canvas;
    public GameObject DamageText;

    public void CreateDamageText(Vector3 position, int DMG, bool Crit)
    {
        GameObject dump = GameObject.Instantiate(DamageText, Canvas.transform);
        dump.GetComponent<DamageText>().Init();
        dump.GetComponent<Text>().text = DMG.ToString();
        if (Crit)
            dump.GetComponent<Text>().color = new Color(1, 0.59f, 0.23f);

        dump.transform.position = position;
    }

    public void CreatePlayerDamageText(Vector3 position, int DMG)
    {
        GameObject dump = GameObject.Instantiate(DamageText, Canvas.transform);
        dump.GetComponent<DamageText>().Init();
        dump.GetComponent<Text>().text = DMG.ToString();
        dump.GetComponent<Text>().color = new Color(1, 0, 0);

        dump.transform.position = position;
    }
}
