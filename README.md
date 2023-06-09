## Electricity Data Aggregator Application

This is a component of the Electricity Data Aggregator application responsible for aggregating data from CSV files and storing it in a database. The component consists of the `GetAggregatedDataQueryHandler` class, which handles the processing and storage of aggregated data.

# Project Setup and Execution

To set up and run the Electricity Data Aggregator project, follow the steps below:

1. Clone the project repository to your local machine.

2. Open the package manager console in Visual Studio or a terminal window.

3. Navigate to the project directory (`src/ElectricityDataAggregator.Persistance`) using the command line.

4. Run the following command to apply the database migrations: `update-database`

5. Wait for the database migration to complete. This step will create the necessary tables and schema in the database.

6. Once the database update is finished, set the `src/ElectricityDataAggregator.API` project as the startup project.

7. Start the project by running it in Visual Studio or using the appropriate command in the terminal.

8. After the project starts, open the Swagger UI by navigating to the API endpoint in your web browser. The URL should be something like `https://localhost:<port>/swagger`.

9. In the Swagger UI, find the desired endpoint and click on "Try it out" to test the API endpoint.

10. Enter the required parameters and click on "Execute" to send the request to the server.

11. Monitor the console log of the project during execution to check the estimated time and memory usage.

Estimated execution time after clicking "Execute" is approximately 7 seconds, and the memory usage is expected to be around 152MB, which will be displayed in the console log of the project.


# Dependencies

- `ElectricityDataAggregator.Application.Infrastructure.Persistance`: Provides the database context (`IElectricityDbContext`).
- `ElectricityDataAggregator.Common.Exceptions`: Contains custom exception classes.
- `ElectricityDataAggregator.Domain.Entities`: Defines the entity classes used in the application.
- `MediatR`: Mediator pattern library for handling queries and commands.
- `Microsoft.Extensions.Configuration`: Provides access to configuration settings.
- `System.Collections.Concurrent`: Provides thread-safe data structures.
- `System.Diagnostics`: Allows performance monitoring and debugging.
- `System.Globalization`: Provides culture-specific formatting and parsing.

# Usage

1. Create an instance of `GetAggregatedDataQueryHandler` by injecting the required dependencies (`IElectricityDbContext` and `IConfiguration`).
2. Call the `Handle` method of the `GetAggregatedDataQueryHandler` class, passing the `GetAggregatedDataQuery` request object and a cancellation token.
3. The method will perform the following steps:
   - Initialize a concurrent dictionary to store the aggregated data.
   - Get the root path for the CSV files using the `GetRootPath` method.
   - Generate the file paths of the CSV files using the `GetCsvFilePaths` method.
   - Process the CSV files using the `ProcessCsvData` method, which reads each line of the CSV files, extracts the required data, and updates the aggregated data dictionary.
   - Store the aggregated data in the database using the `StoreAggregatedDataInDatabase` method.
   - Stop the stopwatch timer and print the elapsed time and allocated memory.
4. Return a `GetAggregatedDataQueryResponse` object containing the aggregated data.

# Helper Methods

- `GetRootPath`: Retrieves the root path for the CSV files based on the current application domain and configuration settings.
- `StoreAggregatedDataInDatabase`: Converts the aggregated data dictionary into a list of `AggregatedData` entities and stores them in the database using the `IElectricityDbContext`.
- `FormatBytes`: Formats a given number of bytes into a human-readable format (e.g., KB, MB, GB, etc.).
- `GetCsvFilePaths`: Generates the file paths of the CSV files based on the root path and desired months.
- `ProcessCsvData`: Processes the CSV files by reading each line, extracting the required data, and updating the aggregated data dictionary.
- `ParseDoubleOrDefault`: Parses a string value into a double, or returns the default value if parsing fails.

Note: This is just a code snippet and may require additional implementation and configuration in a complete application.


# How It Works

The `GetAggregatedDataQueryHandler` class is responsible for aggregating data from CSV files and storing it in a database. Here's an overview of how it works:

1. Dependencies: The handler requires the `IElectricityDbContext` and `IConfiguration` dependencies to access the database and configuration settings, respectively.

2. Request Handling: The `Handle` method is called when a `GetAggregatedDataQuery` request is received. It takes the request object and a cancellation token as parameters.

3. Aggregated Data Dictionary: Inside the `Handle` method, a concurrent dictionary (`aggregatedData`) is initialized to store the aggregated data. This dictionary will hold the region as the key and a tuple of `PPlus` and `PMinus` as the value.

4. CSV File Processing: The handler retrieves the root path for the CSV files and generates the file paths using the `GetRootPath` and `GetCsvFilePaths` helper methods, respectively. The `ProcessCsvData` method is then called to process each CSV file concurrently.

5. Processing Each CSV File: The `ProcessCsvData` method reads each line of the CSV file, extracts the required data, and updates the `aggregatedData` dictionary accordingly. It filters the data based on the condition `obtPavadinimas.Equals("Butas")`.

6. Storing Aggregated Data: After processing all the CSV files, the handler calls the `StoreAggregatedDataInDatabase` method to convert the `aggregatedData` dictionary into a list of `AggregatedData` entities. It then adds these entities to the database using the `IElectricityDbContext`.

7. Performance Monitoring: The handler stops a stopwatch timer and prints the elapsed time and allocated memory using the `stopwatch` and `FormatBytes` helper methods, respectively.

8. Response: Finally, the handler creates a `GetAggregatedDataQueryResponse` object, populates it with the aggregated data from the `aggregatedData` dictionary, and returns it as the response.

Note: Make sure to configure the dependencies and handle any additional error cases or customizations required for your specific application.

## Technology Architecture

The Electricity Data Aggregator project utilizes the following technologies and architecture:

- **Language:** The project is written in C#.

- **ASP.NET Core:** It leverages the ASP.NET Core framework for building web applications and APIs.

- **MediatR:** MediatR is used as a mediator pattern implementation for handling requests and coordinating application logic.

- **Entity Framework Core:** The Entity Framework Core is utilized for database access and ORM capabilities.

- **ConcurrentDictionary:** The `ConcurrentDictionary` class is used to provide thread-safe access to the aggregated data dictionary.

- **CSV File Processing:** The project reads and processes CSV files using the `StreamReader` and `ReadLineAsync` methods.

- **Dependency Injection:** The application utilizes dependency injection to manage and inject dependencies into classes.

- **Configuration:** The `IConfiguration` interface is used to access configuration settings, such as file paths, from the appsettings.json file.

- **Concurrency:** The project utilizes the `Task` class and asynchronous programming to process CSV files concurrently.

- **Logging:** Logging is implemented using the `ILogger` interface from Microsoft.Extensions.Logging.

- **CancellationToken:** The `CancellationToken` is used for managing cancellation of asynchronous operations.

- **Performance Monitoring:** The project utilizes the `Stopwatch` class to measure elapsed time and monitor performance.

- **Memory Allocation:** The `Process.GetCurrentProcess().WorkingSet64` property is used to retrieve and format the allocated memory.

The project architecture follows a layered approach, separating concerns into different layers such as the application layer, domain layer, and infrastructure layer. The MediatR library is used for handling queries and commands, promoting a more decoupled and maintainable codebase.

### Acknowledgements

Thank you for the opportunity to work on this awesome task. I hope I have followed your guidelines and provided the desired solutions.

I would also like to express my appreciation for the task's requirement to implement Docker. Although I was unable to implement Docker in this particular project, I took this opportunity to learn more about Docker and its capabilities. It has been a valuable learning experience, and I look forward to applying my newfound knowledge in future projects.

Once again, thank you for the opportunity, and I am grateful for the chance to work on this task.
