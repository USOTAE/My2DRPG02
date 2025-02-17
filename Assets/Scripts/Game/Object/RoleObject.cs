using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ������� ֮��Ĺ��� ��Ҷ��̳���
/// </summary>
public abstract class RoleObject : MonoBehaviour
{
    //��ɫ���ƶ�����
    protected Vector2 moveDir = Vector2.zero;

    //�����Ծ���ٶ�
    public float jumpSpeed = 10;
    //�������ٶ�
    public float gSpeed = 9.8f;
    //�ƶ��ٶ�
    public int speed = 5;
    //��ǰ��ֱ���׵��ٶ�
    protected float nowYSpeed;
    //�����ٶ�
    protected float nowXSpeed;

    //����sprite
    protected SpriteRenderer roleSprite;
    //��ɫanimator
    protected Animator roleAnimator;
    //�����Ӷ���
    protected Transform bodyTransform;
    //Ӱ�Ӷ���
    protected Transform shadowTransform;

    protected virtual void Awake()
    {
        //����Ӷ���ʱ���ҵ�Ŀ���Ӷ���
        bodyTransform = this.transform.Find("Role");
        shadowTransform = this.transform.Find("Shadow");
        roleSprite = bodyTransform.GetComponentInChildren<SpriteRenderer>();
        roleAnimator = this.GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        //�����ƶ�
        CheckMove();
        //������ɺ���Ծ
        CheckJumpOrHitFly();

    }

    /// <summary>
    /// ���й�����Ϊ
    /// </summary>
    public abstract void Atk();

    /// <summary>
    /// ������������
    /// </summary>
    public abstract void Dead();

    /// <summary>
    /// �������ƶ�����߼�
    /// </summary>
    protected void CheckMove()
    {
        //���ƶ�ǰ���ж� �����ƶ�������ȥ�ƶ�
        if (CanMoving)
        {
            //��ɫ�ƶ��߼�
            this.transform.Translate(moveDir.normalized * speed * Time.deltaTime);
            //�������ת�� ������0 ����ɿ��������ǿ��ת������
            if (moveDir.x > 0)
            {
                roleSprite.flipX = false;
                //shadowTransform.localPosition = Vector3.right * 0.04f;
                shadowTransform.localPosition = new Vector3(-0.04f, -1.2f);
            }
            else if (moveDir.x < 0)
            {
                roleSprite.flipX = true;
                //shadowTransform.localPosition = Vector3.right * -0.04f;
                shadowTransform.localPosition = new Vector3(0.04f, -1.2f);
            }
        }

        //�Ƿ��ƶ�
        if (moveDir == Vector2.zero)
            ChangeAction(E_Action_Type.Idle);
        else
            ChangeAction(E_Action_Type.Move);
    }

    /// <summary>
    /// ��⴦�� ��Ծ���߻����߼�
    /// </summary>
    protected void CheckJumpOrHitFly()
    {
        //���� ��Ծ�߼�
        //�����ڵ����� �Ϳ�����Ծ
        if (roleAnimator.GetBool("isGround") == false)
        {

            //��Ծ�������
            bodyTransform.Translate(Vector2.up * nowYSpeed * Time.deltaTime);
            //��ֱ���� �����߼� �ٶȱ仯
            //v = v - gt
            nowYSpeed -= gSpeed * Time.deltaTime;

            //�����жϸ߶��Ƿ�С�ڵ���0 �����ж��Ƿ����
            //ע�� һ�������ж� == 0 ��Ϊ�� - ֡���ʱ��*�ٶ� �󲿷����������ոյ���0
            //ע��ʹ�õ���localPosition �˽�Position��localPosition
            if (bodyTransform.localPosition.y <= 0)
            {
                //���õ�����
                bodyTransform.localPosition = Vector2.zero;
                //�ı�����ʶ
                ChangeRoleIsGround(true);

                //��غ� ���ܻ��˶����� ����ͣ����
                nowXSpeed = 0;
                //Ӧ�������ӳ�վ����
                Invoke(nameof(DelayClearHitFly), 2f);
            }
        }

        if (nowXSpeed != 0)
        {
            this.transform.Translate(Vector2.right * nowXSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// ���˷���
    /// </summary>
    public virtual void Wound(float hitTime)
    {
        //�������ʱ�ǻ���״̬ ����Ҫ�ٴ������˵Ķ����߼�
        if (roleAnimator.GetBool("isHitFly"))
            return;

        //�����������״̬ ������ ��Ҫ����һ����ʱ����ȡ��
        CancelInvoke(nameof(DelayClearHit));
        //�л����˶���
        ChangeAction(E_Action_Type.Hit);
        //��ʱ���� �����һ��ʱ���������״̬
        Invoke(nameof(DelayClearHit), hitTime);
    }

    /// <summary>
    /// ���ɷ���
    /// </summary>
    public virtual void HitFly(float xSpeed, float ySpeed)
    {
        //����Ѿ��ڻ���״̬ ֱ�ӷ��� �����ٱ�����
        if (roleAnimator.GetBool("isHitFly"))
            return;

        //�����ǰ������״̬ ���ɵ����ȼ������� ��Ҫ�ı�״̬
        if (roleAnimator.GetBool("isHit"))
        {
            //ȡ���ӳ��������״̬
            CancelInvoke(nameof(DelayClearHit));
            //ֱ���������״̬
            roleAnimator.SetBool("isHit", false);
        }


        //�л����ɶ���
        ChangeAction(E_Action_Type.HitFly);
        //�ı���Ҳ��ڵ���
        ChangeRoleIsGround(false);
        //��ʼ��ֱ���׵��ٶ�
        nowYSpeed = ySpeed;
        nowXSpeed = xSpeed;
    }

    private void DelayClearHit()
    {
        roleAnimator.SetBool("isHit", false);
    }

    /// <summary>
    /// �ӳ�����
    /// </summary>
    private void DelayClearHitFly()
    {
        roleAnimator.SetBool("isHitFly", false);
    }

    /// <summary>
    /// �л�����
    /// </summary>
    /// <param name="type"></param>
    protected void ChangeAction(E_Action_Type type)
    {
        switch (type)
        {
            case E_Action_Type.Idle:
                roleAnimator.SetBool("isMoving", false);
                break;
            case E_Action_Type.Move:
                roleAnimator.SetBool("isMoving", true);
                break;
            case E_Action_Type.Jump:
                roleAnimator.SetTrigger("jumpTrigger");
                break;
            case E_Action_Type.JumpAtk:
                roleAnimator.SetTrigger("jumpAtkTrigger");
                break;
            case E_Action_Type.Atk:
                roleAnimator.SetTrigger("atkTrigger");
                break;
            case E_Action_Type.FootAtk:
                roleAnimator.SetTrigger("footAtkTrigger");
                break;
            case E_Action_Type.Hit:
                roleAnimator.SetBool("isHit", true);
                break;
            case E_Action_Type.HitFly:
                roleAnimator.SetBool("isHitFly", true);
                break;
            case E_Action_Type.Dead:
                roleAnimator.SetBool("isDead", true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �л�����Ƿ��ڵ���״̬
    /// </summary>
    /// <param name="isGround"></param>
    protected void ChangeRoleIsGround(bool isGround)
    {
        roleAnimator.SetBool("isGround", isGround);
    }

    protected bool CanMoving
    {
        get
        {
            //ȥ�õ�״̬���������״̬ �ж��Ƿ��ǿ����ƶ���״̬
            //AnimatorStateInfo layerInfo1 = roleAnimator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo layerInfo2 = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (IsAtkState ||
                layerInfo2.IsName("Defend") ||
                layerInfo2.IsName("Hit") ||
                layerInfo2.IsName("HitFly") ||
                layerInfo2.IsName("Pickup") ||
                layerInfo2.IsName("Throw") ||
                roleAnimator.GetBool("isDead"))
                return false;

            return true;
        }
    }

    /// <summary>
    /// �Ƿ��ǹ���״̬
    /// </summary>
    protected bool IsAtkState
    {
        get
        {
            AnimatorStateInfo layerInfo = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (layerInfo.IsName("Atk1") ||
                layerInfo.IsName("Atk2") ||
                layerInfo.IsName("Atk3") ||
                layerInfo.IsName("FootAtk1") ||
                layerInfo.IsName("FootAtk2"))
                return true;
            return false;
        }
    }
}

public enum E_Action_Type
{

    Idle,
    Move,
    Jump,
    /// <summary>
    /// ��Ծ����
    /// </summary>
    JumpAtk,
    Atk,
    /// <summary>
    /// �Ȳ����� ��ӦKick����
    /// </summary>
    FootAtk,
    Hit,
    /// <summary>
    /// ����
    /// </summary>
    HitFly,
    /// <summary>
    /// Ͷ������
    /// </summary>
    Throw,
    Pickup,
    Defend,
    /// <summary>
    /// ����
    /// </summary>
    Dead
}