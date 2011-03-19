using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager
{
    /// <summary>A builder for an ini file in cortex command format.</summary>
    public class IniFileBuilder
    {
        private const string Line = "\n";
        private const string Tab = "\t";

        private int indentationLevel;
        private StringBuilder builder;

        /// <summary>Creates a new IniFileBuilder for building a new ini file.</summary>
        public IniFileBuilder()
        {
            indentationLevel = 0;
            builder = new StringBuilder();
        }

        /// <summary>Writes a line to the buffer.</summary>
        public IniFileBuilder Write()
        {
            WriteLine();
            return this;
        }

        /// <summary>
        /// Writes a string to a line of the ini file, with a new line.
        /// </summary>
        /// <param name="line">The text line to write to the file.</param>
        public IniFileBuilder Write(string line)
        {
            WriteIndentations();
            builder.Append(line);
            WriteLine();
            return this;
        }

        /// <summary>
        /// Writes a setting to the ini file in the format "key = value". Will write a new line.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public IniFileBuilder Write(string key, string value)
        {
            WriteIndentations();
            builder.AppendFormat("{0} = {1}", key, value);
            WriteLine();
            return this;
        }

        /// <summary>Writes a number of lines to the ini file.</summary>
        public IniFileBuilder WriteLines(int count)
        {
            for (var i = 0; i < count; i++)
                WriteLine();
            return this;
        }

        /// <summary>Increases the indentation level by one.</summary>
        public IniFileBuilder Indent()
        {
            indentationLevel++;
            return this;
        }

        /// <summary>Reduces the indentation level by one.</summary>
        public IniFileBuilder Outdent()
        {
            if(indentationLevel > 0)
                indentationLevel--;
            return this;
        }

        /// <summary>Returns the formatted ini file.</summary>
        public override string ToString()
        {
            var built = builder.ToString();

            if (built.Length > 0)
                built = built.Remove(built.Length - 1); //Remove the last newline.

            return built;
        }

        /// <summary>Writes the current number of indentations to the buffer.</summary>
        private void WriteIndentations()
        {
            for (var i = 0; i < indentationLevel; i++)
                builder.Append(Tab);
        }

        /// <summary>Writes a newline to the buffer.</summary>
        private void WriteLine()
        {
            builder.Append(Line);
        }
    }
}
