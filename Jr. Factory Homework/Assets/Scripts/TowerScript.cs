using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TowerScript : MonoBehaviour
{
    private int modelAmount;
    [Range(5f, 10f)] [SerializeField] private int maxModelPerRow;
    [Range(0f, 2f)] [SerializeField] private float xGap;
    [Range(0f, 2f)] [SerializeField] private float yGap;
    [Range(05f, 10f)] [SerializeField] private float yOffset;

    [SerializeField] private List<int> towerCountList = new List<int>();
    [SerializeField] private List<GameObject> towerList = new List<GameObject>();
    public static TowerScript TowerInstance;

    void Start()
    {
        TowerInstance = this;
    }

    public void CreateTower(int modelNo)
    {
        modelAmount = modelNo;
        FillTowerList();
        StartCoroutine(BuildTowerCoroutine());
    }

    void FillTowerList()
    {
        for (int i = 1; i <= maxModelPerRow; i++)
        {
            if (modelAmount < i)
            {
                break;
            }
            modelAmount -= i;
            towerCountList.Add(i);
        }

        for (int i = maxModelPerRow; i > 0; i--)
        {
            if (modelAmount >= i)
            {
                modelAmount -= i;
                towerCountList.Add(i);
                i++;
            }
        }
    }

    IEnumerator BuildTowerCoroutine()
    {
        var towerId = 0;
        transform.DOMoveX(0f, 0.5f).SetEase(Ease.Flash);

        yield return new WaitForSecondsRealtime(0.55f);

        foreach (int towerHumanCount in towerCountList)
        {
            foreach (GameObject child in towerList)
            {
                child.transform.DOLocalMove(child.transform.localPosition + new Vector3(0, yGap, 0), 0.2f).SetEase(Ease.OutQuad);
            }

            var tower = new GameObject("Tower" + towerId);

            tower.transform.parent = transform;
            tower.transform.localPosition = new Vector3(0, 0, 0);

            towerList.Add(tower);

            var towerNewPos = Vector3.zero;
            float tempTowerHumanCount = 0;

            for (int i = 1; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.transform.parent = tower.transform;
                child.transform.localPosition = new Vector3(tempTowerHumanCount * xGap, 0, 0);
                towerNewPos += child.transform.position;
                tempTowerHumanCount++;
                i--;

                if (tempTowerHumanCount >= towerHumanCount)
                {
                    break;
                }
            }

            tower.transform.position = new Vector3(-towerNewPos.x / towerHumanCount, tower.transform.position.y - yOffset,
                tower.transform.position.z);

            towerId++;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }
}
