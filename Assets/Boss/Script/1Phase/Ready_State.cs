using Unity.VisualScripting;
using UnityEngine;

public class Ready_State : StateMachineBehaviour
{
    
    private Transform bossTransform;
    private Boss boss;
    
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>(); //변수 초기화
        bossTransform = animator.GetComponent<Transform>(); // 변수 초기화
        
        float seed = Time.time;
        Random.InitState((int)(seed*5000));
        boss.bossPattern = Random.Range(1, 3);
        Debug.Log(boss.bossPattern);
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (Vector3.Distance(boss._player.position, bossTransform.position) > boss.attackDistance )
        {
            animator.SetBool("IsFollow", true);
            Debug.Log("공격 중지, 재 추격");
        }
        
        else if ( boss.atkDelay01 <=0 && boss.bossPattern == 1)
        {
            animator.SetTrigger("IsAtk01");
            
            //Debug.Log("왼손 찌르기 시전");
        } // 발로 찍기 
        
        else if (boss.atkDelay02 <= 0 && boss.bossPattern == 2)
        {
            animator.SetTrigger("IsAtk02");
            //Debug.Log("왼손 찌르기 시전");
            // 오른손 주먹질
        }

        else 
        {
            animator.SetBool("IsReady", false);
        }
        
        
     
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    
}
