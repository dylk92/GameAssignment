using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] Image HPBar;
    EquipManager EquipMNG;
    PlayableCharacter MyCharacter;
    SpriteRenderer spriteRenderer;
    Animator animate;

    public float speed;

    #region UnityAction

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animate = GetComponent<Animator>();
    }
    
    public void Init()
    {
        MyCharacter = new PlayableCharacter();
        SetStat();
        HPGaugeSet();
    }

    void FixedUpdate()
    {
        Move();
        GameManager.Instance.UIMNG.EXPMNG.GagueUpdate();
    }

    #endregion

    #region HitCheck
    
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
        {
            float DMG = col.gameObject.GetComponent<Enemy>().getDamage();

            if(DMG != 0)
                HitCheck(DMG);
        }
    }

    void HitCheck(float DMG)
    {
        if(MyCharacter.ResultStat.AGI > 0)
        {
            if (MyCharacter.ResultStat.AGI > UnityEngine.Random.Range(0, 100))
            {
                return;
            }
        }

        HitDamage(DMG);
    }

    void HitDamage(float DMG)
    {
        float DEF = MyCharacter.ResultStat.DEF;
        if (DEF != 0)
        {
            float DivDMG = DMG / ((DEF * 0.02f) + 1);

            string DD = DivDMG.ToString("N1");
            DMG = float.Parse(DD);
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos = Camera.main.ScreenToWorldPoint(pos);

        GameManager.Instance.UIMNG.DMGController.CreatePlayerDamageText(pos, (int)DMG);


        bool isDead = MyCharacter.SubHP(DMG);
        HPGaugeSet();

        if (isDead)
        {
            PlayerDead();
        }
    }

    public void HPGaugeSet()
    {
        HPBar.fillAmount = MyCharacter.HPper();
    }

    void PlayerDead()
    {
        Time.timeScale = 0;
        GameManager.Instance.UIMNG.GameOver.GetComponent<GameOverScreen>().TextSet("GAME OVER");
        GameManager.Instance.UIMNG.GameOver.gameObject.SetActive(true);
    }

    #endregion


    void Move()
    {
        Vector3 MoveDir = GameManager.Instance.InputMNG.GetMovePosition();

        if (MoveDir != default(Vector3))
        {
            if (MoveDir.x != 0)
                spriteRenderer.flipX = MoveDir.x == -1;

            animate.SetBool("isWalking", true);

            transform.Translate(MoveDir * MyCharacter.ResultStat.MoveSpeed * Time.deltaTime * speed);

        }
        else
        {
            animate.SetBool("isWalking", false);
        }
    }

    

    #region StatSetting

    public void SetStat() // 스탯을 변경할 때, 스탯을 참조하는 모든 곳을 재설정
    {
        MyCharacter.IncreaseStat = GameManager.Instance.PlayerItemMNG.EquipMNG.EquipStatResult();
        MyCharacter.SetResultStat();

        //MyCharacter.StatView();
    }

    public Stats GetStats()
    {
        return MyCharacter.ResultStat;
    }

    public void JobChange(string job)
    {
        MyCharacter.JobChange(job);
    }

    public void HealthRecovery(float Point) => MyCharacter.PlsHP(Point);
    #endregion
}
