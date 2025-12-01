using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountdownTextAnimation : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    private void Start()
    {
        StartCountdown();
    }

    public void StartCountdown()
    {
        DG.Tweening.Sequence seq = DOTween.Sequence();

        seq.Append(AnimateNumber("3"))
           .Append(AnimateNumber("2"))
           .Append(AnimateNumber("1"))
           .OnComplete(() =>
           {
               countdownText.text = "";
               // TODO: Event when countdown finishes
               // e.g., StartGame();
           });
    }

    private Tween AnimateNumber(string number)
    {
        countdownText.text = number;
        countdownText.transform.localScale = Vector3.zero;

        return countdownText.transform
            .DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack)
            .OnStart(() =>
            {
                // Optional small fade-in
                countdownText.alpha = 0;
                countdownText.DOFade(1f, 0.2f);
            })
            .OnComplete(() =>
            {
                // Optional fade-out before next number
                countdownText.DOFade(0f, 0.2f);
            });
    }
}
