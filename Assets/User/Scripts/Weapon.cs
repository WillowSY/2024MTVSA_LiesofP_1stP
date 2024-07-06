using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // 무기 종류.
    public enum Type
    {
        Melee       //근접무기
    };

    public GameObject _boss;
    public Type type;
    // 무기 데미지
    public int damage;
    // 무기 공격 딜레이
    public float rate;
    // 무기 콜라이더 박스
    public BoxCollider meleeArea;
    // 공격 이펙트
    public TrailRenderer trailEffect;

    void Start()
    {
        _boss = GameObject.FindGameObjectWithTag("Boss");
    }

    public void Use()
    {
        Debug.Log("Use");
        if (type == Type.Melee)
        {
            StartCoroutine(Swing());
            Debug.Log("공격 끝");
        }

        IEnumerator Swing()
        {
            yield return new WaitForSeconds(0.1f); //1초 대기
            meleeArea.enabled = true;
            // 공격 이펙트 활성화
            //trailEffect.enabled = true;
            
            yield return new WaitForSeconds(0.3f);  // 3초 대기
            meleeArea.enabled = false;
            yield break;

        }
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boss")
        {
            Debug.Log("데미지");
            float curHelath = _boss.GetComponent<Boss>().healthPoint;
            _boss.GetComponent<Boss>().healthPoint = Mathf.Max(curHelath -damage, 0);
        }
    }
}
