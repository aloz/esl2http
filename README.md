# Esl2Http
FreeSWITCH Esl2Http cross-platform adapter microservice

#### Preface

First of all I would like to thank ImpacTech for the interesting task to design and to develop the software with using of a modern technologies in a field of VoIP. Hope it could be started well according I've tested to deploy it into a different environments (ARM Linux x32 host under Raspberry Pi, Docker Desktop on Windows x64 with Windows Subsystem for Linux)

Both containers are under Alpine Linux - the most lightest Linux. Dockerfiles contain a little number of layers as little as possible. The microservice is Linux executable, made as a cross-platform application (can be compiled to Windows executable as well) with using of .net 5, building from the sources when the Docker Image is building, that makes possible to hard-code into the source code for security purposes the most critical credentials - ESL password and the FreeSWITCH ESL host and port, to avoid to leak it by DevOps engineers or anybody who are not authorized to have ESL access there.

Some key points of the microservice design:

 - Containers must be as little as possible, with as little as possible numbers of layers;
 - Asynchronious design is required, to minimize blocking on high-load, to avoid performance degradation.
 - SOLID principes. For example:
   - SingleResponsibility, to avoid monolyth of microservice/HTTP handlers, decomposition of tasks;
   - OpenClosed: layered design, able to extend in a future;
 - Avoid a lot of text logs, they are unreadable. Rich database design instead of text logs;
 - `HEARBEAT` - the most important FreeSWITCH event. The microservice subscribes on it always on start, logs it to `SDTOUT` and persists to the database as the last received `HEARTBEAT`
 - `STDOUT` logs should be as much as readable are be clear to understand what is going on;
 - Received ESL events should not be lost;
 - Integrity is not guaranteed on the incoming ESL stream, some data could be lost (possible, this is network buffer and performance issue) during the continious events receiving. It was a very strange but that reproduced only inside the Docker container, and not into the IDE (it could be reproducable only if to stay a long time into the breakpoint, while data arrived and nothing read it) So after the blind fix, taking into account the reason - it never reproduced after.

The micro-service is designed with using of async layers, each of them with a limited responsibility (i.e. ESL client layer, ESL events persister to database layer, events HTTP post layer, events HTTP repost layer) as much as to avoid locks on workflows.
All the events are persisted to the database to avoid lost of them.
All the business logic responsible for events processing resides inside the database (procedures, tables with calculated columns, relations etc.)
Database objects structure are able to give a full information about what's going on, how events are sent, which HTTP handlers are out of service, which events need to resend etc.

TODO TODO TODO
#
#### Before you begin

Before you begin please check your ESL access to the FreeSWITCH host, and please check your HTTP handlers where ESL events to be posted. For test purposes I've used [ptsv2.com - Post Test Server V2](https://ptsv2.com/). You can get there as much handlers as you can, and even to define there HTTP response status code. The pre-configured HTTP handlers used into the default configuration are:

- [https://ptsv2.com/t/1fnkf-1627122772/post](https://ptsv2.com/t/1fnkf-1627122772)
- [https://ptsv2.com/t/lxlxm-1627287724/post](https://ptsv2.com/t/lxlxm-1627287724)
- [https://ptsv2.com/t/iev4l-1627303429/post](https://ptsv2.com/t/iev4l-1627303429)

On Docker image creation the default HTTP handlers are inserted to the `http_post_handlers` table by the SQL script (see below)
During the microservice is working you can insert any HTTP handlers into the `http_post_handlers` table, and the unsent events will be posted ASAP.
