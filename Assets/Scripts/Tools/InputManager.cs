﻿using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("ZetanStudio/管理器/输入管理器")]
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    public InputCustomInfo customInfo;

    public static bool IsTyping => BackpackManager.Instance && BackpackManager.Instance.IsInputFocused ||
        PlantManager.Instance && PlantManager.Instance.IsInputFocused ||
        WarehouseManager.Instance && WarehouseManager.Instance.IsInputFocused;

    void Update()
    {
        if (IsTyping) return;
        //#if UNITY_STANDALONE
        if (Input.GetKeyDown(customInfo.QuestWindowButton))
            QuestManager.Instance.OpenCloseWindow();
        if (Input.GetKeyDown(customInfo.BuildingButton))
            BuildingManager.Instance.OpenCloseWindow();
        if (DialogueManager.Instance.IsUIOpen && !DialogueManager.Instance.IsPausing)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                DialogueManager.Instance.OptionPageUp();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                DialogueManager.Instance.OptionPageDown();
            }
        }
        if (Input.GetKeyDown(customInfo.BackpackButton))
        {
            BackpackManager.Instance.OpenCloseWindow();
        }
        if (Input.GetKeyDown(customInfo.TraceButton))
        {
            PlayerManager.Instance.PlayerController.Trace();
        }
        //#endif
        if (Input.GetKeyDown(customInfo.InteractiveButton) || Input.GetButtonDownMobile("Interactive"))
        {
            if (LootManager.Instance.PickAble)//优先拾取
            {
                if (!LootManager.Instance.IsPicking)
                    LootManager.Instance.OpenWindow();
                else LootManager.Instance.TakeAll();
            }
            else if (GatherManager.Instance.GatherAble)
            {
                GatherManager.Instance.TryGather();
            }
            else if (DialogueManager.Instance.TalkAble)
            {
                if (!DialogueManager.Instance.IsTalking)
                    DialogueManager.Instance.BeginNewDialogue();
                else if (DialogueManager.Instance.CurrentType == DialogueType.Normal && DialogueManager.Instance.OptionsCount < 1
                    && DialogueManager.Instance.NPCHasNotAcptQuests)
                    DialogueManager.Instance.ShowTalkerQuest();
                else if (DialogueManager.Instance.OptionsCount > 0)
                    DialogueManager.Instance.FirstOption.OnClick();
            }
            else if (WarehouseManager.Instance.StoreAble)
                WarehouseManager.Instance.OpenWindow();
            else if (FieldManager.Instance.ManageAble)
                FieldManager.Instance.OpenWindow();
            else if (MakingManager.Instance.MakeAble)
                MakingManager.Instance.OpenWindow();
            else if (BuildingManager.Instance.ManageAble)
                BuildingManager.Instance.ShowInfo();
        }
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDownMobile("Cancel"))
        {
            if (ConfirmManager.Instance.IsUIOpen) ConfirmManager.Instance.Cancel();
            else if (AmountManager.Instance.IsUIOpen) AmountManager.Instance.Cancel();
            else if (BuildingManager.Instance.IsLocating) BuildingManager.Instance.LocationGoBack();
            else if (BuildingManager.Instance.IsPreviewing) BuildingManager.Instance.FinishPreview();
            else if (WindowsManager.Instance.WindowsCount > 0) WindowsManager.Instance.CloseTop();
            else EscapeMenuManager.Instance.OpenWindow();
        }
        if (Input.GetButtonDown("Submit") || Input.GetButtonDownMobile("Submit"))
        {
            if (AmountManager.Instance.IsUIOpen && !ConfirmManager.Instance.IsUIOpen) AmountManager.Instance.Confirm();
            else if (!AmountManager.Instance.IsUIOpen && ConfirmManager.Instance.IsUIOpen) ConfirmManager.Instance.Confirm();
        }
    }
}
