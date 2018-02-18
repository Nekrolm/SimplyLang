#!/bin/bash

clear

./Gplex.exe /unicode SimpleLex.lex
./Gppg.exe /no-lines /gplex SimpleYacc.y
