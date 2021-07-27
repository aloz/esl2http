#!/bin/bash
echo Deleting esl2http docker objects

docker rm --force -v esl2http
docker image rm  --force esl2http

