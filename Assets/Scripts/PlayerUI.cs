using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

    [Header( "Special Camera" )]
    public RawImage[] PictureSlots;
    public TextMeshProUGUI[] PictureTypeTexts;
    public RectTransform SelectionOutline;

    [Header( "Player" )]
    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    public Image DamageOverlayImage;
}
