using System;
using System.Collections.Generic;

namespace PascalCompiler
{
    public class Compiler
    {
        private readonly LexicalAnalyzer lexicalAnalyzer;
        private readonly Scope scope = new();
        private readonly Dictionary<string, Type> availableTypes = new()
        {
            ["INTEGER"] = new IntegerType(),
            ["REAL"] = new RealType(),
            ["STRING"] = new StringType(),
            ["BOOLEAN"] = new BooleanType()
        };
        private Token currentToken;

        public Compiler(string inputPath, string outputPath)
        {
            lexicalAnalyzer = new(inputPath, outputPath);
            GetNextToken();
        }

        public void Start() => Program();

        private Token GetNextToken() => currentToken = lexicalAnalyzer.GetNextToken();

        private void AddError(int code, int position) => lexicalAnalyzer.AddError(code, position);

        private void AddError(int code) => lexicalAnalyzer.AddError(code);

        private bool IsCurrentTokenIdentifier() => currentToken != null && currentToken.Type == TokenType.Identifier;

        private void AcceptOperation(Operation operation)
        {
            if (currentToken == null || currentToken.Type != TokenType.Operation || (currentToken as OperationToken).Operation != operation)
            {
                AddError(OperationErrorMatcher.GetErrorCode(operation));
                throw new OperationException();
            }

            GetNextToken();
        }

        private void AcceptIdentifier()
        {
            if (!IsCurrentTokenIdentifier())
            {
                AddError(100);
                throw new IdentifierException();
            }

            GetNextToken();
        }

        private void SkipTokensTo(List<Operation> operations)
        {
            while (currentToken.Type != TokenType.Operation)
                GetNextToken();

            Operation currentTokenOperation = (currentToken as OperationToken).Operation;

            while (currentToken != null && !operations.Contains(currentTokenOperation))
            {
                GetNextToken();

                if (currentToken.Type == TokenType.Operation)
                    currentTokenOperation = (currentToken as OperationToken).Operation;
            }
        }

        // Программа
        private void Program()
        {
            try
            {
                AcceptOperation(Operation.Program);
                AcceptIdentifier();
                AcceptOperation(Operation.Semicolon);
            }
            catch (OperationException)
            {
                SkipTokensTo(NextTokens.Program);
            }

            Block();
            AcceptOperation(Operation.Point);
        }

        // Блок
        private void Block()
        {
            VariablesPart();
            OperatorsPart();
        }

        // Раздел переменных
        private void VariablesPart()
        {
            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Var)
            {
                AcceptOperation(Operation.Var);
                SameTypeVariables();

                while (IsCurrentTokenIdentifier())
                    SameTypeVariables();
            }
        }

        // Описание однотипных переменных
        private void SameTypeVariables()
        {
            List<IdentifierToken> variables = new();

            try
            {
                AcceptVariable(variables);

                while (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Comma)
                {
                    AcceptOperation(Operation.Comma);
                    AcceptVariable(variables);
                }

                AcceptOperation(Operation.Colon);
                AcceptType(variables);
                AcceptOperation(Operation.Semicolon);
            }
            catch
            {
                SkipTokensTo(NextTokens.SameTypeVariables);

                if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Semicolon)
                    GetNextToken();
            }
        }

        private void AcceptVariable(List<IdentifierToken> variables)
        {
            if (!IsCurrentTokenIdentifier())
            {
                AddError(100);
                throw new IdentifierException();
            }

            variables.Add(currentToken as IdentifierToken);
            GetNextToken();
        }

        private void AcceptType(List<IdentifierToken> variables)
        {
            if (!IsCurrentTokenIdentifier())
            {
                AddError(100);
                throw new IdentifierException();
            }

            IdentifierToken type = currentToken as IdentifierToken;

            if (!scope.IsTypeAvailable(type))
                AddError(101);

            foreach (IdentifierToken variable in variables)
            {
                if (scope.IsVariableDescribed(variable))
                    AddError(102, variable.StartPosition);

                if (variable.Identifier == type.Identifier)
                    AddError(103, variable.StartPosition);

                if (!scope.IsTypeAvailable(type) || variable.Identifier == type.Identifier)
                    scope.AddVariable(variable);
                else
                    scope.AddVariable(variable, type);
            }

            GetNextToken();
        }

        // Раздел операторов
        private void OperatorsPart()
        {
            try
            {
                AcceptOperation(Operation.Begin);
            }
            catch
            {
                SkipTokensTo(NextTokens.OperatorsPart1);

                if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Point)
                    return;

                AcceptOperation(Operation.Begin);
            }

            Operator();

            while (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Semicolon)
            {
                AcceptOperation(Operation.Semicolon);
                Operator();
            }

            try
            {
                AcceptOperation(Operation.End);
            }
            catch
            {
                SkipTokensTo(NextTokens.OperatorsPart2);
            }
        }

        private void Operator() // Оператор
        {
            if (currentToken.Type == TokenType.Identifier)
            {
                AssignmentOperator();
            }
            else if (currentToken.Type == TokenType.Operation)
            {
                Operation operation = (currentToken as OperationToken).Operation;

                if (operation == Operation.Begin)
                    OperatorsPart();
                else if (operation == Operation.If)
                    IfOperator();
                else if (operation == Operation.While)
                    WhileOperator();
            }
        }

        // Оператор присваивания
        private void AssignmentOperator()
        {
            IdentifierToken variable = currentToken as IdentifierToken;

            if (!scope.IsVariableDescribed(variable))
            {
                scope.AddVariable(variable);
                AddError(104);
            }

            Type type = GetVariableType();
            GetNextToken();
            int expressionStartPosition = 0;

            try
            {
                AcceptOperation(Operation.Assignment);
                expressionStartPosition = currentToken.StartPosition;
                Type expressionType = Expression();

                if (!expressionType.IsDerivedTo(type))
                {
                    if (type.ValueType == ValueType.Integer)
                        AddError(106, expressionStartPosition);
                    else if (type.ValueType == ValueType.Real)
                        AddError(107, expressionStartPosition);
                    else if (type.ValueType == ValueType.String)
                        AddError(108, expressionStartPosition);
                }
            }
            catch(Exception exception)
            {
                if (exception is ExpressionException)
                    AddError(105, expressionStartPosition);

                SkipTokensTo(NextTokens.AssignmentOperator);
            }
        }

        // Условный оператор
        private void IfOperator()
        {
            AcceptOperation(Operation.If);
            int expressionStartPosition = currentToken.StartPosition;

            try
            {
                Type expressiontType = Expression();
                if (expressiontType != availableTypes["BOOLEAN"])
                    AddError(109, expressionStartPosition);
            }
            catch
            {
                SkipTokensTo(new List<Operation> { Operation.Then, Operation.Else, Operation.End, Operation.Point });
            }

            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Then)
            {
                try
                {
                    AcceptOperation(Operation.Then);
                    Operator();
                }
                catch
                {
                    SkipTokensTo(new List<Operation> { Operation.Semicolon, Operation.End });
                }
            }

            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Else)
            {
                try
                {
                    AcceptOperation(Operation.Else);
                    Operator();
                }
                catch
                {
                    SkipTokensTo(new List<Operation> { Operation.Semicolon, Operation.End });
                }
            }
        }

        // Цикл с предусловием
        private void WhileOperator()
        {
            AcceptOperation(Operation.While);
            int expressionStartPosition = 0;

            try
            {
                expressionStartPosition = currentToken.StartPosition;
                Type expressionType = Expression();

                if (expressionType != availableTypes["BOOLEAN"])
                    AddError(109, expressionStartPosition);

                AcceptOperation(Operation.Do);
            }
            catch (Exception exception)
            {
                if (exception is ExpressionException)
                    AddError(105, expressionStartPosition);

                SkipTokensTo(NextTokens.WhileOperator);
            }

            Operator();
        }

        // Выражение
        private Type Expression()
        {
            Type leftPartType = SimpleExpression();

            if (IsLogicalOperation())
            {
                GetNextToken();
                Type rightPartType = SimpleExpression();

                if (!rightPartType.IsDerivedTo(leftPartType))
                    throw new ExpressionException();
                else
                    return availableTypes["BOOLEAN"];
            }

            return leftPartType;
        }

        // Простое выражение
        private Type SimpleExpression()
        {
            Type leftPartType = Term();

            while (IsAdditiveOperation())
            {
                GetNextToken();
                Type rightPartType = Term();

                if (!rightPartType.IsDerivedTo(leftPartType))
                    throw new ExpressionException();
            }

            return leftPartType;
        }

        // Слагаемое
        private Type Term()
        {
            Type leftPartType = Factor();

            while (IsMultiplicativeOperation())
            {
                GetNextToken();
                Type rightPartType = Factor();

                if (!rightPartType.IsDerivedTo(leftPartType))
                    throw new ExpressionException();
            }

            return leftPartType;
        }

        // Множитель
        private Type Factor()
        {
            Type factorType;

            if (currentToken.Type == TokenType.Operation)
            {
                try
                {
                    AcceptOperation(Operation.LeftParenthesis);
                    factorType = Expression();
                    AcceptOperation(Operation.RightParenthesis);
                }
                catch
                {
                    throw new ExpressionException();
                }
            }
            else if (currentToken.Type == TokenType.Identifier)
            {
                factorType = GetVariableType();
                GetNextToken();
            }
            else
            {
                factorType = GetConstantType();
                GetNextToken();
            }

            return factorType;
        }

        // Аддитивная операция
        private bool IsAdditiveOperation() => IsOperation(Operations.AdditiveOperatons);

        // Мультипликативная операция
        private bool IsMultiplicativeOperation() => IsOperation(Operations.MultiplicativeOperations);

        // Операция отношения
        private bool IsLogicalOperation() => IsOperation(Operations.LogicalOperations);


        private bool IsOperation(List<Operation> operations)
        {
            if (currentToken == null || currentToken.Type != TokenType.Operation)
                return false;

            Operation currentTokenOperation = (currentToken as OperationToken).Operation;

            foreach (Operation operation in operations)
                if (currentTokenOperation == operation)
                    return true;

            return false;
        }

        private Type GetVariableType()
        {
            IdentifierToken variable = currentToken as IdentifierToken;

            if (!scope.IsVariableDescribed(variable))
            {
                scope.AddVariable(variable);
                AddError(104);
            }

            return scope.GetVariableType(variable);
        }

        private Type GetConstantType()
        {
            ConstantToken constant = currentToken as ConstantToken;

            if (constant.Variant.Type == VariantType.Integer)
                return availableTypes["INTEGER"];
            else if (constant.Variant.Type == VariantType.Real)
                return availableTypes["REAL"];
            else
                return availableTypes["STRING"];
        }
    }
}