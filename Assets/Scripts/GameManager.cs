using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab; // Prefab for the cards
    public Transform cardParent; // Parent object to hold cards
    public List<Sprite> cardSprites; // List of front images
    public int rows = 4; // Number of rows
    public int columns = 4; // Number of columns
    public GameObject winText; // Assign in Inspector

    public Text levelUpText; // Assign in Inspector
    public Text countdownText; // Assign in Inspector
    private bool isLevelComplete = false;
    private float restartCountdown = 3f;

    public AudioSource audioSource; // Assign in the Inspector
    public AudioClip matchSound; // Assign in the Inspector
    public AudioClip allMatchedSound; // Assign in the Inspector

    private List<Card> activeCards = new List<Card>();
    private Card firstFlippedCard;
    private Card secondFlippedCard;
    public Timer timer;

    void Start()
    {
        GenerateCards();
    }

    private void DisplayWinText()
    {
        if (winText != null)
        {
            winText.SetActive(true);
        }
    }

    private void Update()
    {
        if (isLevelComplete)
        {
            HandleLevelComplete();
        }
    }


    void GenerateCards()
    {
        List<int> cardIDs = new List<int>();
        for (int i = 0; i < (rows * columns) / 2; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i); // Duplicate for pairs
        }

        cardIDs = Shuffle(cardIDs);

        for (int i = 0; i < rows * columns; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardParent);
            Card card = newCard.GetComponent<Card>();
            card.Initialize(cardIDs[i], cardSprites[cardIDs[i]]);
            card.button.onClick.AddListener(() => OnCardClicked(card));
        }
    }

    void OnCardClicked(Card clickedCard)
    {
        // Ignore clicks on already flipped cards or if two cards are being compared
        if (clickedCard.IsFlipped() || firstFlippedCard != null && secondFlippedCard != null)
        {
            return;
        }

        clickedCard.FlipCard();

        if (firstFlippedCard == null)
        {
            firstFlippedCard = clickedCard;
        }
        else
        {
            secondFlippedCard = clickedCard;
            CheckForMatch();
        }
    }

    void CheckForMatch()
    {
        if (firstFlippedCard.cardID == secondFlippedCard.cardID)
        {
            PlayMatchSound(); // Play match sound
            // It's a match! Keep cards flipped
            ResetFlipState();

            // Check if all cards are matched
            CheckAllCardsMatched();
        }
        else
        {
            // Not a match, flip cards back after delay
            StartCoroutine(FlipBackCards());
        }
    }

    private void PlayMatchSound()
    {
        if (audioSource != null && matchSound != null)
        {
            audioSource.PlayOneShot(matchSound);
        }
        else
        {
            Debug.LogWarning("Match sound or AudioSource is missing!");
        }
    }

    private void CheckAllCardsMatched()
    {
        foreach (Card card in cardParent.GetComponentsInChildren<Card>())
        {
            if (!card.IsFlipped())
            {
                return;
            }
        }

        // Player wins - stop the timer
        if (timer != null)
        {
            timer.SetTimerRunning(false);
        }

        // Show level up and start countdown
        isLevelComplete = true;
        levelUpText.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        PlayAllMatchedSound();
        DisplayWinText();
    }

    private void HandleLevelComplete()
    {
        if (!isLevelComplete) return;

        restartCountdown -= Time.deltaTime;
        countdownText.text = $"Restarting in: {Mathf.Ceil(restartCountdown)}";

        if (restartCountdown <= 0)
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        // Reset all cards
        foreach (Card card in cardParent.GetComponentsInChildren<Card>())
        {
            Destroy(card.gameObject);
        }

        // Reset UI
        levelUpText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        winText.SetActive(false);

        // Reset variables
        isLevelComplete = false;
        restartCountdown = 3f;
        ResetFlipState();

        // Regenerate cards and restart timer
        GenerateCards();
        if (timer != null)
        {
            timer.SetTimerRunning(true);
            timer.timeRemaining = timer.timeLimit;
            timer.UpdateTimerDisplay(timer.timeRemaining);
        }
    }

    private void PlayAllMatchedSound()
    {
        if (audioSource != null && allMatchedSound != null)
        {
            audioSource.PlayOneShot(allMatchedSound);
        }
        else
        {
            Debug.LogWarning("All matched sound or AudioSource is missing!");
        }
    }

    System.Collections.IEnumerator FlipBackCards()
    {
        yield return new WaitForSeconds(1f);
        firstFlippedCard.ResetCard();
        secondFlippedCard.ResetCard();
        ResetFlipState();
    }

    void ResetFlipState()
    {
        firstFlippedCard = null;
        secondFlippedCard = null;
    }

    List<int> Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}