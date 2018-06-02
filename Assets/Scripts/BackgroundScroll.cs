using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroll : MonoBehaviour {

    // Background scroll speed can be set in Inspector with slider
    [Range(1f, 20f)]
    public float scrollSpeed = 1f;

    // Size of the image for scroll offset purposes
    float imageSize;

    // Start position of background movement
    Vector2 startPos;

    // Backgrounds new position
    float newPos;

    // RectTransform of the object
    RectTransform rect;

    // Image object for background image.
    public Image bgImage;

    // Use this for initialization
    void Start()
    {
        // Getting backgrounds start position and size
        rect = gameObject.GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        imageSize = bgImage.sprite.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculating new backgrounds position repeating it depending on imageSize
        newPos = Mathf.Repeat(Time.time * -scrollSpeed, imageSize);

        // Setting new position
        rect.anchoredPosition = startPos + Vector2.right * newPos;
    }
}
