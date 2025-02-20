## Introduction

This is an Azure Function project using .NET 9. The function listens to messages from Azure Service Bus and uses GovUK Notify to send email notifications.
It is designed for asynchronous processing, ensuring reliable and scalable email delivery.

## Contributing

For detailed instructions on how to contribute to this project, please read [CONTRIBUTING.md](./docs/CONTRIBUTING.md).

## Getting Started

### Prerequisites

- .NET 9 SDK
- Visual Studio 2022 or later
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp#install-the-azure-functions-core-tools)
- [Azure Service Emulator](https://github.com/Azure/azure-service-bus-emulator-installer) (See repository instructions for setup)
- GovUK Notify account and API key

### Installation

1. Clone the repository:

    ```
    git clone https://github.com/HealthResearchAuthority/rsp-email-notifications-function.git
    ```

2. Navigate to the project directory:

    ```
    cd rsp-email-notifications-function
    ```

3. Restore the packages:

    ```
    dotnet restore
    ```

For setting up the Azure Service Emulator, refer to the repository documentation.

## Build and Test

1. To build the project, navigate to the project directory and run:

    ```
    dotnet build
    ```

2. To run tests, use the following command:

    ```
    dotnet test .\tests\UnitTests\Rsp.NotifyFunction.UnitTests\ --no-build
    ```

3. To run the function locally, use:

    You need to have Azure Functions Core Tools installed before running the command below:

    ```
    func start
    ```

Ensure you have the required environment variables set up, including Service Bus connection strings and GovUK Notify API keys.

## License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.

