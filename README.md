# Crypto Dashboard and Arbitrage Recommendation App

This application is a comprehensive dashboard for viewing detailed information on multiple cryptocurrencies, including chart data, statistics, and arbitrage recommendations. The backend constructs a graph of exchange rates between various coins and finds the top 3 arbitrage opportunities based on profitability, using Depth-First Search (DFS) with a maximum depth of 3.

The app fetches cryptocurrency data, prices, and historical chart data using the CoinGecko API.

## Features
- **Crypto Dashboard**: Displays details and statistics for several cryptocurrencies.
- **Chart Data**: Provides historical chart data for selected cryptocurrencies.
- **Arbitrage Suggestions**: Displays top 3 arbitrage opportunities based on profit, analyzing coin-to-coin trades through DFS within a graph structure.

## Tech Stack
- **Frontend**: Built with Next.js, Tailwind CSS, and shadcn for UI styling.
- **Backend**: Developed using .NET for robust API handling.

## Getting Started

### Prerequisites
- Docker and Docker Compose (if you want to run with Docker)
- .NET SDK (if you want to run the backend locally)
- Node.js and npm (for running the frontend locally)

### Clone the Repository
Clone this repository to your local machine:
```bash
git clone https://github.com/Benardo07/Fata_Organa_Test.git
cd Fata_Organa_Test
```

## Running the Application

### Option 1: Run with Docker
You can easily run both the frontend and backend services using Docker Compose.
```bash
docker-compose up --build
```

### Option 2: Run Locally without Docker

1. **Backend**:
   - Navigate to the backend folder:
     ```bash
     cd backend
     ```
   - Install dependencies (if necessary):
     ```bash
     dotnet restore
     ```
   - Run the backend:
     ```bash
     dotnet run
     ```

2. **Frontend**:
   - Open a new terminal and navigate to the frontend folder:
     ```bash
     cd frontend
     ```
   - Install dependencies:
     ```bash
     npm install
     ```
   - Start the frontend development server:
     ```bash
     npm run dev
     ```

   The frontend should now be available at `http://localhost:3000`.

## Creator
- **Name**: Benardo
- **Email**: benardo188@gmail.com
