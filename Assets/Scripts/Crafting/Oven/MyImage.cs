using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyImage : MonoBehaviour
{
    // Custom world space UI component emulating the Image component
    // [SerializeField] private Vector2 imageSize;
    float originPosY;
    private void Start()
    {
        originPosY = transform.localPosition.y;
    }
    public void SetSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            // Correctly center the image (for sprites with non-center pivots)
            float spritePivotY = sprite.pivot.y;
            float spriteCenterY = sprite.rect.size.y / 2;
            transform.localPosition = new Vector3(transform.localPosition.x, (spritePivotY - spriteCenterY) * 0.0625f + originPosY, transform.localPosition.z);
            // Scale the image to the desired size
        }
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
