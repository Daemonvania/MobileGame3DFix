using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AnimateResults : MonoBehaviour
{
    [SerializeField] private RectTransform contents;
    [SerializeField] private Image bg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        bg.DOFade(0, 0).OnComplete(() => 
        {
            bg.DOFade(0.4f, 0.2f).SetEase(Ease.InSine);
        });
        contents.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 1f, 5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
