using UnityEngine;

[ExecuteAlways]
public class IntersectionPlanes : MonoBehaviour
{
    [Header("사거리 코너 (월드 좌표)")]
    public Transform cornerTL;
    public Transform cornerTR;
    public Transform cornerBR;
    public Transform cornerBL;

    [Header("카메라(또는 캐릭터)")]
    public Transform target;

    [Header("Plane 4개")]
    public MeshRenderer[] planes = new MeshRenderer[4];

    void LateUpdate()
    {
        if (target == null) return;
        // 1) 코너 위치 가져오기
        Vector3[] corners = new Vector3[4]
        {
            cornerTL.position,
            cornerTR.position,
            cornerBR.position,
            cornerBL.position
        };

        // 2) 4개의 모서리에 대해 처리
        for (int i = 0; i < 4; i++)
        {
            int iNext = (i + 1) % 4;
            var rend = planes[i];
            // 중간점
            Vector3 mid = (corners[i] + corners[iNext]) * 0.5f;
            // 길이
            float length = Vector3.Distance(corners[i], corners[iNext]);
            // Plane은 기본 Y축이 상향(0,1,0), 앞면이 +Z 축
            rend.transform.position = mid;
            // 카메라-코너 중간으로 향하게 회전
            Vector3 toCam = (target.position - mid).normalized;
            // Plane의 로컬 전면(+Z)이 toCam 방향이 되도록 회전
            rend.transform.rotation = Quaternion.LookRotation(toCam, Vector3.up);
            // 스케일: X = 길이, Y = 높이(원하는 만큼)
            // 기본 Quad 크기가 1×1이라면:
            rend.transform.localScale = new Vector3(length, /*높이*/10f, 1f);
            // 머티리얼은 Cull Front 이고 Unlit Black Material
            // (앞면은 생기는 레이캐스트로 가려지고, 뒷면만 보임)
        }
    }
}
