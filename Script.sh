#!/bin/sh

#  Script.sh
#  
#
#  Created by Артём Семёнов on 09.03.2018.
#  

let variable=9

while [ $variable -gt 0 ]
do
echo "какой тест?"
echo "1. Присваение."
echo "2. Условие."
echo "3. Цикл FOR."
echo "4. Цикл While."
echo "5. Комбинированный вариант."
read variable # чтение

case $variable in
1)
./bin/Debug/SimpleLang.exe ./Test/as/as.txt
;;
2)
./bin/Debug/SimpleLang.exe ./Test/if/if.txt
;;
3)
./bin/Debug/SimpleLang.exe ./Test/for/for.txt
;;
4)
./bin/Debug/SimpleLang.exe ./Test/while/while.txt
;;
5)
./bin/Debug/SimpleLang.exe ./Test/comby/comby.txt
;;
0)
echo "выход"
;;
*)
echo "ошибка"
;;
esac
done
