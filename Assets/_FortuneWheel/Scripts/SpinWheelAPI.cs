using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpinWheelAPI : MonoBehaviour
{
    // Base URL for the API.
    private string baseUrl = "https://testapp.memeprof.com/";

    // The game id for "Spin the Wheel".
    [SerializeField]
    private string spinWheelGameId = "93a7b5c9-f319-42f1-9728-0a435ecd1982";


    private void Start()
    {
        StartCoroutine(GetAvailableRewards((List<Reward> rewards) =>
            {
                if (rewards!=null)
                {
                    foreach (Reward reward in rewards)
                    {
                        Debug.Log("Rewards: "+reward.title);
                    }
                }
        }));
    }

    /// <summary>
    /// Retrieves available rewards for Spin the Wheel.
    /// </summary>
    public IEnumerator GetAvailableRewards(Action<List<Reward>> callback)
    {
        string url = $"{baseUrl}/api/rewards?id={spinWheelGameId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Wrap JSON to use Unity's JsonUtility.
                string jsonResponse = request.downloadHandler.text;
                string wrappedJson = "{\"rewards\":" + jsonResponse + "}";
                RewardListWrapper rewardsWrapper = JsonUtility.FromJson<RewardListWrapper>(wrappedJson);
                List<Reward> rewardsList = new List<Reward>(rewardsWrapper.rewards);
                callback(rewardsList);
            }
            else
            {
                Debug.LogError("Error fetching rewards: " + request.error);
                callback(null);
            }
        }
    }

    /// <summary>
    /// Submits the prize won by the user.
    /// </summary>
    public IEnumerator SubmitPrize(string userId, string rewardId, Action<bool> callback)
    {
        string url = $"{baseUrl}/api/submit-prize";
        SubmitPrizeData data = new SubmitPrizeData
        {
            userId = userId,
            gameId = spinWheelGameId,
            gameRewardId = rewardId
        };

        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success || request.responseCode == 201)
            {
                Debug.Log("Prize submitted successfully");
                callback(true);
            }
            else
            {
                Debug.LogError("Error submitting prize: " + request.error);
                callback(false);
            }
        }
    }
}

[Serializable]
public class Reward
{
    public string id;
    public string title;
    public string value;
    public string image;
    public int chance;
    public string type;
    public string specialConditions;
    public string gameId;
    public string createdAt;
    public string updatedAt;
    public GameInfo game;
}

[Serializable]
public class GameInfo
{
    public string title;
}

[Serializable]
public class RewardListWrapper
{
    public Reward[] rewards;
}

[Serializable]
public class SubmitPrizeData
{
    public string userId;
    public string gameId;
    public string gameRewardId;
}