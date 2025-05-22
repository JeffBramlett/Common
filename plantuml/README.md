# Common Library PlantUML Diagrams

This directory contains PlantUML diagrams that document the structure and behavior of the Common library.

## Available Diagrams

### Package Diagram
- `package_diagram.puml`: High-level overview of the packages and components in the Common library

### Class Diagrams
- `generic_class_diagram.puml`: Class diagram for the Generic namespace
- `scheduling_class_diagram.puml`: Class diagram for the Scheduling namespace
- `serialization_class_diagram.puml`: Class diagram for the Serialization namespace
- `extensions_class_diagram.puml`: Class diagram for the Extensions namespace

### Sequence Diagrams
- `list_extensions_sequence.puml`: Sequence diagram for the ListExtensions.ApplyAction method
- `json_helpers_sequence.puml`: Sequence diagram for the JsonHelpers serialization and deserialization processes

## Viewing the Diagrams

To view these diagrams, you need a PlantUML renderer. Options include:

1. **Online PlantUML Editor**: Copy the content of any .puml file and paste it into the [PlantUML Online Editor](http://www.plantuml.com/plantuml/uml/)

2. **VS Code Extension**: Install the "PlantUML" extension for Visual Studio Code

3. **Command Line**: Use the PlantUML JAR file to generate images:
   ```
   java -jar plantuml.jar filename.puml
   ```

4. **IntelliJ IDEA**: Install the "PlantUML integration" plugin

## Diagram Conventions

- Interfaces are prefixed with "I" (e.g., IScheduler)
- Abstract classes are shown in italics
- Static methods are marked with {static}
- Relationships:
  - Inheritance: Solid line with empty triangle
  - Implementation: Dashed line with empty triangle
  - Association: Solid line with arrow
  - Dependency: Dashed line with arrow