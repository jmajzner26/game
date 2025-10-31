class PhotoMode {
    constructor(gameEngine) {
        this.gameEngine = gameEngine;
        this.isActive = false;
        this.photoOverlay = document.getElementById('photo-mode');
        this.photoControls = document.querySelector('.photo-controls');
        this.camera = null;
        this.controls = null;
        
        this.setupEventListeners();
    }

    setupEventListeners() {
        const btnPhotoMode = document.getElementById('btn-photo-mode');
        const btnPhotoExit = document.getElementById('photo-exit');
        const btnPhotoCapture = document.getElementById('photo-capture');

        if (btnPhotoMode) {
            btnPhotoMode.addEventListener('click', () => this.toggle());
        }
        if (btnPhotoExit) {
            btnPhotoExit.addEventListener('click', () => this.exit());
        }
        if (btnPhotoCapture) {
            btnPhotoCapture.addEventListener('click', () => this.capture());
        }
    }

    toggle() {
        if (this.isActive) {
            this.exit();
        } else {
            this.enter();
        }
    }

    enter() {
        this.isActive = true;
        if (this.photoOverlay) {
            this.photoOverlay.classList.remove('hidden');
        }
        
        // Freeze game
        if (this.gameEngine) {
            this.gameEngine.stop();
        }
    }

    exit() {
        this.isActive = false;
        if (this.photoOverlay) {
            this.photoOverlay.classList.add('hidden');
        }
        
        // Resume game
        if (this.gameEngine) {
            this.gameEngine.start();
        }
    }

    capture() {
        if (!this.gameEngine || !this.gameEngine.renderer) return;

        const canvas = this.gameEngine.renderer.domElement;
        const dataURL = canvas.toDataURL('image/png');
        
        // Create download link
        const link = document.createElement('a');
        link.download = `golf-photo-${Date.now()}.png`;
        link.href = dataURL;
        link.click();
    }

    setCameraPosition(position, lookAt) {
        if (!this.gameEngine || !this.gameEngine.camera) return;
        
        // In a full implementation, this would allow free camera movement
        // For now, just use the current camera position
        if (position) {
            this.gameEngine.camera.position.copy(position);
        }
        if (lookAt) {
            this.gameEngine.camera.lookAt(lookAt);
        }
    }
}

