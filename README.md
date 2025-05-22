# Common

A useful generic C# library with unique solutions for .NET applications.

## Overview

This library provides a collection of reusable components and utilities to simplify common programming tasks:

- Apply **Action** and **Func** delegates in parallel to collections
- Use generic implementations for:
  - Databus (message bus)
  - Service Loader
  - Persistent Buffer
  - Sliding timer
- Leverage static helpers for serialization
- Implement orchestration using generic schedulers
- Apply validations to respond to incorrect input
- Extend Log4Net logging as a background process for all appenders
- Use abstractions for WPF View Models

## Project Structure

The solution contains the following projects:

- **[Common](/workspace/Common/Common)**: Core utilities and helpers
  - **[Extensions](/workspace/Common/Common/Extensions)**: Extension methods for collections and objects
  - **[Generic](/workspace/Common/Common/Generic)**: Generic implementations of common patterns
  - **[Helpers](/workspace/Common/Common/Helpers)**: Utility helper classes
  - **[Scheduling](/workspace/Common/Common/Scheduling)**: Task scheduling components
  - **[Serialization](/workspace/Common/Common/Serialization)**: JSON and other serialization utilities
  - **[Validation](/workspace/Common/Common/Validation)**: Input validation components
- **[Common.Logging](/workspace/Common/Common.Logging)**: Extended logging capabilities
- **[Common.Presentation](/workspace/Common/Common.Presentation)**: WPF-related abstractions and utilities

## Key Features

### Parallel Collection Processing

Process collections in parallel with throttling capabilities:

```csharp
// Apply an action to each item in a collection in parallel
var myCollection = new List<string>() { "item1", "item2", "item3" };
await myCollection.ApplyAction(item => {
    // Process each item in parallel
    Console.WriteLine(item);
}, throttleSize: 10);

// Apply a function to each item and collect results
var results = await myCollection.ApplyFunction(item => {
    // Transform each item in parallel
    return item.ToUpper();
}, throttleSize: 10);
```

### Generic Message Bus

Implement a publish-subscribe pattern with type safety:

```csharp
// Create a message bus
var messageBus = new DelegateMessageBus();

// Subscribe to messages of a specific type
messageBus.Subscribe<string>(message => {
    Console.WriteLine($"Received: {message}");
});

// Publish a message
messageBus.Publish("Hello, World!");
```

### Persistent Buffer

Store and process items with persistence and error handling:

```csharp
// Create a persistent buffer for string items
var buffer = new PersistentBuffer<string>("MyBuffer");

// Add items to the buffer
buffer.Add("Item to process");

// Process items in the buffer
buffer.StartProcessing(item => {
    // Process each item
    Console.WriteLine(item);
});
```

## Getting Started

To use this library in your project:

1. Reference the appropriate assemblies based on your needs
2. Import the necessary namespaces
3. Use the components as shown in the examples above

## Requirements

- .NET Framework 4.5 or higher

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! If you'd like to contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
