#!/bin/bash
echo Running postgres docker container

docker run -it -d --net esl2http --name postgres -v mnt-postgres:/mnt/postgres -p 127.0.0.1:5432:5432 --restart on-failure postgres

