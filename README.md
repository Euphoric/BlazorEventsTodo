# BlazorEventsTodo
Sample application with Blazor for frontent and ASP.NET core with Event Sourcing and EventSourceDB for backend. In .NET 5 and C#9

## Running

Run `docker-compose.runlocal.yml` using

    docker-compose -f .\docker-compose.runlocal.yml up
    
Then open https://localhost:2111 for EventStore admin page or http://localhost:8010 for application page.

## Development

Tested to run against EventSourceDB 20.6.1

Default configuration is for EventSourceDB running in `--insecure` mode. To run in secured mode with certificates, update the connection string in `appsettings.Development.json` file.

Then open the solution using favorite C# editor.
