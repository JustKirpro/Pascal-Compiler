﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PascalCompiler
{
    public class Compiler
    {
        private readonly ILGenerator ILGenerator;
        private readonly LexicalAnalyzer lexicalAnalyzer;
        private readonly Scope scope = new Scope();
        private Token currentToken;

        public Compiler(string inputPath, string outputPath, ILGenerator ILGenerator)
        {
            this.ILGenerator = ILGenerator;
            lexicalAnalyzer = new LexicalAnalyzer(inputPath, outputPath);
            GetNextToken();
        }

        public void Start() => Program();

        private Token GetNextToken() => currentToken = lexicalAnalyzer.GetNextToken();

        private bool IsCurrentTokenIdentifier() => currentToken != null && currentToken.Type == TokenType.Identifier;

        private void AcceptOperation(Operation operation)
        {
            if (currentToken == null || currentToken.Type != TokenType.Operation || (currentToken as OperationToken).Operation != operation)
            {
                AddError(OperationErrorMatcher.GetErrorCode(operation));
                throw new Exception();
            }

            GetNextToken();
        }

        private void AcceptIdentifier()
        {
            if (!IsCurrentTokenIdentifier())
            {
                AddError(18);
                throw new Exception();
            }

            GetNextToken();
        }

        private void AddError(int code, int position) => lexicalAnalyzer.AddError(code, position);

        private void AddError(int code) => lexicalAnalyzer.AddError(code);

        private void HandleExpressionException(Exception exception)
        {
            int errorPosition = (exception as ExpressionException).ErrorPostion;

            if (exception is OperatorException)
                AddError(23, errorPosition);
            else if (exception is TypeException)
                AddError(24, errorPosition);
            else
                AddError(25, errorPosition);
        }

        private void SkipTokensTo(List<Operation> operations, bool alsoSkipToIdentifier = false)
        {
            while (currentToken != null && currentToken.Type != TokenType.Operation)
            {
                if (alsoSkipToIdentifier && IsCurrentTokenIdentifier())
                    return;

                GetNextToken();
            }

            if (currentToken == null)
                return;

            Operation currentTokenOperation = (currentToken as OperationToken).Operation;

            while (currentToken != null && !operations.Contains(currentTokenOperation))
            {
                if (alsoSkipToIdentifier && IsCurrentTokenIdentifier())
                    return;

                GetNextToken();

                if (currentToken != null && currentToken.Type == TokenType.Operation)
                    currentTokenOperation = (currentToken as OperationToken).Operation;
            }
        }

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
                SkipTokensTo(NextTokens.Program);
            }

            Block();

            try
            {
                AcceptOperation(Operation.Point);
            }
            catch
            {
                return;
            }
        }

        private void Block()
        {
            VariablesPart();
            OperatorsPart();
        }

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

        private void SameTypeVariables()
        {
            List<IdentifierToken> variables = new List<IdentifierToken>();

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
                AddError(18);
                throw new Exception();
            }

            variables.Add(currentToken as IdentifierToken);
            GetNextToken();
        }

        private void AcceptType(List<IdentifierToken> variables)
        {
            if (!IsCurrentTokenIdentifier())
            {
                AddError(18);
                throw new Exception();
            }

            IdentifierToken type = currentToken as IdentifierToken;

            if (!scope.IsTypeAvailable(type))
                AddError(20);

            foreach (IdentifierToken variable in variables)
            {
                if (scope.IsVariableDescribed(variable))
                    AddError(21, variable.StartPosition);

                if (variable.Identifier == type.Identifier)
                    AddError(19, variable.StartPosition);

                if (!scope.IsTypeAvailable(type) || variable.Identifier == type.Identifier)
                    scope.AddVariable(variable);
                else
                    scope.AddVariable(variable, type);
            }

            GetNextToken();
        }

        private void OperatorsPart()
        {
            try
            {
                AcceptOperation(Operation.Begin);
            }
            catch
            {
                SkipTokensTo(NextTokens.OperatorsPartStart);

                if (currentToken == null || (currentToken as OperationToken).Operation == Operation.Point)
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
                SkipTokensTo(NextTokens.OperatorsPartEnd);
            }
        }

        private void Operator()
        {
            if (currentToken == null)
                return;

            if (currentToken.Type == TokenType.Constant)
            {
                AddError(17, currentToken.StartPosition);
                SkipTokensTo(NextTokens.OperatorEnd);
            }
            else if (currentToken.Type == TokenType.Identifier)
            {
                AssignmentOperator();
            }
            else
            {
                Operation currentOperator = (currentToken as OperationToken).Operation;

                if (currentOperator is Operation.End || currentOperator is Operation.Point)
                {
                    return;
                }
                else if (currentOperator == Operation.Begin)
                {
                    CompoundOperator();
                }
                else if (currentOperator == Operation.If)
                {
                    IfOperator();
                }
                else if (currentOperator == Operation.While)
                {
                    WhileOperator();
                }
                else
                {
                    AddError(17, currentToken.StartPosition);
                    SkipTokensTo(NextTokens.OperatorEnd);
                }
            }
        }

        private void CompoundOperator()
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

        private void AssignmentOperator()
        {
            IdentifierToken variable = currentToken as IdentifierToken;

            if (!scope.IsVariableDescribed(variable))
            {
                scope.AddVariable(variable);
                AddError(22);
            }

            Type type = GetVariableType();
            GetNextToken();

            try
            {
                AcceptOperation(Operation.Assignment);
                int expressionStartPosition = currentToken.StartPosition;
                Type expressionType = Expression();

                if (!expressionType.IsDerivedTo(type))
                {
                    if (type.ValueType == ValueType.Integer)
                        AddError(26, expressionStartPosition);
                    else if (type.ValueType == ValueType.Real)
                        AddError(27, expressionStartPosition);
                    else if (type.ValueType == ValueType.String)
                        AddError(28, expressionStartPosition);
                }
            }
            catch (Exception exception)
            {
                if (exception is OperatorException || exception is TypeException || exception is OperationException)
                    HandleExpressionException(exception);

                GetNextToken();
                SkipTokensTo(NextTokens.OperatorEnd);
            }
        }

        private void IfOperator()
        {
            AcceptOperation(Operation.If);
            int expressionStartPosition = currentToken.StartPosition;

            try
            {
                Type expressiontType = Expression();

                if (expressiontType != Types.GetType("BOOLEAN"))
                    AddError(29, expressionStartPosition);

                AcceptOperation(Operation.Then);
            }
            catch (Exception exception)
            {
                if (exception is OperatorException || exception is TypeException || exception is OperationException)
                    HandleExpressionException(exception);

                GetNextToken();
                SkipTokensTo(NextTokens.OperatorStart, true);
            }

            Operator();

            if (currentToken != null && currentToken.Type == TokenType.Operation && (currentToken as OperationToken).Operation == Operation.Else)
            {
                AcceptOperation(Operation.Else);
                Operator();
            }
        }

        private void WhileOperator()
        {
            AcceptOperation(Operation.While);
            int expressionStartPosition = currentToken.StartPosition;

            try
            {
                Type expressionType = Expression();

                if (expressionType != Types.GetType("BOOLEAN"))
                    AddError(29, expressionStartPosition);

                AcceptOperation(Operation.Do);
            }
            catch (Exception exception)
            {
                if (exception is OperatorException || exception is TypeException || exception is  OperationException)
                    HandleExpressionException(exception);

                GetNextToken();
                SkipTokensTo(NextTokens.OperatorStart, true);
            }

            Operator();
        }

        private Type Expression()
        {
            Type leftPartType = SimpleExpression();

            if (IsLogicalOperation())
            {
                int operationStartPosition = currentToken.StartPosition;
                GetNextToken();

                Type rightPartType = SimpleExpression();

                if (Types.AreTypesDerived(leftPartType, rightPartType))
                    return Types.GetType("BOOLEAN");
                else
                    throw new TypeException(operationStartPosition);
            }

            return leftPartType;
        }

        private Type SimpleExpression()
        {
            Type leftPartType = Term();

            while (IsAdditiveOperation())
            {
                Operation operation = (currentToken as OperationToken).Operation;
                int operationStartPosition = currentToken.StartPosition;
                GetNextToken();

                Type rightPartType = Term();

                if (Types.AreTypesDerived(leftPartType, rightPartType))
                {
                    leftPartType = Types.DeriveTypes(leftPartType, rightPartType);

                    if (!leftPartType.IsOperationSupported(operation))
                        throw new OperationException(operationStartPosition);
                }
                else
                {
                    throw new TypeException(operationStartPosition);
                }
            }

            return leftPartType;
        }

        private Type Term()
        {
            Type leftPartType = Factor();

            while (IsMultiplicativeOperation())
            {
                Operation operation = (currentToken as OperationToken).Operation;
                int operationStartPosition = currentToken.StartPosition;
                GetNextToken();

                Type rightPartType = Factor();

                if (Types.AreTypesDerived(leftPartType, rightPartType))
                {
                    leftPartType = Types.DeriveTypes(leftPartType, rightPartType);

                    if (leftPartType.ValueType == ValueType.Integer && operation == Operation.Slash)
                        leftPartType = Types.GetType("REAL");

                    if (!leftPartType.IsOperationSupported(operation))
                        throw new OperationException(operationStartPosition);
                }
                else
                {
                    throw new TypeException(operationStartPosition);
                }
            }

            return leftPartType;
        }

        private Type Factor()
        {
            Type factorType;

            if (currentToken.Type == TokenType.Operation)
            {
                int operatorStartPosition = currentToken.StartPosition;

                try
                {
                    AcceptOperation(Operation.LeftParenthesis);
                    factorType = Expression();
                    operatorStartPosition = currentToken.StartPosition;
                    AcceptOperation(Operation.RightParenthesis);
                }
                catch
                {
                    throw new OperatorException(operatorStartPosition);
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

        private bool IsAdditiveOperation() => IsOperation(Operations.AdditiveOperatons);

        private bool IsMultiplicativeOperation() => IsOperation(Operations.MultiplicativeOperations);

        private bool IsLogicalOperation() => IsOperation(Operations.LogicalOperations);

        private bool IsOperation(List<Operation> operations)
        {
            if (currentToken == null || currentToken.Type != TokenType.Operation)
                return false;

            Operation currentTokenOperation = (currentToken as OperationToken).Operation;

            return operations.Contains(currentTokenOperation);
        }

        private Type GetVariableType()
        {
            IdentifierToken variable = currentToken as IdentifierToken;

            if (!scope.IsVariableDescribed(variable))
            {
                scope.AddVariable(variable);
                AddError(22);
            }

            return scope.GetVariableType(variable);
        }

        private Type GetConstantType()
        {
            ConstantToken constant = currentToken as ConstantToken;

            if (constant.Variant.Type == VariantType.Integer)
                return Types.GetType("INTEGER");
            else if (constant.Variant.Type == VariantType.Real)
                return Types.GetType("REAL");
            else
                return Types.GetType("STRING");
        }
    }
}