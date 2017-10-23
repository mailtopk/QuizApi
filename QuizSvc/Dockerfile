FROM microsoft/dotnet

LABEL MAINTAINER Prashant Pooboni


COPY . /app
WORKDIR /app

# Add certificate
#ADD you_certificate.crt:/container/cert/path

# Restore from global.json
RUN ["dotnet", "restore"]

# Build project 
WORKDIR "/app/Main"
RUN ["dotnet", "build"]
RUN ["dotnet", "clean"]

# Run test 
WORKDIR /app/Test
RUN ["dotnet", "test"]

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000

# Entry point
WORKDIR "/app/Main"
ENTRYPOINT ["dotnet", "run"]





