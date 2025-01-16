using UnityEngine;

namespace Unchord
{
    public abstract class AbilityComponent : MonoBehaviour
    {
        // TODO: You should change the type 'object' to class name of stat table after implementing it.
        private object _statTable;

        public void AttachAbility(object statTable)
        {
            Debug.Assert(statTable != null);
            Debug.Assert(_statTable != statTable);

            if (_statTable != null)
                OnDetachAbility(statTable);

            OnAttachAbility(statTable);
            _statTable = statTable;
        }

        public void DetachAbility(object statTable)
        {
            Debug.Assert(statTable != null);
            Debug.Assert(_statTable == statTable);

            OnDetachAbility(statTable);
            _statTable = null;
        }

        protected virtual void OnAttachAbility(object statTable)
        {
            // NOTE: This block is intentionally no operation.
        }

        protected virtual void OnDetachAbility(object statTable)
        {
            // NOTE: This block is intentionally no operation.
        }
    }
}