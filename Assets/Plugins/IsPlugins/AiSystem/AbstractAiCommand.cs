using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
    public abstract class AbstractAiCommand<T> : IAiCommand, IPoolAllocatedObject<T> where T : AbstractAiCommand<T>, new()
    {
        public abstract bool Execute(long deltaTime);
        public void Recycle()
        {
            if (null != m_Pool)
            {
                m_Pool.Recycle(this as T);
            }
        }
        public void InitPool(ObjectPool<T> pool)
        {
            m_DowncastObj = this as T;
            m_Pool = pool;
        }
        public T Downcast()
        {
            return m_DowncastObj;
        }

        private T m_DowncastObj = null;
        private ObjectPool<T> m_Pool = null;
    }
    public abstract class AbstractNpcAiCommand<T> : AbstractAiCommand<T> where T : AbstractAiCommand<T>, new()
    {
        public DashFire.NpcInfo Npc {
            get { return m_Npc; }
        }
        public DashFire.AbstractNpcStateLogic Logic {
            get { return m_Logic; }
        }
        public void SetContext(NpcInfo npc, AbstractNpcStateLogic logic)
        {
            m_Npc = npc;
            m_Logic = logic;
        }

        private NpcInfo m_Npc = null;
        private AbstractNpcStateLogic m_Logic = null;
    }
    public abstract class AbstractUserAiCommand<T> : AbstractAiCommand<T> where T : AbstractAiCommand<T>, new()
    {
        public DashFire.UserInfo User {
            get { return m_User; }
        }
        public DashFire.AbstractUserStateLogic Logic {
            get { return m_Logic; }
        }
        public void SetContext(UserInfo user, AbstractUserStateLogic logic)
        {
            m_User = user;
            m_Logic = logic;
        }

        private UserInfo m_User = null;
        private AbstractUserStateLogic m_Logic = null;
    }
}
