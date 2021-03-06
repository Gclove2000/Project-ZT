﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class QuestBoardAgent : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public QuestAgent questAgent;

    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("标题文字")]
#endif
    private Text TitleText;

    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("目标文字")]
#endif
    private Text ObjectiveText;

    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("已完成目标的颜色")]
#endif
    private Color cmpltObjectv = Color.grey;

    [SerializeField]
#if UNITY_EDITOR
    [DisplayName("目标全部完成时的颜色")]
#endif
    private Color cmpltAllObj = Color.yellow;

    public void UpdateStatus()
    {
        if (questAgent.MQuest) TitleText.text = questAgent.MQuest.Title + (questAgent.MQuest.IsComplete ? "(完成)" : string.Empty);
        string objectives = string.Empty;
        if (questAgent.MQuest.IsComplete)
        {
            objectives = "<color=#" + ColorUtility.ToHtmlStringRGB(cmpltAllObj) + ">-已达成所有目标</color>";
            transform.SetAsFirstSibling();//优先显示完成任务
        }
        else
        {
            for (int i = 0; i < questAgent.MQuest.ObjectiveInstances.Count; i++)
            {
                bool isCmplt = questAgent.MQuest.ObjectiveInstances[i].IsComplete;
                string endLine = i == questAgent.MQuest.ObjectiveInstances.Count - 1 ? string.Empty : "\n";
                objectives += (isCmplt ? "<color=#" + ColorUtility.ToHtmlStringRGB(cmpltObjectv) + ">" : string.Empty) + "-" + questAgent.MQuest.ObjectiveInstances[i].DisplayName +
                              (isCmplt ? "(达成)</color>" + endLine :
                              "[" + questAgent.MQuest.ObjectiveInstances[i].CurrentAmount + "/" + questAgent.MQuest.ObjectiveInstances[i].Amount + "]" + endLine);
            }
        }
        ObjectiveText.text = objectives;
    }

    private void OnClick()
    {
        QuestManager.Instance.OpenWindow();
        if (questAgent.parent && !questAgent.parent.IsExpanded) questAgent.parent.IsExpanded = true;
        questAgent.OnClick();
    }

    private void RightClick()
    {
        QuestManager.Instance.TraceQuest(questAgent.MQuest);
    }

    public void Init(QuestAgent qa)
    {
        questAgent = qa;
        UpdateStatus();
    }

    public void Recycle()
    {
        questAgent = null;
        ObjectPool.Put(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && touchTime < 0.5f)
        {
            OnClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)//用于安卓追踪
    {
#if UNITY_ANDROID
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (pressCoroutine != null) StopCoroutine(pressCoroutine);
            pressCoroutine = StartCoroutine(Press());
        }
#endif
    }
    public void OnPointerUp(PointerEventData eventData)//用于安卓追踪
    {
#if UNITY_ANDROID
        if (pressCoroutine != null) StopCoroutine(pressCoroutine);
#endif
    }

    readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    Coroutine pressCoroutine;
    float touchTime = 0;
    IEnumerator Press()
    {
        touchTime = 0;
        bool isPress = true;
        while (isPress)
        {
            touchTime += Time.fixedDeltaTime;
            if (touchTime >= 0.5f)
            {
                RightClick();
                yield break;
            }
            yield return WaitForFixedUpdate;
        }
    }
}
