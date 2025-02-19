using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;
public class FollowCam : MonoBehaviour
{

    //접근 용도 트래킹존 스크립트
    private TrackingZone trackingZone; //트래킹존 스크립트를 담을 변수

    private Transform camTargetTr; //카메라가 추적할 타겟의 트랜스폼 정보
    private Vector2Int minRange; // 추적 범위 최솟값 
    private Vector2Int maxRange; // 추적 범위 최댓값


    [Header("타겟 거리")]
    //타겟과의 거리를 참고해 카메라가 따라가게 하기 위한 기존 수치
    //[Foldout("접기 테스트")]
    [Tooltip("타겟과의 x축 거리")]
    [Range(0.0f, 2.0f)]
    public float distX = 1.0f; //타겟과의 x축 거리

    //[Foldout("접기 테스트")]
    [Tooltip("타겟과의 y축 거리")]
    [Range(0.0f, 2.0f)]
    public float distY = 1.0f; //타겟과의 y축 거리

    //추적 보간 수치
   // [Foldout("접기 테스트")]
    [Tooltip("x축 추적시 부드러움 정도")]
    [Range(1.0f, 10.0f)]
    public float smoothX = 5.0f; //x축 추적시 부드러움 정도

   // [Foldout("접기 테스트")]
    [Tooltip("y축 추적시 부드러움 정도")]
    [Range(1.0f, 10.0f)]
    public float smoothY = 5.0f; //y축 추적시 부드러움 정도

    private void Awake()
    {
        camTargetTr = GameObject.FindWithTag("CameraTarget")?.transform; //카메라 타겟 태그의 트랜스폼 정보를 저장함.
        trackingZone = GameObject.Find("Gizmo_TrackingZone")?.GetComponent<TrackingZone>(); //TrackingZone 스크립트 저장. 게임 오브젝트 이름 검색해 그 오브젝트의 스크립트 저장
        minRange = trackingZone.minXAndY; //최솟값에 트랙킹 존 스크립트의 최솟값 저장
        maxRange = trackingZone.maxXAndY; //최댓값에 트랙킹 존 스크립트의 최댓값 저장

    }

    //거리 검사
    //x축 거리 검사
    bool CheckDistanceX()
    {
        //Mathf는 수학함수, Abs는 절댓값으로 반환(음수는 양수됨)
        //타겟이 카메라보다 멀어졌으면 추적해야 하니 참 반환 
        return Mathf.Abs(transform.position.x - camTargetTr.position.x) > distX; //X축 타겟과의 거리가 distX 거리값을 넘어서면 참을 반환
    }
    //y축 거리 검사
    bool CheckDistanceY()
    {
        return Mathf.Abs(transform.position.y - camTargetTr.position.y) > distY; //Y축 타겟과의 거리가 distY 거리값을 넘어서면 참을 반환
    }

    //카메라 추적 함수
    void CameraTracking()
    {
        // 추적 대상이 없다면 중단
        if (camTargetTr == null) return;

        float currentX = transform.position.x;
        float currentY = transform.position.y;
        float targetX = camTargetTr.position.x;
        float targetY = camTargetTr.position.y;

        // (1) x축 추적
        if (Mathf.Abs(currentX - targetX) > distX)
        {
            // Lerp로 부드럽게 이동
            currentX = Mathf.Lerp(currentX, targetX, Time.deltaTime * smoothX);
        }
        // (2) y축 추적
        if (Mathf.Abs(currentY - targetY) > distY)
        {
            currentY = Mathf.Lerp(currentY, targetY, Time.deltaTime * smoothY);
        }

        // (3) TrackingZone 범위 안에 한정
        currentX = Mathf.Clamp(currentX, minRange.x, maxRange.x);
        currentY = Mathf.Clamp(currentY, minRange.y, maxRange.y);

        // (4) 최종 카메라 위치 갱신
        transform.position = new Vector3(currentX, currentY, transform.position.z);

        //float camPosX = transform.position.x; //카메라의 x포지션
        //float camPosY = transform.position.y; //카메라의 y포지션

        ////x보다 멀어졌다면 (좌우이동)
        //if (CheckDistanceX())
        //{
        //    //Lerp 함수로 카메라 이동하는 것을 부드럽게 해줌
        //    // 거리의 차이 보간, 카메라와 타겟 사이값들을 부드럽게 해줌
        //    camPosX = Mathf.Lerp(transform.position.x, camTargetTr.position.x, smoothX * Time.deltaTime); //타겟 X축 추적 //smoothx에 증가하는 시간을 곱해 부드럽게 증가
        //}
        ////y보다 멀어졌다면 (점프)
        //if (CheckDistanceY())
        //{
        //    camPosY = Mathf.Lerp(transform.position.y, camTargetTr.position.y, smoothY * Time.deltaTime); //타겟 Y축 추적
        //}
        ////추적 범위 안에서만 추적
        //camPosX = Mathf.Clamp(camPosX, minRange.x, maxRange.x); // 카메라 X축 설정 범위내로 추적을 제한함. 
        //camPosY = Mathf.Clamp(camPosY, minRange.y, maxRange.y); // 카메라 Y축 설정 범위내로 추적을 제한함. 

        ////카메라 위치 갱신
        //transform.position = new Vector3(camPosX, camPosY, transform.position.z); //카메라 포지션을 갱신
    }
    //====================================================================================================
    // 매 프레임 후반에 카메라 추적
    //====================================================================================================
    void LateUpdate() //기존은 FixedUpdate
    {
        //카메라가 타겟 추적하기
        CameraTracking();
    }
}
