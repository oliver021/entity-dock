using System;
using System.Collections.Generic;

namespace EntityDock.Extensions.Query
{
    /// <summary>
    /// Simple container some paremters
    /// </summary>
    public struct FilterRule : IEquatable<FilterRule>
    {
        public string field;
        public string condition;
        public object Value;
       

        public FilterRule(string field, string condition, string value) : this()
        {
            this.field = field;
            this.condition = condition;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is FilterRule rule && Equals(rule);
        }

        public bool Equals(FilterRule other)
        {
            return field == other.field &&
                   condition == other.condition &&
                   EqualityComparer<object>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(field, condition, Value);
        }

        public static bool operator ==(FilterRule left, FilterRule right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FilterRule left, FilterRule right)
        {
            return !(left == right);
        }
    }
}