using UnityEngine;
using UnityEngine.UI;

public class UILineConnector : MonoBehaviour
{
    public RectTransform pointA;
    public RectTransform pointB;
    public Image lineImage;

    void Update()
    {
        Vector3 startPos = pointA.position;
        Vector3 endPos = pointB.position;

        Vector3 direction = endPos - startPos;
        float distance = direction.magnitude;

        lineImage.rectTransform.position = startPos + direction / 2f;
        lineImage.rectTransform.sizeDelta = new Vector2(distance, 3f); // altura = 3
        lineImage.rectTransform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }
}