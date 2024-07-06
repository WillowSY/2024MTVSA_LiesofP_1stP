using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
    
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    public CharacterController cc; 
    public float healthPoint;
    public float maxHealth;         /* 추가 */
    public float groggyGauge;
    public float Speed;
    public int bossPattern;
    public float attackDistance;
    
    //보스의 status를 나타내는 함수

    public Vector3 attackSize = new Vector3(8f, 30f, 9f);
    public LayerMask playerLayer;
    
    
    public float atk01CT = 5;
    public float atk02CT = 10;
    
    public float atkDelay01;
    public float atkDelay02;
    public float atkDelay03;
    public float atkDelay04;
    public float atkDelay05;
    
    public Transform _player;
    //public _Player player;
    Animator animator;
    private Attack_State_01 _atk01;
    
    /* 추가 */
    public Slider hpSlider;
    public TMP_Text tmp;
    void Start()
    {
        animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        attackDistance = Mathf.Min(attackSize.x, attackSize.y, attackSize.z);
        healthPoint = maxHealth;    /* 추가 */
    }
    
    // Update is called once per frame
    void Update()
    {
        if (atkDelay01 >= 0)
        {
            atkDelay01 -= Time.deltaTime;
        }
        
        if (atkDelay02 >= 0)
        {
            atkDelay02 -= Time.deltaTime;
        }

        //System.Random Random = new System.Random(System.DateTime.Now.Millisecond);



        /*float seed = Time.time;
        Random.InitState((int)(seed*1000));
        bossPattern = Random.Range(1, 6); */
        //Debug.Log(bossPattern);

        
        
        
        Collider[] colliders = Physics.OverlapBox(transform.position, Vector3.one * attackDistance/2f,
            Quaternion.identity, playerLayer);
        //Debug.Log(colliders);
        
        /* 추가 */
        hpSlider.value = healthPoint / maxHealth;
        tmp.text = healthPoint.ToString();

        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }
}
