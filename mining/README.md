# Mining Game - WebGL Deployment

This folder hosts the Unity WebGL build for the Mining Game.

## Build from Unity
1. Open the Unity project at `UnityProject/` in Unity Hub/Editor
2. File → Build Settings → Select Platform: WebGL → Switch Platform
3. Add your scene (e.g., `Assets/_Game/Scenes/MiningGame.unity`) to Scenes In Build
4. Click Build, choose output folder: this repository's `mining/`
5. Unity will create files like:
   - `index.html`
   - `Build/xxx.loader.js`, `xxx.framework.js`, `xxx.data`, `xxx.wasm`
   - `TemplateData/` (if applicable)

After building, you can open `mining/index.html` directly or serve the site.

## Local testing
If you use a local server, serve the repo root so the link from the main page works:

- Root page: `/index.html`
- Mining game: `/mining/index.html`

## Notes
- If you rebuild, Unity will overwrite files in `mining/`
- Keep large build files out of version control if desired by updating `.gitignore`

