using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScratchCardAPI : MonoBehaviour
{
    // Base URL for the API.
    private string baseUrl = "https://testapp.memeprof.com/";

    // The game id for "Scratch the Card". Adjust this value if needed.
    [SerializeField]
    private string scratchCardGameId = "539fba2d-b095-4bf8-8769-e18672417f50";

    public ImageDisplay imageDisplay;

    public Reward[] rewards;
    public GameManager gameManager;
    private void Start()
    {
        StartCoroutine(GetAvailableRewards((List<Reward> fetchedRewards) =>
        {
            if (fetchedRewards != null && fetchedRewards.Count > 0)
            {
                // Convert List to Array and assign it to rewards
                rewards = fetchedRewards.ToArray();

                // Iterate through the rewards and log them
                foreach (Reward reward in rewards)
                {
                    Debug.Log("Reward: " + reward.title);
                    Debug.Log("Image: " + reward.image);

                    if (imageDisplay.IsValidURL(reward.image))
                    {
                        imageDisplay.LoadImageFromURL(reward.image);
                    }
                }
            }
        }));
    }

    public void RewardPicked(int indexer)
    {

        StartCoroutine(SubmitPrize(gameManager.currentUserID, rewards[indexer].id, (bool success) =>
        {
            if (success)
            {
                Debug.Log("Prize submitted successfully!");
            }
            else
            {
                Debug.LogError("Failed to submit prize.");
            }
        }));
    }
    /// <summary>
    /// Retrieves available rewards for Scratch the Card.
    /// </summary>
    /// <param name="callback">Callback to return the list of rewards</param>
    public IEnumerator GetAvailableRewards(Action<List<Reward>> callback)
    {
        string url = $"{baseUrl}/api/rewards?id={scratchCardGameId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Since the API returns a JSON array, we wrap it in an object so that Unity's JsonUtility can parse it.
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
    /// <param name="userId">The user's unique ID</param>
    /// <param name="rewardId">The reward id that was won</param>
    /// <param name="callback">Callback returning true if the submission was successful</param>
    public IEnumerator SubmitPrize(string userId, string rewardId, Action<bool> callback)
    {
        string url = $"{baseUrl}/api/submit-prize";
        SubmitPrizeData data = new SubmitPrizeData
        {
            userId = userId,
            gameId = scratchCardGameId,
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