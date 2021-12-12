﻿using System;
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

        private void GetNextToken() => currentToken = lexicalAnalyzer.GetNextToken();

        private void AddError(int errorCode) => lexicalAnalyzer.AddError(errorCode, currentToken.StartPosition);

        private void AddError(int code, int position) => lexicalAnalyzer.AddError(code, position);

        private void AcceptOperation(Operation operation)
        {
            if (currentToken != null && currentToken is OperationToken && (currentToken as OperationToken).Operation == operation)
            {
                GetNextToken();
                return;
            }

            int errorCode = OperationErrorMatcher.GetErrorCode(operation);
            AddError(errorCode);
            throw new Exception("Ожидался оператор");
        }

        private void AcceptIdentifier()
        {
            if (currentToken != null && currentToken is IdentifierToken)
            {
                GetNextToken();
                return;
            }

            AddError(100);
            throw new Exception("Ожидался идентификатор");
        }

        private void SkipTo(List<Operation> operations)
        {
            while (currentToken.Type != TokenType.Operation)
            {
                GetNextToken();
            }
            Operation currentOperaton = (currentToken as OperationToken).Operation;

            while (currentToken != null && !operations.Contains(currentOperaton))
            {
                GetNextToken();

                if (currentToken.Type == TokenType.Operation)
                    currentOperaton = (currentToken as OperationToken).Operation;
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
            catch
            {
                SkipTo(new List<Operation> { Operation.Var, Operation.Begin });
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

                while (currentToken != null && currentToken.Type == TokenType.Identifier)
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
                SkipTo(new List<Operation> { Operation.Semicolon, Operation.Begin });

                if (currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Semicolon)
                    GetNextToken();
            }
        }

        private void AcceptVariable(List<IdentifierToken> variables)
        {
            if (currentToken == null || currentToken.Type != TokenType.Identifier)
            {
                AddError(100);
                throw new Exception("Ожидался идентификатор");
            }

            variables.Add(currentToken as IdentifierToken);
            GetNextToken();
        }

        private void AcceptType(List<IdentifierToken> variables)
        {
            if (currentToken == null || currentToken.Type != TokenType.Identifier)
            {
                AddError(100);
                throw new Exception("Ожидался идентификатор");
            }

            IdentifierToken type = currentToken as IdentifierToken;

            if (!scope.IsTypeAvailable(type))
            {
                AddError(101);
            }

            foreach (IdentifierToken variable in variables)
            {
                if (scope.IsVariableDescribed(variable))
                {
                    AddError(102, variable.StartPosition);
                }

                if (variable.Identifier == type.Identifier)
                {
                    AddError(103, variable.StartPosition);
                }

                if (!scope.IsTypeAvailable(type) || variable.Identifier == type.Identifier)
                {
                    scope.AddVariable(variable);
                }
                else
                {
                    scope.AddVariable(variable, type);
                }
            }

            GetNextToken();
        }

        private void OperatorsPart() // Раздел операторов
        {

            if (currentToken.Type != TokenType.Operation || (currentToken as OperationToken).Operation != Operation.Begin)
                SkipTo(new List<Operation> { Operation.Begin });

            AcceptOperation(Operation.Begin);
            Operator();

            while (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Semicolon) 
            {
                AcceptOperation(Operation.Semicolon);
                Operator();
            }

            AcceptOperation(Operation.End);
        }

        private void Operator() // Оператор
        {
            if (currentToken == null)
            {
                AddError(54);
                return;
            }

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

        // TODO syntax
        private void AssignmentOperator() // Оператор присваивания
        {
            int variableStartPosition = currentToken.StartPosition;

            if (!scope.IsVariableDescribed(currentToken as IdentifierToken))
            {
                scope.AddVariable(currentToken as IdentifierToken);
                AddError(104, variableStartPosition);
            }

            Type variableType = GetVariableType();
            AcceptOperation(Operation.Assignment);
            int expressionStartPosition = currentToken.StartPosition;
            Type expressionType = Expression();

            if (!expressionType.IsDerivedTo(variableType)) 
            {
                switch (variableType.ValueType)
                {
                    case ValueType.Integer:
                        AddError(106, expressionStartPosition);
                        return;
                    case ValueType.Real:
                        AddError(107, expressionStartPosition);
                        return;
                    case ValueType.String:
                        AddError(108, expressionStartPosition);
                        return;
                }
                
            }
        }

        private void IfOperator() // Условный оператор
        {
            AcceptOperation(Operation.If);
            int expressionStartPosition = currentToken.StartPosition;
            Type expressionType = Expression();

            if (!(expressionType is BooleanType))
                AddError(109, expressionStartPosition);

            AcceptOperation(Operation.Then);
            Operator();

            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Else)
            {
                AcceptOperation(Operation.Else);
                Operator();
            }
        }

        private void WhileOperator() // Цикл с предусловием
        {
            AcceptOperation(Operation.While);
            int expressionStartPosition = currentToken.StartPosition;
            Type expressionType = Expression();

            if (!(expressionType is BooleanType))
            {
                AddError(109, expressionStartPosition);
            }

            AcceptOperation(Operation.Do);
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

                if (!leftPartType.IsDerivedTo(rightPartType))
                {
                    AddError(105);
                }

                return availableTypes["BOOLEAN"];
            }

            return leftPartType;
        }

        // Простое выражение
        private Type SimpleExpression()
        {
            Type leftPatyType = Term();

            while (IsAdditiveOperation())
            {
                GetNextToken();
                Type rightPartType = Term();

                if (!rightPartType.IsDerivedTo(leftPatyType)) 
                {
                    AddError(105);
                }
            }

            return leftPatyType;
        }

        // Слагаемое
        private Type Term()
        {
            Type leftPartType = Factor();

            while (IsMultOperation())
            {
                GetNextToken();
                Type rightPartType = Factor();

                if (!rightPartType.IsDerivedTo(leftPartType))
                {
                    AddError(105);
                }
            }

            return leftPartType;
        }

        // Множитель
        private Type Factor()
        {
            Type factorType;

            if (currentToken.Type == TokenType.Operation)
            {
                AcceptOperation(Operation.LeftParenthesis);
                factorType = Expression();
                AcceptOperation(Operation.RightParenthesis);
            }
            else if (currentToken.Type == TokenType.Constant)
            {
                factorType = GetConstantType();
                GetNextToken();
            }
            else
            {
                factorType = GetVariableType();
                GetNextToken();
            }

            return factorType;
        }

        // Аддитивная операция
        private bool IsAdditiveOperation() => IsOperation(new List<Operation> { Operation.Plus, Operation.Minus, Operation.Or });

        // Мультипликативная операция
        private bool IsMultOperation() => IsOperation(new List<Operation> { Operation.Asterisk, Operation.Slash, Operation.Div, Operation.Mod, Operation.And });

        // Операция отношения
        private bool IsLogicalOperation() => IsOperation(new List<Operation> { Operation.Less, Operation.LessOrEqual, Operation.Greater, Operation.GreaterOrEqual, Operation.Equals, Operation.NotEqual });

        private bool IsOperation(List<Operation> operations)
        {
            if (currentToken == null || currentToken.Type != TokenType.Operation)
                return false;

            Operation currentOperation = (currentToken as OperationToken).Operation;

            foreach (Operation operation in operations)
                if (currentOperation == operation)
                    return true;

            return false;
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

        private Type GetVariableType()
        {
            IdentifierToken variable = currentToken as IdentifierToken;

            if (!scope.IsVariableDescribed(variable))
            {
                AddError(104);
                scope.AddVariable(variable);
            }

            return scope.GetVariableType(variable);
        }
    }
}