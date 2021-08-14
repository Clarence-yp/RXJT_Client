//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright 漏 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// UIDragDropItem is a base script for your own Drag & Drop operations.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItemForDFM : MonoBehaviour
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
  protected UIGridForDFM mGrid;
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
    if (restriction != Restriction.None) {
      if (restriction == Restriction.Horizontal) {
        Vector2 delta = UICamera.currentTouch.totalDelta;
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;
      } else if (restriction == Restriction.Vertical) {
        Vector2 delta = UICamera.currentTouch.totalDelta;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) return;
      } else if (restriction == Restriction.PressAndHold) {
        if (mPressTime + 1f > RealTime.time) return;
      }
    }

    if (cloneOnDrag) {
      GameObject clone = NGUITools.AddChild(transform.parent.gameObject, gameObject);
      clone.transform.localPosition = transform.localPosition;
      clone.transform.localRotation = transform.localRotation;
      clone.transform.localScale = transform.localScale;

      UIButtonColor bc = clone.GetComponent<UIButtonColor>();
      if (bc != null) bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;

      UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);

      UICamera.currentTouch.pressed = clone;
      UICamera.currentTouch.dragged = clone;

      UIDragDropItemForDFM item = clone.GetComponent<UIDragDropItemForDFM>();
      item.Start();
      item.OnDragDropStart();
    } else OnDragDropStart();
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
  private Vector3 mypos;
  protected virtual void OnDragDropStart()
  {
    if (UnityEngine.Time.realtimeSinceStartup - UIManager.dragtime < 0.5f) { return; }
    // Automatically disable the scroll view
    if (mDragScrollView != null) mDragScrollView.enabled = false;

    // Disable the collider so that it doesn't intercept events
    if (mCollider != null) mCollider.enabled = false;
    mypos = mTrans.transform.localPosition;
    mTouchID = UICamera.currentTouchID;
    mParent = mTrans.parent;
    mRoot = NGUITools.FindInParents<UIRoot>(mParent);
    //mGrid = NGUITools.FindInParents<UIGridForDFM>(mParent);
    //mTable = NGUITools.FindInParents<UITable>(mParent);

    // Re-parent the item
    if (UIDragDropRoot.root != null)
      mTrans.parent = UIDragDropRoot.root;

    Vector3 pos = mTrans.localPosition;
    pos.z = 0f;
    mTrans.localPosition = pos;

    // Notify the widgets that the parent has changed
    NGUITools.MarkParentAsChanged(gameObject);

    //if (mTable != null) mTable.repositionNow = true;
    //if (mGrid != null) mGrid.repositionNow = true;
  }

  /// <summary>
  /// Adjust the dragged object's position.
  /// </summary>

  protected virtual void OnDragDropMove(Vector3 delta)
  {
    mTrans.localPosition += delta;
    //mTrans.transform.position = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
  }

  /// <summary>
  /// Drop the item onto the specified object.
  /// </summary>

  protected virtual void OnDragDropRelease(GameObject surface)
  {
    UIManager.dragtime = UnityEngine.Time.realtimeSinceStartup;
    if (!cloneOnDrag) {
      mTouchID = int.MinValue;
      if (mCollider != null) mCollider.enabled = true;

      // Is there a droppable container?
      UIDragDropContainer container = surface ? NGUITools.FindInParents<UIDragDropContainer>(surface) : null;
      //DF淇?敼//////////////////////////////////////////////////////////////////////////////////////////////////
      //鍥炲綊鍘熶綅
      mTrans.parent = mParent;
      //鍦ㄧ墿鍝佹爮浜ゆ崲浣嶇疆鎴栧洖褰掑師浣屘
      if (surface != null && surface.transform.name.Contains("Item")) {
        mTrans.transform.localPosition = surface.transform.localPosition;
        surface.transform.localPosition = mypos;
        string name = mTrans.transform.name;
        mTrans.transform.name = surface.transform.name;
        surface.transform.name = name;
      } else {
        mTrans.localPosition = mypos;
      }
      if (container != null) {
        if (container.reparentTarget.name == "Equipment") {
          //鑳屽寘瑁呭?绌垮埌浜虹墿瑁呭?鏍幪
          ItemClick ic = mTrans.gameObject.GetComponent<ItemClick>();
          if (ic != null) {
            if (surface != null) {
              int slotid = 0;
              string str = surface.transform.name;
              if (str != null) {
                char[] ch = str.ToCharArray();
                if (ch != null && ch.Length >= 5) {
                  if (System.Int32.TryParse(ch[4].ToString(), out slotid)) {
                    EquipmentInfo ei = GamePokeyManager.GetEquipmentInfo(slotid);
                    if (ei != null) {
                      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_mount_equipment", "lobby", ic.ID, ic.PropertyId, slotid);
                    }
                  }
                }
              }
            }
          }
        }
      }

      if (mDragScrollView != null)
        mDragScrollView.enabled = true;

      // Notify the widgets that the parent has changed
      NGUITools.MarkParentAsChanged(gameObject);
    } else NGUITools.Destroy(gameObject);
  }
}
