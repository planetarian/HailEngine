// Type: GalaSoft.MvvmLight.Helpers.WeakAction
// Assembly: GalaSoft.MvvmLight.WPF4, Version=0.0.0.0, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// Assembly location: S:\Projects\MetroPM\MoldMaintenance\bin\Release\GalaSoft.MvvmLight.WPF4.dll

using System;

namespace Hail.Messenger
{
    /// <summary>
    /// Stores an <see cref="P:Valhalla.Messenger.WeakAction.Action"/> without causing a hard reference
    ///             to be created to the Action's owner. The owner can be garbage collected at any time.
    /// 
    /// </summary>
    public class WeakAction
    {
        private readonly Action action;
        private WeakReference reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Valhalla.Messenger.WeakAction"/> class.
        /// 
        /// </summary>
        /// <param name="target">The action's owner.</param><param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action action)
        {
            reference = new WeakReference(target);
            this.action = action;
        }

        /// <summary>
        /// Gets the Action associated to this instance.
        /// 
        /// </summary>
        public Action Action
        {
            get { return action; }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        ///             by the Garbage Collector already.
        /// 
        /// </summary>
        public bool IsAlive
        {
            get { return reference != null && reference.IsAlive; }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a <see cref="T:System.WeakReference"/>.
        /// 
        /// </summary>
        public object Target
        {
            get { return reference == null ? null : reference.Target; }
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        ///             is still alive.
        /// 
        /// </summary>
        public void Execute()
        {
            if (action == null || !IsAlive)
                return;
            action();
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// 
        /// </summary>
        public void MarkForDeletion()
        {
            reference = null;
        }
    }
}