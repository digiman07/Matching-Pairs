using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText; // Reference to the UI Text element
    public GameObject YouLooseText; // Reference to the UI Text element
    public float timeLimit = 60f; // Total time for the countdown
    public float timeRemaining; // Tracks time left

    public bool timerRunning = false; // Controls timer state

    private void Start()
    {
        timeRemaining = timeLimit; // Initialize with full time
        timerRunning = true; // Start the timer
    }

    private void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Reduce time
                UpdateTimerDisplay(timeRemaining); // Update UI
            }
            else
            {
                timeRemaining = 0; // Ensure time doesn't go negative
                timerRunning = false; // Stop timer
                OnTimerEnd(); // Trigger end-of-timer behavior
            }
        }
    }

    public void SetTimerRunning(bool running)
    {
        timerRunning = running;
    }

    public void UpdateTimerDisplay(float time) // Changed from private to public
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnTimerEnd()
    {
        YouLooseText.SetActive(true);
        timerText.text = "";
        Debug.Log("Time's up!"); // Placeholder for end-of-timer action
        // You can add additional logic here, such as ending the game
    }
}
