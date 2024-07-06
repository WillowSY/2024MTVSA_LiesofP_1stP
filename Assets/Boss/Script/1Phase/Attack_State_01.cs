using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Attack_State_01 : StateMachineBehaviour // 왼손찌르기
{
    private Boss boss;
    public Transform _player;
    private Transform bossTransform;
    public float damageAmount = 10f;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        boss.attackDistance = Mathf.Min(boss.attackSize.x, boss.attackSize.y, boss.attackSize.z+7f);
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("왼손 휘두르기 공격");

        AttackPlayer();
    }

    void AttackPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); //Player 태그를 가진 대상을 player에 저장
        Debug.Log(player);
        _Player playerHealth = player.GetComponent<_Player>(); //
        if (playerHealth !=null)
        {
            playerHealth.TakeDamage(damageAmount);
            Debug.Log(damageAmount); 
        }
        
    } 
    
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        
    } 

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.atkDelay01 = boss.atk01CT;
        
        Debug.Log("1번 스킬 쿹타임 적용중");
      //  Debug.Log(boss.atk01CT);
        
        
    }
    
}
