using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Helpers
{
    class NullExpandoObject : DynamicObject
    {
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name.ToLower();
            result = _dictionary.ContainsKey(name) ? _dictionary[name] : null;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name.ToLower()] = value;
            return true;
        }
    }
}
