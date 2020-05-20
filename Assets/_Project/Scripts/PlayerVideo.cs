using agora_gaming_rtc;
using System;
using UnityEngine;

public class PlayerVideo : MonoBehaviour
{
    public VideoSurface videoSurface;
    public void Set(uint uid, Action action = null)
    {
        videoSurface.SetForUser(uid);
        videoSurface.SetEnable(true);
        videoSurface.SetGameFps(30);
        action?.Invoke();
    }

    public void Clear()
    {
        videoSurface.SetEnable(false);
        videoSurface.gameObject.SetActive(false);
    }
}