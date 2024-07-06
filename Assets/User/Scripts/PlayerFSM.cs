using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine.UI;

public class PlayerFSM : MonoBehaviour
{

    // Player 상태 상수
    enum PlayerState
    {
        Idle,
        Move_Walk,
        Move_Run,
        Attack1,
        Dodge,
        Damaged
    }

    // PLyaer 상태 변수
    PlayerState p_State;

    // Plyaer 캐릭터 콘트롤러 컴포넌트
    CharacterController cc;
    private Animator anim;
    private _Player _player;

    public void Initialize(_Player parent)
    {
        _player = parent;
    }

    void Start()
    {
        p_State = PlayerState.Idle;
        cc = GetComponent<CharacterController>();
        anim = transform.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        switch (p_State)
        {
            case PlayerState.Idle:
                Idle();
                break;
            case PlayerState.Move_Walk:
                MoveWalk();
                break;
            case PlayerState.Move_Run:
                MoveRun(); 
                break;
            case PlayerState.Dodge:
                Dodge();
                break;
            // // case PlayerState.Move_Sprint:
            // //     MoveSprint();
            // //     break;
            //case PlayerState.Attack1:
            //     Attack1();
            //     break;
            // case PlayerState.Damaged:
            //     Damaged();
            //     break;
            // case PlayerState.Die:
            //     Die();
            //     break;
        }
    }



    // 대기 상태 메서드.
    void Idle()
    {
        goAttack();
        goDamaged();
        goDodge();
        if (_player.aDown || _player.dDown || _player.wDown || _player.sDown)
        {
            p_State = PlayerState.Move_Walk;
            anim.SetTrigger("IdleToMoveWalk");
            print("상태 전환 : Idle -> Move_Walk");
            //}
        }
    }

    // 걷기 상태 메서도
    void MoveWalk()
    {
        goAttack();
        goDamaged();
        goDodge();
        _player.Movement();
        if (_player.runDown)
        {
            p_State = PlayerState.Move_Run;
            anim.SetTrigger("MoveWalkToMoveRun");
            print("상태 전환 : Move_Walk->Move_Run");
        }
        if (!_player.aDown && !_player.sDown && !_player.wDown && !_player.dDown)
        {
            anim.SetTrigger("MoveWalkToIdle");
            p_State = PlayerState.Idle;
            print("상태 전환 : Move_Walk -> Idle");
        }
        //FIXME : 전력질주 미구현
        //else if (PlayerMove.sprintDown)
        // {
        //     p_State = PlayerState.Move_Sprint;
        //     print("상태 전환 : Idle->Move_Sprint");
        // }
    }

    // 달리기 상태 메서드.
    void MoveRun()
    {
        goAttack();
        goDamaged();
        goDodge();
        _player.finalSpeed = _player.runSpeed;
        _player.Movement();
        if (!_player.runDown){
            p_State = PlayerState.Move_Walk;
            anim.SetTrigger("MoveRunToMoveWalk");
            print("상태 전환 : Move_Run->Move_Walk");
        }
        else if (!_player.aDown && !_player.sDown && !_player.wDown && !_player.dDown)
        {
            p_State = PlayerState.Move_Walk;
            anim.SetTrigger("MoveRunToMoveWalk");
            print("상태 전환 : Move_Run->Move_Walk");
        }
    }
    // 공격 상태로 전이 및 공격 상태 메서드.
    void goAttack()
    {
        
        if (_player.fDown&&_player.isFireReady)
        {
            p_State = PlayerState.Attack1;
            anim.SetTrigger("ToAttack1");
            _player.equipWeapon.Use();
            print("상태 전환 : Idle->Attack1");
            _player.fireDelay = 0;
            exitAttack();
        }
    }

    // 공격 -> Idle 전이 메서드.
    void exitAttack()
    {
        print("상태 전환 : Attack1->Idle");
        anim.SetTrigger("Attack1ToIdle");
        p_State = PlayerState.Idle;
    }

    // Dodge상태로 전이 메서드.
    void goDodge()
    {
        if (_player.dodgeDown&&_player.isDodgeReady )
        {
            anim.SetTrigger("ToDodge");
            Debug.Log("상태 전환 : -> Dodge");
            p_State = PlayerState.Dodge;
            _player.isDodge = true;
        }
    }
    
    // Dodge 상태 메서드.
    // FIXME : Dodge 4방향 모두 가능하게 변경하기. 지금은 항상 플레이어 캐릭터 뒤로 백스텝 회피.
    void Dodge(){
        
        StartCoroutine(_Dodge());
        IEnumerator _Dodge()
        {
            _player.dodgeDelay = 0;
            _player.finalSpeed = _player.dodgeSpeed;
            _player.transform.position -= _player.transform.forward*_player.finalSpeed*Time.deltaTime;
            yield return new WaitForSeconds(0.3f);
            exitDodge();
            yield break;
        }
    }

    // Dodge -> Idle 상태 저닝 메서드.
    void exitDodge()
    {
        print("상태 전환 : Dodge -> Idle");
        anim.SetTrigger("DodgeToIdle");
        p_State = PlayerState.Idle;
        _player.isDodge = false;
    }
    
    // Damaged 상태로 전이 메서드.
    void goDamaged()
    {
        // 플레이어가 회피 상태가 아니고 데미지를 받은 상태이면 피해 받음.
        if (_player.isDamaged&&!_player.isDodge)
        {
            anim.SetTrigger("ToDamaged");
            Damaged();
        }

    }
    // Damaged 상태( 데미지 받는 상태) 메서드.
    void Damaged()
    {
        StartCoroutine(Damage());
        Debug.Log("피해");

        IEnumerator Damage()
        {
            p_State = PlayerState.Damaged;
            _player.curHealth = _player.curHealth - _player.bossDamage;//Mathf.Max(_player.curHealth - _player.bossDamage, 0);
                                   yield return new WaitForSeconds(0.3f);
            exitDamaged();
            yield break;
        }
    }
    
    // Damaged -> Idle 상태 전이 메서드.
    void exitDamaged(){
        print("상태 전환: Damaged -> Idle");
        anim.SetTrigger("DamagedToIdle");
        p_State = PlayerState.Idle;
        _player.isDamaged = false;
    }
}