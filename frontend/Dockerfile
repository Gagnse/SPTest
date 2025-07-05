# Version prod with SPA routing support

# Étape 1 : build avec Node
FROM node:20-alpine AS build

WORKDIR /app
COPY . .

# 👇 Copie la variable d'environnement dans Vite
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

RUN npm install
RUN npm run build

# Étape 2 : serveur statique avec Nginx + configuration SPA
FROM nginx:alpine

# Copy built files
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx configuration for SPA routing support
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80