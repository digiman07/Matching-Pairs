using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardID; // Unique ID for each pair
    public Image frontImage; // Image for the card front
    public Image backImage; // Image for the card back
    public Button button;
    public AudioSource audioSource; // Reference to the AudioSource

    private bool isFlipped = false;

    public void Initialize(int id, Sprite frontSprite)
    {
        cardID = id;
        frontImage.sprite = frontSprite;
        ResetCard();
    }

    public void FlipCard()
    {
        if (audioSource != null) // Play the click sound
        {
            audioSource.Play();
        }

        if (isFlipped) return;

        isFlipped = true;
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
    }

    public void ResetCard()
    {
        isFlipped = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }
}