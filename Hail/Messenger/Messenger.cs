// Type: GalaSoft.MvvmLight.Messaging.Messenger
// Assembly: GalaSoft.MvvmLight.WPF4, Version=0.0.0.0, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// Assembly location: S:\Projects\MetroPM\MoldMaintenance\bin\Release\GalaSoft.MvvmLight.WPF4.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hail.Messenger
{
    /// <summary>
    /// The Messenger is a class allowing objects to exchange messages.
    /// 
    /// </summary>
    public class Messenger : IMessenger
    {
        private static readonly object CreationLock = new object();
        private static Messenger defaultInstance;
        private readonly object _registerLock = new object();
        private Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;
        private Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;

        static Messenger()
        {
        }

        /// <summary>
        /// Gets the Messenger's default instance, allowing
        ///             to register and send messages in a static manner.
        /// 
        /// </summary>
        public static Messenger Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    lock (CreationLock)
                    {
                        if (defaultInstance == null)
                            defaultInstance = new Messenger();
                    }
                }
                return defaultInstance;
            }
        }

        #region IMessenger Members

        /// <summary>
        /// Registers a recipient for a type of message TMessage. The action
        ///             parameter will be executed when a corresponding message is sent.
        /// 
        /// <para>
        /// Registering a recipient does not create a hard reference to it,
        ///             so if this recipient is deleted, no memory leak is caused.
        /// </para>
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        ///             for.</typeparam><param name="recipient">The recipient that will receive the messages.</param><param name="action">The action that will be executed when a message
        ///             of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            Register(recipient, null, false, action);
        }

        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        ///             The action parameter will be executed when a corresponding
        ///             message is sent. See the receiveDerivedMessagesToo parameter
        ///             for details on how messages deriving from TMessage (or, if TMessage is an interface,
        ///             messages implementing TMessage) can be received too.
        /// 
        /// <para>
        /// Registering a recipient does not create a hard reference to it,
        ///             so if this recipient is deleted, no memory leak is caused.
        /// </para>
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        ///             for.</typeparam><param name="recipient">The recipient that will receive the messages.</param><param name="receiveDerivedMessagesToo">If true, message types deriving from
        ///             TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
        ///             and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
        ///             and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        ///             and ExecuteOrderMessage to the recipient that registered.
        /// 
        /// <para>
        /// Also, if TMessage is an interface, message types implementing TMessage will also be
        ///             transmitted to the recipient. For example, if a SendOrderMessage
        ///             and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
        ///             and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        ///             and ExecuteOrderMessage to the recipient that registered.
        /// </para>
        /// </param><param name="action">The action that will be executed when a message
        ///             of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            Register(recipient, null, receiveDerivedMessagesToo, action);
        }

        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        ///             The action parameter will be executed when a corresponding
        ///             message is sent.
        /// 
        /// <para>
        /// Registering a recipient does not create a hard reference to it,
        ///             so if this recipient is deleted, no memory leak is caused.
        /// </para>
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        ///             for.</typeparam><param name="recipient">The recipient that will receive the messages.</param><param name="token">A token for a messaging channel. If a recipient registers
        ///             using a token, and a sender sends a message using the same token, then this
        ///             message will be delivered to the recipient. Other recipients who did not
        ///             use a token when registering (or who used a different token) will not
        ///             get the message. Similarly, messages sent without any token, or with a different
        ///             token, will not be delivered to that recipient.</param><param name="action">The action that will be executed when a message
        ///             of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            Register(recipient, token, false, action);
        }

        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        ///             The action parameter will be executed when a corresponding
        ///             message is sent. See the receiveDerivedMessagesToo parameter
        ///             for details on how messages deriving from TMessage (or, if TMessage is an interface,
        ///             messages implementing TMessage) can be received too.
        /// 
        /// <para>
        /// Registering a recipient does not create a hard reference to it,
        ///             so if this recipient is deleted, no memory leak is caused.
        /// </para>
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        ///             for.</typeparam><param name="recipient">The recipient that will receive the messages.</param><param name="token">A token for a messaging channel. If a recipient registers
        ///             using a token, and a sender sends a message using the same token, then this
        ///             message will be delivered to the recipient. Other recipients who did not
        ///             use a token when registering (or who used a different token) will not
        ///             get the message. Similarly, messages sent without any token, or with a different
        ///             token, will not be delivered to that recipient.</param><param name="receiveDerivedMessagesToo">If true, message types deriving from
        ///             TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
        ///             and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
        ///             and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        ///             and ExecuteOrderMessage to the recipient that registered.
        /// 
        /// <para>
        /// Also, if TMessage is an interface, message types implementing TMessage will also be
        ///             transmitted to the recipient. For example, if a SendOrderMessage
        ///             and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
        ///             and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        ///             and ExecuteOrderMessage to the recipient that registered.
        /// </para>
        /// </param><param name="action">The action that will be executed when a message
        ///             of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo,
                                               Action<TMessage> action)
        {
            lock (_registerLock)
            {
                Type local0 = typeof (TMessage);
                Dictionary<Type, List<WeakActionAndToken>> local1;
                if (receiveDerivedMessagesToo)
                {
                    if (_recipientsOfSubclassesAction == null)
                        _recipientsOfSubclassesAction = new Dictionary<Type, List<WeakActionAndToken>>();
                    local1 = _recipientsOfSubclassesAction;
                }
                else
                {
                    if (_recipientsStrictAction == null)
                        _recipientsStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
                    local1 = _recipientsStrictAction;
                }
                lock (local1)
                {
                    List<WeakActionAndToken> local2;
                    if (!local1.ContainsKey(local0))
                    {
                        local2 = new List<WeakActionAndToken>();
                        local1.Add(local0, local2);
                    }
                    else
                        local2 = local1[local0];
                    var local3 = new WeakAction<TMessage>(recipient, action);
                    var local4 = new WeakActionAndToken
                                     {
                                         Action = local3,
                                         Token = token
                                     };
                    local2.Add(local4);
                }
            }
            Cleanup();
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will
        ///             reach all recipients that registered for this message type
        ///             using one of the Register methods.
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam><param name="message">The message to send to registered recipients.</param>
        public virtual void Send<TMessage>(TMessage message)
        {
            SendToTargetOrType(message, null, null);
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will
        ///             reach only recipients that registered for this message type
        ///             using one of the Register methods, and that are
        ///             of the targetType.
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam><typeparam name="TTarget">The type of recipients that will receive
        ///             the message. The message won't be sent to recipients of another type.</typeparam><param name="message">The message to send to registered recipients.</param>
        public virtual void Send<TMessage, TTarget>(TMessage message)
        {
            SendToTargetOrType(message, typeof (TTarget), null);
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will
        ///             reach only recipients that registered for this message type
        ///             using one of the Register methods, and that are
        ///             of the targetType.
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam><param name="message">The message to send to registered recipients.</param><param name="token">A token for a messaging channel. If a recipient registers
        ///             using a token, and a sender sends a message using the same token, then this
        ///             message will be delivered to the recipient. Other recipients who did not
        ///             use a token when registering (or who used a different token) will not
        ///             get the message. Similarly, messages sent without any token, or with a different
        ///             token, will not be delivered to that recipient.</param>
        public virtual void Send<TMessage>(TMessage message, object token)
        {
            SendToTargetOrType(message, null, token);
        }

        /// <summary>
        /// Unregisters a messager recipient completely. After this method
        ///             is executed, the recipient will not receive any messages anymore.
        /// 
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        public virtual void Unregister(object recipient)
        {
            UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
            UnregisterFromLists(recipient, _recipientsStrictAction);
        }

        /// <summary>
        /// Unregisters a message recipient for a given type of messages only.
        ///             After this method is executed, the recipient will not receive messages
        ///             of type TMessage anymore, but will still receive other message types (if it
        ///             registered for them previously).
        /// 
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param><typeparam name="TMessage">The type of messages that the recipient wants
        ///             to unregister from.</typeparam>
        public virtual void Unregister<TMessage>(object recipient)
        {
            Unregister(recipient, null, (Action<TMessage>) null);
        }

        /// <summary>
        /// Unregisters a message recipient for a given type of messages only and for a given token.
        ///             After this method is executed, the recipient will not receive messages
        ///             of type TMessage anymore with the given token, but will still receive other message types
        ///             or messages with other tokens (if it registered for them previously).
        /// 
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param><param name="token">The token for which the recipient must be unregistered.</param><typeparam name="TMessage">The type of messages that the recipient wants
        ///             to unregister from.</typeparam>
        public virtual void Unregister<TMessage>(object recipient, object token)
        {
            Unregister(recipient, token, (Action<TMessage>) null);
        }

        /// <summary>
        /// Unregisters a message recipient for a given type of messages and for
        ///             a given action. Other message types will still be transmitted to the
        ///             recipient (if it registered for them previously). Other actions that have
        ///             been registered for the message type TMessage and for the given recipient (if
        ///             available) will also remain available.
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        ///             to unregister from.</typeparam><param name="recipient">The recipient that must be unregistered.</param><param name="action">The action that must be unregistered for
        ///             the recipient and for the message type TMessage.</param>
        public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            Unregister(recipient, null, action);
        }

        /// <summary>
        /// Unregisters a message recipient for a given type of messages, for
        ///             a given action and a given token. Other message types will still be transmitted to the
        ///             recipient (if it registered for them previously). Other actions that have
        ///             been registered for the message type TMessage, for the given recipient and other tokens (if
        ///             available) will also remain available.
        /// 
        /// </summary>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        ///             to unregister from.</typeparam><param name="recipient">The recipient that must be unregistered.</param><param name="token">The token for which the recipient must be unregistered.</param><param name="action">The action that must be unregistered for
        ///             the recipient and for the message type TMessage.</param>
        public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
            UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
            Cleanup();
        }

        #endregion

        /// <summary>
        /// Provides a way to override the Messenger.Default instance with
        ///             a custom instance, for example for unit testing purposes.
        /// 
        /// </summary>
        /// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
        public static void OverrideDefault(Messenger newMessenger)
        {
            defaultInstance = newMessenger;
        }

        /// <summary>
        /// Sets the Messenger's default (static) instance to null.
        /// 
        /// </summary>
        public static void Reset()
        {
            defaultInstance = null;
        }

        private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (lists == null)
                return;
            lock (lists)
            {
                var local_0 = new List<Type>();
                foreach (var item_2 in lists)
                {
                    var local_2 = new List<WeakActionAndToken>();
                    foreach (WeakActionAndToken item_0 in item_2.Value)
                    {
                        if (item_0.Action == null || !item_0.Action.IsAlive)
                            local_2.Add(item_0);
                    }
                    foreach (WeakActionAndToken item_1 in local_2)
                        item_2.Value.Remove(item_1);
                    if (item_2.Value.Count == 0)
                        local_0.Add(item_2.Key);
                }
                foreach (Type item_3 in local_0)
                    lists.Remove(item_3);
            }
        }

        private static bool Implements(Type instanceType, Type interfaceType)
        {
            if (interfaceType == null || instanceType == null)
                return false;
#if WINRT
            foreach (Type type in instanceType.GetTypeInfo().ImplementedInterfaces)
#else
            foreach (Type type in instanceType.GetInterfaces())
#endif
            {
                if (type == interfaceType)
                    return true;
            }
            return false;
        }

        private static void SendToList<TMessage>(TMessage message, IEnumerable<WeakActionAndToken> list,
                                                 Type messageTargetType, object token)
        {
            if (list == null)
                return;
            foreach (WeakActionAndToken weakActionAndToken in list.Take(list.Count()).ToList())
            {
                var executeWithObject = weakActionAndToken.Action as IExecuteWithObject;
                if (executeWithObject != null && weakActionAndToken.Action.IsAlive &&
                    weakActionAndToken.Action.Target != null &&
                    (messageTargetType == null || weakActionAndToken.Action.Target.GetType() == messageTargetType ||
                     Implements(weakActionAndToken.Action.Target.GetType(), messageTargetType)) &&
                    (weakActionAndToken.Token == null && token == null ||
                     weakActionAndToken.Token != null && weakActionAndToken.Token.Equals(token)))
                    executeWithObject.ExecuteWithObject(message);
            }
        }

        private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (recipient == null || lists == null || lists.Count == 0)
                return;
            lock (lists)
            {
                foreach (Type item_1 in lists.Keys)
                {
                    foreach (WeakActionAndToken item_0 in lists[item_1])
                    {
                        WeakAction local_2 = item_0.Action;
                        if (local_2 != null && recipient == local_2.Target)
                            local_2.MarkForDeletion();
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(object recipient, object token, Action<TMessage> action,
                                                          Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            Type key = typeof (TMessage);
            if (recipient == null || lists == null || (lists.Count == 0 || !lists.ContainsKey(key)))
                return;
            lock (lists)
            {
                foreach (WeakActionAndToken item_0 in lists[key])
                {
                    var local_2 = item_0.Action as WeakAction<TMessage>;
                    if (local_2 != null && recipient == local_2.Target && (action == null || action == local_2.Action) &&
                        (token == null || token.Equals(item_0.Token)))
                        item_0.Action.MarkForDeletion();
                }
            }
        }

        private void Cleanup()
        {
            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
        }

        private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            Type type = message.GetType();
            if (_recipientsOfSubclassesAction != null)
            {
                foreach (
                    Type index in
                        (_recipientsOfSubclassesAction.Keys).Take((_recipientsOfSubclassesAction).Count()).ToList())
                {
                    List<WeakActionAndToken> list = null;
#if WINRT
                    if (type == index || type.GetTypeInfo().IsSubclassOf(index) || Implements(type, index))
#else
                    if (type == index || type.IsSubclassOf(index) || Implements(type, index))
#endif
                    {
                        lock (_recipientsOfSubclassesAction)
                            list =
                                (_recipientsOfSubclassesAction[index]).Take(
                                    (_recipientsOfSubclassesAction[index]).Count()).ToList();
                    }
                    SendToList(message, list, messageTargetType, token);
                }
            }
            if (_recipientsStrictAction != null && _recipientsStrictAction.ContainsKey(type))
            {
                List<WeakActionAndToken> list;
                lock (_recipientsStrictAction)
                    list = (_recipientsStrictAction[type]).Take((_recipientsStrictAction[type]).Count()).ToList();
                SendToList(message, list, messageTargetType, token);
            }
            Cleanup();
        }

        #region Nested type: WeakActionAndToken

        private struct WeakActionAndToken
        {
            public WeakAction Action;
            public object Token;
        }

        #endregion
    }
}