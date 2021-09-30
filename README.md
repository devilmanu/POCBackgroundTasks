
# Getting Started

Tye is a tool that makes developing, testing, and deploying microservices and distributed applications easier. Project Tye includes a local orchestrator to make developing microservices easier and the ability to deploy microservices to Kubernetes with minimal configuration.

## Installing Tye

1. Install [.NET Core 3.1](<http://dot.net>).
1. Install tye via the following command:

    ```text
    dotnet tool install -g Microsoft.Tye --version "0.10.0-alpha.21420.1"
    ```

    OR if you already have Tye installed and want to update:

    ```text
    dotnet tool update -g Microsoft.Tye --version "0.10.0-alpha.21420.1"
    ```

    > If using Mac and, if getting "command not found" errors when running `tye`, you may need to ensure that the `$HOME/.dotnet/tools` directory has been added to `PATH`.
    >
    > For example, add the following to the end of your `~/.zshrc` or `~/.zprofile`:
    >
    > ```
    > # Add .NET global tools (like Tye) to PATH.
    > export PATH=$HOME/.dotnet/tools:$PATH
    > ```

## How to run 
`tye run`