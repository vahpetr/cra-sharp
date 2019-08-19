# create-react-core

Example project on asp net core 2.2, react(create react app + styled components + antd), postgres, redis, docker.
I think in real application, you need to use Identity and a separate frontend project, do not public sourcemap, and not divide the frontend into two independent parts.

## How run

1. Setup .env file (in real app this file must be ignore)
1. `docker-compose up --build`
1. `open http://localhost:5000`

## TODO

1. Enable auto reconnect on redis
1. Add [unit tests](https://docs.microsoft.com/ru-ru/aspnet/core/mvc/controllers/testing?view=aspnetcore-2.2)
1. Add [e2e tests](https://docs.microsoft.com/ru-ru/aspnet/core/test/integration-tests?view=aspnetcore-2.2)
1. [Add swagger](https://docs.microsoft.com/ru-ru/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio)
1. [Add logging with ElasticSearch, Kibana](https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker)

## Docs

1. [Other test information](https://docs.microsoft.com/ru-ru/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps)
