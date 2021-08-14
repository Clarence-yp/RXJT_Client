using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Lidgren.Network;

namespace DashFire.Network
{
    class MessageDispatch
    {
        public delegate void MsgHandler(object msg, NetConnection user);
        MyDictionary<Type, MsgHandler> m_DicHandler = new MyDictionary<Type, MsgHandler>();
        public void RegisterHandler(Type t, MsgHandler handler)
        {
            m_DicHandler[t] = handler;
        }
        public bool Dispatch(object msg, NetConnection conn)
        {
            MsgHandler msghandler;
            if (m_DicHandler.TryGetValue(msg.GetType(), out msghandler))
            {
                //Type[] param = new Type[] { msg.GetType() };
                //object[] param = new object[] { msg, conn };
                msghandler.Invoke(msg, conn);
                return true;
            }
            return false;
        }
    }
}
