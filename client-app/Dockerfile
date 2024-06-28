FROM node:latest AS build-stage
ARG QUASAR_GATEWAY_BASE_URL="http://localhost:7000"
ENV QUASAR_GATEWAY_BASE_URL=$QUASAR_GATEWAY_BASE_URL
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY ./ .
RUN npm run build

FROM nginx AS production-stage
RUN mkdir /app
COPY --from=build-stage /app/dist /app
COPY nginx.conf /etc/nginx/nginx.conf
