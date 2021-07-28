# Esl2Http
FreeSWITCH Esl2Http cross-platform adapter microservice

#### Preface

First of all I would like to thank ImpacTech for the interesting task to design and to develop the software with using of a modern technologies in a field of VoIP. Hope it could be started well according I've tested to deploy it into a different environments (ARM Linux x32 host under Raspberry Pi, Docker Desktop on Windows x64 with Windows Subsystem for Linux)

Both containers are under Alpine Linux - the most lightest Linux, built with a little number of layers as little as possible. The microservice is Linux executable, made as cross-platform application (can be compiled to Windows executable as well) with using of .net 5, building from the sources when the Docker Image is building, that makes possible to hard-code into the source code for security purposes the most critical credentials - ESL password and the FreeSWITCH host and port, to avoid to leak it by DevOps engineers or anybody who are not authorized to have ESL access there.
#
#### Before you begin

Before you begin please check your ESL access to the FreeSWITCH host, and please check your HTTP handlers where ESL events to be posted. For test purposes I've used [ptsv2.com - Post Test Server V2](https://ptsv2.com/) You can get there as much handlers as you can. The pre-configured HTTP handlers used into the default configuration are:

[https://ptsv2.com/t/1fnkf-1627122772/post](https://ptsv2.com/t/1fnkf-1627122772)
[https://ptsv2.com/t/lxlxm-1627287724/post](https://ptsv2.com/t/lxlxm-1627287724)
[ttps://ptsv2.com/t/iev4l-1627303429/post](ttps://ptsv2.com/t/iev4l-1627303429)

You can insert any HTTP handlers into the `http_post_handlers` table, and the adapter will send the unsent events as soon as the handler inserted.
