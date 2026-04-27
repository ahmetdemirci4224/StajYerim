# Build Stage
FROM node:20-alpine as build
WORKDIR /app

# Bağımlılıkları kopyala ve kur
COPY package*.json ./
RUN npm install

# Kaynak kodları kopyala
COPY . .

# Production için build al (Vite)
RUN npm run build

# Serve Stage (Nginx)
FROM nginx:alpine
# Build edilen dosyaları Nginx'in yayın klasörüne kopyala
COPY --from=build /app/dist /usr/share/nginx/html

# Özel Nginx konfigürasyonunu kopyala
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
