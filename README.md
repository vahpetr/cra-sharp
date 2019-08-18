# create-react-core

Example project on asp net core 2.2, react(create react app + styled components + antd), postgres, redis, docker.
I think in real application, you need to use Identity and a separate frontend project, do not public sourcemap, and not divide the frontend into two independent parts.

## How run

1. Setup .env file (in real app this file must be ignore)
1. `docker-compose up --build`
1. `open http://localhost:5000`

## TODO

1. For load html need add [Cookie auth](https://oloshcoder.com/2018/05/21/jwt-token-with-cookie-authentication-in-asp-net-core/)
1. Enable auto reconnect on redis
1. Add more test
1. [Add swagger](https://docs.microsoft.com/ru-ru/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio)
1. [Add logging with ElasticSearch, Kibana](https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker)
