using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameObject TapToPlay;
    [SerializeField] private GameObject handIcon;
    [SerializeField] private PlayerScript PlayerSc;
    [SerializeField] private GameObject canvas;

    void Start()
    {
        PlayerSc.gameState = false;
        TapToPlay.transform.DOScale(1.1f, 0.5f).SetLoops(10000, LoopType.Yoyo).SetEase(Ease.InOutFlash);
        handIcon.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-300f,-200f),1f).SetLoops(10000, LoopType.Yoyo).SetEase(Ease.InOutFlash);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerSc.gameState == false)
        {
            PlayerSc.gameState = true;

            canvas.SetActive(false);
        }
    }
}
