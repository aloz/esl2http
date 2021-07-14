# The most lightweigh image
# NB: RUN commands are joined
# to optimize container build process
# by decreasing number of layers
# taking into account layers caching behavior
FROM alpine

LABEL description="FreeSWITCH Esl2Http cross-platform adapter microservice"
LABEL author=aloz

################ Build arguments ################

# The source code need to be built inside the container
ARG DIR_BUILD=/tmp/esl2http-build
ARG DIR_PUBLISH=/opt

ARG MICROSVC_NAME=esl2http
ARG MICROSVC_USER=esl2http

ARG DOTNET_VERSION=5.0
ARG DOTNET_INSTALL_DIR=/usr/share/dotnet

################ Build arguments ################

# Applying the latest updates
# Installing dotnet prepequesites
# File manager for debug purposes
RUN apk update && \
    apk upgrade && \
    apk add bash icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
#    apk add mc

# The user to run the microservice
RUN adduser -D -S -g "FreeSWITCH Esl2Http cross-platform adapter microservice" $MICROSVC_USER

WORKDIR $DIR_BUILD

# Installing .NET SDK by the oficial Microsoft online script
# This script depends on wget as far as I see
# If the script is changed then update wget as well
ADD https://dot.net/v1/dotnet-install.sh .
RUN chmod +x dotnet-install.sh && \
    apk add wget && \
    ./dotnet-install.sh --channel $DOTNET_VERSION --install-dir $DOTNET_INSTALL_DIR && \
    chown -R $MICROSVC_USER $DOTNET_INSTALL_DIR
ENV PATH=$PATH:$DOTNET_INSTALL_DIR

# Building microservice from the sources
WORKDIR $DIR_BUILD/src/$MICROSVC_NAME
COPY $MICROSVC_NAME.sln .
COPY $MICROSVC_NAME/* $MICROSVC_NAME/
# Building solution
# Cleanup build directory
RUN dotnet restore $MICROSVC_NAME.sln && \
    dotnet build $MICROSVC_NAME.sln -c Release -o $DIR_BUILD/build/$MICROSVC_NAME && \
    dotnet publish $MICROSVC_NAME.sln -c Release -o $DIR_PUBLISH/$MICROSVC_NAME && \
    chown -R $MICROSVC_USER $DOTNET_INSTALL_DIR && \
    rm -rf $DIR_BUILD

USER $MICROSVC_USER
ENV ESL2HTTP_ENTRYPOINT=$DIR_PUBLISH/$MICROSVC_NAME/$MICROSVC_NAME
ENTRYPOINT $ESL2HTTP_ENTRYPOINT