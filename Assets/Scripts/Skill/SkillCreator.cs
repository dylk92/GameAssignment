using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class BaseArrow : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(GameManager.Instance.PlayerObject);
        MoveMouseLocation();

        SetMoveAction();
        SetDestroyAction();
    }
}

class Drill : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(GameManager.Instance.PlayerObject);
        MoveMouseLocation();

        SetMoveAction();
        SetDestroyAction();
    }
}

class ArrowBurst : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);
        SpawnTargetTransform = player.transform;

        Prepare();
        CreateDelay();
        SpawnTargetLocation(player);
        MoveMouseLocation();

        SetMoveAction();
        SetDestroyAction();
    }
}

class MultiShot : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(GameManager.Instance.PlayerObject);
        MoveMouseLocation();

        SetMoveAction();
        SetDestroyAction();
    }
}

class BowSweep : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);
        

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);
        MoveMouseLocation();

        SetMoveAction(1);
        SetMoveAction(2);
        SetDestroyAction();
    }
}

class ArrowRain : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnMouseLocation();

        SetDestroyAction();
    }
}

class Stinger : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);
        SpawnTargetTransform = player.transform;

        Prepare();
        CreateDelay();
        MoveTargetTracking(player);
        MoveMouseLocation();

        SetMoveAction(1);
        SetMoveAction(2);
        SetDestroyAction();
    }
}

class SpreadShot : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);
        SpawnTargetTransform = player.transform;

        Prepare();
        CreateDelay();
        SpawnTargetLocation(player);
        SetLotationVector(Vector3.right);

        SetMoveAction();
        SetDestroyAction();
    }
}

class AutoFire : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);
        GameObject target = FindNearEnemy(player);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetLocation(target);

        SetMoveAction();
        SetDestroyAction();
    }
}

class WhirlWind : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);

        SetMoveAction(1);
        SetDestroyAction();
    }
}

class ArrowStorm : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);
        MoveMouseLocation();

        SetMoveAction(1);
        SetMoveAction(7);
        SetMoveAction(2);
        SetDestroyAction();
    }
}

class ArrowBlast : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);
        MoveMouseLocation();

        SetMoveAction(1);
        SetMoveAction(2);
        SetDestroyAction();
    }
}

class Sniping : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnMouseLocation();

        SetDestroyAction();
    }
}

class BombArrow : Skill
{
    public BombArrow()
    {
        ChildSkill = new BombArrowChild();
    }
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveMouseLocation();

        SetMoveAction();
        SetChild();
        SetChild(1);
        SetDestroyAction(1);
        SetDestroyAction();
    }
}

class BombArrowChild : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(ExtraTarget);

        SetDestroyAction();
    }
}

class ArrowDrive : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveMouseLocation();

        SetMoveAction();
        SetDestroyAction();
    }
}

class Frangible : Skill
{
    public Frangible()
    {
        ChildSkill = new FrangibleChild();
    }

    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveMouseLocation();

        SetMoveAction();
        SetChild();
        SetChild(1);
        SetChild(2);
        SetDestroyAction(1);
        SetDestroyAction();
    }
}

class FrangibleChild : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(ExtraTarget);
        SetLotationQuaternion(ExtraQuaternion);

        SetMoveAction();
        SetDestroyAction();
    }
}

class WildRoar : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);
        SpawnTargetTransform = player.transform;

        Prepare();
        CreateDelay();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);

        SetMoveAction(1);
        SetDestroyAction();
    }
}

class WoodenTurret : Skill
{
    public WoodenTurret()
    {
        ChildSkill = new WoodenTurretChild();
    }

    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);

        SetChild();
        SetChild(1);
        SetDestroyAction();

        ChildCreateLoop();
    }
}

class WoodenTurretChild : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(ExtraTarget);
        MoveTargetLocation(FindNearEnemy(ExtraTarget));

        SetMoveAction();
        SetDestroyAction();
    }
}

class BirdStrike : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveMouseLocation();

        SetMoveAction();
        SetDestroyAction();
    }
}

class VineChain : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnMouseLocation();

        SetDestroyAction();
    }
}

class LeafRotation : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);
        SetLotationVector(Vector3.right);

        SetMoveAction(1);
        SetMoveAction(3);
        SetMoveAction(8);
        SetDestroyAction();
    }
}

class RapidLaser : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);
        SpawnTargetTransform = player.transform;

        Prepare();
        CreateDelayWithAngle();
        MoveTargetTracking(player);

        SetMoveAction(1);
        SetMoveAction(2);
        SetDestroyAction();
    }
}

class MagicArrow : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(ExtraTarget);
        if ((player.transform.position - ExtraTarget.transform.position).sqrMagnitude < 300)
            MoveTargetLocation(FindNearEnemy(ExtraTarget));
        else
            MoveTargetLocation(player);

        SetMoveAction();
        SetChild(3);
        SetDestroyAction(() => {
            ChildSkill = new MagicArrow();
            SetChild();
            SkillObject skills = Skills[0].GetComponent<SkillObject>();
            skills.ChildLocationSkillPoint();
        });
        SetDestroyAction(1);
        SetDestroyAction();
    }
}

class Meteor : Skill
{
    public Meteor()
    {
        ChildSkill = new MeteorChild();
    }

    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnMouseLocation();
        Vector3 vec = Skills[0].transform.position;
        vec.y += skilldata.skillStat.Speed * 1.05f;
        Skills[0].transform.position = vec;
        SetLotationVector(Vector3.down);

        SetMoveAction();
        SetChild();
        SetChild(1);
        SetDestroyAction(1);
        SetDestroyAction();
    }
}

class MeteorChild : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(ExtraTarget);

        SetDestroyAction();
    }
}

class ArrowRotation : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);
        SetLotationVector(Vector3.right);

        SetMoveAction(1);
        SetMoveAction(3);
        SetMoveAction(8);
        SetDestroyAction();
    }
}

class ManaEruption : Skill
{
    public override void Activate(SkillData ss)
    {
        base.Activate(ss);

        Prepare();
        CreateOnce();
        SpawnTargetLocation(player);
        MoveTargetTracking(player);
        MoveMouseLocation();

        SetMoveAction(1);
        SetMoveAction(7);
        SetMoveAction(2);
        SetDestroyAction();
    }
}