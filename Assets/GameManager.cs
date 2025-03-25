using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public string currentUserID;
    public bool runOnce = false;
    public void SetUserId(string userId)
    {
        if (runOnce)
        {
            return;
        }
        currentUserID = userId;
        Debug.Log("Received User ID from API:" + userId);
        ScratchCardAPI.instance.userIDfromAPI = currentUserID;
        runOnce = true;
    }
}
