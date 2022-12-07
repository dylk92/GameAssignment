using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum dir
{
    none = -1,
    left = 0,
    right,
    up,
    down
}

public class TilemapController : MonoBehaviour
{

    [SerializeField] GameObject Tile;
    GameObject[,] Tilemap = new GameObject[3,3];
    Vector3 pivot;

    GameObject player;

    public float SetDistance = 28.8f;
    float distance;

    void Start()
    {
        player = GameManager.Instance.PlayerObject;
    }

    public void Init()
    {
        InitMap();
        distance = SetDistance * 0.5f;
    }
    

    void Update()
    {
        PosCheck();
    }

    // [7][8][9]
    // [4][5][6]
    // [1][2][3]
    void InitMap()
    {
        pivot = player.transform.position;

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                GameObject temp = Instantiate(Tile, gameObject.transform);
                temp.transform.position = new Vector3(pivot.x + (SetDistance * j), pivot.y + (SetDistance * i), 0);
                Tilemap[i + 1, j + 1] = temp;
            }
        }
    }

    void PosCheck()
    {
        Vector3 dis = player.transform.position - pivot;
        dir direction = dir.none;

        //Debug.Log(dis);

        if(distance < dis.x) // 플레이어가 오른쪽으로 일정이상 이동 시
            direction = dir.right;
        else if(dis.x < -distance) // 왼쪽으로 일정 이상 이동 시
            direction = dir.left;
        else if (distance < dis.y) // 위로 일정 이상 이동 시
            direction = dir.up;
        else if(dis.y < -distance) // 아래로 일정 이상 이동 시
            direction = dir.down;

        MoveTile(direction);
    }

    void MoveTile(dir direction)
    {
        if (direction == dir.none)
            return;

        GameObject[,] newGo = new GameObject[3, 3];

        if(direction == dir.left)
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    if (j != 0)
                    {
                        newGo[i, j] = Tilemap[i, j - 1];
                    }
                    else
                    {
                        newGo[i, j] = Tilemap[i, 2];

                        Vector3 vec = newGo[i, j].transform.position;
                        vec.x-= SetDistance * 3;
                        newGo[i, j].transform.position = vec;
                    }

                }
            }
        }
        else if (direction == dir.right)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (j < 2)
                    {
                        newGo[i, j] = Tilemap[i, j + 1];
                    }
                    else
                    {
                        newGo[i, j] = Tilemap[i, 0];

                        Vector3 vec = newGo[i, j].transform.position;
                        vec.x += SetDistance * 3;
                        newGo[i, j].transform.position = vec;
                    }
                }
            }
        }
        else if(direction == dir.up)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i < 2)
                    {
                        newGo[i, j] = Tilemap[i + 1, j];
                    }
                    else
                    {
                        newGo[i, j] = Tilemap[0, j];

                        Vector3 vec = newGo[i, j].transform.position;
                        vec.y += SetDistance * 3;
                        newGo[i, j].transform.position = vec;
                    }
                }
            }
        }
        else if (direction == dir.down)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i != 0)
                    {
                        newGo[i, j] = Tilemap[i - 1, j];
                    }
                    else
                    {
                        newGo[i, j] = Tilemap[2, j];

                        Vector3 vec = newGo[i, j].transform.position;
                        vec.y -= SetDistance * 3;
                        newGo[i, j].transform.position = vec;
                    }
                }
            }
        }

        Tilemap = newGo;
        pivot = newGo[1, 1].transform.position;
    }
}
