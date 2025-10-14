using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity; // Recorder 컴포넌트 제어를 위해 추가 (이 스크립트에서는 사용하지 않지만, VoiceController와의 연관성을 고려)

public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;
    private float moveSpeed = 5.0f; // 이동 속도 변수 추가
    private float rotationSpeed = 720.0f; // 회전 속도 (초당 720도)

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // 생성된 직후의 모든 상태를 최대한 상세하게 로그로 남깁니다.
        string ownerInfo = (photonView.Owner != null) ? photonView.Owner.NickName : "아직 없음";
        Debug.Log($"<color=yellow>[PlayerController AWAKE] '{gameObject.name}' 생성 완료! ViewID: {photonView.ViewID}, IsMine: {photonView.IsMine}, Owner: {ownerInfo}</color>");
    }

    void Start()
    {
        string ownerInfo = (photonView.Owner != null) ? photonView.Owner.NickName : "알 수 없음";
        Debug.Log($"<color=green>[PlayerController START] '{gameObject.name}' 시작! ViewID: {photonView.ViewID}, IsMine: {photonView.IsMine}, Owner: {ownerInfo}</color>");

        if (photonView.IsMine)
        {
            // 내 캐릭터일 때만 색상을 노란색으로 변경
            // Renderer 컴포넌트가 없다면 오류가 발생하므로 확인 필요
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }
        }
        else
        {
            // 다른 플레이어 캐릭터일 때 색상을 회색으로 변경
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.gray;
            }
            enabled = false; // 다른 플레이어의 PlayerController는 비활성화
        }
    }

    void Update()
    {
        // 내 캐릭터만 조작 가능
        if (!photonView.IsMine)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // 입력 벡터 생성
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized; // normalized로 대각선 이동 속도 보정

        // 이동
        if (movement.magnitude > 0.1f) // 작은 입력에도 회전하는 것을 방지
        {
            transform.Translate(movement * Time.deltaTime * moveSpeed, Space.World); // Space.World를 사용하여 로컬 좌표계 영향 없이 이동

            // 회전: 입력 방향으로 부드럽게 회전
            Quaternion toRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}