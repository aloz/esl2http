@echo off
echo Running esl2http docker container

docker run -it -d --net esl2http --name esl2http --restart on-failure --env-file="../../../esl2http.env" esl2http
pause 
