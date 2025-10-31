// Main game initialization
let gameEngine = null;
let hud = null;
let menuSystem = null;
let photoMode = null;
let clubSelector = null;

// Initialize game when page loads
window.addEventListener('DOMContentLoaded', () => {
    initGame();
});

function initGame() {
    // Get canvas element
    const canvas = document.getElementById('game-canvas');
    if (!canvas) {
        console.error('Canvas element not found');
        return;
    }

    // Create game engine
    gameEngine = new GameEngine();
    gameEngine.init(canvas);

    // Create UI systems
    hud = new HUD();
    menuSystem = new MenuSystem(gameEngine);
    photoMode = new PhotoMode(gameEngine);
    clubSelector = new ClubSelector(gameEngine);

    // Make accessible globally
    window.gameEngine = gameEngine;
    window.hud = hud;
    window.menuSystem = menuSystem;
    window.photoMode = photoMode;
    window.clubSelector = clubSelector;

    // Setup swing system callbacks
    gameEngine.swingSystem.swingCompleteCallback = (swingData) => {
        handleSwingComplete(swingData);
    };

    // Setup mouse click handler for swing
    canvas.addEventListener('click', (e) => {
        handleSwingInput();
    });

    // Setup keyboard handler
    window.addEventListener('keydown', (e) => {
        if (e.key === ' ' || e.key === 'Enter') {
            e.preventDefault();
            handleSwingInput();
        }
        if (e.key.toLowerCase() === 'c') {
            e.preventDefault();
            clubSelector.toggle();
        }
    });

    // Club button
    const btnClub = document.getElementById('btn-club');
    if (btnClub) btnClub.addEventListener('click', () => clubSelector.toggle());

    // Setup gamepad handler
    setupGamepadHandler();

    // Setup aim preview
    setupAimPreview();

    // Update HUD on swing state changes
    setInterval(() => {
        if (gameEngine && hud) {
            const gameState = gameEngine.getGameState();
            if (gameState) {
                const hole = gameEngine.courseSystem.getCurrentHole();
                const lieType = gameEngine.courseSystem.getTerrainType(gameState.ballPosition);
                const lie = gameEngine.physicsEngine.getLieType(lieType);
                const clubName = gameEngine.progressionSystem.getEquippedClub()?.name || 'Club';
                
                hud.update({
                    ...gameState,
                    hole: hole,
                    lie: lie,
                    wind: gameEngine.windSystem.getWindDisplay(),
                    isSwinging: gameEngine.swingSystem.isSwinging,
                    swingPower: gameEngine.swingSystem.getPowerPercentage(),
                    swingTempo: gameEngine.swingSystem.getTempoAngle(),
                    swingBackspin: gameEngine.swingSystem.backspin,
                    swingSidespin: gameEngine.swingSystem.sidespin,
                    clubName
                });
            }
        }
    }, 100);
}

function handleSwingInput() {
    if (!gameEngine || !gameEngine.swingSystem) return;

    if (gameEngine.swingSystem.swingPhase === 'idle') {
        // Start new swing
        const spinValues = hud ? hud.getSpinValues() : { backspin: 0, sidespin: 0 };
        gameEngine.swingSystem.setBackspin(spinValues.backspin);
        gameEngine.swingSystem.setSidespin(spinValues.sidespin);
        gameEngine.swingSystem.startSwing();
        
        if (hud) {
            hud.showSwingInterface();
        }
    } else if (gameEngine.swingSystem.swingPhase === 'powering') {
        // First click: set power, start timing
        gameEngine.swingSystem.handleSwingInput();
    } else if (gameEngine.swingSystem.swingPhase === 'timing') {
        // Second click: complete swing
        gameEngine.swingSystem.handleSwingInput();
    }
}

function handleSwingComplete(swingData) {
    if (!gameEngine) return;

    // Increment stroke counter
    gameEngine.strokes++;

    // Reset spin controls
    if (hud) {
        hud.resetSpinControls();
        hud.hideSwingInterface();
    }

    // Calculate swing parameters
    const hole = gameEngine.courseSystem.getCurrentHole();
    const club = gameEngine.progressionSystem.getEquippedClub();
    const ball = gameEngine.progressionSystem.getEquippedBall();
    
    // Base power from swing
    const basePower = (swingData.power / 100) * club.power;
    const powerWithBall = basePower * (ball.power / 100);
    
    // Apply tempo penalty (perfect tempo = no penalty, bad tempo = penalty)
    const tempoPenalty = 1 - (Math.abs(swingData.tempo) * 0.2);
    const finalPower = powerWithBall * tempoPenalty;
    
    // Calculate launch angle (club-dependent)
    let launchAngle = Math.PI / 3; // ~60 degrees default
    if (club.id === 'putter') {
        launchAngle = Math.PI / 6; // ~30 degrees for putter
    } else if (club.id === 'wedge') {
        launchAngle = Math.PI / 2.5; // ~72 degrees for wedge
    }
    
    // Get wind vector
    const windVector = gameEngine.windSystem.getWindVector();
    
    // Get current ball position and lie
    const ballPosition = gameEngine.ballController.getPosition();
    const lieType = gameEngine.courseSystem.getTerrainType(ballPosition);
    const lie = gameEngine.physicsEngine.getLieType(lieType);
    
    // Apply spin with club and ball modifiers
    const backspin = swingData.backspin * (club.spin / 100) * (ball.spin / 100);
    const sidespin = swingData.sidespin * (club.spin / 100) * (ball.spin / 100);
    
    // Hit the ball
    gameEngine.ballController.hitBall(
        finalPower * 0.1, // Convert to m/s (rough conversion)
        launchAngle,
        backspin,
        sidespin,
        windVector,
        lie.multiplier
    );
    
    // Update ball position callback
    gameEngine.ballController.onTrajectoryComplete = (position) => {
        gameEngine.ballPosition = position;
        
        // Check distance to pin (handled by mode update loop)
        // Mode will check and call onHoleComplete
    };
    
    // Stroke counter incremented in handleSwingComplete (before hitting ball)
}

function setupGamepadHandler() {
    let gamepadButtonPressed = false;
    
    setInterval(() => {
        const gamepads = navigator.getGamepads();
        if (gamepads && gamepads[0]) {
            const gamepad = gamepads[0];
            const buttonPressed = gamepad.buttons[0]?.pressed || false;
            
            if (buttonPressed && !gamepadButtonPressed) {
                // Button just pressed
                handleSwingInput();
                gamepadButtonPressed = true;
            } else if (!buttonPressed) {
                gamepadButtonPressed = false;
            }
        }
    }, 50);
}

function setupAimPreview() {
    const canvas = document.getElementById('aim-preview');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    
    function drawAimPreview() {
        if (!gameEngine || !hud) return;
        
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        
        // Draw simplified top-down view of course
        ctx.strokeStyle = '#22c55e';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(50, canvas.height - 50);
        ctx.lineTo(canvas.width - 50, 50);
        ctx.stroke();
        
        // Draw ball position
        ctx.fillStyle = '#ffffff';
        ctx.beginPath();
        ctx.arc(50, canvas.height - 50, 5, 0, Math.PI * 2);
        ctx.fill();
        
        // Draw pin
        ctx.fillStyle = '#ff0000';
        ctx.beginPath();
        ctx.arc(canvas.width - 50, 50, 8, 0, Math.PI * 2);
        ctx.fill();
        
        // Draw trajectory preview (simplified)
        if (gameEngine.swingSystem.swingPhase === 'powering' || 
            gameEngine.swingSystem.swingPhase === 'timing') {
            const power = gameEngine.swingSystem.getPowerPercentage() / 100;
            ctx.strokeStyle = 'rgba(255, 255, 255, 0.5)';
            ctx.setLineDash([5, 5]);
            ctx.beginPath();
            ctx.moveTo(50, canvas.height - 50);
            const targetX = 50 + (canvas.width - 100) * power;
            const targetY = canvas.height - 50 - (canvas.height - 100) * power;
            ctx.lineTo(targetX, targetY);
            ctx.stroke();
            ctx.setLineDash([]);
        }
    }
    
    // Update aim preview periodically
    setInterval(drawAimPreview, 100);
}

// Error handling
window.addEventListener('error', (e) => {
    console.error('Game error:', e.error);
});

// Show loading screen initially
window.addEventListener('load', () => {
    const loadingScreen = document.getElementById('loading-screen');
    if (loadingScreen) {
        setTimeout(() => {
            loadingScreen.classList.add('hidden');
        }, 1000);
    }
});

