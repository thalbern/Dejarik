
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance = null; //singleton instance

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");

        //PhotonNetwork.JoinRandomRoom();

        var roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        if (!PhotonNetwork.JoinOrCreateRoom("debug", roomOptions, TypedLobby.Default))
        {
            Debug.LogError("Failed to queue JoinRoom op.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room (#{returnCode}): {message}");
    }
}
