using System;
using System.Collections.Generic;
using System.Text;

namespace Compilator
{
    class Lex
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public Lex(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
