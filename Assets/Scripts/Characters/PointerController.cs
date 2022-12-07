using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    InputManager InputMNG;

    public float distance = 3;

    private void Awake()
    {
        InputMNG = GameManager.Instance.InputMNG;
    }

    

    private void FixedUpdate()
    {
        AngleUpdate();
    }

    void AngleUpdate()
    {
        Vector3 dir = InputMNG.MouseVec;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        dir = dir.normalized * distance;

        transform.rotation = rotation;
        transform.localPosition = dir;
    }
}
