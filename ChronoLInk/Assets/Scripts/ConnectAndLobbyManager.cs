using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq; 
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class ConnectAndLobbyManager : MonoBehaviourPunCallbacks
{
   
    public const string PLAYER_CHARACTER_KEY = "character";

    [Header("UI Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject roomPanel;

    [Header("Login Panel")]
    [SerializeField] private InputField nicknameInputField;
    [SerializeField] private Button loginButton;

    [Header("Lobby Panel")]
    [SerializeField] private Button showCreateRoomButton;
    [SerializeField] private GameObject roomListContent;
    [SerializeField] private GameObject roomEntryPrefab;

    [Header("Create Room Panel")]
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private Button confirmCreateRoomButton;

    [Header("Room Panel")]
    [SerializeField] private Text roomNameText;
    [SerializeField] private GameObject playerListContent;
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private Button selectPastButton;
    [SerializeField] private Button selectFutureButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    #region Unity & Connection Flow
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // 패널 각각 비활성화 로그인 패널이 처음 로그인 UI임
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(false);

        // 인풋필드 리스너
        nicknameInputField.onValueChanged.AddListener(OnNicknameInputValueChanged);
        roomNameInputField.onValueChanged.AddListener(OnRoomNameInputValueChanged);

        // 버튼 리스너
        loginButton.onClick.AddListener(Login);
        showCreateRoomButton.onClick.AddListener(() => createRoomPanel.SetActive(true));
        confirmCreateRoomButton.onClick.AddListener(CreateRoom);
        leaveRoomButton.onClick.AddListener(LeaveRoom);
        selectPastButton.onClick.AddListener(() => SelectCharacter("Past"));
        selectFutureButton.onClick.AddListener(() => SelectCharacter("Future"));
        startGameButton.onClick.AddListener(StartGame);

        
        loginButton.interactable = false;
        confirmCreateRoomButton.interactable = false;
    }
    #endregion

    #region UI Input & Button Logic
    private void OnNicknameInputValueChanged(string value)
    {
        loginButton.interactable = !string.IsNullOrWhiteSpace(value);
    }

    private void OnRoomNameInputValueChanged(string value)
    {
        confirmCreateRoomButton.interactable = !string.IsNullOrWhiteSpace(value);
    }
    #endregion

    #region Photon Login & Room Logic
    private void Login()
    {
        loginButton.interactable = false;
        PhotonNetwork.NickName = nicknameInputField.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void CreateRoom()
    {
        confirmCreateRoomButton.interactable = false;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2, IsVisible = true, IsOpen = true };
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    
    private void SelectCharacter(string character)
    {
        PhotonHashtable props = new PhotonHashtable();
        props[PLAYER_CHARACTER_KEY] = character;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    
    private void StartGame()
    {
        // 마스터 클라이언트만 게임을 시작할 수 있음
        if (PhotonNetwork.IsMasterClient)
        {
            // 게임 시작 조건을 다시 한번 확인
            if (CheckAllPlayersReady())
            {
                Debug.Log("모든 플레이어 준비 완료, 게임을 시작합니다!");
                PhotonNetwork.LoadLevel("GameScene"); // 연결되는 게임씬 이름 설정 
                                                        // 다만 비동기 로딩으로 바꿀거라 바뀔 수 있음
            }
            else
            {
                Debug.LogWarning("모든 플레이어가 역할을 선택하지 않았습니다.");
            }
        }
    }
    #endregion

    #region Photon Callbacks

    // 아무것도 건들지 말아줘. 오류나면 어디서부터 고쳐야 할지 감이 안와
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        confirmCreateRoomButton.interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        UpdateRoomListUI();
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }

   
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, PhotonHashtable changedProps)
    {
       
        UpdatePlayerList();
    }
    #endregion

    #region UI Update
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            if (info.RemovedFromList) cachedRoomList.Remove(info.Name);
            else cachedRoomList[info.Name] = info;
        }
    }

    private void UpdateRoomListUI()
    {
        foreach (Transform child in roomListContent.transform) Destroy(child.gameObject);
        foreach (var roomEntry in cachedRoomList.Values)
        {
            if (roomEntry.PlayerCount <= 0 || !roomEntry.IsVisible) continue;
            GameObject newEntry = Instantiate(roomEntryPrefab, roomListContent.transform);
            Text roomNameText = newEntry.transform.Find("RoomNameText").GetComponent<Text>();
            Text playerCountText = newEntry.transform.Find("PlayerCountText").GetComponent<Text>();
            Button entryButton = newEntry.GetComponent<Button>();
            roomNameText.text = roomEntry.Name;
            playerCountText.text = $"{roomEntry.PlayerCount}/{roomEntry.MaxPlayers}";
            entryButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomEntry.Name));
        }
    }

   
    private void UpdatePlayerList()
    {
        foreach (Transform child in playerListContent.transform) Destroy(child.gameObject);
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            GameObject newEntry = Instantiate(playerEntryPrefab, playerListContent.transform);
            Text playerNameText = newEntry.transform.Find("PlayerNameText").GetComponent<Text>();

           
            if (player.CustomProperties.TryGetValue(PLAYER_CHARACTER_KEY, out object character))
            {
                playerNameText.text = $"{player.NickName} ({character})";
            }
            else
            {
                playerNameText.text = $"{player.NickName} (선택 중...)";
            }
        }
       
        UpdateRoomButtons();
    }

    
    private void UpdateRoomButtons()
    {
       
        HashSet<string> selectedCharacters = new HashSet<string>();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(PLAYER_CHARACTER_KEY, out object character))
            {
                selectedCharacters.Add((string)character);
            }
        }

        // 선택 버튼 활성화라ㅇ 비활성화
        selectPastButton.interactable = !selectedCharacters.Contains("Past");
        selectFutureButton.interactable = !selectedCharacters.Contains("Future");

        // 시작 버튼은 방장한테만 보이게 해놓고 후에 플레이어가 모두 역할을 선택했다면 스타트 시작 가능
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        startGameButton.interactable = CheckAllPlayersReady();
    }

   
    private bool CheckAllPlayersReady()
    {
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.PlayerCount != 2)
        {
            return false;
        }

        var players = PhotonNetwork.PlayerList;
        bool pastSelected = players.Any(p => p.CustomProperties.ContainsValue("Past"));
        bool futureSelected = players.Any(p => p.CustomProperties.ContainsValue("Future"));

        return pastSelected && futureSelected;
    }
    #endregion
}