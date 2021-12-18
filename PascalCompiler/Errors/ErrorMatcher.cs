﻿using System.Collections.Generic;

namespace PascalCompiler
{
    public static class ErrorMatcher
    {
        private static readonly Dictionary<int, string> dictionary = new()
        {
            // Лексические ошибки
            [1] = "Открытие незакрытого комментария",
            [2] = "Закрытие неоткрытого комментария",
            [3] = "Ошибка в описании строковой константы",
            [4] = "Ошибка в описании вещественной константы",
            [5] = "Значение целочисленной константы превышает предел",
            [6] = "Длина идентификатора превышает предел",
            [7] = "Запрещённый символ",
            // Синтаксические ошибки
            [8] = "Ожидалось ключевое слово PROGRAM",
            [9] = "Ожидалось ключевое слово BEGIN",
            [10] = "Ожидалось ключевое слово END",
            [11] = "Ожидалось ключевое слово THEN",
            [12] = "Ожидалось ключевое слово DO",
            [13] = "Ожидался оператор :=",
            [14] = "Ожидался оператор ;",
            [15] = "Ожидался оператор :",
            [16] = "Ожидался оператор .",
            [17] = "Ожидался оператор",
            [18] = "Ожидался идентификатор",
            // Семантические ошибки
            [19] = "Название переменной не может совпадать с названием её типа",
            [20] = "Недопустимый тип",
            [21] = "Повторное описание переменной",
            [22] = "Неописанная переменная",
            [23] = "Ошибка в выражении",
            [24] = "Неприводимые типы в выражении",
            [25] = "Недопустимая операция для данного типа выражения",
            [26] = "Ожидался целочисленный тип",
            [27] = "Ожидался тип, приводимый к вещественному",
            [28] = "Ожидался строковой тип",
            [29] = "Ожидался логический тип",
        };

        public static string GetErrorDescription(int errorCode) => dictionary[errorCode];
    }
}