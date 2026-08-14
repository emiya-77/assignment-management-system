FROM node:22-alpine

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

ENV NEXT_PUBLIC_API_URL=http://localhost:5251/api

RUN npm run build

EXPOSE 3000

CMD ["npm", "start"]