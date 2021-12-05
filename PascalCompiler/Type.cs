using System;

namespace PascalCompiler
{
    public enum EType
    {
        Integer,
        Real,
        String,
        Boolean
    };

    public abstract class CType
    {
        public EType Type { get; protected set; }

        public abstract bool IsDerivedTo(CType type);

    }

    public class IntegerType : CType
    {
        public IntegerType()
        {
            Type = EType.Integer;
        }

        public override bool IsDerivedTo(CType type)
        {
            if (type.Type == EType.Integer || type.Type == EType.Real)
                return true;

            return false;    
        }
    }


}
