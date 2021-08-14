using System.Collections;
using System.Collections.Generic;

namespace StoryDlg
{
  //单个剧情数据
  public class StoryDlgInfo
  {
    public int ID = 0;
    public string StoryName = "";
    public StoryDlgPanel.StoryDlgType DlgType = StoryDlgPanel.StoryDlgType.Small;
    public float IntervalTime = 1.0f;
    public List<StoryDlgItem> StoryItems = new List<StoryDlgItem>();
  }
}
