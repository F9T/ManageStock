using System.Reflection;

namespace Application.CommandManager.Commands
{
    public class PropertyCommand : CommandBase
    {
        private string m_PropertyName;
        private object m_Source;
        private object m_OldValue;
        private object m_NewValue;
        private PropertyInfo m_PropertyCache;

        public PropertyCommand(object _Source, string _PropertyName, object _OldValue, object _NewValue)
        {
            m_Source = _Source;
            m_PropertyName = _PropertyName;
            m_OldValue = _OldValue;
            m_NewValue = _NewValue;
            Context = _Source;
        }

        private PropertyInfo GetProperty()
        {
            if (m_PropertyCache == null)
            {
                m_PropertyCache = m_Source.GetType().GetProperty(m_PropertyName);
            }

            return m_PropertyCache;
        }

        public override void Undo() => GetProperty().SetValue(m_Source, m_OldValue, null);

        public override void Redo() => GetProperty().SetValue(m_Source, m_NewValue, null);

        public override bool IsUndoable => true;
    }
}
