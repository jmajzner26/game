class DrivingRangeMode {
    constructor(gameEngine) {
        this.gameEngine = gameEngine;
        this.targets = [];
        this.ballsRemaining = 20;
        this.totalDistance = 0;
        this.shotsFired = 0;
    }

    init() {
        // Create driving range layout
        this.createRange();
        this.ballsRemaining = 20;
        this.totalDistance = 0;
        this.shotsFired = 0;
        
        // Set ball at starting position
        this.gameEngine.ballController.setPosition(new THREE.Vector3(0, 0, 0));
        
        // Generate wind
        this.gameEngine.windSystem.generateWind(0, 10);
    }

    createRange() {
        // Create targets at various distances
        const distances = [100, 150, 200, 250, 300]; // yards
        
        distances.forEach(distance => {
            const targetGeometry = new THREE.CylinderGeometry(5, 5, 2, 16);
            const targetMaterial = new THREE.MeshStandardMaterial({ color: 0xff0000 });
            const target = new THREE.Mesh(targetGeometry, targetMaterial);
            target.position.set(0, 1, distance * 0.91); // Convert yards to meters
            this.gameEngine.scene.add(target);
            this.targets.push({ mesh: target, distance: distance });
        });
    }

    update(deltaTime) {
        // Update HUD
        if (window.hud) {
            window.hud.update({
                hole: { number: 'DR', par: '-' },
                distance: `${this.ballsRemaining} balls remaining`,
                lie: { name: 'Tee' },
                wind: this.gameEngine.windSystem.getWindDisplay(),
                strokes: this.shotsFired,
                isSwinging: this.gameEngine.swingSystem.isSwinging,
                swingPower: this.gameEngine.swingSystem.getPowerPercentage(),
                swingTempo: this.gameEngine.swingSystem.getTempoAngle()
            });
        }
        
        // Check if ball stopped
        if (!this.gameEngine.ballController.isMoving && this.gameEngine.swingSystem.swingPhase === 'idle') {
            this.handleBallStop();
        }
    }

    handleBallStop() {
        const ballPos = this.gameEngine.ballController.getPosition();
        const distance = Math.sqrt(ballPos.x * ballPos.x + ballPos.z * ballPos.z) * 1.09361; // meters to yards
        
        this.totalDistance += distance;
        this.shotsFired++;
        
        // Check for targets hit
        this.targets.forEach(target => {
            const targetDistance = ballPos.distanceTo(target.mesh.position);
            if (targetDistance < 5) {
                // Hit target
                alert(`Hit ${target.distance} yard target!`);
            }
        });
        
        if (this.ballsRemaining > 0) {
            // Reset ball position
            this.ballsRemaining--;
            this.gameEngine.ballController.setPosition(new THREE.Vector3(0, 0, 0));
            
            // Generate new wind
            this.gameEngine.windSystem.generateWind(0, 10);
        } else {
            // Session complete
            const avgDistance = this.totalDistance / this.shotsFired;
            alert(`Driving Range Complete!\n\nAverage Distance: ${Math.round(avgDistance)} yards`);
            
            // Check for unlocks
            if (avgDistance >= 300) {
                this.gameEngine.progressionSystem.checkUnlockConditions('drive_300');
            }
            
            if (window.menuSystem) {
                window.menuSystem.quitToMenu();
            }
        }
    }

    restart() {
        this.init();
    }
}

