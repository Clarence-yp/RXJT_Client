using System;
using System.Collections.Generic;
using System.IO;
using ScriptRuntime;
using DashFireSpatial;

namespace DashFire
{
  public sealed class SightManager
  {
    private sealed class CellVisitor
    {
      public void SetClosure(int camp, CharacterInfo self, ref Vector3 pos)
      {
        sight_camp_ = camp;
        self_ = self;
        pos_ = pos;
      }
      public void Visit(CharacterInfo obj)
      {
        if (self_ == obj)
          return;
        NpcInfo npc = obj.CastNpcInfo();
        if (null != npc && npc.NpcType == (int)NpcTypeEnum.PvpTower)//塔不需要放在可见对象表中（全场景可见）
          return;
        /*if (self_.IsUser && obj.IsUser) {
          LogSystem.Debug("Sight:{0}({1},{2}) see {3}({4},{5})", self_.GetLinkId(), self_.SightCell.row, self_.SightCell.col, obj.GetLinkId(), obj.SightCell.row, obj.SightCell.col);
        }*/
        bool isblue = (sight_camp_ == (int)CampIdEnum.Blue);
        int objId = obj.GetId();
        if (obj.GetCampId() != sight_camp_ && (isblue && !obj.CurBlueCanSeeMe || !isblue && !obj.CurRedCanSeeMe)) {
          Vector3 posd = obj.GetMovementStateInfo().GetPosition3D();
          float distSqr = DashFire.Geometry.DistanceSquare(pos_, posd);
          if (CharacterInfo.CanSee(self_, obj, distSqr, pos_, posd)) {
            if (isblue)
              obj.CurBlueCanSeeMe = true;
            else
              obj.CurRedCanSeeMe = true;
          }
        }
      }
      private int sight_camp_ = 0;
      private CharacterInfo self_ = null;
      private Vector3 pos_ = new Vector3();
    }

    public void Init(string mapFile, NpcManager npcMgr, ISpatialSystem spatialSys)
    {
      npc_manager_ = npcMgr;
      spatial_system_ = spatialSys;
      cell_manager_.Init(DashFire.HomePath.GetAbsolutePath(mapFile));
    }

    public void AddObject(CharacterInfo obj)
    {
      AddObjectImpl(obj);
      cell_manager_.AddObject(obj);
    }

    public void RemoveObject(CharacterInfo obj)
    {
      RemoveObjectImpl(obj);
      cell_manager_.RemoveObject(obj);
    }

    public void UpdateObject(CharacterInfo obj)
    {
      cell_manager_.UpdateObject(obj);
    }

    public void Tick()
    {
      if (camp_users_.ContainsKey((int)CampIdEnum.Blue)) {
        List<UserInfo> list = camp_users_[(int)CampIdEnum.Blue];
        int ct = list.Count;
        for (int i = 0; i < ct; ++i) {
          UserInfo user = list[i];
          user.PrepareUpdateSight();
        }
      }
      if (camp_users_.ContainsKey((int)CampIdEnum.Red)) {
        List<UserInfo> list = camp_users_[(int)CampIdEnum.Red];
        int ct = list.Count;
        for (int i = 0; i < ct; ++i) {
          UserInfo user = list[i];
          user.PrepareUpdateSight();
        }
      }
      for (LinkedListNode<NpcInfo> node = npc_manager_.Npcs.FirstValue; null != node; node = node.Next) {
        NpcInfo npc = node.Value;
        if (null != npc) {
          npc.PrepareUpdateSight();
        }
      }
      if (camp_users_.ContainsKey((int)CampIdEnum.Blue)) {
        List<UserInfo> list = camp_users_[(int)CampIdEnum.Blue];
        int ct = list.Count;
        for (int i = 0; i < ct; ++i) {
          UserInfo user = list[i];
          CalcSight(user);
        }
      }
      if (camp_users_.ContainsKey((int)CampIdEnum.Red)) {
        List<UserInfo> list = camp_users_[(int)CampIdEnum.Red];
        int ct = list.Count;
        for (int i = 0; i < ct; ++i) {
          UserInfo user = list[i];
          CalcSight(user);
        }
      }
      for (LinkedListNode<NpcInfo> node = npc_manager_.Npcs.FirstValue; null != node; node = node.Next) {
        NpcInfo npc = node.Value;
        if (null != npc) {
          //先保留对塔的计算（应该没必要）
          if (npc.NpcType == (int)NpcTypeEnum.PvpTower) {
            npc.CurBlueCanSeeMe = true;
            npc.CurRedCanSeeMe = true;
          }
          CalcSight(npc);
        }
      }
    }

    public void Reset()
    {
      camp_users_.Clear();
      cell_manager_.Reset();
    }
    
    public IList<UserInfo> GetCampUsers(int campid)
    {
      IList<UserInfo> users = null;
      if (camp_users_.ContainsKey(campid)) {
        users = camp_users_[campid];
      } else {
        users = new List<UserInfo>();
      }
      return users;
    }

    public void VisitWatchingObjUsers(CharacterInfo obj,MyAction<UserInfo> visitor)
    {
      int objId = obj.GetId();
      NpcInfo npc = obj.CastNpcInfo();
      if (null != npc && npc.NpcType == (int)NpcTypeEnum.PvpTower) {
        for (LinkedListNode<UserInfo> node = obj.UserManager.Users.FirstValue; null != node; node = node.Next) {
          UserInfo user = node.Value;
          if (null != user) {
            visitor(user);
          }
        }
      } else {
        if (obj.CurBlueCanSeeMe) {
          int campId = (int)CampIdEnum.Blue;
          if (camp_users_.ContainsKey(campId)) {
            List<UserInfo> list = camp_users_[campId];
            int ct = list.Count;
            for (int i = 0; i < ct; ++i) {
              visitor(list[i]);
            }
          }
        }
        if (obj.CurRedCanSeeMe) {
          int campId = (int)CampIdEnum.Red;
          if (camp_users_.ContainsKey(campId)) {
            List<UserInfo> list = camp_users_[campId];
            int ct = list.Count;
            for (int i = 0; i < ct; ++i) {
              visitor(list[i]);
            }
          }
        }
      }
    }

    private void CalcSight(CharacterInfo obj)
    {
      int campId = obj.GetCampId();
      //只有2个玩家阵营需要计算视野
      if (campId == (int)CampIdEnum.Blue)
        obj.CurBlueCanSeeMe = true;
      else if (campId == (int)CampIdEnum.Red)
        obj.CurRedCanSeeMe = true;
      else
        return;
      Vector3 pos = obj.GetMovementStateInfo().GetPosition3D();
      visitor_.SetClosure(campId, obj, ref pos);
      cell_manager_.VisitSightObjects(pos, visitor_.Visit);
    }

    private void AddObjectImpl(CharacterInfo obj)
    {
      UserInfo user = obj.CastUserInfo();
      if (null != user) {
        int camp = user.GetCampId();
        List<UserInfo> users = null;
        if (!camp_users_.ContainsKey(camp)) {
          users = new List<UserInfo>();
          camp_users_.Add(camp, users);
        } else {
          users = camp_users_[camp];
        }
        if (!users.Contains(user)) {
          users.Add(user);
        }
      }
    }
    private void RemoveObjectImpl(CharacterInfo obj)
    {
      UserInfo user = obj.CastUserInfo();
      if (null != user) {
        int camp = user.GetCampId();
        if (camp_users_.ContainsKey(camp)) {
          List<UserInfo> users = camp_users_[camp];
          users.Remove(user);
        }
      }
    }
    
    private MyDictionary<int, List<UserInfo>> camp_users_ = new MyDictionary<int, List<UserInfo>>();
    private CellVisitor visitor_ = new CellVisitor();
    private NpcManager npc_manager_ = null;
    private ISpatialSystem spatial_system_ = null;
    private HexagonalCellManager cell_manager_ = new HexagonalCellManager();
  }
  
  public class HexagonalCellManager
  {
    public void Init(string filename)
    {
      if (!FileReaderProxy.Exists(filename)) {
        return;
      }
      try {
        float w = 1, h = 1;
        using (MemoryStream fs = FileReaderProxy.ReadFileAsMemoryStream(filename)) {
          using (BinaryReader br = new BinaryReader(fs)) {
            w = (float)br.ReadDouble();
            h = (float)br.ReadDouble();
            br.Close();
          }
        }
        Init(w, h);
      } catch (Exception e) {
        LogSystem.Error("{0}\n{1}", e.Message, e.StackTrace);
      }
    }

    public void Init(float width, float height)
    {
      map_width_ = width;
      map_height_ = height;
      GetCell(new Vector3(width, 0, height), out max_row_, out max_col_);
      max_row_++;
      max_col_++;
      if (max_col_ % 2 == 0) {
        max_row_++;
      }
      cells_arr_ = new LinkedListDictionary<int, CharacterInfo>[max_row_, max_col_];
      for (int i = 0; i < max_row_; ++i) {
        for (int j = 0; j < max_col_; ++j) {
          cells_arr_[i, j] = new LinkedListDictionary<int, CharacterInfo>();
        }
      }
    }

    public void Reset()
    {
      cells_arr_ = null;
      map_width_ = 0;
      map_height_ = 0;
      max_row_ = 0;
      max_col_ = 0;
    }

    public void GetCell(Vector3 pos, out int cell_row, out int cell_col)
    {
      int grid_y = (int)(pos.Z / c_GridHeight);
      float y = pos.Z - grid_y * c_GridHeight;

      int grid_x = (int)(pos.X / c_GridWidth);
      float x = pos.X - grid_x * c_GridWidth;

      if (((grid_x + grid_y) & 1) == 0) {
        //   _______
        //  |___/___| a--sections 
        cell_row = grid_y / 2;
        if (x * c_GridWidth + y * c_GridHeight < c_SectionsA) {   // left
          cell_col = grid_x;
          if (grid_x % 2 != 0) {
            cell_row = (grid_y - 1) / 2;
          }
        } else { // right
          cell_col = grid_x + 1;
          if (grid_x % 2 != 0) {
            cell_row = (grid_y + 1) / 2;
          }
        }
      } else {
        //   _______
        //  |___\___| b--sections 
        cell_row = grid_y / 2;
        if (x * c_GridWidth - y * c_GridHeight < c_SectionsB) {   // left
          cell_col = grid_x;
          if (grid_x % 2 == 0) {
            cell_row = (grid_y + 1) / 2;
          }
        } else { // right
          cell_col = grid_x + 1;
          if (grid_x % 2 == 0) {
            cell_row = (grid_y - 1) / 2;
          }
        }
      }
    }
    
    public Vector3 GetCellCenter(int row, int col)
    {
      Vector3 center = new Vector3();
      center.X = (float)(1.5 * col * c_CellWidth);
      center.Z = (float)(2 * c_GridHeight * row);
      if (col % 2 != 0) {
        center.Z += c_GridHeight;
      }
      return center;
    }
    
    public void AddObject(CharacterInfo obj)
    {
      int row, col;
      GetCell(obj.GetMovementStateInfo().GetPosition3D(), out row, out col);
      AddObject(row, col, obj);
    }

    public void RemoveObject(CharacterInfo obj)
    {
      CellPos cell = obj.SightCell;
      int id = obj.GetId();
      if (IsValid(cell.row, cell.col)) {
        cells_arr_[cell.row, cell.col].Remove(id);
      }
    }

    public void UpdateObject(CharacterInfo obj)
    {
      CellPos cell = obj.SightCell;
      int row, col;
      GetCell(obj.GetMovementStateInfo().GetPosition3D(), out row, out col);
      if (row != cell.row || col != cell.col) {
        RemoveObject(obj);
        AddObject(row, col, obj);
      }
    }

    public void VisitSightObjects(Vector3 pos, MyAction<CharacterInfo> visitor)
    {
      int row, col;
      GetCell(pos, out row, out col);
      if (IsValid(row, col)) {
        int minCol = col - 2;
        if (minCol < 0)
          minCol = 0;
        int maxCol = col + 2;
        if (maxCol >= max_col_)
          maxCol = max_col_ - 1;
        if (col % 2 == 0) {
          //最上面的格子(至多3格)
          if (row - 2 >= 0) {
            VisitCell(row - 2, col, visitor);
            if (col - 1 >= 0) VisitCell(row - 2, col - 1, visitor);
            if (col + 1 < max_col_) VisitCell(row - 2, col + 1, visitor);
          }
          //中间三行
          if (row - 1 >= 0) {
            for (int ci = minCol; ci <= maxCol; ++ci) {
              VisitCell(row - 1, ci, visitor);
            }
          }
          for (int ci = minCol; ci <= maxCol; ++ci) {
            VisitCell(row, ci, visitor);
          }
          if (row + 1 < max_row_) {
            for (int ci = minCol; ci <= maxCol; ++ci) {
              VisitCell(row + 1, ci, visitor);
            }
          }
          //最下面的格子（至多1格）
          if (row + 2 < max_row_) {
            VisitCell(row + 2, col, visitor);
          }
        } else {
          //最上面的格子（至多1个）
          if (row - 2 >= 0) {
            VisitCell(row - 2, col, visitor);
          }
          //中间三行
          if (row - 1 >= 0) {
            for (int ci = minCol; ci <= maxCol; ++ci) {
              VisitCell(row - 1, ci, visitor);
            }
          }
          for (int ci = minCol; ci <= maxCol; ++ci) {
            VisitCell(row, ci, visitor);
          }
          if (row + 1 < max_row_) {
            for (int ci = minCol; ci <= maxCol; ++ci) {
              VisitCell(row + 1, ci, visitor);
            }
          }
          //最下面的格子(至多3个)
          if (row + 2 < max_row_) {
            VisitCell(row + 2, col, visitor);
            if (col - 1 >= 0) VisitCell(row + 2, col - 1, visitor);
            if (col + 1 < max_col_) VisitCell(row + 2, col + 1, visitor);
          }
        }
      }
    }
    
    public int GetMaxRow() { return max_row_; }
    public int GetMaxCol() { return max_col_; }
    public float GetMapWidth() { return map_width_; }
    public float GetMapHeight() { return map_height_; }
    public float GetCellWidth() { return c_CellWidth; }
    public float GetGridHeight() { return c_GridHeight; }

    private bool IsValid(int row, int col)
    {
      return row >= 0 && row < max_row_ && col >= 0 && col < max_col_;
    }

    private void AddObject(int row, int col, CharacterInfo obj)
    {
      int id = obj.GetId();
      if (IsValid(row, col) && !cells_arr_[row, col].Contains(id)) {
        cells_arr_[row, col].AddLast(id, obj);
        obj.SightCell = new CellPos(row, col);
      }
    }

    private void VisitCell(int row, int col, MyAction<CharacterInfo> visitor)
    {
      cells_arr_[row, col].VisitValues(visitor);
    }

    // private attribute------------------------------------------------------
    private float map_width_;   // 地图的宽度
    private float map_height_;  // 地图的高度
    private int max_row_;
    private int max_col_;
    private LinkedListDictionary<int,CharacterInfo>[,] cells_arr_;

    private const float c_CellWidth = 10;//这里的值与pvp里的最大视野有关，大约为最大视野的2/5    
    private const float c_GridWidth = c_CellWidth * 1.5f;//对应矩形网格宽
    private const float c_GridHeight = (float)(c_CellWidth * 0.8660254f);//对应矩形网格高
    private const float c_SectionsA = (c_GridWidth * c_GridWidth + c_GridHeight * c_GridHeight) / 2.0f;
    private const float c_SectionsB = (c_GridWidth * c_GridWidth - c_GridHeight * c_GridHeight) / 2.0f;
  }
}