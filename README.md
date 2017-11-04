# Project Details
This is a RESTful web service to build Quiz/Flashcard .

It has three part
   1. Topic - Topic/Chapters are root objects.
   2. Question - These are the child objects of topic.
   3. Answers - Answers to the questions. 

Example : -
>Topic - *Math.*
>Question - *What is 2 + 4.*
>Answer - *6.*

*Note*: This project is initial prototype 


## Topics
![ScreenShot](/QuizSvc/Images/topics.png)

## Questions
![ScreenShot](/QuizSvc/Images/questions.png)


*   Directory structure
```
/QuizSvc
  |
  |__/Main
  |    |_/Src
  |    |    |_/Controllers
  |    |    |_/QuizManager
  |    |    |_/ResponseData
  |    |_/Main.csproj
  |
  |__/Repository
  |     |_/src
  |     |    |__/TopicRepository
  |     |    |__/QuestionsRepository
  |     |    |_/AnswerRepository
  |     |_/Repository.csproj
  |
  |__/DatabaseAccess
  |     |_/Src
  |     |_/DatabaseAccess.csproj
  |
  |__/Helper
  |     |_/Helper.csproj
  |
  |__/Cache
  |     |_/Src
  |     |_/Cache.csproj
  |
  |__/Test
  |     |_/ClassLevel
  |     |__/ClassLevel
  |
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
Build docker images, web service, mongodb and redis caching.
Make sure your current directory is QuizSvc
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
Get list of all the endpoint, below api explorer
```
    http://localhost:8080/swagger/ui/index.html
```

At this point quiz web service is ready to use.

### Cleanup docker stop and remove containers, networks
```
    $ docker-compose down
```

### To do
- [x] Support to update Topic
- [x] Support to update Question
- [ ] Support to update Answer
- [ ] Build UI
- [x] Add Swagger documentation
- [ ] Multiple answer for one questions 
