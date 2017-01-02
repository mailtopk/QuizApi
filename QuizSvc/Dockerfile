FROM microsoft/dotnet:latest
COPY . /app
WORKDIR /app

# Restore from global.json
RUN ["dotnet", "restore"]

# Build project 
WORKDIR "/app/Main"
RUN ["dotnet", "build"]

# Run test 
WORKDIR /app/Test
RUN ["dotnet", "test"]

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000

# Entry point
WORKDIR "/app/Main"
ENTRYPOINT ["dotnet", "run"]





