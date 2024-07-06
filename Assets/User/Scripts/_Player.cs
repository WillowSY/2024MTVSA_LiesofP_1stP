using System;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class _Player : MonoBehaviour
{
    // 외부 참조용 player instance
    public static _Player Instance
    {
        get;
        private set;
    } 

    /*
     * Plyaer 관련 수치
     */
    
    // 플레이어 이동 관련 변수들
    public float _speed = 5f;           // 플레이어 기본 walk 속도
    public float _runSpeed;             // 플레이어 Run 속도 : speed의 1.5배
    public float _dodgeSpeed;           // 플레이어 Dodge 속도 : speed의 2배
    public float _finalSpeed;           // 최종 speed, 이동 시 이 변수 속도 활용.
    private Vector3 moving;             // 움직일 벡터.
    public float smoothness;           // 플레이어 회전 감도.
    // 쿨타임
    public float _dodgeRate = 1f;
    
    public float _fireDelay = 0;
    public float _dodgeDelay = 0;
    
    // 플레이어 체력
    private float _maxHealth = 100f;              //플레이어 최대 체력
    public float _curHealth;              //플레이어 현재 체력
    public float _bossDamage = 0;        // 보스가 플레이어에게 입히는 데미지.
    //public float _playerDamage;
    // 마우스 민감도
    public float _sensitivity = 100f;

    // 시야 관련 변수.
    public float _clampAngle = 70f;     // 시야 각도 제한.
    private float rotX;                 // 마우스 각도 인풋.
    private float rotY;
    //public bool toggleCameraRotation; // 둘러보기 기능용 변수.

    // 키 입력 상태 저장 변수들.
    private bool _fDown;
    public bool _aDown;                 // 좌 이동. : A
    public bool _dDown;                 // 우 이동. : D
    public bool _wDown;                 // 상 이동. : W
    public bool _sDown;                 // 하 이동. " S
    public bool _runDown;               //달리기 : Left Shift.
    public bool _dodgeDown;             //회피 : Space bar
    // FIXME : 미구현 조작키들
    //public static bool sprintDown;      //전력질주
    //public static bool defenceDown;     //가드
    
    // activity 활성화 여부 변수들.
    public bool _isFireReady = false;   // 공격 가능.
    public bool _isDodgeReady= false;   // 회피 가능.
    public bool _isDamaged = false;     // 데미지 들어올 시 활성화.
    public bool _isDodge = false;
    
    // 오브젝트 참조
    public Transform realCamera;        // 실제 메인 카메라.
    public Weapon equipWeapon;          // 사용하는 무기 오브젝트.
    public Slider hpSlider;             // 플레이어 체력 바.
    public TMP_Text tmp;                // 플레이어 체력 수치.
    
    #region Properties

        public float speed
        {
            get { return _speed; }
        }
        public float runSpeed
        {
            get { return _runSpeed; }
            private set {  _runSpeed = value; }
        }
        public float dodgeSpeed
        {
            get { return _dodgeSpeed; }
            private set {  _dodgeSpeed = value; }
        }

        public float finalSpeed
        {
            get { return _finalSpeed; }
            set { _finalSpeed = value; }
        }
        public float maxHealth
        {
            get { return _maxHealth; }
        }
        public float curHealth
        {
            get { return _curHealth; }
            set { _curHealth = value; }
        }
        public float sensitivity
        {
            get { return _sensitivity; }
        }
        public float clampAngle
        {
            get { return _clampAngle; }
        }
        public bool fDown
        {
            get { return _fDown; }
            private set { _fDown = value; }
        }

        public bool aDown
        {
            get { return _aDown; }
            private set { _aDown = value; }
        }

        public bool dDown
        {
            get { return _dDown; }
            private set { _dDown = value; }
        }

        public bool wDown
        {
            get { return _wDown; }
            private set { _wDown = value; }
        }

        public bool sDown
        {
            get { return _sDown; }
            private set { _sDown = value; }
        }

        public bool runDown
        {
            get { return _runDown; }
            private set { _runDown = value; }
        }
        
        public bool dodgeDown
        {
            get { return _dodgeDown; }
            private set { _dodgeDown = value; }
        }
        
        public float dodgeRate
        {
            get { return _dodgeRate; }
        }
        public bool isDodgeReady
        {
            get { return _isDodgeReady ; }
            private set { _isDodgeReady = value; }
        }

        public bool isFireReady
        {
            get { return _isFireReady ; }
            private set { _isFireReady = value; }
        }

        public float fireDelay
        {
            get { return _fireDelay; }
            set { _fireDelay = value; }
        }
        public float dodgeDelay
        {
            get { return _dodgeDelay; }
            set { _dodgeDelay = value; }
        }

        public bool isDamaged
        {
            get { return _isDamaged; }
            set { _isDamaged = value; }
        }

        public bool isDodge
        {
            get { return _isDodge; }
            set { _isDodge = value; }
        }
        public float bossDamage
        {
            get { return _bossDamage; }
            set { _bossDamage = value; }
        }

        // public float playerDamage
        // {
        //     get { return _playerDamage; }
        //     set { _playerDamage = value; }
        // }
    #endregion

    private void Awake()
    {
        Instance = this;

        // Player 상태 하위 클래스 initialize
        var fsm = GetComponent<PlayerFSM>();
        if (fsm!=null)
        {
            fsm.Initialize(this);
        }
        
    }

    private void Start()
    {
        curHealth = maxHealth;
        runSpeed = 1.5f * speed;
        dodgeSpeed = 2f * speed;
        //playerDamage = equipWeapon.damage;
    }
    
    void Update()
    {
        GetInput();
        CalcDelay();
        hpSlider.value = (float)curHealth / (float)maxHealth;
        tmp.text = curHealth.ToString();
        
    }

    /*
     * GetInput() : 유저 키 입력 받는 method
     */
    void GetInput()
    {
        /*
         * 각 키 별 정보는 상단 변수 선언 영역에 있습니다.
         */
        fDown = Input.GetButtonDown("Fire1");
        aDown = Input.GetKey(KeyCode.A);
        dDown = Input.GetKey(KeyCode.D);
        wDown = Input.GetKey(KeyCode.W);
        sDown = Input.GetKey(KeyCode.S);
        runDown = Input.GetKey(KeyCode.LeftShift);
        dodgeDown = Input.GetKey(KeyCode.Space);

    }
    
    public void Movement()
    {
        finalSpeed = (runDown) ? runSpeed : speed;
        moving = Vector3.zero;
        if (aDown)
        {
            moving.x = -finalSpeed * Time.deltaTime;
        }
        if (dDown)
        {
            moving.x = +finalSpeed* Time.deltaTime;
        }
        if (wDown)
        {   
            moving.z = +finalSpeed * Time.deltaTime;
        }
        if (sDown)
        {
            moving.z = -finalSpeed * Time.deltaTime;
        }
        /*
         *   FIXME : player lookrotation 등 구현 메소드 의존성 최소화.
         */
        // Player Rotation
        // playerRotate : 카메라가 바라보는 xz평면상 방향 계산.
        Vector3 playerRotate = Vector3.Scale(realCamera.transform.forward, new Vector3(1, 0, 1));
        float targetAngle = Mathf.Atan2(moving.x, moving.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate)*targetRotation, 
            Time.deltaTime * smoothness);
        
        // Player movement
        Vector3 forward = realCamera.forward;
        Vector3 right = realCamera.right;
        Vector3 moveDirection = right * moving.x+ forward * moving.z;
        moveDirection.y = 0f;
        transform.position += moveDirection.normalized * finalSpeed * Time.deltaTime;
        
    }

    void CalcDelay()
    {
        fireDelay += Time.deltaTime;
        dodgeDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;
        isDodgeReady = dodgeRate < dodgeDelay;
    }

    /*
     * 보스 공격 시 플레이어에게 bossdamage 만큼의 피해를 입힘.
     * 보스가 플레이어 공격 시 호출 할 메서드.
     */
    public void TakeDamage(float damage)
    {
        bossDamage = damage;
        isDamaged = true;
    }

}
    
    
    
