using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HandCursorWindow : WindowBase
{
    [SerializeField] private Image _handImage;
    [SerializeField] private RectTransform _handTransform;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _clickedSprite;
    
    private bool _animateClick = false;
    private Vector3 _defaultScale = Vector3.one;
    private Tween _clickTween;
    public override WindowId WindowID => WindowId.HandCursor;

    private void Update()
    {
        if (_animateClick == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlayClickDownAnimation();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            PlayClickUpAnimation();
        }
    }

    private void LateUpdate()
    {
        _handTransform.transform.position = Input.mousePosition;
    }

    protected override void _Close()
    {
        KillTween();
        base._Close();
    }

    public void SetActiveHand(bool value)
    {
        _handTransform.gameObject.SetActive(value);
    }

    public void SetHandClickAnimation(bool value)
    {
        if (value == false)
        {
            KillTween();
        }
        _animateClick = value;
    }

    public void SetHandScale(float value)
    {
        _handTransform.localScale = Vector3.one * value;
        _defaultScale = _handTransform.localScale;
    }

    private void PlayClickUpAnimation()
    {
        _handImage.sprite = _defaultSprite;
        _clickTween = _handTransform.DOScale(_defaultScale, 0.05f).SetEase(Ease.OutQuad);
    }

    private void PlayClickDownAnimation()
    {
        KillTween();
        _handImage.sprite = _clickedSprite;
        _clickTween = _handTransform.DOScale(_defaultScale * 0.9f, 0.05f).SetEase(Ease.OutQuad);
    }

    
    private void PlayClickAnimation()
    {
        KillTween();

        _clickTween = _handTransform
                .DOScale(_defaultScale * 0.9f, 0.05f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _handImage.sprite = _clickedSprite;
                    _handTransform
                        .DOScale(_defaultScale, 0.05f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() => _handImage.sprite = _defaultSprite);
                });
    }

    private void KillTween()
    {
        _clickTween?.Kill();
        _handImage.sprite = _defaultSprite;
    }
}
