# the most lightweigh image
FROM alpine

################ Build arguments ################

# the source code builds inside the container
ARG DIR_BUILD=/tmp/esl2http-build
ARG DIR_PUBLISH=/opt

ARG MICROSVC_NAME=esl2http
ARG MICROSVC_USER=esl2http

ARG DOTNET_VERSION=5.0
ARG DOTNET_INSTALL_DIR=/usr/share/dotnet

################ Build arguments ################

LABEL description="FreeSWITCH Esl2Http adapter microservice"
LABEL author=aloz

# Applying the latest updates
RUN apk update
RUN apk upgrade

# Installing dotnet prepequesites
RUN apk add bash icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
RUN apk add wget

# File manager - for test purposes
RUN apk add mc

WORKDIR $DIR_BUILD

# Installing .NET SDK by the oficial Microsoft online script
RUN wget https://dot.net/v1/dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh --channel $DOTNET_VERSION --install-dir $DOTNET_INSTALL_DIR
ENV PATH=$PATH:$DOTNET_INSTALL_DIR

# Building microservice from the sources
WORKDIR $DIR_BUILD/src/$MICROSVC_NAME
COPY $MICROSVC_NAME.sln .
COPY $MICROSVC_NAME/* $MICROSVC_NAME/
RUN dotnet clean -c Debug
RUN dotnet clean -c Release
RUN dotnet restore $MICROSVC_NAME.sln
RUN dotnet build $MICROSVC_NAME.sln -c Release -o $DIR_BUILD/build/$MICROSVC_NAME
RUN dotnet publish $MICROSVC_NAME.sln -c Release -o $DIR_PUBLISH/$MICROSVC_NAME

# Cleanup
RUN rm -rf $DIR_BUILD

# Create a new user and set the ownership
RUN adduser --disabled-password $MICROSVC_USER
RUN chown -R $MICROSVC_USER $DIR_PUBLISH/$MICROSVC_NAME
RUN chown -R $MICROSVC_USER $DOTNET_INSTALL_DIR

USER $MICROSVC_USER
ENV ESL2HTTP_ENTRYPOINT=$DIR_PUBLISH/$MICROSVC_NAME/$MICROSVC_NAME
ENTRYPOINT $ESL2HTTP_ENTRYPOINT