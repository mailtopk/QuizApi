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

### Required Software
    - Git
    - [Docker](https://www.docker.com/)

### Clone the project
In general you can clone the repo with below command.
```
https://github.com/mailtopk/QuizApi.git
```
*Trouble shot : -* Some of the environments - you may see error *Cloning into 'QuizApi'...
fatal: unable to access https://github.com/mailtopk/QuizApi.git/ Could not resolve host: github.com*, try setting git config url
```
    $ git config --global url.git://github.com/.insteadOf https://github.com/
```

After successful clone to git repo, you should see a folder *QuizSvc*. cd to *QuizSvc/QuizSvc* to build docker images
```
    $ cd QuizSvc/QuizSwagger/
```

### Build docker images (web service, mongodb and redis, graylog)
Build docker images, web service, mongodb redis caching and docker.
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

### Log
Access log data - Graylog.
Default username and password of graylog
```
    http://localhost:9000/
```

At this point quiz web service is ready to use.

### Cleanup docker stop and remove containers, networks
```
    $ docker-compose down
```

### To do
- [x] Support to update Topic
- [x] Support to update Question
- [x] Support for logging (Graylog)
    - [ ] Script to configure Graylog settings
- [ ] Support to update Answer
- [ ] Build UI
- [x] Add Swagger documentation
- [ ] Multiple answer for one questions
