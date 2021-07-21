@echo off
echo Running esl2http docker container

docker run -it -d --net esl2http --name esl2http --restart on-failure -e esl2http_DBConnectionString="Host=postgres;Username=esl2http;Password=esl2http;Database=esl2http" esl2http
pause 
