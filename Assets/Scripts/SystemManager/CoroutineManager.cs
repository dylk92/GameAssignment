using System;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager
{
    // 반복된 WFS생성을 막기 위해 딕셔너리에 사용한 WFS를 저장시켜둔다.
    Dictionary<float, WaitForSeconds> WFS_Storage = new Dictionary<float, WaitForSeconds>();

    public WaitForSeconds GetWFS(float time)
    {
        time = float.Parse(time.ToString("F2")); // 소수점 둘째자리 까지만

        WaitForSeconds wfs;

        if (!WFS_Storage.TryGetValue(time, out wfs))
        {
            wfs = new WaitForSeconds(time);
            WFS_Storage.Add(time, wfs);
        }

        return wfs;
    }
}
