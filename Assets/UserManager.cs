using UnityEngine;
using System;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }

    [SerializeField]
    private string userId = "";

    private void Awake()
    {
        // Ensure only one instance exists.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Attempt to load the user id from PlayerPrefs.
        if (PlayerPrefs.HasKey("UserId"))
        {
            string storedId = PlayerPrefs.GetString("UserId");
            // Validate the stored userId is a proper GUID.
            if (IsValidGuid(storedId))
            {
                userId = storedId;
                Debug.Log("Loaded valid User ID: " + userId);
            }
            else
            {
                // Generate a new GUID if the stored value is invalid.
                userId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("UserId", userId);
                PlayerPrefs.Save();
                Debug.Log("Stored User ID was invalid. Generated new User ID: " + userId);
            }
        }
        else
        {
            // No stored userId, so generate a new one.
            userId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("UserId", userId);
            PlayerPrefs.Save();
            Debug.Log("Generated new User ID: " + userId);
        }
    }

    /// <summary>
    /// Returns the current user ID.
    /// </summary>
    public string GetUserId()
    {
        return userId;
    }

    /// <summary>
    /// Sets the user ID if it's a valid GUID.
    /// </summary>
    public void SetUserId(string newUserId)
    {
        if (IsValidGuid(newUserId))
        {
            userId = newUserId;
            PlayerPrefs.SetString("UserId", newUserId);
            PlayerPrefs.Save();
            Debug.Log("User ID updated to: " + userId);
        }
        else
        {
            Debug.LogError("Provided user ID is not a valid GUID: " + newUserId);
        }
    }

    /// <summary>
    /// Checks if the given string is a valid GUID.
    /// </summary>
    private bool IsValidGuid(string guidString)
    {
        Guid guid;
        return Guid.TryParse(guidString, out guid);
    }
}