using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public string currentUserID;
    public void SetUserId(string userId)
    {
        currentUserID = userId;
        Debug.Log("Received User ID:" + userId);
    }
}
