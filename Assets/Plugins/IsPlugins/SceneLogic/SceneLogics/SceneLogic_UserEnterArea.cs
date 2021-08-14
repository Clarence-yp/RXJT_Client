using System;
using System.Collections.Generic;
using ScriptRuntime;

namespace DashFire
{
    public class SceneLogic_UserEnterArea : AbstractSceneLogic
    {
        public override void Execute(SceneLogicInfo info, long deltaTime)
        {
            if (null == info || info.IsLogicFinished) return;
            info.Time += deltaTime;
            if (info.Time > 100)
            {
                UserEnterAreaLogicInfo data = info.LogicDatas.GetData<UserEnterAreaLogicInfo>();
                if (null == data)
                {
                    data = new UserEnterAreaLogicInfo();
                    info.LogicDatas.AddData<UserEnterAreaLogicInfo>(data);

                    SceneLogicConfig sc = info.SceneLogicConfig;
                    if (null != sc)
                    {
                        List<float> pts = Converter.ConvertNumericList<float>(sc.m_Params[0]);
                        data.m_Area = new Vector3[pts.Count / 2];
                        for (int ix = 0; ix < pts.Count - 1; ix += 2)
                        {
                            data.m_Area[ix / 2].X = pts[ix];
                            data.m_Area[ix / 2].Z = pts[ix + 1];
                        }
                        data.m_TriggerType = (UserEnterAreaLogicInfo.TiggerTypeEnum)int.Parse(sc.m_Params[1]);
                    }
                }
                info.Time = 0;
                //执行逻辑
                if (null != data && !data.m_IsTriggered)
                {
                    if (data.m_TriggerType == UserEnterAreaLogicInfo.TiggerTypeEnum.All)
                    {
                        int ct = 0;
                        info.SpatialSystem.VisitObjectInPolygon(data.m_Area, (float distSqr, DashFireSpatial.ISpaceObject obj) =>
                        {
                            if (obj.GetObjType() == DashFireSpatial.SpatialObjType.kUser)
                            {
                                ++ct;
                            }
                        });
                        if (ct == info.UserManager.Users.Count)
                        {
                            SceneLogicSendStoryMessage(info, "alluserenterarea:" + info.ConfigId, ct);
                            data.m_IsTriggered = true;
                        }
                    }
                    else
                    {
                        int id = 0;
                        info.SpatialSystem.VisitObjectInPolygon(data.m_Area, (float distSqr, DashFireSpatial.ISpaceObject obj) =>
                        {
                            if (obj.GetObjType() == DashFireSpatial.SpatialObjType.kUser)
                            {
                                id = (int)obj.GetID();
                                return false;
                            }
                            return true;
                        });
                        if (id > 0)
                        {
                            SceneLogicSendStoryMessage(info, "anyuserenterarea:" + info.ConfigId, id);
                            data.m_IsTriggered = true;
                        }
                    }
                }
            }
        }
    }
}
