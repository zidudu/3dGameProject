using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingZone : MonoBehaviour
{
    //사각형으로 감싸기
    [Header("X,Y 최소최대값 및 컬러")]
    [Tooltip("x,y 최솟값 정의")]
    //x y 값을 인스펙터 창에서 조절할 수 있음.
    public Vector2Int minXAndY; //x,y 최솟값
    [Tooltip("x,y 최댓값 정의")]
    public Vector2Int maxXAndY; //x,y 최댓값
    [Tooltip("색깔 정의")]
    public Color mainColor = new Color(0.0f, 1.0f, 1.0f, 1.0f); // 메인 라인 컬러


    [Header("타이틀 메쉬")]
    [Tooltip("드로우 메쉬로 활용할 타이틀 메쉬 정의")]
    public Mesh titleMesh; //드로우 메쉬로 활용할 타이틀 메쉬

    [Tooltip("타이틀 좌우조절")]
    [Range(0.0f, 10.0f)] //이 범위만큼의 슬라이드바가 나와서 조절하기 쉬움.
    public float titleXpos = 1.0f; //타이틀 좌우조절
    [Tooltip("타이틀 상하조절")]
    [Range(-1.0f, 1.0f)]
    public float titleYpos = 1.0f; //타이틀 상하조절
    [Tooltip("타이틀 크기조절")]
    [Range(0.0f, 1.0f)]
    public float titleSize = 1.0f; //타이틀 크기조절


    private void OnDrawGizmos()
    {
        Color subColor = new Color(mainColor.r, mainColor.g, mainColor.b, 0.1f * mainColor.a); //보조컬러 //반투명 색깔
        Vector3 titlePos = new Vector3(maxXAndY.x - titleXpos, maxXAndY.y + titleYpos, 0.0f); //타이틀 위치 //범위 기준값
        Vector3 titleScale = new Vector3(titleSize, titleSize, titleSize); //타이틀 크기

        Gizmos.DrawMesh(titleMesh, titlePos, transform.rotation, titleScale); // 타이틀 메쉬 기즈모 //타이틀 메쉬 3d 있음.

        //가로세로 그려서 사각형 만듬
        //세로
        for (int x = minXAndY.x; x <= maxXAndY.x; x++) //X축 최솟값이 최댓값보다 작거나 같을때까지 반복함
        {
            if (x - maxXAndY.x == 0 || x - minXAndY.x == 0) //X축 값이 최솟값,최댓값 같을때 실행 //즉 끝줄이라 판단되면 메인컬러 넣음
            {
                Gizmos.color = mainColor; // 메인컬러로 지정
            }
            else // 그외에 선은 흐린 선
            {
                Gizmos.color = subColor; //서브컬러로 지정
            }
            //세로줄 시작과 끝
            Vector3 pos1 = new Vector3(x,minXAndY.y, 0); //세로줄 시작점
            Vector3 pos2 = new Vector3(x, maxXAndY.y, 0); //세로줄 끝점

            Gizmos.DrawLine(pos1, pos2); //시작점부터 끝점으로 세로줄을 그림.
        }

        //세로
        for (int y = minXAndY.y; y <= maxXAndY.y; y++) //Y축 최솟값이 최댓값보다 작거나 같을때까지 반복함
        {
            if (y - maxXAndY.y == 0 || y - minXAndY.y == 0) //Y축 값이 최솟값,최댓값 같을때 실행
            {
                Gizmos.color = mainColor; // 메인컬러로 지정
            }
            else
            {
                Gizmos.color = subColor; //서브컬러로 지정
            }
            //세로줄 시작과 끝
            Vector3 pos1 = new Vector3(minXAndY.x, y, 0); //가로줄 시작점
            Vector3 pos2 = new Vector3(maxXAndY.x, y, 0); //가로줄 끝점

            Gizmos.DrawLine(pos1, pos2); //시작점부터 끝점으로 가로줄을 그림.
        }



    }
}
