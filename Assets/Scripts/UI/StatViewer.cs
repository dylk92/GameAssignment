using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatViewer : MonoBehaviour
{
    [SerializeField] Text[] StatList;

    public void SetStat()
    {
        Player player = GameManager.Instance.PlayerObject.GetComponent<Player>();
        Stats stat = player.GetStats();

        StatList[0].text = stat.MaxHP.ToString();
        StatList[1].text = stat.ATK.ToString();
        StatList[2].text = stat.DEF.ToString();
        StatList[3].text = stat.AGI.ToString();
        StatList[4].text = stat.CritPer.ToString();
        StatList[5].text = stat.MoveSpeed.ToString();
        StatList[6].text = stat.IncreaseDMG.ToString();
        StatList[7].text = stat.ATKSpeed.ToString();
        StatList[8].text = stat.ATKRange.ToString();
    }
}
