@echo off
echo Building esl2http docker image

cd ../../
docker build -t esl2http .
pause