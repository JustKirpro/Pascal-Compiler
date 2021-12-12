program Hello;
var
    integer : integer; {Создаст переменную неизвестного типа}
    x : integer; {Название типа уже переопределенно на название переменной}
    b, c: real;
    d, b :string; {Повторное описание переменной b}
begin
    integer  := 'fd' + 5; {Ошибка в выражении}
    b := a / 3 + (a + 12) MOD 7; {Неописанная переменная}
    c := 'fdfds'; {Ожидался тип real}

    if a  + (5 + a) then {Ожидалось логическое значение}
        b := 323232.325
    else
        a:= 45;

    while (a > 0) AND (a * 12 < 100) do
        begin
            b := b * a;
            a := a - 1;
        End
end.