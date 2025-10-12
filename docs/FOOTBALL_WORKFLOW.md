# Football Workflow

## Overview
The Football workflow fetches football scores for a specified club and season, stores them in a SQL database, and sends them via console output and email.

## Components

### 1. Daisy.Workflows.Football
- Core workflow project that manages the Football workflow execution

### 2. Daisy.Receivers.FootballEvent
- Event-based receiver that waits for club name events from the Starter workflow

### 3. Daisy.Abilities.Football
- **FootballService**: Fetches football scores from an external API (falls back to mock data)
- **GetFootballScoresPath**: Processes the club name and retrieves scores
- Path traversal order: 60

### 4. Daisy.Abilities.DatabaseStorage
- **DatabaseService**: Stores football scores in SQL database (falls back to mock storage)
- **StoreFootballScoresPath**: Stores the retrieved scores
- Path traversal order: 65

### 5. Daisy.Transmitters.Email
- Sends football scores via email (falls back to mock email if SMTP not configured)

## Usage

### Basic Usage
1. Run the application: `dotnet run` from the `src/Daisy` directory
2. Type: `Football: <club_name>` (e.g., `Football: Manchester United`)
3. The workflow will:
   - Fetch scores for the club
   - Store them in the database
   - Display them in the console
   - Send them via email

### Example
```
Football: Barcelona
```

Output will show:
- Mock scores for Barcelona (Season 2024)
- Database storage confirmation
- Email sending confirmation

## Configuration

### API Configuration (appconfig.json)
```json
"Football": {
  "Url": "https://api.football-data.org/v4/",
  "ApiKey": "YOUR_FOOTBALL_API_KEY"
}
```

### Database Configuration (appconfig.json)
```json
"Database": {
  "ConnectionString": "Server=your_server;Database=your_db;..."
}
```

### Email Configuration (Environment Variables)
- `SMTP_HOST`: SMTP server hostname
- `SMTP_PORT`: SMTP server port (default: 587)
- `SMTP_USERNAME`: SMTP username
- `SMTP_PASSWORD`: SMTP password
- `EMAIL_FROM`: Sender email address (default: noreply@daisy.com)
- `EMAIL_TO`: Recipient email address (default: user@example.com)

## Workflow Flow

1. User inputs: `Football: <club_name>`
2. Starter workflow triggers Football workflow
3. FootballEvent receiver receives the event
4. GetFootballScoresPath fetches scores (PathTraverseOrder: 60)
5. StoreFootballScoresPath stores in database (PathTraverseOrder: 65)
6. Console transmitter displays output
7. Email transmitter sends email

## Mock Mode
When API keys, database connection string, or SMTP settings are not configured, the workflow operates in mock mode:
- **Football API**: Returns mock scores for any club
- **Database**: Logs storage to console instead of database
- **Email**: Displays email content in console instead of sending

## Testing
Unit tests are available in:
- `tests/Daisy.Tests.Abilities/Daisy.Tests.Abilities.Football/`
- `tests/Daisy.Tests.Abilities/Daisy.Tests.Abilities.DatabaseStorage/`

Run tests with: `dotnet test`
