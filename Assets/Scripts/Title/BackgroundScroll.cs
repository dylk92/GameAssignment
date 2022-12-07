using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] GameObject HowToPlay;

    public float[] Speed;
    public Transform[] Background;
    const float BGWidth = 38;

    private void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        MoveBG();
    }

    void MoveBG()
    {
        for(int i = 0; i < Background.Length; i++)
        {
            Vector3 CurVec = Background[i].position;
            Vector3 PlsVec = Vector3.left * Speed[i] * Time.deltaTime;
            Background[i].position = CurVec + PlsVec;

            if(Background[i].position.x <= -BGWidth)
            {
                Vector3 vec = Background[i].position;
                vec.x += 38;
                Background[i].position = vec;
            }
        }
    }
}
