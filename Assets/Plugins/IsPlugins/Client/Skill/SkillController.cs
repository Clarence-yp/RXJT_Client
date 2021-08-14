using System.Collections;
using System.Collections.Generic;
using ScriptRuntime;

namespace DashFire
{
    public class SkillController : ISkillController
    {
        public static List<int> m_UnlockedSkills = new List<int>();
        protected Dictionary<SkillCategory, SkillNode> m_SkillCategoryDict = new Dictionary<SkillCategory, SkillNode>();
        protected bool m_IsAttacking = false;
        protected List<SkillNode> m_WaiteSkillBuffer = new List<SkillNode>();
        protected SkillNode m_LastSkillNode = null;
        protected SkillNode m_CurSkillNode = null;
        protected SkillQECanInputHandler m_SkillQECanInputHandler = null;
        protected SkillStartHandler m_SkillStartHandler = null;
        protected bool m_IsHaveBreakSkillTask = false;

        public virtual void Init()
        {
        }

        public SkillNode InitCategorySkillNode(List<SkillLogicData> skills, SkillLogicData ss)
        {
            SkillNode first = new SkillNode();
            first.SkillId = ss.SkillId;
            first.Category = ss.Category;
            SkillLogicData nextSkillScript = GetSkillById(skills, ss.NextSkillId);
            if (nextSkillScript != null)
            {
                first.NextSkillNode = InitCategorySkillNode(skills, nextSkillScript);
            }
            SkillLogicData qSkillScript = GetSkillById(skills, ss.QSkillId);
            if (qSkillScript != null)
            {
                first.SkillQ = InitCategorySkillNode(skills, qSkillScript);
            }
            SkillLogicData eSkillScript = GetSkillById(skills, ss.ESkillId);
            if (eSkillScript != null)
            {
                first.SkillE = InitCategorySkillNode(skills, eSkillScript);
            }
            return first;
        }

        public virtual void OnTick()
        {
            DealBreakSkillTask();
            UpdateAttacking();
            UpdateSkillNodeCD();
            if (m_WaiteSkillBuffer.Count <= 0)
            {
                return;
            }
            SkillNode node = m_WaiteSkillBuffer[m_WaiteSkillBuffer.Count - 1];
            if (node == null)
            {
                m_WaiteSkillBuffer.Remove(node);
                return;
            }
            SkillNode nextNode = null;
            if ((node.Category == SkillCategory.kSkillQ || node.Category == SkillCategory.kSkillE)
                && m_WaiteSkillBuffer.Count >= 2)
            {
                SkillNode lastone = m_WaiteSkillBuffer[m_WaiteSkillBuffer.Count - 2];
                if (node.Category == SkillCategory.kSkillQ && lastone.SkillQ != null
                    && lastone.SkillQ.SkillId == node.SkillId)
                {
                    nextNode = node;
                    node = lastone;
                }
                else if (node.Category == SkillCategory.kSkillE && lastone.SkillE != null
                         && lastone.SkillE.SkillId == node.SkillId)
                {
                    nextNode = node;
                    node = lastone;
                }
            }
            if (m_CurSkillNode == null || IsSkillCanBreak(m_CurSkillNode, node))
            {
                SkillCannotCastType cannot_type = SkillCannotCastType.kUnknow;
                if (!IsSkillCanStart(node, out cannot_type))
                {
                    LogSystem.Debug("skill can't start");
                    if (IsPlayerSelf())
                    {
                        GfxSystem.PublishGfxEvent("ge_skill_cannot_cast", "ui", cannot_type);
                    }
                    m_WaiteSkillBuffer.Clear();
                    return;
                }
                if (m_CurSkillNode != null)
                {
                    StopSkill(m_CurSkillNode, node);
                }
                if (StartSkill(node))
                {
                    OnSkillStart(node);
                    if (nextNode != null)
                    {
                        m_WaiteSkillBuffer.Add(nextNode);
                    }
                    PostSkillStart(node);
                }
            }
        }

        public virtual bool IsPlayerSelf()
        {
            return false;
        }

        public void LearnSkill(int skillid)
        {
            SkillNode node = GetSkillNodeById(skillid);
            if (node != null)
            {
                node.IsLocked = false;
                m_UnlockedSkills.Add(skillid);
            }
        }

        public virtual SkillNode ChangeCategorySkill(SkillCategory category, SkillNode new_head)
        {
            SkillNode old_node = null;
            m_SkillCategoryDict.TryGetValue(category, out old_node);
            m_SkillCategoryDict[category] = new_head;
            return old_node;
        }

        public virtual void RegisterSkillQECanInputHandler(SkillQECanInputHandler handler)
        {
            m_SkillQECanInputHandler = handler;
        }

        public virtual void RegisterSkillStartHandler(SkillStartHandler handler)
        {
            m_SkillStartHandler = handler;
        }

        public virtual void PostSkillStart(SkillNode node)
        {
        }

        public virtual void AddBreakSkillTask()
        {
            m_IsHaveBreakSkillTask = true;
        }

        public virtual SkillInputData GetSkillInputData(SkillCategory category)
        {
            return null;
        }

        public void CancelBreakSkillTask()
        {
            m_IsHaveBreakSkillTask = false;
        }

        public virtual void BeginSkillCD(SkillNode node)
        {
        }

        public virtual void SetFaceDir(Vector3 targetpos)
        {
        }

        public virtual void StartAttack()
        {
            CancelBreakSkillTask();
            SkillNode node = AddAttackNode();
            if (node != null)
            {
                node.TargetPos = Vector3.Zero;
            }
            m_IsAttacking = true;
        }

        public virtual void StartAttack(Vector3 targetpos)
        {
            CancelBreakSkillTask();
            SetFaceDir(targetpos);
            SkillNode node = AddAttackNode();
            if (node != null)
            {
                node.TargetPos = targetpos;
            }
            m_IsAttacking = true;
        }

        protected SkillNode AddAttackNode()
        {
            SkillNode node = null;
            if (CanInput(SkillCategory.kAttack))
            {
                node = AddCategorySkillNode(SkillCategory.kAttack);
            }
            return node;
        }

        public virtual void StopAttack()
        {
            m_IsAttacking = false;
        }

        // child should override this function
        public virtual void ShowSkillTip(SkillCategory category, Vector3 targetpos)
        {
        }

        // child should override this function
        public virtual void HideSkillTip(SkillCategory category)
        {
        }

        public virtual void PushSkill(SkillCategory category, Vector3 targetpos)
        {
            //LogSystem.Debug("in push skill {0}", category);
            CancelBreakSkillTask();
            if (CanInput(category))
            {
                if (IsCategorySkillInCD(category))
                {
                    HideSkillTip(category);
                    LogSystem.Warn("skill is in cd");
                    return;
                }
                SkillNode target_node = AddCategorySkillNode(category);
                //LogSystem.Debug("add skill success!");
                if (target_node != null)
                {
                    target_node.TargetPos = targetpos;
                    if (SkillCategory.kSkillQ != category && SkillCategory.kSkillE != category)
                    {
                        List<SkillNode> qeSkills = new List<SkillNode>();
                        if (target_node.SkillQ != null)
                        {
                            qeSkills.Add(target_node.SkillQ);
                        }
                        if (target_node.SkillE != null)
                        {
                            qeSkills.Add(target_node.SkillE);
                        }
                        if (m_SkillQECanInputHandler != null)
                        {
                            m_SkillQECanInputHandler(GetWaitInputTime(m_CurSkillNode), qeSkills);
                        }
                    }
                }
                else
                {
                    HideSkillTip(category);
                    LogSystem.Debug("----not find skill " + category);
                }
            }
            else
            {
                LogSystem.Debug("skill can't input");
            }
        }

        public SkillNode GetHead(SkillCategory category)
        {
            SkillNode target = null;
            m_SkillCategoryDict.TryGetValue(category, out target);
            return target;
        }

        public virtual List<SkillNode> QuerySkillQE(SkillCategory category, int times)
        {
            List<SkillNode> result = new List<SkillNode>();
            SkillNode node = QuerySkillNode(category, times);
            if (node != null)
            {
                if (node.SkillQ != null)
                {
                    result.Add(node.SkillQ);
                }
                if (node.SkillE != null)
                {
                    result.Add(node.SkillE);
                }
            }
            return result;
        }

        public virtual void AddBreakSection(int skillid, int breaktpye, int starttime, int endtime, bool isinterrupt)
        {
        }

        public SkillNode QuerySkillNode(SkillCategory category, int times)
        {
            if (times <= 0)
            {
                times = 1;
            }
            int index = 1;
            SkillNode head = null;
            if (!m_SkillCategoryDict.TryGetValue(category, out head))
            {
                return null;
            }
            SkillNode result = head;
            while (index < times)
            {
                result = result.NextSkillNode;
                if (result == null)
                {
                    result = head;
                }
                index++;
            }
            return result;
        }

        // child should override this function
        public virtual float GetWaitInputTime(SkillNode node)
        {
            if (node == null)
            {
                return 0;
            }
            return node.StartTime + 5;
        }

        // child should override this function
        public virtual float GetLockInputTime(SkillNode node, SkillCategory category)
        {
            return 0;
        }

        public virtual float GetSkillCD(SkillNode node)
        {
            return 3;
        }

        public virtual bool IsSkillActive(SkillNode node)
        {
            if (node == null)
            {
                return false;
            }
            return true;
        }

        // child should override this function
        public virtual bool IsSkillCanBreak(SkillNode node, SkillNode nextnode = null)
        {
            if (m_CurSkillNode == null)
            {
                return true;
            }
            return false;
        }

        public virtual bool IsSkillCanBreak(SkillNode node, int breaktype, out bool isinterrupt)
        {
            isinterrupt = false;
            if (node == null)
            {
                return true;
            }
            return false;
        }

        // child should override this function
        public virtual bool IsSkillCanStart(SkillNode node, out SkillCannotCastType reason)
        {
            reason = SkillCannotCastType.kUnknow;
            return true;
        }

        public virtual bool IsCategorySkillInCD(SkillCategory category)
        {
            return IsSkillInCD(GetHead(category));
        }

        public virtual bool IsSkillInCD(SkillNode node)
        {
            return false;
        }

        // child should override this function
        public virtual bool StartSkill(SkillNode node)
        {
            return true;
        }

        public virtual bool ForceStartSkill(int skillid)
        {
            return true;
        }

        public virtual bool BreakCurSkill(int breaktype)
        {
            return true;
        }

        public virtual void ForceInterruptCurSkill()
        {
        }

        // child should override this function
        public virtual bool StopSkill(SkillNode node, SkillNode nextnode)
        {
            return true;
        }

        public virtual void ForceStopCurSkill()
        {
        }

        protected SkillNode AddCategorySkillNode(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.kSkillQ:
                case SkillCategory.kSkillE:
                    return AddQESkillNode(category);
                default:
                    return AddNextBasicSkill(category);
            }
        }

        protected SkillNode GetSkillNodeById(int skillid)
        {
            SkillNode result = null;
            foreach (SkillNode head in m_SkillCategoryDict.Values)
            {
                result = FindSkillNodeInChildren(head, skillid);
                if (result != null)
                {
                    return result;
                }
            }
            return result;
        }

        protected SkillNode FindSkillNodeInChildren(SkillNode head, int target_id)
        {
            if (head.SkillId == target_id)
            {
                return head;
            }
            if (head.NextSkillNode != null)
            {
                SkillNode node = FindSkillNodeInChildren(head.NextSkillNode, target_id);
                if (node != null)
                {
                    return node;
                }
            }
            if (head.SkillQ != null)
            {
                SkillNode node = FindSkillNodeInChildren(head.SkillQ, target_id);
                if (node != null)
                {
                    return node;
                }
            }
            if (head.SkillE != null)
            {
                SkillNode node = FindSkillNodeInChildren(head.SkillE, target_id);
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }

        private SkillNode AddNextBasicSkill(SkillCategory category)
        {
            float now = TimeUtility.GetServerMilliseconds() / 1000.0f;
            if (m_CurSkillNode != null && m_CurSkillNode.Category == category)
            {
                if (m_CurSkillNode.NextSkillNode != null && !m_CurSkillNode.NextSkillNode.IsLocked &&
                    now < GetWaitInputTime(m_CurSkillNode))
                {
                    m_WaiteSkillBuffer.Add(m_CurSkillNode.NextSkillNode);
                    return m_CurSkillNode.NextSkillNode;
                }
            }
            SkillNode firstNode = null;
            if (m_SkillCategoryDict.TryGetValue(category, out firstNode))
            {
                if (!firstNode.IsLocked)
                {
                    m_WaiteSkillBuffer.Add(firstNode);
                    return firstNode;
                }
            }
            return null;
        }

        private SkillNode AddQESkillNode(SkillCategory category)
        {
            float now = TimeUtility.GetServerMilliseconds() / 1000.0f;
            if (m_CurSkillNode == null)
            {
                return null;
            }
            SkillNode parent = m_CurSkillNode;
            bool isHaveWaiteNode = false;
            if (m_WaiteSkillBuffer.Count > 0)
            {
                parent = m_WaiteSkillBuffer[m_WaiteSkillBuffer.Count - 1];
                isHaveWaiteNode = true;
            }
            if (parent == null)
            {
                return null;
            }
            if (isHaveWaiteNode || now < GetWaitInputTime(m_CurSkillNode))
            {
                SkillNode target = null;
                if (category == SkillCategory.kSkillQ)
                {
                    target = parent.SkillQ;
                }
                if (category == SkillCategory.kSkillE)
                {
                    target = parent.SkillE;
                }
                if (target != null && !target.IsLocked)
                {
                    m_WaiteSkillBuffer.Add(target);
                    return target;
                }
            }
            return null;
        }

        protected bool CanInput(SkillCategory next_category)
        {
            float now = TimeUtility.GetServerMilliseconds() / 1000.0f;
            if (now < GetLockInputTime(m_CurSkillNode, next_category))
            {
                return false;
            }
            return true;
        }

        private void OnSkillStart(SkillNode node)
        {
            HideSkillTip(SkillCategory.kNone);
            m_LastSkillNode = m_CurSkillNode;
            m_CurSkillNode = node;
            m_CurSkillNode.StartTime = TimeUtility.GetServerMilliseconds() / 1000.0f;
            m_CurSkillNode.IsCDChecked = false;

            m_WaiteSkillBuffer.RemoveAt(m_WaiteSkillBuffer.Count - 1);
            List<SkillNode> new_buffer_element = new List<SkillNode>();
            new_buffer_element.AddRange(m_WaiteSkillBuffer);
            m_WaiteSkillBuffer.Clear();

            if (m_CurSkillNode.NextSkillNode != null)
            {
                if (new_buffer_element.Count >= 1)
                {
                    SkillNode last = new_buffer_element[new_buffer_element.Count - 1];
                    if (m_CurSkillNode != null && last != null &&
                        last.Category == m_CurSkillNode.Category)
                    {
                        PushSkill(last.Category, Vector3.Zero);
                        new_buffer_element.Clear();
                    }
                }
            }

            if (m_LastSkillNode != null && m_LastSkillNode.Category != SkillCategory.kAttack &&
                m_LastSkillNode.Category != m_CurSkillNode.Category)
            {
                if (!m_LastSkillNode.IsCDChecked)
                {
                    BeginSkillCategoryCD(m_LastSkillNode.Category);
                    m_LastSkillNode.IsCDChecked = true;
                }
            }

            if (null != m_SkillStartHandler)
            {
                m_SkillStartHandler();
            }
        }

        private void UpdateSkillNodeCD()
        {
            if (m_CurSkillNode != null && !m_CurSkillNode.IsCDChecked)
            {
                if (m_CurSkillNode.NextSkillNode == null)
                {
                    BeginSkillCategoryCD(m_CurSkillNode.Category);
                    m_CurSkillNode.IsCDChecked = true;
                }
                else if (GetWaitInputTime(m_CurSkillNode) < TimeUtility.GetServerMilliseconds() / 1000.0f)
                {
                    BeginSkillCategoryCD(m_CurSkillNode.Category);
                    m_CurSkillNode.IsCDChecked = true;
                }
            }
        }

        public virtual void BeginCurSkillCD()
        {
        }

        protected virtual void BeginSkillCategoryCD(SkillCategory category)
        {
            SkillNode head = null;
            if (m_SkillCategoryDict.TryGetValue(category, out head))
            {
                GfxSystem.PublishGfxEvent("ge_cast_skill_cd", "ui",
                                           GetCategoryName(head.Category),
                                           GetSkillCD(head));
            }
        }

        protected string GetCategoryName(SkillCategory category)
        {
            switch (category)
            {
                case SkillCategory.kSkillA:
                    return "SkillA";
                case SkillCategory.kSkillB:
                    return "SkillB";
                case SkillCategory.kSkillC:
                    return "SkillC";
                case SkillCategory.kSkillD:
                    return "SkillD";
                default:
                    return "";
            }
        }

        private void UpdateAttacking()
        {
            if (m_IsAttacking)
            {
                SkillNode nextnode = null;
                if (m_CurSkillNode != null)
                {
                    nextnode = m_CurSkillNode.NextSkillNode;
                }
                if (nextnode == null)
                {
                    nextnode = GetHead(SkillCategory.kAttack);
                }
                if (m_WaiteSkillBuffer.Count <= 0 && CanInput(SkillCategory.kAttack) && IsSkillCanBreak(m_CurSkillNode, nextnode))
                {
                    SkillNode nextAttackNode = AddAttackNode();
                    if (nextAttackNode != null)
                    {
                        nextAttackNode.TargetPos = Vector3.Zero;
                    }
                }
            }
        }

        protected virtual void DealBreakSkillTask()
        {
            if (m_IsHaveBreakSkillTask)
            {
                if (m_CurSkillNode == null || IsSkillCanBreak(m_CurSkillNode))
                {
                    if (m_IsAttacking)
                    {
                        return;
                    }
                    if (m_CurSkillNode != null)
                    {
                        StopSkill(m_CurSkillNode, null);
                    }
                    m_IsHaveBreakSkillTask = false;
                    if (m_WaiteSkillBuffer.Count > 0)
                    {
                        HideSkillTip(SkillCategory.kNone);
                    }
                    //m_WaiteSkillBuffer.Clear();
                }
            }
        }

        private SkillLogicData GetSkillById(List<SkillLogicData> skills, int id)
        {
            foreach (SkillLogicData ss in skills)
            {
                if (ss.SkillId == id)
                {
                    return ss;
                }
            }
            return null;
        }

    }
}