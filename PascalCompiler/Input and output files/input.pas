program Hello;
var
    a : integer;
begin
    a := 100 mod 12 + 9 div 4; {a = 6}

    while a >= 0 do
    begin
        writeln(a);

        if a mod 2 = 0 then
            writeln('This number is even')
        else
            writeln('And this is an odd one');

        a := a - 1;
    end;

end.