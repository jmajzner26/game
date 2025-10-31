class SwingSystem {
    constructor() {
        this.isSwinging = false;
        this.power = 0;
        this.maxPower = 100;
        this.tempo = 0; // -1 (slow) to 1 (fast), 0 is perfect
        this.backspin = 0;
        this.sidespin = 0;
        this.swingPhase = 'idle'; // idle, powering, timing, complete
        this.lastClickTime = 0;
        this.powerDirection = 1; // 1 for up, -1 for down
        this.perfectTempoZone = 0.15; // ±15% is perfect
        this.swingCompleteCallback = null;
        
        // Controller support
        this.gamepad = null;
        this.gamepadButtonPressed = false;
    }

    startSwing() {
        if (this.isSwinging) return;
        
        this.isSwinging = true;
        this.swingPhase = 'powering';
        this.power = 0;
        this.powerDirection = 1;
        this.lastClickTime = Date.now();
    }

    update(deltaTime) {
        if (!this.isSwinging) return;

        // Check for input (mouse or controller)
        this.checkInput();

        if (this.swingPhase === 'powering') {
            this.updatePower(deltaTime);
        } else if (this.swingPhase === 'timing') {
            this.updateTempo();
        }
    }

    checkInput() {
        // Check for gamepad
        const gamepads = navigator.getGamepads();
        if (gamepads && gamepads[0]) {
            this.gamepad = gamepads[0];
            const buttonPressed = gamepads[0].buttons[0]?.pressed || false;
            
            if (buttonPressed && !this.gamepadButtonPressed) {
                this.handleSwingInput();
                this.gamepadButtonPressed = true;
            } else if (!buttonPressed) {
                this.gamepadButtonPressed = false;
            }
        }
    }

    handleSwingInput() {
        if (this.swingPhase === 'powering') {
            // First click: set power, start tempo phase
            this.swingPhase = 'timing';
            this.lastClickTime = Date.now();
        } else if (this.swingPhase === 'timing') {
            // Second click: set tempo and complete swing
            const currentTime = Date.now();
            const timeSinceLastClick = (currentTime - this.lastClickTime) / 1000;
            this.calculateTempo(timeSinceLastClick);
            this.completeSwing();
        }
    }

    updatePower(deltaTime) {
        const powerSpeed = 80; // Power units per second
        
        if (this.powerDirection === 1) {
            this.power += powerSpeed * deltaTime;
            if (this.power >= this.maxPower) {
                this.power = this.maxPower;
                this.powerDirection = -1;
            }
        } else {
            this.power -= powerSpeed * deltaTime;
            if (this.power <= 0) {
                this.power = 0;
                this.powerDirection = 1;
            }
        }
    }

    updateTempo() {
        // Tempo meter visual feedback - oscillates during timing phase
        const timeSincePowerSet = (Date.now() - this.lastClickTime) / 1000;
        this.tempo = Math.sin(timeSincePowerSet * 2) * 0.5;
    }

    calculateTempo(timeBetweenClicks) {
        // Ideal tempo is around 0.3-0.5 seconds between clicks
        const idealTime = 0.4;
        const deviation = (timeBetweenClicks - idealTime) / idealTime;
        
        if (Math.abs(deviation) <= this.perfectTempoZone) {
            this.tempo = 0; // Perfect
        } else if (deviation < 0) {
            this.tempo = -Math.min(1, Math.abs(deviation) * 2); // Too fast (negative = fast)
        } else {
            this.tempo = Math.min(1, deviation * 2); // Too slow (positive = slow)
        }
    }

    completeSwing() {
        this.swingPhase = 'complete';
        this.isSwinging = false;
        
        if (this.swingCompleteCallback) {
            this.swingCompleteCallback({
                power: this.power,
                tempo: this.tempo,
                backspin: this.backspin,
                sidespin: this.sidespin
            });
        }
        
        // Reset for next swing
        this.swingPhase = 'idle';
        this.power = 0;
        this.tempo = 0;
    }

    setBackspin(value) {
        this.backspin = Math.max(0, Math.min(100, value));
    }

    setSidespin(value) {
        this.sidespin = Math.max(-100, Math.min(100, value));
    }

    getPowerPercentage() {
        return (this.power / this.maxPower) * 100;
    }

    getTempoAngle() {
        // Convert tempo (-1 to 1) to angle for visual meter
        // -1 = -90 degrees, 0 = 0 degrees, 1 = 90 degrees
        return this.tempo * 90;
    }

    isInPerfectTempo() {
        return Math.abs(this.tempo) <= this.perfectTempoZone;
    }

    cancelSwing() {
        this.isSwinging = false;
        this.swingPhase = 'idle';
        this.power = 0;
        this.tempo = 0;
    }
}

