using UnityEngine;
using UnityEngine.UI;

public class MoveTiles : MonoBehaviour
{
    public RawImage Image;
    public float Speed = 1;

    void Update()
    {
        var rect = Image.uvRect;
        rect.y += Time.unscaledDeltaTime * Speed;
        Image.uvRect = rect;
    }
}
