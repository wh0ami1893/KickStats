# KickStats

A Blazor-based web application that allows users—such as companies or teams—to track and manage their table football matches. Keep a history of games, scores, and players to fuel your competitive spirit!

## 🚀 Features

- 📊 Match tracking with player names and scores  
- 🏆 Leaderboards and match history  
- 🕹️ Easy-to-use interface for quick game entry  
- 🧑‍🤝‍🧑 Designed for teams, departments, or entire companies  

## 🛠️ Tech Stack

- **Frontend & Backend**: Blazor (ASP.NET Core)  
- **Database**: SQLite
- **Hosting**: Self-hosted Docker container

## 📦 Getting Started
### Docker Container
To run the latest version in a Docker container you can pull the image from github:
```bash
# Pull Image
docker pull ghcr.io/wh0ami1893/kickstats:latest

# Run Container
docker run -d -p 8080:8080 kickstats:latest
```
### Build from Source

```bash
# Clone the repository
git clone git@github.com:wh0ami1893/KickStats.git

# Navigate into the project folder
cd KickStats/KickStats

# Run the application
dotnet run
Then open your browser and go to https://localhost:5001 or whatever port is configured.
```
