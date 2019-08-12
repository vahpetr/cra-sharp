# test and build backend
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS builder-backend
WORKDIR /app
COPY backend/*.csproj ./
RUN dotnet restore
COPY ./backend ./
RUN dotnet publish -c Release -o out

# test and build ui-public
FROM node:10.16.1-alpine AS builder-ui-public
WORKDIR /usr/src/app
ARG REACT_APP_API_HOST
ENV CI=true REACT_APP_API_HOST=$REACT_APP_API_HOST
ADD ui-public/package.json ui-public/package-lock.json ./
RUN npm install
COPY ui-public/public public
COPY ui-public/src src
RUN npm run test
RUN npm run build
RUN sed -i -e 's/\/static/\/public\/static/g' build/index.html
RUN for f in `find build -type f -name '*.html' -o -name '*.ico' -o -name '*.css' -o -name '*.js' -o -name '*.json'`; \
    do gzip -9c "$f">"$f.gz"; \
    done
RUN find build -name "*.map" -delete

# test and build ui-private
FROM node:10.16.1-alpine AS builder-ui-private
WORKDIR /usr/src/app
ARG REACT_APP_API_HOST
ENV CI=true REACT_APP_API_HOST=$REACT_APP_API_HOST
ADD ui-private/package.json ui-private/package-lock.json ./
RUN npm install
COPY ui-private/public public
COPY ui-private/src src
RUN npm run test
RUN npm run build
RUN sed -i -e 's/\/static/\/private\/static/g' build/index.html
RUN for f in `find build -type f -name '*.html' -o -name '*.ico' -o -name '*.css' -o -name '*.js' -o -name '*.json'`; \
    do gzip -9c "$f">"$f.gz"; \
    done
RUN find build -name "*.map" -delete

# build result image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
ENV ASPNETCORE_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production
WORKDIR /app
COPY --from=builder-backend /app/out .
COPY --from=builder-ui-public /usr/src/app/build ./wwwroot/public
COPY --from=builder-ui-private /usr/src/app/build ./wwwroot/private
ENTRYPOINT ["dotnet", "Backend.dll"]
