# Project Details
Web api's are build to help students to build there own Quize.
It has three part
   1. Topic - Topic/Chapers are root objects which contains Questions
   2. Question - Questions fall into one topic 
   3. Answers - Answers are the object which has one - one relations with Questions 

*Note*: This project is initial prototype 

*   Directory strcture
```
/Main
  |__/QuestionsSvc
  |      |__/Main
  |           |__src
  |_/Questions.Test
         |_Interation
  |__/Repository
         |__/QuestionRepository
      |__Test Files
      |__project.json
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
$dotnet build **/project.json
```

### Build lib
```
$dotnet new -t lib
```