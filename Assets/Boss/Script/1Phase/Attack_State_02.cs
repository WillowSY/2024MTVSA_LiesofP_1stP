using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class Attack_State_02 : StateMachineBehaviour // 양손 후리기
{
    private Boss boss;
    public Transform _player;
    private Transform bossTransform;
    public float damageAmount = 20f;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("발차기");
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
        boss.atkDelay02 = boss.atk02CT;
        Debug.Log("2번 스킬 쿹타임 적용중");
        Debug.Log(boss.atk02CT);
    }

    
}
