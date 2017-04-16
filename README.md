# Project Details
This is a RESTful web service to build Quiz/Flashcard .

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
  |    |    |_/Controllers
  |    |    |_/QuizManager
  |    |    |_/ResponseData
  |    |_/project.json
  |
  |__/Repository
  |     |_/src
  |     |    |__/TopicRepository
  |     |    |__/QuestionsRepository
  |     |    |_/AnswerRepository
  |     |_/project.json
  |
  |__/DatabaseAccess
  |     |_/Src
  |     |_/project.json
  |
  |__/Helper
  |     |_/project.json
  |
  |__/Cache
  |     |_/Src
  |     |_/project.json
  |
  |__/Test
  |     |_/ClassLevel
  |     |__/project.json
  |
  |__/global.json
  |__/Dockerfile
  |__/docker-compose.yml
  |__/Readme.md

```


# Build and Run project in Docker Environment

### Clone the project
```
https://github.com/mailtopk/QuizApi.git
```
* Required Software 
    - [Docker](https://www.docker.com/)
    
### Build docker images (web service, mongodb and redis)
Build docker images, web service, mongodb and redis cacheing.
Make sure your current directrly is QuizSvc
```
    $ docker-compose build
```

### Run the docker containers.
```
    $ docker-compose up -d
```
At this point docker container's should be up and running.

### Verify containers are up and running
```
    $ docker ps --format "table {{.ID}} \t{{.Image}} \t{{.Names}} \t{{.Ports}}"
``` 

### Run API explorer
Get list of all the enpoint, below api explorer
```
    http://localhost:8080/swagger/ui/index.html
```

At this point quiz web service is ready to use.

### Cleanup docker stop and remove containers, networks
```
    $ docker-compose down
```

## Delete all docker images
```
```
