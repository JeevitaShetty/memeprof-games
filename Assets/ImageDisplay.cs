using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ImageDisplay : MonoBehaviour
{
    // UI Image component to display the fetched image.
    [SerializeField]
    private SpriteRenderer[] targetImage;
    public int rewardIndex;
    /// <summary>
    /// Initiates fetching and displaying the image from the specified URL.
    /// </summary>
    /// <param name="imageUrl">URL of the image to fetch.</param>
    public void LoadImageFromURL(string imageUrl)
    {
        StartCoroutine(FetchAndDisplayImage(imageUrl));
    }

    public bool IsValidURL(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }


    /// <summary>
    /// Coroutine that fetches an image from a URL and assigns it as a sprite to the UI Image.
    /// </summary>
    /// <param name="url">The URL of the image.</param>
    private IEnumerator FetchAndDisplayImage(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded texture.
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                // Convert texture to a sprite.
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                              new Vector2(0.5f, 0.5f));

                // Assign the sprite to the target UI Image.
                if(rewardIndex<= targetImage.Length)
                {
                    targetImage[rewardIndex].sprite = sprite;
                    rewardIndex++;
                }
            }
            else
            {
                Debug.LogError("Error fetching image: " + request.error);
            }
        }
    }
}