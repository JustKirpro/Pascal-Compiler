﻿using System.Collections.Generic;

namespace PascalCompiler
{
    public class SyntaxAnalyzer
    {
        private readonly LexicalAnalyzer lexer;
        private Token currentToken;

        public SyntaxAnalyzer(string inputPath, string outputPath)
        {
            lexer = new(inputPath, outputPath);
            GetNextToken();
        }

        private void GetNextToken()
        {
            currentToken = lexer.GetNextToken();
        }

        private void AddError(string errorText)
        {
            lexer.Io.Errors.Add(errorText);
        }

        private void AcceptOperation(Operation operation)
        {
            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == operation)
                GetNextToken();
            else
                AddError($"Ошибка! Ожидался символ {operation}");
        }

        private void AcceptIdentifier()
        {
            if (currentToken != null && currentToken.Type == TokenType.Identifier)
                GetNextToken();
            else
                AddError("Ошибка! Ожидалось имя");
        }

        public void StartAnylysis()
        {
            Program();
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
            TypesPart();
            VariablesPart();
            OperatorsPart();
        }

        private void TypesPart() // Раздел типов
        {
            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Type)
            {
                AcceptOperation(Operation.Type);
                TypeDefinition();
            }
        }

        private void TypeDefinition() // Определение типа
        {
            AcceptIdentifier();
            AcceptOperation(Operation.Equals);
            AcceptIdentifier();
            AcceptOperation(Operation.Semicolon);

            while (currentToken != null && currentToken.Type == TokenType.Identifier)
            {
                AcceptIdentifier();
                AcceptOperation(Operation.Equals);
                AcceptIdentifier();
                AcceptOperation(Operation.Semicolon);
            }
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
                AddError("Ошибка! Ожидался оператор");
                return;
            }

            if (currentToken.Type == TokenType.Identifier)
            {
                AssignmentOperator();
            }
            else
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

        private void Expression() // Выражение
        {
            SimpleExpression();

            while (IsLogicalOperation())
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
            if (currentToken == null)
            {
                AddError("Ошибка! Ожидалось выражение");
                return;
            }

            if (currentToken.Type == TokenType.Operation)
            {
                AcceptOperation(Operation.LeftParenthesis);
                Expression();
                AcceptOperation(Operation.RightParenthesis);
            }
            else
                GetNextToken();
        }

        private bool IsAddOperation() // Аддитивная операция
        {
            return IsOperation(new List<Operation> { Operation.Plus, Operation.Minus, Operation.Or });
        }

        private bool IsMultOperation() // Мультипликативная операция
        {
            return IsOperation(new List<Operation> { Operation.Asterisk, Operation.Slash, Operation.Div, Operation.Mod, Operation.And });
        }

        private bool IsLogicalOperation() // Операция отношения
        {
            return IsOperation(new List<Operation> { Operation.Less, Operation.LessOrEqual, Operation.Greater, Operation.GreaterOrEqual, Operation.Equals, Operation.NotEqual });
        }

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