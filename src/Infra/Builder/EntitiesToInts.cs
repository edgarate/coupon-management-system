﻿using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;
using Core.Model;

namespace Infra.Builder
{
    public class EntitiesToInts : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            if (c.SourceProp.Name != c.TargetProp.Name) return false;
            var s = c.SourceProp.Type;
            var t = c.TargetProp.Type;

            if (!s.IsGenericType || !t.IsGenericType
                || s.GetGenericTypeDefinition() != typeof(ICollection<>)
                || t.GetGenericTypeDefinition() != typeof(IEnumerable<>)) return false;

            return t.GetGenericArguments()[0] == (typeof(int))
                   && (s.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)));
        }

        protected override object SetValue(ConventionInfo v)
        {
            return v.SourceProp.Value == null ? null : (v.SourceProp.Value as IEnumerable<Entity>).Select(o => o.Id);
        }
    }
}