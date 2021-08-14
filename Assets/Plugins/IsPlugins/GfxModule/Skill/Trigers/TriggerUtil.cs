using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GfxModule.Skill.Trigers
{
    public class TriggerUtil
    {
        private static float m_RayCastMaxDistance = 50;
        private static int m_TerrainLayer = 1 << 16;
        private static int m_CurCameraControlId = 0;
        private static bool m_IsMoveCameraTriggerContol = false;

        public static int CAMERA_CONTROL_FAILED = -1;
        public static int CAMERA_NO_ONE_CONTROL = 0;
        public static int CAMERA_CONTROL_START_ID = 1;

        public static void OnFingerDown(DashFire.GestureArgs e)
        {
            if (DashFire.LogicSystem.PlayerSelfInfo != null)
            {
                DashFire.LogicSystem.PlayerSelfInfo.IsTouchDown = true;
            }
        }

        public static void OnFingerUp(DashFire.GestureArgs e)
        {
            if (DashFire.LogicSystem.PlayerSelfInfo != null)
            {
                DashFire.LogicSystem.PlayerSelfInfo.IsTouchDown = false;
            }
        }

        public static Transform GetChildNodeByName(GameObject gameobj, string name)
        {
            if (gameobj == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            Transform[] ts = gameobj.transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in ts)
            {
                if (t.name == name)
                {
                    return t;
                }
            }
            return null;
        }

        public static bool AttachNodeToNode(GameObject source,
                                         string sourcenode,
                                         GameObject target,
                                         string targetnode)
        {
            Transform source_child = GetChildNodeByName(source, sourcenode);
            Transform target_child = GetChildNodeByName(target, targetnode);
            if (source_child == null || target_child == null)
            {
                return false;
            }
            target.transform.parent = source_child;
            target.transform.localRotation = Quaternion.identity;
            target.transform.localPosition = Vector3.zero;
            Vector3 ss = source_child.localScale;
            Vector3 scale = new Vector3(1 / ss.x, 1 / ss.y, 1 / ss.z);
            Vector3 relative_motion = (target_child.position - target.transform.position);
            target.transform.position -= relative_motion;
            //target.transform.localPosition = Vector3.Scale(target.transform.localPosition, scale);
            return true;
        }

        public static void MoveChildToNode(GameObject obj, string childname, string nodename)
        {
            Transform child = GetChildNodeByName(obj, childname);
            if (child == null)
            {
                DashFire.LogSystem.Debug("----not find child! {0} on {1}", childname, obj.name);
                return;
            }
            Transform togglenode = TriggerUtil.GetChildNodeByName(obj, nodename);
            if (togglenode == null)
            {
                DashFire.LogSystem.Debug("----not find node! {0} on {1}", nodename, obj.name);
                return;
            }
            child.parent = togglenode;
            child.localRotation = Quaternion.identity;
            child.localPosition = Vector3.zero;
        }


        public static BeHitState GetBeHitStateFromStr(string str)
        {
            BeHitState result = BeHitState.kDefault;
            if (str.Equals("kDefault"))
            {
                result = BeHitState.kDefault;
            }
            else if (str.Equals("kStand"))
            {
                result = BeHitState.kStand;
            }
            else if (str.Equals("kStiffness"))
            {
                result = BeHitState.kStiffness;
            }
            else if (str.Equals("kKnockDown"))
            {
                result = BeHitState.kKnockDown;
            }
            else if (str.Equals("kLauncher"))
            {
                result = BeHitState.kLauncher;
            }
            return result;
        }

        public static GameObject DrawCircle(Vector3 center, float radius, Color color, float circle_step = 0.05f)
        {
            GameObject obj = new GameObject();
            LineRenderer linerender = obj.AddComponent<LineRenderer>();
            linerender.SetWidth(0.05f, 0.05f);

            Shader shader = Shader.Find("Particles/Additive");
            if (shader != null)
            {
                linerender.material = new Material(shader);
            }
            linerender.SetColors(color, color);

            float step_degree = Mathf.Atan(circle_step / 2) * 2;
            int count = (int)(2 * Mathf.PI / step_degree);

            linerender.SetVertexCount(count + 1);

            for (int i = 0; i < count + 1; i++)
            {
                float angle = 2 * Mathf.PI / count * i;
                Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                linerender.SetPosition(i, pos);
            }
            return obj;
        }

        public static bool IsPlayerSelf(GameObject obj)
        {
            return DashFire.LogicSystem.PlayerSelf == obj;
        }

        public static GameObject GetCameraObj()
        {
            GameObject gfx_root = GameObject.Find("GfxGameRoot");
            return gfx_root;
        }

        public static bool IsControledCamera(int control_id)
        {
            if (m_CurCameraControlId == CAMERA_NO_ONE_CONTROL)
            {
                return false;
            }
            if (control_id <= CAMERA_CONTROL_FAILED)
            {
                return false;
            }
            if (control_id == m_CurCameraControlId)
            {
                return true;
            }
            return false;
        }

        public static int ControlCamera(bool is_control, bool is_move_trigger = false)
        {
            if (m_IsMoveCameraTriggerContol && !is_move_trigger)
            {
                return CAMERA_CONTROL_FAILED;
            }
            GameObject gfx_root = GameObject.Find("GfxGameRoot");
            if (gfx_root != null)
            {
                if (is_control)
                {
                    if (++m_CurCameraControlId < 0)
                    {
                        m_CurCameraControlId = CAMERA_CONTROL_START_ID;
                    }
                    if (is_move_trigger)
                    {
                        m_IsMoveCameraTriggerContol = true;
                    }
                    gfx_root.SendMessage("BeginShake");
                }
                else
                {
                    m_CurCameraControlId = CAMERA_NO_ONE_CONTROL;
                    m_IsMoveCameraTriggerContol = false;
                    gfx_root.SendMessage("EndShake");
                }
                return m_CurCameraControlId;
            }
            return CAMERA_CONTROL_FAILED;
        }

        public static List<GameObject> GetRayHitObjects(string layer_name, Vector3 touch_pos)
        {
            List<GameObject> result = new List<GameObject>();
            if (Camera.main == null)
            {
                return result;
            }
            Ray ray = Camera.main.ScreenPointToRay(touch_pos);
            int layermask = 1 << LayerMask.NameToLayer(layer_name);
            RaycastHit[] rch = Physics.RaycastAll(ray, 200f, layermask);
            foreach (RaycastHit node in rch)
            {
                if (null != node.collider.gameObject)
                {
                    result.Add(node.collider.gameObject);
                }
            }
            return result;
        }

        public static void SetFollowEnable(bool is_enable)
        {
            GameObject gfx_root = GameObject.Find("GfxGameRoot");
            if (gfx_root != null)
            {
                gfx_root.SendMessage("SetFollowEnable", is_enable);
            }
        }

        public static List<GameObject> FindTargetInSector(Vector3 center,
                                                      float radius,
                                                      Vector3 direction,
                                                      Vector3 degreeCenter,
                                                      float degree)
        {
            List<GameObject> result = new List<GameObject>();
            Collider[] colliders = Physics.OverlapSphere(center, radius, 1 << LayerMask.NameToLayer("Character"));
            direction.y = 0;
            foreach (Collider co in colliders)
            {
                GameObject obj = co.gameObject;
                Vector3 targetDir = obj.transform.position - degreeCenter;
                targetDir.y = 0;
                if (Mathf.Abs(Vector3.Angle(targetDir, direction)) <= degree)
                {
                    result.Add(obj);
                }
            }
            return result;
        }

        public static GameObject GetObjectByPriority(GameObject source, List<GameObject> list,
                                                     float distance_priority, float degree_priority,
                                                     float max_distance, float max_degree)
        {
            GameObject target = null;
            float max_score = -1;
            foreach (GameObject obj in list)
            {
                float distance = (obj.transform.position - source.transform.position).magnitude;
                float distance_score = 1 - distance / max_distance;
                Vector3 targetDir = obj.transform.position - source.transform.position;
                float angle = Vector3.Angle(targetDir, source.transform.forward);
                float degree_score = 1 - angle / max_degree;
                float final_score = distance_score * distance_priority + degree_score * degree_priority;
                if (final_score > max_score)
                {
                    target = obj;
                    max_score = final_score;
                }
            }
            return target;
        }

        public static List<GameObject> FiltEnimy(GameObject source, List<GameObject> list)
        {
            List<GameObject> result = new List<GameObject>();
            foreach (GameObject obj in list)
            {
                if (SkillDamageManager.IsEnemy(source, obj) && !IsObjectDead(obj))
                {
                    result.Add(obj);
                }
            }
            return result;
        }

        public static bool IsObjectDead(GameObject obj)
        {
            DashFire.SharedGameObjectInfo si = DashFire.LogicSystem.GetSharedGameObjectInfo(obj);
            if (si.Blood <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static float ConvertToSecond(long delta)
        {
            return delta / 1000000.0f;
        }

        public static StateImpact ParseStateImpact(ScriptableData.CallData stCall)
        {
            StateImpact stateimpact = new StateImpact();
            stateimpact.m_State = GetBeHitStateFromStr(stCall.GetParamId(0));
            for (int i = 1; i < stCall.GetParamNum(); i = i + 2)
            {
                ImpactData im = new ImpactData();
                im.ImpactId = int.Parse(stCall.GetParamId(i));
                if (stCall.GetParamNum() > i + 1)
                {
                    im.RemainTime = int.Parse(stCall.GetParamId(i + 1));
                }
                else
                {
                    im.RemainTime = -1;
                }
                stateimpact.m_Impacts.Add(im);
            }
            return stateimpact;
        }

        public static void SetObjVisible(GameObject obj, bool isShow)
        {
            Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renders)
            {
                r.enabled = isShow;
            }
        }

        public static void UpdateObjTransform(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            DashFire.LogicSystem.NotifyGfxUpdatePosition(obj, obj.transform.position.x, obj.transform.position.y, obj.transform.position.z,
                                                         0, (float)(obj.transform.rotation.eulerAngles.y * Math.PI / 180.0f), 0);
        }

        public static void UpdateObjWantDir(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            DashFire.LogicSystem.NotifyGfxChangedWantDir(obj, (float)(obj.transform.rotation.eulerAngles.y * Math.PI / 180.0f));
        }

        public static void UpdateObjPosition(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            DashFire.LogicSystem.NotifyGfxUpdatePosition(obj, obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
        }

        public static void MoveObjTo(GameObject obj, Vector3 position)
        {
            CharacterController ctrl = obj.GetComponent<CharacterController>();
            if (null != ctrl)
            {
                ctrl.Move(position - obj.transform.position);
            }
            else
            {
                obj.transform.position = position;
            }
        }

        public static float GetObjFaceDir(GameObject obj)
        {
            return obj.transform.rotation.eulerAngles.y * UnityEngine.Mathf.PI / 180.0f;
        }

        public static void MoveChildToNode(int actorid, string childname, string nodename)
        {
            GameObject obj = DashFire.LogicSystem.GetGameObject(actorid);
            MoveChildToNode(obj, childname, nodename);
        }

        public static float GetHeightWithGround(GameObject obj)
        {
            return GetHeightWithGround(obj.transform.position);
        }

        public static float GetHeightWithGround(Vector3 pos)
        {
            if (Terrain.activeTerrain != null)
            {
                return pos.y - Terrain.activeTerrain.SampleHeight(pos);
            }
            else
            {
                RaycastHit hit;
                Vector3 higher_pos = pos;
                higher_pos.y += 2;
                if (Physics.Raycast(higher_pos, -Vector3.up, out hit, m_RayCastMaxDistance, m_TerrainLayer))
                {
                    return pos.y - hit.point.y;
                }
                return m_RayCastMaxDistance;
            }
        }

        public static bool IsCollideGrounded(CharacterController controller)
        {
            if (controller == null)
            {
                return false;
            }
            if ((controller.collisionFlags & CollisionFlags.Below) != 0)
            {
                return true;
            }
            return false;
        }

        public static Vector3 GetGroundPos(Vector3 pos)
        {
            Vector3 sourcePos = pos;
            RaycastHit hit;
            pos.y += 2;
            if (Physics.Raycast(pos, -Vector3.up, out hit, m_RayCastMaxDistance, m_TerrainLayer))
            {
                sourcePos.y = hit.point.y;
            }
            return sourcePos;
        }

        public static bool FloatEqual(float a, float b)
        {
            if (Math.Abs(a - b) <= 0.0001)
            {
                return true;
            }
            return false;
        }

        public static GameObject GetFinalOwner(GameObject source, int skillid, out int final_skillid)
        {
            DashFire.SharedGameObjectInfo result = null;
            DashFire.SharedGameObjectInfo si = DashFire.LogicSystem.GetSharedGameObjectInfo(source);
            final_skillid = skillid;
            int break_protector = 10000;
            while (si != null)
            {
                result = si;
                if (si.SummonOwnerActorId >= 0)
                {
                    final_skillid = si.SummonOwnerSkillId;
                    si = DashFire.LogicSystem.GetSharedGameObjectInfo(si.SummonOwnerActorId);
                }
                else
                {
                    break;
                }
                if (break_protector-- <= 0)
                {
                    break;
                }
            }
            if (result != null)
            {
                return DashFire.LogicSystem.GetGameObject(result.m_ActorId);
            }
            else
            {
                return source;
            }
        }

        public static bool IsTouching(GameObject obj)
        {
            DashFire.SharedGameObjectInfo share_info = DashFire.LogicSystem.GetSharedGameObjectInfo(obj);
            if (share_info == null || !share_info.IsTouchDown)
            {
                return false;
            }
            return true;
        }

        public static Vector3 GetTouchPos(GameObject obj)
        {
            Vector3 pos = Vector3.zero;
            pos.x = DashFire.GfxSystem.GetTouchPointX();
            pos.y = DashFire.GfxSystem.GetTouchPointY();
            pos.z = DashFire.GfxSystem.GetTouchPointZ();
            Ray ray = Camera.main.ScreenPointToRay(pos);
            int layermask = 1 << LayerMask.NameToLayer("Terrains");
            RaycastHit[] rch = Physics.RaycastAll(ray, 200f, layermask);
            if (rch.Length > 0)
            {
                return rch[0].point;
            }
            else
            {
                float height = ray.origin.y - obj.transform.position.y;
                float distance = Math.Abs(height * ray.direction.magnitude / ray.direction.y);
                Vector3 height_pos = ray.GetPoint(distance);
                return height_pos;
            }
        }

    }
}
