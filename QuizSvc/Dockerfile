FROM microsoft/aspnetcore-build:2.0

LABEL MAINTAINER Prashant Pooboni


COPY . /app

# Add certificate
#ADD you_certificate.crt:/container/cert/path


# Build project 
WORKDIR "/app/Main"
RUN ["dotnet", "restore"]
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





