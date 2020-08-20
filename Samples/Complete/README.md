# NeverFoundry.Wiki.Samples.Simple
This project is a full-featured example project which uses [Docker](https://www.docker.com) to
launch a database, search client, monitoring tools, and an MVC wiki site. It depends on the
`docker-compose-mvc` project.

The sample is intended as a complete example of a working wiki MVC site. It uses
[Marten](https://martendb.io) as its `DataStore`, has a full implementation of [ASP.NET Core
Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) for user
management with the same [PostgreSQL](https://www.postgresql.org) database as its backing store, and
uses [ElasticSearch](https://www.elastic.co/elasticsearch) as its search client. Logging is handled
by [Serilog](https://serilog.net), and both [pgAdmin](https://www.pgadmin.org) and
[Kibana](https://www.elastic.co/kibana) are loaded in the container for monitoring.

## Customization
Needless to say, although this sample is a far more complete starting point for developing your own
wiki site than the simpler samples, it also requires a great deal more customization to ensure that
sample and development configuration values are replaced by your own information.

A complete guide to replacing sample values with your own information is impractical, since this
sample has been so fully developed into a complete, working example.

It is advisable to start your own project from scratch, and merely use the sample as a guide when
implementing various features. This will help you to avoid accidentally leaving any
sample/development values in your own code.

If you do decide to clone the sample instead, and modify it for your own needs, it is advisable to
go through the entire project, file by file, and make certain that you have replaced all the
constants, settings, and configurations with your own values.