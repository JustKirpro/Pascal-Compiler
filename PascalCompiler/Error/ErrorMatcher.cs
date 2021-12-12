using System.Collections.Generic;

namespace PascalCompiler
{
    public static class ErrorMatcher
    {
        private static readonly Dictionary<int, string> dictionary = new()
        {
            [1] = "Открытие незакрытого комментария",
            [2] = "Закрытие неоткрытого комментария",
            [3] = "Ошибка в описании строковой константы",
            [4] = "Ошибка в описании вещественной константы",
            [5] = "Значение целочисленной константы превышает предел",
            [6] = "Длина идентификатора превышает предел",
            [7] = "Запрещённый символ",
            [8] = "Ожидался идентификатор",
            [9] = "Недопустимый тип",
            [10] = "Повторное описание переменной",
            [11] = "Название переменной не может совпадать с названием её типа"
        };

        public static string GetErrorDescription(int errorCode) => dictionary[errorCode];
    }
}