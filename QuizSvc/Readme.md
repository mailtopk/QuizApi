# Project Details
Web api's project is to help students, make there own Quize.
It has three part
   1. Topic - Topic/Chapers are root objects.
   2. Question - Questions fall into one bucket of topic
   3. Answers - Answers to the questions. 

*Note*: This project is initial prototype 

*   Directory strcture
```
/QuizSvc
  |
  |__/Main
  |    |_/Src
  |    |_/project.json
  |
  |__/TopicRepository
  |     |_/src
  |     |_/project.json
  |
  |__/QuestionRepository
  |       |__/Src
  |       |_/project.json
  |
  |__/AnswerRepository
  |      |__/Src
  |      |_/project.json
  |
  |__/Test
  |     |_/Topic
  |     |__/project.json
  |
  |_/DatabaseAccess
  |     |_/Src
  |     |_/project.json
  |
  |_/global.json
  |_/Dockerfile
  |_/docker-compose.yml
  |_/Readme.md

```
* Required Software 
    - [Microsoft dotnet core](https://www.microsoft.com/net/core) 
    - [Docker](https://www.docker.com/)

# Docker Environment

## Using docker compose

### Build docker images (web service and mongodb)
```
    $ docker-compose build
```

### Run docker images
```
    $ docker-compose up
```

### Stop containers
```
    $ docker-compose down 
```

### Remove all images
```
    $ docker rmi $(docker images -a -q)
```


## Build and run docker images (web service and mongodb) separately 
### Build and run quiz web service image
```
    $ docker build -t quizserver:QuizWebApiServer .

    $ docker run -d -p 8080:5000 -t quizserver:QuizWebApiServer
```

## Mongo DB
* Attach to mongo process
```
    $ docker exec -it <containerId> bash
```

## Building Project using dotnet 
### Restore 

```
    $ cd - ../QuizSvc/Main
    $ dotnet restore
```

### Build 
```
    cd - QuizSvc/Main
    $ dotnet build
```
### Run
```
    $ dotnet run
```
## References 
### To Create simple dotnet console
```
    $ dotnet new 
```
### Build lib
```
    $ dotnet new -t lib
```
### To create a *test* project
```
     $ dotnet new -t xunittest
```
###   Run test
```
    $ dotnet test
```