class CourseMode {
    constructor(gameEngine, holeCount = 18) {
        this.gameEngine = gameEngine;
        this.holeCount = holeCount;
        this.currentHoleIndex = 0;
        this.strokes = 0;
        this.totalStrokes = 0;
        this.score = 0;
        this.parTotal = 0;
        this.isCheckingPosition = false;
    }

    init() {
        // Load course
        this.gameEngine.courseSystem.loadCourse('main', this.holeCount);
        this.currentHoleIndex = 0;
        this.totalStrokes = 0;
        this.strokes = 0;
        this.score = 0;
        this.parTotal = this.gameEngine.courseSystem.holes.reduce((sum, hole) => sum + hole.par, 0);
        
        // Start first hole
        this.gameEngine.strokes = 0;
        this.gameEngine.startHole();
    }

    update(deltaTime) {
        // Mode-specific updates
        const gameState = this.gameEngine.getGameState();
        this.strokes = this.gameEngine.strokes || 0;
        
        // Update HUD with current game state
        if (window.hud) {
            const hole = this.gameEngine.courseSystem.getCurrentHole();
            if (gameState.ballPosition && hole) {
                const lieType = this.gameEngine.courseSystem.getTerrainType(gameState.ballPosition);
                const lie = this.gameEngine.physicsEngine.getLieType(lieType);
                
                window.hud.update({
                    ...gameState,
                    hole: hole,
                    lie: lie,
                    wind: this.gameEngine.windSystem.getWindDisplay(),
                    strokes: this.strokes,
                    isSwinging: this.gameEngine.swingSystem.isSwinging,
                    swingPower: this.gameEngine.swingSystem.getPowerPercentage(),
                    swingTempo: this.gameEngine.swingSystem.getTempoAngle(),
                    swingBackspin: this.gameEngine.swingSystem.backspin,
                    swingSidespin: this.gameEngine.swingSystem.sidespin
                });
            }
        }
        
        // Check if ball has stopped moving
        if (!this.gameEngine.ballController.isMoving && 
            this.gameEngine.swingSystem.swingPhase === 'idle' &&
            !this.isCheckingPosition) {
            // Ball has stopped, ready for next shot
            setTimeout(() => this.checkBallPosition(), 500);
        }
    }

    checkBallPosition() {
        if (this.isCheckingPosition) return;
        this.isCheckingPosition = true;
        
        const hole = this.gameEngine.courseSystem.getCurrentHole();
        const ballPos = this.gameEngine.ballController.getPosition();
        const distanceToPin = ballPos.distanceTo(hole.pin);
        
        // Check if ball is in hole (0.5 meter tolerance)
        if (distanceToPin < 0.5) {
            setTimeout(() => {
                this.onHoleComplete();
                this.isCheckingPosition = false;
            }, 500);
        } else {
            this.isCheckingPosition = false;
        }
    }

    onHoleComplete() {
        const hole = this.gameEngine.courseSystem.getCurrentHole();
        const par = hole.par;
        const strokes = this.gameEngine.strokes || this.strokes;
        
        // Calculate score relative to par
        const scoreDiff = strokes - par;
        let scoreText = '';
        if (scoreDiff === -2) scoreText = 'EAGLE!';
        else if (scoreDiff === -1) scoreText = 'BIRDIE!';
        else if (scoreDiff === 0) scoreText = 'PAR';
        else if (scoreDiff === 1) scoreText = 'BOGEY';
        else if (scoreDiff === 2) scoreText = 'DOUBLE BOGEY';
        else scoreText = `+${scoreDiff}`;
        
        this.totalStrokes += strokes;
        this.score += scoreDiff;
        
        // Alert result
        alert(`Hole ${hole.number} Complete!\n${scoreText} (${strokes} strokes, Par ${par})`);
        
        // Move to next hole or finish course
        if (this.gameEngine.courseSystem.nextHole()) {
            this.gameEngine.strokes = 0;
            this.strokes = 0;
            this.gameEngine.startHole();
        } else {
            this.onCourseComplete();
        }
    }

    onCourseComplete() {
        const totalPar = this.parTotal;
        const totalStrokes = this.totalStrokes;
        const finalScore = totalStrokes - totalPar;
        
        let resultText = `Course Complete!\n\n`;
        resultText += `Total Strokes: ${totalStrokes}\n`;
        resultText += `Par: ${totalPar}\n`;
        resultText += `Final Score: ${finalScore > 0 ? '+' : ''}${finalScore}\n\n`;
        
        if (finalScore < 0) {
            resultText += 'Excellent round!';
        } else if (finalScore === 0) {
            resultText += 'Par round!';
        } else {
            resultText += 'Good effort!';
        }
        
        alert(resultText);
        
        // Check for unlocks
        if (finalScore < 0) {
            this.gameEngine.progressionSystem.checkUnlockConditions('score_under_par');
        }
        
        // Return to menu
        if (window.menuSystem) {
            window.menuSystem.quitToMenu();
        }
    }

    restart() {
        this.init();
    }

    onSwingStart() {
        // Handle swing start
        const spinValues = window.hud ? window.hud.getSpinValues() : { backspin: 0, sidespin: 0 };
        this.gameEngine.swingSystem.setBackspin(spinValues.backspin);
        this.gameEngine.swingSystem.setSidespin(spinValues.sidespin);
    }

    onSwingComplete(swingData) {
        // Swing completion handled by GameEngine
        this.strokes++;
        
        // Reset spin controls
        if (window.hud) {
            window.hud.resetSpinControls();
        }
    }
}

