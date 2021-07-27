#!/bin/bash
echo Building esl2http docker image

cd ../../../
docker build -f Dockerfile-esl2http-raspi -t esl2http .
