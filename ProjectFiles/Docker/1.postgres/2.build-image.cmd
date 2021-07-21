@echo off
echo Building postgres docker image

cd ../../../

docker network create esl2http
docker volume create mnt-postgres
docker build -f Dockerfile-postgres -t postgres .
pause