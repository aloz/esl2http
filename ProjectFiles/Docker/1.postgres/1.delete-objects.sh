#!/bin/bash
echo Deleting postgres docker objects

docker rm --force -v postgres
docker volume rm --force mnt-postgres
docker image rm  --force postgres
docker network rm esl2http
