using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // 생성된 직후의 모든 상태를 최대한 상세하게 로그로 남깁니다.
        string ownerInfo = (photonView.Owner != null) ? photonView.Owner.NickName : "아직 없음";
        Debug.Log($"<color=yellow>[PlayerController AWAKE] '{gameObject.name}' 생성 완료! ViewID: {photonView.ViewID}, IsMine: {photonView.IsMine}, Owner: {ownerInfo}</color>");
    }

    void Start()
    {
        // Start 시점의 소유권 정보를 다시 한번 확인합니다.
        string ownerInfo = (photonView.Owner != null) ? photonView.Owner.NickName : "알 수 없음";
        Debug.Log($"<color=green>[PlayerController START] '{gameObject.name}' 시작! ViewID: {photonView.ViewID}, IsMine: {photonView.IsMine}, Owner: {ownerInfo}</color>");

        if (photonView.IsMine)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.gray;
            enabled = false; // 내가 주인이 아니면 Update 비활성화
        }
    }

    void Update()
    {
        // 이동 코드 (변경 없음)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(movement * Time.deltaTime * 5.0f);
    }
}