# CodeGenerator

Application/API used to generate various types of code files. Generated files are based off of a given database schema.


## Output File Types
- C# Model - Basic C# model (class) file.
- T-SQL Queries - T-SQL CRUD queries.
- More to come...


## Project Descriptions
- CodeGenerator - .Net Core class library. Primary container for application functionality. This library can be consumed directly for custom use cases.
- CodeGenerator.Command - .Net Core console app. Simple mechanism for accessing functionality.
- CodeGenerator.Test - MS unit test project. Contains tests for the CodeGenerator library.


## CodeGenerator.Command Configuration/Options

### ApplicationConfiguration
- CodeDirectory - Location to place the generated code files.
- SourceConnectionString - The connection string for the source database.
- ConnectionType - The platform the source database is located on (only MS SQL Server is currently supported).
- RequestFullConnectionOnExec - Whether or not the user should be prompted for full connection details on application execution (this will override any connection string set in the config file).
- RequestCatalogOnlyOnExec - Whether or not the user should be prompted for the source catalog (i.e., database name) on application execution.

### CSharpModelConfiguration
- Active - Whether or not this file generator should be used.
- ModelNameSpace - The name space to use for generated model files.
- RequestNameSpaceOnExec - Whether or not the user should be prompted for the model name space on application execution (this will override any name space set in the config file).
- AddAnnotations - Whether or not data annotations (e.q., Required, StringLength, DataType, etc.) should be added to the generated model files.
- OnlyExactMatchForAnnotations - The application will attempt to generate annotations for properties based on loose approximations by default (this applies primarily to DataType annotations). Turn this on to have annotations generated only when the application has what is considers to be 100% confidence.

### TSqlQueryConfiguration
- Active - Whether or not this file generator should be used.
- QuoteIdentifiers - Whether or not T-SQL identifiers should be quoted (e.g, [dbo].[TableName] instead of dbo.TableName).
