using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InputManager : MonoBehaviour
{
    private Vector3 _MouseVec;
    
    private float CameraPosition;
    private Vector3 MousePosition;

    private Transform PlayerTransfrom;

    public Vector3 MouseVec {
        get { return _MouseVec; }
        private set { _MouseVec = value; }
    }

    private void Awake()
    {
        CameraPosition = Camera.main.transform.position.z;
        MousePosition = Input.mousePosition;
        PlayerTransfrom = GameManager.Instance.PlayerObject.transform;
    }
    

    private void Update()
    {
        SetMousePosition();
        //isMouseDown();
        isKeyDown();
    }

    private void SetMousePosition()
    {
        Vector3 Mvec = GetMouseLocation();
        
        MouseVec = Mvec.normalized;
    }

    public Vector3 GetMouseLocation()
    {
        Vector3 Mvec = new Vector3();
        MousePosition = Input.mousePosition;
        Mvec = Camera.main.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, -CameraPosition));
        Mvec -= PlayerTransfrom.position;

        return Mvec;
    }

    public Vector3 GetMovePosition()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        return new Vector3(h, v, 0); //.normalized;
    }

    public void isMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
        if (Input.GetMouseButtonDown(1))
        {
            GameManager.Instance.PlayerMNG.LevelUP();
        }
        if (Input.GetMouseButtonDown(2))
        {
            GameManager.Instance.Init();
        }
    }

    public void isKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.UIMNG.GamePause.gameObject.activeSelf)
            {
                GameManager.Instance.UIMNG.GamePause.gameObject.SetActive(false);
                Time.timeScale = 1;
            }

            else
            {
                GameManager.Instance.UIMNG.Pause();
            }
        }
    }
}
