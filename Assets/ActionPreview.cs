using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPreview : MonoBehaviour
{
    [SerializeField] private Image mainImage;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image leftImage;
    [SerializeField] private Image topEffectImage;
    [SerializeField] private Image downEffectImage;

    public void UpdatePreview(PreviewData data)
    {
        UpdateImage(data.sprite, mainImage);
        UpdateImage(data.additionalSprite,weaponImage);
    }

    private void UpdateImage(Sprite sprite, Image image)
    {
        if (sprite != null)
        {
            image.sprite = sprite;
            image.enabled = true;
            return;
        }

        image.enabled = false;
    }
}