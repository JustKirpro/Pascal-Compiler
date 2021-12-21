using System.Collections.Generic;

namespace PascalCompiler
{
    public class Scope
    {
        private readonly Dictionary<IdentifierToken, Type> variablesTable = new Dictionary<IdentifierToken, Type>();

        public Scope() { }

        public bool IsTypeAvailable(IdentifierToken type)
        {
            string typeName = type.Identifier.ToUpper();

            return typeName == "INTEGER" || typeName == "REAL" || typeName == "STRING" && !IsVariableDescribed(type);
        }

        public bool IsVariableDescribed(IdentifierToken newVariable)
        {
            foreach (IdentifierToken variable in variablesTable.Keys)
                if (newVariable.Identifier == variable.Identifier)
                    return true;

            return false;
        }

        public int GetVariableIndex(IdentifierToken variable) //
        {
            int index = -1;

            foreach (IdentifierToken identifierToken in variablesTable.Keys)
            {
                index++;

                if (variable.Identifier == identifierToken.Identifier)
                    break;
            }

            return index;
        }

        public void AddVariable(IdentifierToken variable, IdentifierToken type)
        {
            if (IsVariableDescribed(variable))
            {
                IdentifierToken describedVariable = GetVariable(variable);

                if (Types.GetType(type.Identifier) != variablesTable[describedVariable])
                {
                    variablesTable.Remove(describedVariable);
                    variablesTable.Add(variable, Types.GetType("UNKNOWN"));
                }
            }
            else
            {
                variablesTable.Add(variable, Types.GetType(type.Identifier));
            }
        }

        public void AddVariable(IdentifierToken variable)
        {
            if (IsVariableDescribed(variable))
            {
                IdentifierToken describedVariable = GetVariable(variable);
                variablesTable.Remove(describedVariable);
            }

            variablesTable.Add(variable, Types.GetType("UNKNOWN"));
        }

        public Type GetVariableType(IdentifierToken variable) => variablesTable[GetVariable(variable)];

        private IdentifierToken GetVariable(IdentifierToken newVariable)
        {
            foreach (IdentifierToken variable in variablesTable.Keys)
                if (newVariable.Identifier == variable.Identifier)
                    return variable;

            return null;
        }
    }
}