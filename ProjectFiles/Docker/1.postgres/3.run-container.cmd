@echo off
echo Running postgres docker container

docker run -it -d --name postgres -v mnt-postgres:/mnt/postgres -p 5432:5432 --restart on-failure postgres
pause 
