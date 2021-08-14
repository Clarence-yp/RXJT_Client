using System;
using StoryDlg;
using DashFire;
using UnityEngine;
using System.Collections.Generic;

public class StoryDlgPanel : MonoBehaviour
{
  private GameObject m_StoryDlgGO = null;
  private StoryDlgInfo m_StoryInfo = null;
  private int m_Count = 0;
  private int m_StepNumber = 0;
  private bool m_IsStoryDlgActive = false;
  private int m_StoryId = 0;
  private StoryDlgType m_StoryDlgType = StoryDlgType.Big;
  private float m_IntervalTime = 5.0f;

  public enum StoryDlgType {
    Small,
    Big,
  }

  public void OnTriggerStory(StoryDlgInfo storyInfo)
  {  
	  StoryDlgManager.Instance.FireStoryStartMsg();
    if(m_IsStoryDlgActive == false){           
      m_StoryInfo = storyInfo;
      if (m_StoryInfo != null) {
        //Debug.LogError("===== Trigger A New Story !!! " + arg.m_IntervalTime);
        m_StoryId = m_StoryInfo.ID;
        m_StoryDlgType = m_StoryInfo.DlgType;
        m_IntervalTime = m_StoryInfo.IntervalTime;
        m_IsStoryDlgActive = true;
        m_StoryDlgGO = this.gameObject;
        m_Count = 0;    //剧情对话计数器，触发一个新的剧情时重置为0         
        m_StepNumber = m_StoryInfo.StoryItems.Count;
        StoryDlgItem item = m_StoryInfo.StoryItems[m_Count];
        UpdateStoryDlg(m_StoryDlgGO.transform, item);
        NGUITools.SetActive(m_StoryDlgGO, true);
        if (m_StoryDlgType == StoryDlgType.Big) {
        }
        m_Count++;
        if (item.IntervalTime > 0.0f) {
          Invoke("NextStoryItem", item.IntervalTime);
        }        
      }   
    } 
  }
  public void OnNextBtnClicked()
  {   
    this.NextStoryItem();
  }
  public void OnStopBtnClicked()
  {    
    this.StopStoryDlg();
  }  

  private void UpdateStoryDlg(Transform storyTrans, StoryDlgItem item)
  {
    UILabel lblName = storyTrans.Find("SpeakerName").GetComponent<UILabel>();    
    UILabel lblWords = storyTrans.Find("SpeakerWords").GetComponent<UILabel>();    
    UISprite spriteLeft = storyTrans.Find("SpeakerImageLeft").GetComponent<UISprite>();     
    UISprite spriteRight = storyTrans.Find("SpeakerImageRight").GetComponent<UISprite>();
    if (m_StoryDlgType == StoryDlgType.Big) {
      //小对话已经作废
      lblName.text = string.Format("[c9b2ae]{0}:[-]", item.SpeakerName);
      lblWords.text = item.Words;
      spriteLeft.spriteName = item.ImageLeftSmall;
      spriteRight.spriteName = item.ImageRightSmall;
    } else {
      lblName.text = string.Format("[c9b2ae]{0}:[-]", item.SpeakerName);
	   item.Words = item.Words.Replace("[\\n]","\n");
      lblWords.text = item.Words;
      GameObject goAtlas = ResourceSystem.GetSharedResource(item.ImageLeftAtlas) as GameObject;
      if (goAtlas != null) {
        NGUITools.SetActive(spriteLeft.gameObject, true);
        UIAtlas atlas = goAtlas.GetComponent<UIAtlas>();
        if (atlas != null){ 
          spriteLeft.atlas = atlas;
          spriteLeft.spriteName = item.ImageLeft;
        }
      } else {
        NGUITools.SetActive(spriteLeft.gameObject, false);
        Debug.Log("!!!ImageLeftAtlas is null.");
      }
      goAtlas = ResourceSystem.GetSharedResource(item.ImageRightAtlas) as GameObject;
      if (goAtlas != null) {
        NGUITools.SetActive(spriteRight.gameObject, true);
        UIAtlas atlas = goAtlas.GetComponent<UIAtlas>();
        if (atlas != null) {
          spriteRight.atlas = atlas;
          spriteRight.spriteName = item.ImageRight;
        }
      } else {
        NGUITools.SetActive(spriteRight.gameObject, false);
        Debug.Log("!!!ImageLeftAtlas is null.");
      }
    }   
  }
  //下一句
  private void NextStoryItem()
  {
    //剧情对话框处于活跃状态时，处理单击操作    
    if (m_IsStoryDlgActive == true) {
      CancelInvoke("NextStoryItem");
      if (null != m_StoryDlgGO) {
        bool isActive = NGUITools.GetActive(m_StoryDlgGO);
        if (isActive == true) {
          if (m_Count < m_StepNumber) {
            StoryDlgItem item = m_StoryInfo.StoryItems[m_Count];            
            UpdateStoryDlg(m_StoryDlgGO.transform, item);
            NGUITools.SetActive(m_StoryDlgGO, true);
            m_Count++;
            if (item.IntervalTime > 0.0f) {
              Invoke("NextStoryItem", item.IntervalTime);
            } 
          } else {
            FinishStoryDlg();
          }
        }
      }
    }
  }
  //直接结束剧情对话
  private void StopStoryDlg()
  {
    //剧情对话框处于活跃状态时，处理单击操作
    CancelInvoke("NextStoryItem");
    if (m_IsStoryDlgActive == true) {
      if (null != m_StoryDlgGO) {
        FinishStoryDlg();
      }
    }
  }
  private void FinishStoryDlg()
  {
    m_IsStoryDlgActive = false;
    NGUITools.SetActive(m_StoryDlgGO, false);
    if (m_StoryDlgType == StoryDlgType.Big) {
    }  
    m_StoryDlgGO = null;
    m_StoryInfo = null;
    RaiseStoryDlgOverEvent();
  }
  //剧情对话结束引发事件
  private void RaiseStoryDlgOverEvent()
  {
    DashFire.LogicSystem.SendStoryMessage("dialogover:" + m_StoryId);
    StoryDlgManager.Instance.FireStoryEndMsg(m_StoryId);
  }
}
	

