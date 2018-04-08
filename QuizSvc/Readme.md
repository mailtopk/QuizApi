# Project Details
This is a RESTful web service to build Quiz/Flashcard .

## Graylog Setup
Login to Graylog dashboard (http://localhost:900), using default username and password and setup *System/Input*.
Navigate to System/input page (graylog menu -> System -> Inputs), Select *GELF UDP* in drop down box, and click on *Launch new input* button.
Enter title, select node and save.

## Debugging under Docker environment 
* Logging is supported by Graylog 
* Get container info if running
``` 
    $ docker inspect <containerId>
``` 
## Mongodb 
Log on to mongodb containers
```
    $ docker exec -it <containerId> bash
```
Launch mongo client
```
    $ mongo
```
From mongo console List all dbs
```
    > show dbs
```

List all the collections
```
    > db.getCollectionNames();
```

List all the documents in collections
```
    > get.<collectionname>.find()
```

## Redis Cache
Run redis container
```
    $ docker exec -it <containerId> bash
```
Run redis client
```
    $ redis-cli
```
List all the key
```
    > keys *
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

## Build and run docker images (web service and mongodb) separately 

Build Quiz web service image
```
    $ docker build -t quizserver:QuizWebApiServer .
```

Run image
```
    $ docker run -d -p 8080:5000 -t quizserver:QuizWebApiServer
```

## Building Project using dotnet 
This will help in debugging application on local machine

* Required Software
    - [Microsoft dotnet core](https://www.microsoft.com/net/core) 

### Restore 

```
    $ cd to ../QuizSvc/Main
    $ dotnet restore
```

### Build 
```
    $ dotnet build
```
### Run
```
    $ dotnet run
```
###   Run test
```
    $ dotnet test
```
### Docker clean
```
    $ docker rm -f $(docker ps -a -q)
    $ docker volume rm $(docker volume ls)
    $ docker rmi -f $(docker images -q)
```