@echo off
echo Running esl2http docker container

docker run -d --name esl2http --restart on-failure esl2http
pause 
