using UnityEngine;

public class ForceLandscape : MonoBehaviour
{
    // Assign a UI panel (or any GameObject) that shows the rotate message.
    public GameObject rotateWarningPanel;

    void Start()
    {
        CheckOrientation();
    }

    void Update()
    {
        // Continuously check orientation in case the user rotates the device.
        CheckOrientation();
    }

    void CheckOrientation()
    {
        // For WebGL builds, we compare width and height to detect orientation.
        if (Screen.width < Screen.height)
        {
            // In portrait mode: show the warning panel.
            if (rotateWarningPanel != null)
            {
                rotateWarningPanel.SetActive(true);
            }
        }
        else
        {
            // In landscape mode: hide the warning panel.
            if (rotateWarningPanel != null)
            {
                rotateWarningPanel.SetActive(false);
            }
        }
    }
}