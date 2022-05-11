using System;
using System.Collections.Generic;

namespace EntityDock.Extensions.Query
{
    /// <summary>
    /// The text count item model
    /// </summary>
    public class TextCount
    {
        public string Text { get; set; }
        public int Count { get; set; } = 1;
        public HashSet<string> Fields { get; set; } = new HashSet<string>();

        /// <summary>
        /// Parameterless required to json deserialization
        /// </summary>
        public TextCount()
        {
        }

        /// <summary>
        /// Simple initialization
        /// </summary>
        /// <param name="text"></param>
        public TextCount(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text;
        }

        /// <summary>
        /// Basic initialization with field
        /// </summary>
        /// <param name="text"></param>
        /// <param name="initialField"></param>
        public TextCount(string text, string initialField) : this(text)
        {
            Fields.Add(initialField);
        }

        public void Increment()
        {
            Count++;
        }

        public void RegisterField(string field)
        {
            if (!Fields.Contains(field))
            {
                Fields.Add(field);
            }
        }
    }
}
