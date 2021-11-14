program Hello; {first comment!} {second comment?}
var
    a : integer?;
    b,    c :real;
begin
    b:=4.5;
    a := b * (4 + a);
    c := b * (32 + a * (b + 3));
    if a + 5 > 0 then
        b := 32.4
    else
        c:= 45.3;

    while a > 0 do
        a := a - 1
end.