@echo off
echo Deleting postgres docker objects

docker rm --force -v postgres
docker volume rm --force mnt-postgres
docker image rm  --force postgres
pause