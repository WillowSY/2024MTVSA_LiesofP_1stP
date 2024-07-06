using Unity.VisualScripting;
using UnityEngine;

public class Follow_State : StateMachineBehaviour
{
    private Transform bossTransform;
    private Boss boss;
    private CharacterController cc;
    
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>(); //변수 초기화
        bossTransform = animator.GetComponent<Transform>(); // 변수 초기화
        cc = animator.GetComponent<CharacterController>();
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(boss._player.position, bossTransform.position) >
            boss.attackDistance) // player 와 boss 사이의 거리 비교 기준을 attack distance 로 설정
        {
            animator.SetBool("IsFollow", true);
            Debug.Log("추적 시작");

            Vector3 dir = (boss._player.position - boss.transform.position).normalized;
            cc.Move(dir * boss.Speed * Time.deltaTime);
            
        }
        else if (Vector3.Distance(boss._player.position, bossTransform.position) <= boss.attackDistance)
        {
            animator.SetBool("IsFollow", false);
            Debug.Log("추격중지, 공격 준비");
            
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    
}
