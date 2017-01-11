# Project Details
Web api's project is to help students, make there own Quize.
It has three part
   1. Topic - Topic/Chapers are root objects which contains Questions
   2. Question - Questions fall into one topic 
   3. Answers - Answers are the object which has one - one relations with Questions 

*Note*: This project is initial prototype 

*   Directory strcture
```
/QuizSvc
  |__/AnswerRepository
  |      |__/Src
  |      |_/project.json
  |
  |_/Main
  |    |_/Src
  |    |_/project.json
  |
  |__/QuestionRepository
  |       |__/Src
  |       |_/project.json
  |
  |__/Test
  |     |_/Topic
  |     |__/project.json
  |
  |_/TopicRepository
  |     |_/src
  |     |_/project.json
  |
  |_/global.json
  |_/Dockerfile
  |_/docker-compose.yml
  |_/Readme.md

```

# Docker Environment

### Running from Docker
* Build and run quiz web service image
```
    $ docker build -t quizserver:QuizWebApiServer .

    $ docker run -d -p 8080:5000 -t quizserver:QuizWebApiServer
```

### Clean and remove containers
```
    $ docker stop $(docker ps -a -q) - Stop all containers
    $ docker rm $(docker ps -a -q)  - remove all containers
```

### Remove all images
```
    $ docker rmi $(docker images -a -q)
```


### Build docker compose
```
    docker-compose build
```

### Run docker compose
```
    docker-compose up
```

### Stop containers
```
    docker-compose stop
```

## Mongo DB
* Attach to mongo process
```
    $docker exec -it <containerId> bash
```

## Building Project
### Compaile
```
    cd - ../QuestionsSvc
    $dotnet restore

    cd - QuestionsSvc
    $dotnet build
    $dotnet run
```

## Create Test project
1. Create Src and test folders
2. Create *global.json* at root folder
3. cd to *test* folders

### Create a *test* project
```
     $dotnet new -t xunittest
```
*   Run test
```
    $dotnet test
```

### Building the solutions

#### With the project.json in SolutionDir/Src/ProjectName:
```
    $dotnet build */**/project.json
```

#### If project.json in SolutionDir/ProjectName:
```
    $ dotnet build **/project.json
```

### Build lib
```
    $ dotnet new -t lib
```