
# WordStreamAnalyser

WordStreamAnalyser is a .NET-based application that processes large streams of text, analyzes word and character frequency, and generates detailed reports.

---

## Table of Contents

- [Features](#features)  
- [Getting Started](#getting-started)  
- [Prerequisites](#prerequisites)  
- [Building the Project](#building-the-project)  
- [Running the Application](#running-the-application)  
- [Running Tests](#running-tests)  
- [Project Structure](#project-structure)  
- [Contributing](#contributing)  

---

## Features

- Efficiently reads and processes large text streams.  
- Tracks character and word frequencies concurrently.  
- Reports top smallest and largest words.  
- Logs processing milestones and generates summary reports.  
- Uses MediatR for command handling and clean architecture principles.  
- Provides console output and logging integration.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later  
- A supported IDE (Visual Studio, VS Code, JetBrains Rider)  

---

### Building the Project

Clone the repository:

```bash
git clone https://github.com/ianjacksonnz/WordStreamAnalyser.git
cd WordStreamAnalyser
```

Restore dependencies and build:

```bash
dotnet restore
dotnet build
```

---

## Running the Application

Run the console app via:

```bash
dotnet run --project WordStreamAnalyser.ConsoleApp
```

The app processes a text stream and outputs statistics and logs to the console.

---

## Running Tests

Run all unit tests with:

```bash
dotnet test
```

Tests use xUnit, NSubstitute, and FluentAssertions frameworks.

---

## Project Structure

```
/WordStreamAnalyser
├── Application/          # Core business logic, commands, handlers
├── Domain/               # Domain models and entities
├── Infrastructure/       # Implementations of interfaces (e.g., word streams, report writers)
├── ConsoleApp/           # Console application entry point
├── Tests/                # Unit tests for Application and Infrastructure
```

---

## Contributing

Contributions are welcome! Please open issues or pull requests for bugs, improvements, or features.  
Ensure tests pass and code style remains consistent.

---


*Created by Ian Jackson*
