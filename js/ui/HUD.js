class HUD {
    constructor() {
        this.holeElement = document.getElementById('hud-hole');
        this.parElement = document.getElementById('hud-par');
        this.distanceElement = document.getElementById('hud-distance');
        this.lieElement = document.getElementById('hud-lie');
        this.windElement = document.getElementById('hud-wind');
        this.strokesElement = document.getElementById('hud-strokes');
        this.powerBar = document.getElementById('power-bar');
        this.tempoNeedle = document.getElementById('tempo-needle');
        this.backspinValue = document.getElementById('backspin-value');
        this.sidespinValue = document.getElementById('sidespin-value');
        this.backspinSlider = document.getElementById('backspin-slider');
        this.sidespinSlider = document.getElementById('sidespin-slider');
        this.swingInterface = document.getElementById('swing-interface');
        
        this.setupSpinControls();
    }

    setupSpinControls() {
        if (this.backspinSlider && this.sidespinSlider) {
            this.backspinSlider.addEventListener('input', (e) => {
                const value = parseInt(e.target.value);
                this.backspinValue.textContent = value;
            });
            
            this.sidespinSlider.addEventListener('input', (e) => {
                const value = parseInt(e.target.value);
                this.sidespinValue.textContent = value;
            });
        }
    }

    update(gameState) {
        if (!gameState) return;

        // Update hole info
        if (gameState.hole) {
            if (this.holeElement) this.holeElement.textContent = gameState.hole.number || 1;
            if (this.parElement) this.parElement.textContent = gameState.hole.par || 4;
            
            // Calculate distance to pin
            if (gameState.ballPosition && gameState.hole.pin) {
                const distance = gameState.ballPosition.distanceTo(gameState.hole.pin) * 1.09361; // Convert meters to yards
                if (this.distanceElement) {
                    this.distanceElement.textContent = Math.round(distance) + ' yds';
                }
            }
        }

        // Update lie
        if (gameState.lie && this.lieElement) {
            this.lieElement.textContent = gameState.lie.name || 'Fairway';
        }

        // Update wind
        if (gameState.wind && this.windElement) {
            this.windElement.textContent = gameState.wind;
        }

        // Update strokes
        if (this.strokesElement && gameState.strokes !== undefined) {
            this.strokesElement.textContent = gameState.strokes;
        }

        // Update swing interface
        this.updateSwingInterface(gameState);
    }

    updateSwingInterface(gameState) {
        if (!this.swingInterface) return;

        // Show/hide swing interface based on game state
        const isSwinging = gameState.isSwinging || false;
        if (isSwinging && this.swingInterface.classList.contains('hidden')) {
            this.swingInterface.classList.remove('hidden');
        } else if (!isSwinging && !this.swingInterface.classList.contains('hidden')) {
            this.swingInterface.classList.add('hidden');
        }

        // Update power bar
        if (this.powerBar && gameState.swingPower !== undefined) {
            this.powerBar.style.width = gameState.swingPower + '%';
        }

        // Update tempo needle
        if (this.tempoNeedle && gameState.swingTempo !== undefined) {
            // Convert tempo angle (-90 to 90) to rotation
            const rotation = gameState.swingTempo;
            this.tempoNeedle.style.transform = `translateX(-50%) rotate(${rotation}deg)`;
        }

        // Update spin controls
        if (gameState.swingBackspin !== undefined && this.backspinSlider) {
            this.backspinSlider.value = gameState.swingBackspin;
            this.backspinValue.textContent = gameState.swingBackspin;
        }

        if (gameState.swingSidespin !== undefined && this.sidespinSlider) {
            this.sidespinSlider.value = gameState.swingSidespin;
            this.sidespinValue.textContent = gameState.swingSidespin;
        }
    }

    show() {
        const hud = document.getElementById('game-hud');
        if (hud) hud.classList.remove('hidden');
    }

    hide() {
        const hud = document.getElementById('game-hud');
        if (hud) hud.classList.add('hidden');
    }

    showSwingInterface() {
        if (this.swingInterface) {
            this.swingInterface.classList.remove('hidden');
        }
    }

    hideSwingInterface() {
        if (this.swingInterface) {
            this.swingInterface.classList.add('hidden');
        }
    }

    getSpinValues() {
        return {
            backspin: this.backspinSlider ? parseInt(this.backspinSlider.value) : 0,
            sidespin: this.sidespinSlider ? parseInt(this.sidespinSlider.value) : 0
        };
    }

    resetSpinControls() {
        if (this.backspinSlider) {
            this.backspinSlider.value = 0;
            this.backspinValue.textContent = '0';
        }
        if (this.sidespinSlider) {
            this.sidespinSlider.value = 0;
            this.sidespinValue.textContent = '0';
        }
    }
}

