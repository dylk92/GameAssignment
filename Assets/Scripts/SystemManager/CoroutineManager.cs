using System;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager
{
    // �ݺ��� WFS������ ���� ���� ��ųʸ��� ����� WFS�� ������ѵд�.
    Dictionary<float, WaitForSeconds> WFS_Storage = new Dictionary<float, WaitForSeconds>();

    public WaitForSeconds GetWFS(float time)
    {
        time = float.Parse(time.ToString("F2")); // �Ҽ��� ��°�ڸ� ������

        WaitForSeconds wfs;

        if (!WFS_Storage.TryGetValue(time, out wfs))
        {
            wfs = new WaitForSeconds(time);
            WFS_Storage.Add(time, wfs);
        }

        return wfs;
    }
}
