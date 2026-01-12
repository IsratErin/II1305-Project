# Boom or Bust

![Unity Version](https://img.shields.io/badge/Unity-2021.3.23f1-black?logo=unity)
![PlayFab](https://img.shields.io/badge/Backend-PlayFab-blue)
![License](https://img.shields.io/badge/License-MIT-green)

> A multiplayer tower defense game that combines classic Minesweeper mechanics with strategic tower defense gameplay.

**Boom or Bust** is an innovative strategy game developed as part of KTH course II1305. Players compete in real-time battles where they must strategically place towers and mines, detect opponent's traps using Minesweeper mechanics, and launch tactical troop assaults to destroy the enemy city.

---

## Table of Contents

- [Features](#-features)
- [Gameplay](#-gameplay)
- [Technical Stack](#-technical-stack)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Project Structure](#-project-structure)
- [How to Play](#-how-to-play)
- [Architecture](#-architecture)
- [Contributing](#-contributing)
- [License](#-license)

---

## Features

### Core Gameplay

- **Hybrid Minesweeper-Tower Defense Mechanics** - Unique combination of puzzle-solving and real-time strategy
- **Multi-Phase Battle System** - Setup → Mine Detection → Combat flow
- **4 Tower Types** - Defensive structures with varying range, damage, and fire rate
- **4 Troop Types** - Diverse units including Archer, Tank, Wizard, and Naruto Runner (melee/ranged)
- **Strategic Path Drawing** - Players draw custom attack routes through safe squares
- **Real-Time Combat** - Towers auto-target and engage approaching enemy troops

### Multiplayer & Progression

- **PlayFab Matchmaking** - Three queue types:
  - **Normal Mode** - Casual matches for practice
  - **Competitive Mode** - Ranked play with trophy gains/losses (±30 per match)
  - **Friendly Battle** - Private matches with lobby codes
- **Global Leaderboard** - Track your ranking against all players
- **Trophy System** - Competitive ranking based on wins and losses
- **Match History** - View your past game statistics
- **User Profiles** - Persistent accounts with stats (Trophies, Wins, Losses)

### Technical Features

- **Authentication System** - Email/password login with PlayFab
- **Unity Lobby Services** - Private match creation with shareable codes
- **Real-Time Networking** - TCP socket communication for game state synchronization
- **Responsive UI** - Built with Unity's uGUI and TextMesh Pro
- **Audio System** - Background music tracks and sound effects

---

## Gameplay

### Game Flow

```
┌─────────────┐    ┌──────────────┐    ┌────────────────┐    ┌─────────────┐
│  Base Phase │ -> │  Mine Phase  │ -> │ Minesweeper    │ -> │ Battle Phase│
│             │    │              │    │ Phase          │    │             │
│ Place       │    │ Place Mines  │    │ Detect Enemy   │    │ Deploy      │
│ Towers      │    │ Strategically│    │ Mines (Limited │    │ Troops &    │
│             │    │              │    │ Clicks)        │    │ Attack      │
└─────────────┘    └──────────────┘    └────────────────┘    └─────────────┘
```

### Victory Conditions

- **Destroy the Enemy City** - Reduce opponent's city health to zero
- **Defend Your City** - Prevent enemy troops from destroying your city

---

## Technical Stack

| Component         | Technology                         |
| ----------------- | ---------------------------------- |
| **Game Engine**   | Unity 2021.3.23f1 (LTS)            |
| **Backend**       | PlayFab (Auth, Matchmaking, Stats) |
| **Lobby System**  | Unity Lobby Services 1.0.3         |
| **Networking**    | Custom TCP Socket (Port 11000)     |
| **Input System**  | Unity New Input System 1.5.1       |
| **UI Framework**  | Unity uGUI + TextMesh Pro 3.0.6    |
| **Serialization** | Newtonsoft.Json 3.2.0              |
| **Graphics**      | 2D Feature Set                     |

---

## Prerequisites

Before you begin, ensure you have the following installed:

- **Unity Hub** (latest version)
- **Unity Editor 2021.3.23f1 LTS** (required version)
- **.NET Framework 4.x** (included with Unity)
- **PlayFab Account** - [Sign up here](https://playfab.com/)
- **Unity Account** - For Lobby Services

---

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/II1305-Project.git
cd II1305-Project
```

### 2. Open in Unity

1. Launch **Unity Hub**
2. Click **Open** and select the project directory
3. Unity will automatically import assets and resolve package dependencies

### 3. Backend Server Setup

The game requires a TCP server for game state synchronization:

```bash
# If you have a separate server repository
git clone <server-repository-url>
cd server
# Follow server-specific setup instructions
```

Default server configuration: `localhost:11000`

---

## ⚙️ Configuration

### PlayFab Setup

1. **Create a PlayFab Title**

   - Go to [PlayFab Dashboard](https://developer.playfab.com/)
   - Create a new title or use existing Title ID: `8E20E`

2. **Configure in Unity**
   - Navigate to `Window > PlayFab > Editor Extensions > Settings`
   - Enter your Title ID
   - Configure authentication settings

### Unity Lobby Services

1. **Enable Unity Gaming Services**

   - Open `Edit > Project Settings > Services`
   - Link your Unity project
   - Enable **Lobby** service

2. **Configure Lobby Settings**
   - Lobby IDs are managed automatically through the matchmaking system

### Network Configuration

Edit server connection settings in `GameManager.cs`:

```csharp
private const string SERVER_IP = "localhost";
private const int SERVER_PORT = 11000;
```

---

## 📁 Project Structure

```
Assets/
├── Scenes/
│   ├── LoginScene.unity          # User authentication & main menu
│   ├── GameBoardScene.unity      # Main gameplay scene
│   └── Win screen.unity          # Post-game results
│
├── Scripts/
│   ├── GameManager.cs            # Core game orchestrator (560 lines)
│   ├── Login/                    # PlayFab authentication system
│   │   ├── LoginPanelScript.cs
│   │   └── SignupPanelScript.cs
│   ├── Matchmaking/              # Matchmaking & lobby management
│   │   ├── MatchMakingSystem.cs
│   │   └── LobbyManager.cs
│   ├── LeaederBoard/             # Global ranking system
│   ├── Profile/                  # User statistics & profiles
│   ├── Tower/                    # Tower behavior & targeting AI
│   ├── Troop/                    # Troop AI, movement, combat
│   ├── Bullet/                   # Projectile physics
│   ├── Mine/                     # Mine explosion mechanics
│   ├── City/                     # Player city (win condition)
│   ├── Base/                     # Defensive base squares
│   ├── Minefield/                # Minesweeper grid logic
│   ├── Safe/                     # Path squares & troop spawning
│   └── Utility/                  # Helpers, data models, JSON
│
├── Prefabs/
│   ├── Towers/                   # Tower0, Tower1, Tower2, Tower3
│   ├── Troops/                   # Archer, Tank, Wizard, Troop
│   ├── Bullets/                  # Projectile prefabs
│   ├── Mines/                    # Mine prefabs
│   ├── Squares/                  # Grid square prefabs
│   └── City/                     # City prefabs
│
├── Materials/                    # Visual materials for game states
├── Music Tracks/                 # Background audio
├── Sprites/                      # 2D textures & UI elements
└── PlayFabEditorExtensions/      # PlayFab SDK integration
```

### Key Scripts

| Script                                                                  | Purpose                                                     |
| ----------------------------------------------------------------------- | ----------------------------------------------------------- |
| [GameManager.cs](Assets/Scripts/GameManager.cs)                         | Central game state controller, board generation, networking |
| [LoginPanelScript.cs](Assets/Scripts/Login/LoginPanelScript.cs)         | PlayFab authentication (login/register/password reset)      |
| [MatchMakingSystem.cs](Assets/Scripts/Matchmaking/MatchMakingSystem.cs) | Queue selection, matchmaking tickets, player matching       |
| [Tower.cs](Assets/Scripts/Tower/Tower.cs)                               | Tower targeting logic, auto-shooting, bullet instantiation  |
| [Troop.cs](Assets/Scripts/Troop/Troop.cs)                               | Troop AI, waypoint navigation, combat behavior              |
| [Minefield.cs](Assets/Scripts/Minefield/Minefield.cs)                   | Minesweeper mechanics, mine reveal, click detection         |
| [Safe.cs](Assets/Scripts/Safe/Safe.cs)                                  | Path drawing system, troop spawning logic                   |
| [LeaderBoardPanel.cs](Assets/Scripts/LeaederBoard/LeaderBoardPanel.cs)  | Global leaderboard display with pagination                  |

---

## How to Play

### Starting a Match

1. **Login/Register** - Create an account or sign in with existing credentials
2. **Select Queue Type**:
   - **Normal** - Practice matches
   - **Competitive** - Earn/lose 30 trophies per match
   - **Friendly Battle** - Create private lobby with code
3. **Wait for Matchmaking** - System finds an opponent

### Game Phases

#### Phase 1: Base Setup

- Click on your **base squares** (bottom area)
- Select and place **towers** to defend your city
- Each tower has unique stats (range, damage, fire rate)

#### Phase 2: Mine Placement

- Click on **minefield squares** (middle area)
- Place mines strategically to trap opponents
- Mines explode when enemy troops step on them

#### Phase 3: Minesweeper

- Limited clicks to detect opponent's mines
- Numbers indicate surrounding mine count (classic Minesweeper)
- Use information to plan safe attack paths

#### Phase 4: Battle

- Draw paths through safe squares using mouse
- Deploy troops along your drawn paths
- Troops automatically move and attack
- Defend against opponent's troops with your towers

---

## 🏗 Architecture

### Design Patterns

- **Singleton Pattern** - GameManager for global state access
- **Component-Based** - Health, movement, combat as separate components
- **Observer Pattern** - Event-driven UI updates
- **Factory Pattern** - Prefab instantiation for troops, towers, bullets

### Networking Architecture

```
┌──────────────┐         TCP Socket           ┌──────────────┐
│   Client 1   │ <──────────────────────────> │   Server     │
│  (Player A)  │      Port 11000              │  (Game State)│
└──────────────┘                              └──────────────┘
                                                      ↕
┌──────────────┐                              ┌──────────────┐
│   Client 2   │ <────────────────────────────│   PlayFab    │
│  (Player B)  │    Auth, Matchmaking, Stats  │   Backend    │
└──────────────┘                              └──────────────┘
```

### Game State Management

- **State Machine** - Manages game phases (Base → Mine → Minesweeper → Battle)
- **Turn Timer** - 30-second countdown per phase
- **Synchronization** - TCP messages exchange board states between players

---

---
