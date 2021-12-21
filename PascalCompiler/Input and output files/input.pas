program Hello;
var 
    a : integer;
    b : real;
    c : real;
begin
    a := 10;
    b := 20.4;
    c := b + a;
    writeln(c);

    if c > 30 then
        writeln('Greater than 30')
    else
        writeln('Not greater than 30');

    while a > 0 do
    begin
        writeln(a);
        a := a - 1
    end

end.
