@echo off
echo Building esl2http docker image

cd ../../../
docker build -f Dockerfile-esl2http -t esl2http .
pause