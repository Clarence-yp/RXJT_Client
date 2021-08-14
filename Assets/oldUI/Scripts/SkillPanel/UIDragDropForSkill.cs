using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISkillSlot))]
public class UIDragDropForSkill : MonoBehaviour
{

    public enum Restriction
    {
        None,
        Horizontal,
        Vertical,
        PressAndHold,
    }

    /// <summary>
    /// What kind of restriction is applied to the drag & drop logic before dragging is made possible.
    /// </summary>

    public Restriction restriction = Restriction.None;

    /// <summary>
    /// Whether a copy of the item will be dragged instead of the item itself.
    /// </summary>

    public bool cloneOnDrag = false;

    #region Common functionality

    protected Transform mTrans;
    protected Transform mParent;
    protected Collider mCollider;
    protected UIRoot mRoot;
    protected UIGrid mGrid;
    protected UITable mTable;
    protected int mTouchID = int.MinValue;
    protected float mPressTime = 0f;
    protected UIDragScrollView mDragScrollView = null;

    /// <summary>
    /// Cache the transform.
    /// </summary>

    protected virtual void Start()
    {
        mTrans = transform;
        mCollider = GetComponent<Collider>();
        mDragScrollView = GetComponent<UIDragScrollView>();
    }

    /// <summary>
    /// Record the time the item was pressed on.
    /// </summary>

    void OnPress(bool isPressed) { if (isPressed) mPressTime = RealTime.time; }

    /// <summary>
    /// Start the dragging operation.
    /// </summary>

    void OnDragStart()
    {
        if (!enabled || mTouchID != int.MinValue) return;

        // If we have a restriction, check to see if its condition has been met first
        if (restriction != Restriction.None)
        {
            if (restriction == Restriction.Horizontal)
            {
                Vector2 delta = UICamera.currentTouch.totalDelta;
                if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;
            }
            else if (restriction == Restriction.Vertical)
            {
                Vector2 delta = UICamera.currentTouch.totalDelta;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) return;
            }
            else if (restriction == Restriction.PressAndHold)
            {
                if (mPressTime + 1f > RealTime.time) return;
            }
        }

        if (cloneOnDrag)
        {
            Vector3 screenPos = new Vector3(UICamera.currentTouch.pos.x, UICamera.currentTouch.pos.y, 0);
            Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(screenPos);
            //将clone放在UISkillSetting或者UISkillStorage下
            GameObject clone = null;
            UISkillSetting skillSetting = NGUITools.FindInParents<UISkillSetting>(gameObject);
            if (skillSetting != null)
            {
                clone = NGUITools.AddChild(skillSetting.gameObject, gameObject);
            }
            else
            {
                UISkillStorage skillStorage = NGUITools.FindInParents<UISkillStorage>(gameObject);
                if (skillStorage != null)
                {
                    clone = NGUITools.AddChild(skillStorage.gameObject, gameObject);
                }
            }
            clone.transform.position = pos;
            clone.transform.localRotation = transform.localRotation;
            clone.transform.localScale = transform.localScale;

            UIButtonColor bc = clone.GetComponent<UIButtonColor>();
            if (bc != null) bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;

            UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);

            UICamera.currentTouch.pressed = clone;
            UICamera.currentTouch.dragged = clone;

            UIDragDropForSkill item = clone.GetComponent<UIDragDropForSkill>();
            UISkillSlot skillSlot = this.GetComponent<UISkillSlot>();
            if (null != skillSlot)
            {
                //如果拖动的Slot内不含有任何技能，则不允许拖动
                if (skillSlot.SkillId == -1 || (!skillSlot.m_IsUnlock && skillSlot.slotType == SlotType.SkillStorage))
                {
                    NGUITools.DestroyImmediate(clone);
                    return;
                }
                else
                {
                    skillSlot.SetIcon("");
                }

            }
            UISkillSlot cloneSlot = clone.GetComponent<UISkillSlot>();
            cloneSlot.SkillId = skillSlot.SkillId;
            cloneSlot.slotType = skillSlot.slotType;
            item.Start();
            item.OnDragDropStart();


        }
        else OnDragDropStart();
    }

    /// <summary>
    /// Perform the dragging.
    /// </summary>

    void OnDrag(Vector2 delta)
    {
        if (!enabled || mTouchID != UICamera.currentTouchID) return;
        OnDragDropMove((Vector3)delta * mRoot.pixelSizeAdjustment);
    }

    /// <summary>
    /// Notification sent when the drag event has ended.
    /// </summary>

    void OnDragEnd()
    {
        if (!enabled || mTouchID != UICamera.currentTouchID) return;
        OnDragDropRelease(UICamera.hoveredObject);
    }

    #endregion

    /// <summary>
    /// Perform any logic related to starting the drag & drop operation.
    /// </summary>

    protected virtual void OnDragDropStart()
    {
        // Automatically disable the scroll view
        if (mDragScrollView != null) mDragScrollView.enabled = false;

        // Disable the collider so that it doesn't intercept events
        if (mCollider != null) mCollider.enabled = false;

        mTouchID = UICamera.currentTouchID;
        mParent = mTrans.parent;
        mRoot = NGUITools.FindInParents<UIRoot>(mParent);
        mGrid = NGUITools.FindInParents<UIGrid>(mParent);
        mTable = NGUITools.FindInParents<UITable>(mParent);

        // Re-parent the item
        if (UIDragDropRoot.root != null)
            mTrans.parent = UIDragDropRoot.root;

        Vector3 pos = mTrans.localPosition;
        pos.z = 0f;
        mTrans.localPosition = pos;

        // Notify the widgets that the parent has changed
        NGUITools.MarkParentAsChanged(gameObject);

        if (mTable != null) mTable.repositionNow = true;
        if (mGrid != null) mGrid.repositionNow = true;
    }

    /// <summary>
    /// Adjust the dragged object's position.
    /// </summary>

    protected virtual void OnDragDropMove(Vector3 delta)
    {
        mTrans.localPosition += delta;
    }

    /// <summary>
    /// Drop the item onto the specified object.
    /// </summary>

    protected virtual void OnDragDropRelease(GameObject surface)
    {
        if (cloneOnDrag)
        {
            UISkillSlot dragedSlot = gameObject.GetComponent<UISkillSlot>();
            //判断dragedslot是属于SkillSeting下的还是SkillStorage下
            //从SkillSeting拖出
            if (dragedSlot.slotType == SlotType.SkillSetting)
            {
                UISkillSetting ddSkillSeting = null;
                UISkillPanel skillPanel = NGUITools.FindInParents<UISkillPanel>(gameObject);
                if (skillPanel != null) ddSkillSeting = skillPanel.uiSkillSetting;
                if (ddSkillSeting == null) return;
                if (surface == null)
                {
                    //通知卸载该技能
                    ddSkillSeting.UnloadSkill(dragedSlot);
                    NGUITools.Destroy(gameObject);
                    return;
                }
                UISkillSlot surfaceSlot = surface.GetComponent<UISkillSlot>();
                if (surfaceSlot != null)
                {
                    //surface含有UISkillSlot组件并属于SkillSeting或者SkillStorage时，进行交换
                    if (surfaceSlot.slotType == SlotType.SkillSetting)
                    {
                        ddSkillSeting.ExchangeSlot(dragedSlot, surfaceSlot);
                    }
                    else
                    {
                        //技能图标拖到非SkillSetting和非SkillStorage上时，卸载
                        ddSkillSeting.UnloadSkill(dragedSlot);
                    }
                }
                else
                {
                    //surface没有怪UISkillSlot时，卸载
                    ddSkillSeting.UnloadSkill(dragedSlot);
                }
                NGUITools.Destroy(gameObject);
            }
            //从SkillStorage拖出
            else if (dragedSlot.slotType == SlotType.SkillStorage)
            {
                UISkillStorage ddSkillStorage = NGUITools.FindInParents<UISkillStorage>(gameObject);
                if (null == ddSkillStorage) return;
                //surface为空的话需要重置dragedslot
                if (surface == null)
                {
                    ddSkillStorage.ResetSlot(dragedSlot);
                    NGUITools.Destroy(gameObject);
                    return;
                }
                UISkillSlot surfaceSlot = surface.GetComponent<UISkillSlot>();
                if (surfaceSlot != null)
                {
                    if (surfaceSlot.slotType == SlotType.SkillSetting)
                    {
                        //从Storage拖到SkillSetting时，交换
                        ddSkillStorage.ExchangeSlot(dragedSlot, surfaceSlot);
                        ddSkillStorage.ResetSlot(dragedSlot);

                    }
                    else
                    {
                        //如果surface不是SkillSetting类型时，重置
                        ddSkillStorage.ResetSlot(dragedSlot);
                    }

                }
                else
                {
                    //surface中不含有UISkillSlot组件时，重置
                    ddSkillStorage.ResetSlot(dragedSlot);
                }
                NGUITools.Destroy(gameObject);
            }
        }
    }


}
