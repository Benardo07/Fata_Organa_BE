# Use the Node.js image for building and running Next.js
FROM node:18 AS build-env
WORKDIR /app

# Copy package.json and install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the code and build the application
COPY . .
RUN npm run build

# Use a lighter image to serve the app
FROM node:18-alpine
WORKDIR /app
COPY --from=build-env /app ./

# Set the environment and start the Next.js server
ENV NODE_ENV=production
EXPOSE 3000
CMD ["npm", "start"]
