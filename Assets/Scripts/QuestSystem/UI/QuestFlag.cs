﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class QuestFlag : MonoBehaviour
{
    private Image icon;
    private RectTransform iconRectTransform;
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Sprite notAccepted;
    [SerializeField]
    private Sprite accepted;
    [SerializeField]
    private Sprite complete;

    private Talker questHolder;
    private MapIcon mapIcon;

    public void Init(Talker questHolder)
    {
        this.questHolder = questHolder;
        if (MapManager.Instance)
        {
            if (mapIcon) MapManager.Instance.RemoveMapIcon(mapIcon, true);
            mapIcon = MapManager.Instance.CreateMapIcon(notAccepted, Vector2.one * 48, questHolder.Data.currentPosition, false, MapIconType.Quest, false);
            mapIcon.iconImage.raycastTarget = false;
            mapIcon.Hide();
        }
        UpdateUI();
        Update();
        if (QuestManager.Instance) QuestManager.Instance.OnQuestStatusChange += UpdateUI;
    }

    private bool hasObjective;
    public void UpdateUI()
    {
        //Debug.Log(questHolder.TalkerName);
        hasObjective = questHolder.Data.objectivesTalkToThis.FindAll(x => x.AllPrevObjCmplt && !x.IsComplete).Count > 0
            || questHolder.Data.objectivesSubmitToThis.FindAll(x => x.AllPrevObjCmplt && !x.IsComplete).Count > 0;
        if (questHolder.QuestInstances.Count < 1 && !hasObjective)
        {
            if (icon.enabled) icon.enabled = false;
            mapIcon.Hide();
            return;
        }
        //Debug.Log("enter");
        if (hasObjective)//该NPC有未完成的谈话任务
        {
            icon.overrideSprite = accepted;
            mapIcon.iconImage.overrideSprite = accepted;
            return;
        }
        foreach (var quest in questHolder.QuestInstances)
        {
            if (!quest.IsComplete && !quest.InProgress && QuestManager.Instance.IsQuestAcceptable(quest))//只要有一个没接取
            {
                icon.overrideSprite = notAccepted;
                mapIcon.iconImage.overrideSprite = notAccepted;
                return;
            }
            else if (quest.IsComplete && quest.InProgress)//只要有一个完成
            {
                icon.overrideSprite = complete;
                mapIcon.iconImage.overrideSprite = complete;
                return;
            }
        }
        icon.overrideSprite = accepted;
        mapIcon.iconImage.overrideSprite = accepted;
    }

    public void Recycle()
    {
        questHolder = null;
        if (mapIcon) mapIcon.Recycle();
        ObjectPool.Put(gameObject);
    }

    void Awake()
    {
        icon = GetComponent<Image>();
        iconRectTransform = icon.rectTransform;
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (questHolder)
        {
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(questHolder.transform.position + questHolder.questFlagOffset);
            float sqrDistance = Vector3.SqrMagnitude(Camera.main.transform.position - questHolder.transform.position);
            if (viewportPoint.z <= 0 || viewportPoint.x > 1 || viewportPoint.x < 0 || viewportPoint.y > 1 || viewportPoint.y < 0 ||
                !questHolder.isActiveAndEnabled || questHolder.QuestInstances.Count < 1 && !hasObjective || sqrDistance > 900f)
            {
                if (icon.enabled) icon.enabled = false;
                if (!questHolder.isActiveAndEnabled || questHolder.QuestInstances.Count < 1 && !hasObjective) mapIcon.Hide();
            }
            else if (questHolder.isActiveAndEnabled && (questHolder.QuestInstances.Count > 0 || hasObjective))
            {
                if (!icon.enabled) icon.enabled = true;
                Vector2 position = new Vector2(Screen.width * viewportPoint.x, Screen.height * viewportPoint.y);
                iconRectTransform.position = position;
                if (sqrDistance > 625 && sqrDistance <= 900)
                {
                    float percent = (900 - sqrDistance) / 275;
                    canvasGroup.alpha = percent;
                    iconRectTransform.localScale = new Vector3(percent, percent, 1);
                }
                else
                {
                    canvasGroup.alpha = 1;
                    iconRectTransform.localScale = Vector3.one;
                }
            }
            if (questHolder.isActiveAndEnabled && (questHolder.QuestInstances.Count > 0 || hasObjective)) mapIcon.Show(false);
            else mapIcon.Hide();
        }
    }

    private void OnDestroy()
    {
        if (MapManager.Instance) MapManager.Instance.DestroyMapIcon(mapIcon);
        if (QuestManager.Instance) QuestManager.Instance.OnQuestStatusChange -= UpdateUI;
    }
}