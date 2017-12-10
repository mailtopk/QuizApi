FROM microsoft/aspnetcore-build:2.0 AS build-env
LABEL MAINTAINER Prashant Pooboni

WORKDIR /app
COPY Main/*.csproj ./Main
COPY Cache/*.csproj ./Cache
COPY DatabaseAccess/*.csproj ./DatabaseAccess
COPY Helper/*.csproj ./Helper
COPY QuizSwagger/*csproj ./QuizSwagger
COPY Repository/*.csproj ./Repository

COPY . ./

WORKDIR /app/Main
RUN dotnet publish -c Release -o out

WORKDIR /app/Cache
RUN dotnet publish -c Release -o out


FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/Main/out .
COPY --from=build-env /app/Cache/out .
ENTRYPOINT ["dotnet", "Main.dll"]





