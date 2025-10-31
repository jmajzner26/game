class DailyEventMode {
    constructor(gameEngine) {
        this.gameEngine = gameEngine;
        this.eventType = null;
        this.dailySeed = null;
        this.score = 0;
        this.strokes = 0;
    }

    init() {
        // Generate daily seed based on date
        const today = new Date();
        this.dailySeed = today.getFullYear() * 10000 + (today.getMonth() + 1) * 100 + today.getDate();
        
        // Select random event type
        const eventTypes = ['closest-to-pin', 'longest-drive', 'score-challenge'];
        this.eventType = eventTypes[Math.floor(Math.random() * eventTypes.length)];
        
        this.score = 0;
        this.strokes = 0;
        
        // Load event based on type
        this.loadEvent();
    }

    loadEvent() {
        // Set random seed for consistent daily challenges
        Math.seedrandom(this.dailySeed);
        
        switch (this.eventType) {
            case 'closest-to-pin':
                this.loadClosestToPin();
                break;
            case 'longest-drive':
                this.loadLongestDrive();
                break;
            case 'score-challenge':
                this.loadScoreChallenge();
                break;
        }
    }

    loadClosestToPin() {
        // Create a par 3 hole with special scoring
        this.gameEngine.courseSystem.loadCourse('daily', 1);
        const hole = this.gameEngine.courseSystem.getCurrentHole();
        hole.par = 3;
        
        this.gameEngine.startHole();
    }

    loadLongestDrive() {
        // Driving range style with longest drive tracking
        const rangeGeometry = new THREE.PlaneGeometry(200, 1000);
        const rangeMaterial = new THREE.MeshStandardMaterial({ color: 0x228B22 });
        const range = new THREE.Mesh(rangeGeometry, rangeMaterial);
        range.rotation.x = -Math.PI / 2;
        this.gameEngine.scene.add(range);
        
        this.gameEngine.ballController.setPosition(new THREE.Vector3(0, 0, 0));
    }

    loadScoreChallenge() {
        // 9-hole score challenge
        this.gameEngine.courseSystem.loadCourse('daily', 9);
        this.gameEngine.startHole();
    }

    update(deltaTime) {
        if (window.hud) {
            const hole = this.gameEngine.courseSystem.getCurrentHole();
            window.hud.update({
                hole: { number: 'EVENT', par: '-' },
                distance: `Daily Event: ${this.eventType}`,
                lie: { name: 'Event' },
                wind: this.gameEngine.windSystem.getWindDisplay(),
                strokes: this.strokes,
                isSwinging: this.gameEngine.swingSystem.isSwinging,
                swingPower: this.gameEngine.swingSystem.getPowerPercentage(),
                swingTempo: this.gameEngine.swingSystem.getTempoAngle()
            });
        }
    }

    onEventComplete() {
        // Calculate final score
        let message = `Daily Event Complete!\n\n`;
        message += `Event Type: ${this.eventType}\n`;
        message += `Score: ${this.score}\n`;
        
        // Save daily event completion
        const today = new Date().toDateString();
        localStorage.setItem(`dailyEvent_${today}`, JSON.stringify({
            type: this.eventType,
            score: this.score
        }));
        
        alert(message);
        
        if (window.menuSystem) {
            window.menuSystem.quitToMenu();
        }
    }

    restart() {
        this.init();
    }
}

// Simple seeded random number generator for daily events
Math.seedrandom = function(seed) {
    let value = seed;
    return function() {
        value = (value * 9301 + 49297) % 233280;
        return value / 233280;
    };
};

