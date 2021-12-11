﻿using System.Collections.Generic;

namespace PascalCompiler
{
    public class Compiler
    {
        private readonly LexicalAnalyzer lexicalAnalyzer;
        private readonly Scope scope = new();
        private readonly Dictionary<string, Type> availableTypes = new()
        {
            { "INTEGER", new IntegerType() },
            { "REAL", new RealType() },
            { "STRING", new StringType() },
            { "BOOLEAN", new BooleanType() }
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

        private void AcceptOperation(Operation operation)
        {
            if (currentToken != null && currentToken is OperationToken && (currentToken as OperationToken).Operation == operation)
                GetNextToken();
        }

        private void AcceptIdentifier()
        {
            if (currentToken != null && currentToken is IdentifierToken)
                GetNextToken();
        }

        private void Program() // Программа
        {
            AcceptOperation(Operation.Program);
            AcceptIdentifier();
            AcceptOperation(Operation.Semicolon);
            Block();
            AcceptOperation(Operation.Point);
        }

        private void Block() // Блок
        {
            VariablesPart();
            OperatorsPart();
        }

        private void VariablesPart() // Раздел переменных
        {
            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Var)
            {
                AcceptOperation(Operation.Var);
                SameTypeVariables();
                AcceptOperation(Operation.Semicolon);

                while (currentToken != null && currentToken.Type == TokenType.Identifier)
                {
                    SameTypeVariables();
                    AcceptOperation(Operation.Semicolon);
                }
            }
        }

        private void SameTypeVariables() // Описание однотипных переменных
        {
            AcceptIdentifier();

            while (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Comma)
            {
                AcceptOperation(Operation.Comma);
                AcceptIdentifier();
            }

            AcceptOperation(Operation.Colon);
            AcceptIdentifier();
        }

        private void OperatorsPart() // Раздел операторов
        {
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

        private void AssignmentOperator() // Оператор присваивания
        {
            AcceptIdentifier();
            AcceptOperation(Operation.Assignment);
            Expression();
        }

        private void IfOperator() // Условный оператор
        {
            AcceptOperation(Operation.If);
            Expression();
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
            Expression();
            AcceptOperation(Operation.Do);
            Operator();
        }

        private  void Expression() // Выражение
        {
            SimpleExpression();

            if (IsLogicalOperation())
            {
                GetNextToken();
                SimpleExpression();
            }
        }

        private void SimpleExpression() // Простое выражение
        {
            Term();

            while (IsAddOperation())
            {
                GetNextToken();
                Term();
            }
        }

        private void Term() // Слагаемое
        {
            Factor();

            while (IsMultOperation())
            {
                GetNextToken();
                Factor();
            }
        }

        private void Factor() // Множитель
        {
            if (currentToken.Type == TokenType.Operation)
            {
                AcceptOperation(Operation.LeftParenthesis);
                Expression();
                AcceptOperation(Operation.RightParenthesis);
            }
            else
                GetNextToken();
        }

        private bool IsAddOperation() => IsOperation(new List<Operation> { Operation.Plus, Operation.Minus, Operation.Or });

        private bool IsMultOperation() => IsOperation(new List<Operation> { Operation.Asterisk, Operation.Slash, Operation.Div, Operation.Mod, Operation.And });

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
    }
}