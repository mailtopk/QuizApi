# Project Details
Web api's project is to help students, make there own Quiz.
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
  |__/Repository
  |     |_/src
  |     |_/project.json
  |
  |__/AnswerRepository
  |      |__/Src
  |      |_/project.json
  |
  |_/DatabaseAccess
  |     |_/Src
  |     |_/project.json
  |
  |_/Cache
  |   |_/Src
  |   |_/project.json
  |
  |__/Test
  |     |_/ClassLevel
  |     |__/project.json
  |
  |_/global.json
  |_/Dockerfile
  |_/docker-compose.yml
  |_/Readme.md

```
* Required Software 
    - [Docker](https://www.docker.com/)

# Run in Docker Environment
Docker yml build two images, API web service and backend mongodb.

### Build docker images (web service and mongodb)
```
    $ docker-compose build
```

### Run docker images
```
    $ docker-compose up -d
```

### Run API explorer
```
    http://localhost:8080/swagger/ui/index.html
```



## Debuging
* Mongodb 
Log on to mongodb containers

List all the collections
```
    > db.getCollectionNames();
```

List all the documents in collections
```
    > get.<collectionname>.find()
```


* Redis Cache
List all the key
```
    > key *
```
Show all the values in a give key 
```
    > hgetall <key>
```

### To stop running containers
```
    $ docker-compose stop $(docker ps -aq) 
```

### To remove all images
```
    docker rmi -f $(docker images)
```

# Build and run docker images (web service and mongodb) separately 

Build Quiz web service image
```
    $ docker build -t quizserver:QuizWebApiServer .
```

Run image
```
    $ docker run -d -p 8080:5000 -t quizserver:QuizWebApiServer
```

## Mongo DB
* Attach to mongo process
```
    $ docker exec -it <containerId> bash
```

# Building Project using dotnet 
This will help in debuging application on local machine

* Required Software
    - [Microsoft dotnet core](https://www.microsoft.com/net/core) 

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