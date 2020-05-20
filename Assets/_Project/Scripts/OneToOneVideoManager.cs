using UnityEngine;
using UnityEngine.UI;

using agora_gaming_rtc;

#if (UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif

namespace IsmaelNascimentoAssets
{
    public class OneToOneVideoManager : MonoBehaviour
    {
        #region VARIABLES

        [Header("AgoraIO Settings")]
        //The name of the channel the user should join
        [SerializeField] private string channelName = "defaultChannel";

        //The AppID of the Agora Project, from the Dashboard
        // Get your own App ID at https://dashboard.agora.io/
        [SerializeField] private string appId = "a9fb9eebeec746c082765d1a2a043f5d";

        [Header("Scene References")]
        [SerializeField] private Button callButton;
        [SerializeField] private Sprite joinCallSprite;
        [SerializeField] private Sprite leaveCallSprite;
        [SerializeField] private PlayerVideo localPlayer;
        [SerializeField] private PlayerVideo otherPlayer;

        //The Agora chat engine
        private IRtcEngine mRtcEngine = null;
        private uint myId;

        #endregion

        #region MONOBEHAVIOUR_METHODS

        private void Start()
        {

#if (UNITY_2018_3_OR_NEWER)
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif

            mRtcEngine = IRtcEngine.GetEngine(appId);

            //Add the listener to the join button to allow the player to join the channel.
            callButton.onClick.AddListener(JoinChannel);
        }

        #endregion

        #region PUBLIC_METHODS

        public void SetChannel(string channelName)
        {
            this.channelName = channelName;
        }

        #endregion

        #region PRIVATE_METHODS

        private void JoinChannel()
        {
            Debug.LogFormat("Joining Channel...");

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            //Add our callbacks to handle Agora events
            mRtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
            mRtcEngine.OnUserJoined += OnUserJoined;
            mRtcEngine.OnUserOffline += OnUserLeave;
            mRtcEngine.OnLeaveChannel += OnLeaveChannel;

            callButton.onClick.RemoveListener(JoinChannel);
            callButton.onClick.AddListener(LeaveChannel);
            callButton.GetComponent<Image>().sprite = leaveCallSprite;

            if (string.IsNullOrEmpty(channelName))
            {
                return;
            }

            mRtcEngine.JoinChannel(channelName, "extra", 0);
        }

        //When you join the channel...
        private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
        {
            Debug.Log("Joined with uid " + uid);
            myId = uid;

            //Set to zero to show local input
            localPlayer.gameObject.SetActive(true);
            localPlayer.Set(0);
        }

        //When a remote user joins the channel
        private void OnUserJoined(uint uid, int elapsed)
        {
            string userJoinedMessage = string.Format("onUserJoined callback uid {0} {1}", uid, elapsed);

            Debug.Log(userJoinedMessage);

            otherPlayer.Set(uid);
            otherPlayer.gameObject.SetActive(true);
        }

        //Handles the other player leaving
        private void OnUserLeave(uint uid, USER_OFFLINE_REASON reason)
        {
            otherPlayer.Clear();
        }

        //Handles leaving the channel when the button is pressed
        private void LeaveChannel()
        {
            Debug.LogFormat("Leaving Channel");

            callButton.onClick.RemoveListener(LeaveChannel);
            callButton.onClick.AddListener(JoinChannel);
            callButton.GetComponent<Image>().sprite = joinCallSprite;

            mRtcEngine.LeaveChannel();
            localPlayer.Clear();
            otherPlayer.Clear();

            mRtcEngine.DisableVideoObserver();

            mRtcEngine.OnJoinChannelSuccess -= OnJoinChannelSuccess;
            mRtcEngine.OnUserJoined -= OnUserJoined;
            mRtcEngine.OnUserOffline -= OnUserLeave;
            mRtcEngine.OnLeaveChannel -= OnLeaveChannel;
        }

        //When someone leaves the channel...
        private void OnLeaveChannel(RtcStats stats)
        {
            otherPlayer.Clear();
        }

        #endregion


    }
}