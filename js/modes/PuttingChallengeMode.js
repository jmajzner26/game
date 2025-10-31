class PuttingChallengeMode {
    constructor(gameEngine) {
        this.gameEngine = gameEngine;
        this.challenges = [];
        this.currentChallenge = 0;
        this.strokes = 0;
        this.attemptsRemaining = 3;
    }

    init() {
        // Create putting challenges
        this.createChallenges();
        this.currentChallenge = 0;
        this.attemptsRemaining = 3;
        this.loadChallenge(0);
    }

    createChallenges() {
        // Create different putting challenges at various distances and slopes
        this.challenges = [
            { distance: 10, slope: 0.02, par: 2 },
            { distance: 15, slope: 0.03, par: 2 },
            { distance: 20, slope: 0.04, par: 3 },
            { distance: 25, slope: 0.05, par: 3 },
            { distance: 30, slope: 0.06, par: 3 }
        ];
    }

    loadChallenge(index) {
        if (index >= this.challenges.length) {
            this.onChallengeComplete();
            return;
        }
        
        const challenge = this.challenges[index];
        
        // Create green with slope
        this.createSlopedGreen(challenge.distance, challenge.slope);
        
        // Set ball position at start
        this.gameEngine.ballController.setPosition(new THREE.Vector3(0, 0, 0));
        
        // Set pin position
        const pinPos = new THREE.Vector3(0, challenge.slope * challenge.distance, challenge.distance);
        this.createPin(pinPos);
        
        this.strokes = 0;
        this.attemptsRemaining = 3;
    }

    createSlopedGreen(distance, slope) {
        // Create green mesh with slope
        const greenGeometry = new THREE.PlaneGeometry(50, distance + 10, 50, 50);
        
        // Adjust vertices for slope
        const positions = greenGeometry.attributes.position;
        for (let i = 0; i < positions.count; i++) {
            const y = positions.getY(i);
            positions.setY(i, y * slope);
        }
        positions.needsUpdate = true;
        
        const greenMaterial = new THREE.MeshStandardMaterial({
            color: 0x90EE90,
            roughness: 0.3
        });
        
        const green = new THREE.Mesh(greenGeometry, greenMaterial);
        green.rotation.x = -Math.PI / 2;
        green.position.set(0, 0, distance / 2);
        green.receiveShadow = true;
        this.gameEngine.scene.add(green);
    }

    createPin(position) {
        const pinGeometry = new THREE.CylinderGeometry(0.05, 0.05, 3, 8);
        const pinMaterial = new THREE.MeshStandardMaterial({ color: 0xffffff });
        const pin = new THREE.Mesh(pinGeometry, pinMaterial);
        pin.position.copy(position);
        pin.position.y = 1.5;
        this.gameEngine.scene.add(pin);
    }

    update(deltaTime) {
        const challenge = this.challenges[this.currentChallenge];
        
        if (window.hud) {
            window.hud.update({
                hole: { number: `Challenge ${this.currentChallenge + 1}`, par: challenge.par },
                distance: `${challenge.distance} yds`,
                lie: { name: 'Green' },
                wind: 'Calm',
                strokes: this.strokes,
                isSwinging: this.gameEngine.swingSystem.isSwinging,
                swingPower: this.gameEngine.swingSystem.getPowerPercentage(),
                swingTempo: this.gameEngine.swingSystem.getTempoAngle()
            });
        }
        
        // Check if ball stopped
        if (!this.gameEngine.ballController.isMoving && this.gameEngine.swingSystem.swingPhase === 'idle') {
            this.checkBallPosition();
        }
    }

    checkBallPosition() {
        const challenge = this.challenges[this.currentChallenge];
        const ballPos = this.gameEngine.ballController.getPosition();
        const pinPos = new THREE.Vector3(0, challenge.slope * challenge.distance, challenge.distance);
        const distanceToPin = ballPos.distanceTo(pinPos);
        
        if (distanceToPin < 0.5) {
            // In the hole
            this.onHoleComplete();
        } else if (distanceToPin > 10) {
            // Too far, attempt failed
            this.attemptsRemaining--;
            if (this.attemptsRemaining > 0) {
                this.resetBall();
            } else {
                this.onChallengeFailed();
            }
        } else {
            // Close, ready for next putt
            this.strokes++;
        }
    }

    onHoleComplete() {
        const challenge = this.challenges[this.currentChallenge];
        this.strokes++;
        
        alert(`Challenge ${this.currentChallenge + 1} Complete!\n${this.strokes} strokes (Par ${challenge.par})`);
        
        // Move to next challenge
        this.currentChallenge++;
        if (this.currentChallenge < this.challenges.length) {
            this.loadChallenge(this.currentChallenge);
        } else {
            this.onChallengeComplete();
        }
    }

    onChallengeFailed() {
        alert(`Challenge ${this.currentChallenge + 1} Failed!`);
        
        // Move to next challenge or end
        this.currentChallenge++;
        if (this.currentChallenge < this.challenges.length) {
            this.loadChallenge(this.currentChallenge);
        } else {
            this.onChallengeComplete();
        }
    }

    resetBall() {
        this.gameEngine.ballController.setPosition(new THREE.Vector3(0, 0, 0));
        this.strokes = 0;
    }

    onChallengeComplete() {
        alert('Putting Challenge Complete!');
        
        // Check for unlocks
        this.gameEngine.progressionSystem.checkUnlockConditions('accuracy_80');
        
        if (window.menuSystem) {
            window.menuSystem.quitToMenu();
        }
    }

    restart() {
        this.init();
    }
}

