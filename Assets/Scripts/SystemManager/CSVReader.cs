using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
    public List<Dictionary<string, string>> CharacterStatsList = new List<Dictionary<string, string>>();
    public List<Dictionary<string, string>> EnemyDataList = new List<Dictionary<string, string>>();
    public List<Dictionary<string, string>> SkillDataList = new List<Dictionary<string, string>>();
    public List<Dictionary<string, string>> EquipDataList = new List<Dictionary<string, string>>();

    public List<Dictionary<string, string>> SkillInfoList = new List<Dictionary<string, string>>();
    public List<Dictionary<string, string>> EquipInfoList = new List<Dictionary<string, string>>();

    public void AwakeParse()
    {
        DataParse(CharacterStatsList, "CharacterStats");
        DataParse(EnemyDataList, "EnemyData");
        DataParse(SkillDataList, "SkillData");
        DataParse(EquipDataList, "EquipData");

        DataParse(SkillInfoList, "SkillInfo");
        DataParse(EquipInfoList, "EquipInfo");
    }

    void DataParse(List<Dictionary<string, string>> DataList, string Path)
    {
        if(DataList.Count < 0)
        {
            Debug.Log(DataList);
            Debug.Log(DataList.Count);
            Debug.LogError(Path + " is already parsed");
            return;
        }

        TextAsset ParseData = Resources.Load<TextAsset>("CSV/" + Path);

        if(ParseData == null)
        {
            Debug.LogError(Path + " is Null");
            return;
        }
        SetDictionary(ParseData, DataList);
    }

    void SetDictionary(TextAsset ParseData, List<Dictionary<string, string>> addList)
    {
        var lines = ParseData.text.Split('\n');
        if (lines.Length <= 1) return;

        var header = lines[0].Split(',');
        if (header.Length <= 0) return;

        for (int i = 1; i < lines.Length; i++)
        {
            var StatsDic = new Dictionary<string, string>();
            var dump = lines[i].Split(',');

            if (dump.Length == header.Length)
            {
                for (int j = 0; j < header.Length; j++)
                {
                    StatsDic.Add(header[j], dump[j]);
                    //Debug.Log(header[j] + ", " + dump[j]);
                }
                addList.Add(StatsDic);
            }
        };
    }
}
