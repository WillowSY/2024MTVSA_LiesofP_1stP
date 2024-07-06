using Unity.Mathematics.Geometry;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform objectTofollow;
    public float followSpeed=10f;
    public float sensitivity;
    public float clampAngle = 70f;
    
    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector4 finalDir;
    public float minDistance;
    public float maxDistance;
    public float finalDistance;
    public float smoothness = 10f;
    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.position.normalized;
        finalDistance = realCamera.localPosition.magnitude;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    } 
    void Update()
    {
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    
    // FIXME : 시작 시 카메라 시점 가끔 이상하게 튀는 현상 고치기.
    // TODO : 적 타겟시 시야 고정 기능 추가.
    // TODO : 둘러보기 기능 추가. 인게임에 존재하는지 확인 필요.
    // TODO : 메인카메라와 플레이어 사이에 오브젝트가 존재할 때 카메라 위치 오브젝트 앞으로 이동하기
    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, objectTofollow.position, followSpeed);
        finalDir = dirNormalized * maxDistance;
        // 
        // RaycastHit hit;
        //
        // if (Physiacs.Linecast(transform.position, finalDir, out hit))
        // {
        //     finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        // }
        // else
        // {
        //     finalDistance = maxDistance;
        // }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized*finalDistance, Time.deltaTime*smoothness);
    }
}
