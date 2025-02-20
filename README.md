## Introduction

This is an ASP.NET Core WebAPI project using .NET 8. This project provides a robust and scalable framework for building Web APIs and Microservices

# Contributing

For detailed instructions on how to contribute to this project, please read [CONTRIBUTING.md](./docs/CONTRIBUTING.md) 

# Getting Started

## Prerequisites

- .NET 9 SDK
- Visual Studio 2022 or later

## Installation

1. Clone the repository

```
git clone https://FutureIRAS@dev.azure.com/FutureIRAS/Research%20Systems%20Programme/_git/rsp-iras-service
```
2. Navigate to the project directory

```
cd rsp-iras-service
```

3. Restore the packages
```
dotnet restore
```
# Build and Test

1. To build the project, navigate to the project directory and run the following command:

```
dotnet build
```

2. To run the tests, use the following command. Path to the test project is needed if you are running the tests from outside the test project directory.

```
 dotnet test .\tests\UnitTests\Rsp.IrasService.UnitTests\ --no-build

 dotnet test .\tests\IntegrationTests\Rsp.IrasService.IntegrationTests\ --no-build
```

3. To run the application, use the following command:

```
dotnet run --project .\src\WebApi\Rsp.IrasService.WebApi\
```
# License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.