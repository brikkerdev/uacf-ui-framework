using System;
using System.Collections.Generic;

namespace UACF.UI.Screens
{
    public class NavigationEntry
    {
        public string screenId;
        public UIScreen instance;
        public object data;
        public DateTime timestamp;

        public NavigationEntry(string screenId, UIScreen instance, object data)
        {
            this.screenId = screenId;
            this.instance = instance;
            this.data = data;
            timestamp = DateTime.UtcNow;
        }
    }

    public class NavigationStack
    {
        private readonly Stack<NavigationEntry> _stack = new Stack<NavigationEntry>();

        public void Push(NavigationEntry entry) => _stack.Push(entry);
        public NavigationEntry Pop() => _stack.Count > 0 ? _stack.Pop() : null;
        public NavigationEntry Peek() => _stack.Count > 0 ? _stack.Peek() : null;
        public int Count => _stack.Count;

        public void Clear() => _stack.Clear();

        public List<string> GetHistory()
        {
            var list = new List<string>();
            foreach (var e in _stack)
                list.Add(e.screenId);
            return list;
        }
    }
}
