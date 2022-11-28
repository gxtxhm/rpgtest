using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject DisconnectPanel;
    public GameObject InGamePanel;
    public GameObject player;
    public Canvas canvas;
    PhotonView PV;
    public Text NickNameText;

    [Header("Disconnect")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();
    public InputField EmailInput, PasswordInput, UsernameInput;
    

    [Header("Lobby")]
    //public InputField UserNickNameInput;
    //public Text LobbyInfoText, UserNickNameText;

    [Header("Room")]
    //public InputField SetDataInput;
    //public GameObject SetDataBtnObj;
    //public Text UserHouseDataText, RoomNameInfoText, RoomNumInfoText;

    bool isLoaded;




    #region 플레이팹
    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PV = GetComponent<PhotonView>();
    }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PhotonNetwork.LocalPlayer.NickName = NickNameText.text;
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => 
        { GetLeaderboard(result.PlayFabId); PhotonNetwork.ConnectUsingSettings(); }, (error) => print("로그인 실패"));
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => 
        { print("회원가입 성공"); SetStat(); SetData("default"); }, (error) => print("회원가입 실패"));
    }



    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("값 저장실패"));
    }

    void GetLeaderboard(string myID)
    {
        PlayFabUserList.Clear();

        for (int i = 0; i < 10; i++)
        {
            var request = new GetLeaderboardRequest { StartPosition = i * 100, StatisticName = "IDInfo", MaxResultsCount = 100,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true } };
            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    PlayFabUserList.Add(result.Leaderboard[j]);
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }
            },
            (error) => { });
        }
    }



    void SetData(string curData)
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { "Home", curData } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { }, (error) => print("데이터 저장 실패"));
    }

    void GetData(string curID)
    {
        //PlayFabClientAPI.GetUserData(new GetUserDataRequest() { PlayFabId = curID }, (result) =>
        //UserHouseDataText.text = curID + "\n" + result.Data["Home"].Value,
        //(error) => print("데이터 불러오기 실패"));
    }
    #endregion



    #region 로비
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        
    }

    //void Update() => LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";

    [PunRPC]
    void SetPlayer(int id)
    {
        Instantiate(player);
    }
    public override void OnJoinedLobby()
    {
        // 방에서 로비로 올 땐 딜레이없고, 로그인해서 로비로 올 땐 PlayFabUserList가 채워질 시간동안 1초 딜레이
        //if (isLoaded)
        //{

        //}
        //else Invoke("OnJoinedLobbyDelay", 1);

        OnJoinedLobbyDelay();
    }

    void OnJoinedLobbyDelay()
    {
        isLoaded = true;
        PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;
        DisconnectPanel.SetActive(false);

        Instantiate(player);

        // 캐릭터,다른 캐릭터와 몬스터들 다 로딩 - 몬스터매니저에서 다 받아오기
        //PV.RPC("SetPlayer", RpcTarget.AllBuffered);

        //if(PhotonNetwork.CountOfPlayers-PhotonNetwork.CountOfPlayersInRooms>=2)
        {
            for(int i=0;i< PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms;i++)
            {
                //Instantiate(PhotonNetwork.PlayerList[i].);
            }
        }

        InGamePanel.SetActive(true);
        canvas.GetComponent<InventoryUI>().enabled = true;
    }

    void ShowPanel(GameObject CurPanel)
    {
        DisconnectPanel.SetActive(false);

        CurPanel.SetActive(true);
    }

    void ShowUserNickName()
    {
        //UserNickNameText.text = "";
        //for (int i = 0; i < PlayFabUserList.Count; i++) UserNickNameText.text += PlayFabUserList[i].DisplayName + "\n";
    }

    public void XBtn()
    {
        if (PhotonNetwork.InLobby) PhotonNetwork.Disconnect();
        else if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isLoaded = false;
        ShowPanel(DisconnectPanel);
    }
    #endregion



    #region 방
    public void JoinOrCreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions() { MaxPlayers = 2 }, null);

        Debug.Log("방입장");

    }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");


    // 방에 참가하면 호출되는 콜백함수이다. 
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            //PV.RPC("Matched" , RpcTarget.All);

            // 보스맵 입장 및 2명 생성

            Debug.Log("매칭 성공!");
        }

        //string curName = PhotonNetwork.CurrentRoom.Name;
        //RoomNameInfoText.text = curName;
        
        //유저방이면 데이터 가져오기
        {
            

            //string curID = PhotonNetwork.CurrentRoom.CustomProperties["PlayFabID"].ToString();
            //GetData(curID);

            // 현재 방 PlatyFabID 커스텀 프로퍼티가 나의 PlayFabID와 같다면 값을 저장할 수 있음
            //if (curID == MyPlayFabInfo.PlayFabId)
            {
                //RoomNameInfoText.text += " (나의 방)";

                //SetDataInput.gameObject.SetActive(true);
                //SetDataBtnObj.SetActive(true);
            }
        }
    }



    public void SetDataBtn()
    {
        // 자기자신의 방에서만 값 저장이 가능하고, 값 저장 후 1초 뒤에 값 불러오기
        //SetData(SetDataInput.text);
        Invoke("SetDataBtnDelay", 1);
    }

    void SetDataBtnDelay() => GetData(PhotonNetwork.CurrentRoom.CustomProperties["PlayFabID"].ToString());
    #endregion
}
