# AI Context: VoidCraft Launcher

## 1. Project Overview
**Project Name**: VoidCraft Launcher
**Purpose**: Custom Minecraft Launcher tailored for the VOID-CRAFT community.
**Framework**: .NET 9.0 (C#)
**UI Framework**: Avalonia UI (Cross-platform XAML)
**Key External Libraries**: 
- `CmlLib.Core` (Minecraft Launch Logic)
- `CommunityToolkit.Mvvm` (MVVM Pattern)
- `Microsoft.Identity.Client` (MSAL - Microsoft Auth)
- `System.Text.Json` (Serialization)

## 2. Directory Structure (`VoidCraftLauncher/src`)
The project follows a standard MVVM architecture.

### 📂 `Views/`
Contains the UI definitions (XAML).
- **`MainWindow.axaml`**: The single main window of the application.
  - **Structure**: Sidebar (Left) + Content Area (Right).
  - **Overlays**: Contains a `Grid` overlay for the Login Modal.
  - **Navigation**: Uses visibility toggling of UI sections based on `MainViewModel` state.

### 📂 `ViewModels/`
Application logic and state management.
- **`MainViewModel.cs`**: The core "Brain" of the UI.
  - **Responsibilities**: Navigation, Auth state, Modpack management, Download progress, Server status polling.
  - **State**: Holds `ObservableCollection<ModpackInfo>` (Library) and `BrowserResults`.
  - **Commands**: `LoginMicrosoft`, `LoginOffline`, `PlayModpack`, `InstallModpackFromBrowser`, etc.

### 📂 `Services/`
Business logic and external interactions.
- **`AuthService.cs`**: 
  - Handles Microsoft OAuth 2.0 (Azure AD) with Scope `XboxLive.signin`.
  - Handles Offline (Warez) login creating a dummy session.
  - Manages Token Cache persistence.
- **`LauncherService.cs`**: 
  - Manages `CmlLib.Core` session.
  - Handles `LaunchAsync` (Process start).
  - Loads/Saves `LauncherConfig` (JSON).
- **`ModpackInstaller.cs`**: 
  - Core logic for downloading and installing modpacks.
  - **Smart Updates**: Compares `ModpackManifestInfo` to avoid redownloading unchanged versions.
  - Handles `overrides/` copying and mod downloading from CurseForge/Modrinth APIs.
- **`CurseForgeApi.cs` / `ModrinthApi.cs`**: 
  - HTTP Clients for searching and fetching metadata from respective platforms.
- **`LogService.cs`**: 
  - Writes logs to `%Documents%/.voidcraft/launcher.log`.

### 📂 `Models/`
Data structures and DTOs.
- **`ModpackInfo`**: Represents an installed instance (Name, Path, Version, Icon).
- **`LauncherConfig`**: Global settings (Java Path, RAM, JVM Args, Optimization flags).
- **`InstanceConfig`**: Per-modpack overrides (e.g. specialized RAM settings for heavy packs).

## 3. Key Workflows

### Authentication
1. **User clicks "Přihlásit se"**: Opens `IsLoginModalVisible` overlay.
2. **Microsoft**: Calls `AuthService.LoginWithBrowserAsync`. Uses system browser for OAuth -> Returns `MSession`.
3. **Offline**: Validates username -> Calls `AuthService.LoginOffline` -> Returns Offline `MSession`.

### Modpack Installation
1. User searches in Browser (CurseForge/Modrinth).
2. `MainViewModel` creates a placeholder `ModpackInfo` in Library.
3. Background Task downloads `.zip` / `.mrpack`.
4. `ModpackInstaller` extracts and installs:
   - Resolves dependencies.
   - Downloads mods.
   - Copies overrides.
5. `installed_modpacks.json` is updated.

### Launching
1. **Check**: Is User logged in? Is Modpack valid?
2. **Update**: Checks for updates via `ModpackInstaller` (Version verification).
3. **Optimizations**: Applies JVM arguments (G1GC / ZGC) based on `LauncherConfig` or `InstanceConfig`.
4. **Process**: `LauncherService` starts `java.exe` process.
5. **Monitoring**: Redirects `STDOUT`/`STDERR` to `LogService` and console.

## 4. Specific Mechanics
- **Self-Contained**: The application is published as a self-contained executable (includes .NET Runtime).
- **Auto-Update**: Checks GitHub Releases (`venom74cz/VOID-CRAFT.EU-Launcher-remake`) on startup. Downloads new `.exe`, renames old to `.bak`, and restarts.
- **File Storage**:
  - Configuration: `launcher_config.json`
  - Library Registry: `installed_modpacks.json`
  - Instances: `Before version 1.0.3` logic mixed, currently standardizing on specific instance folders managed by `LauncherService`.

## 5. Important Constants
- **Microsoft Client ID**: `a12295b0-3505-46f1-a299-88ae9cc80174`
- **Server IP**: `mc.void-craft.eu`
- **Redirect URI**: (Default/Localhost for Desktop Apps)

## 6. Known "Tech Debt" / Notes for AI
- `MainViewModel.cs` is very large (God Object). Future refactoring should split it (e.g., `BrowserViewModel`, `LibraryViewModel`).
- Navigation is boolean-flag based (`IsHomeView`, `IsSettingsView`, etc.) rather than using a proper Routing/View injection system.
- XAML `MainWindow.axaml` contains the Login Overlay directly in the visual tree. 
- Exception handling is often generic (`catch (Exception ex)`).

---
*Created: 2026-01-05*
