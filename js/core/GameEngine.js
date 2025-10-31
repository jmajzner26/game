class GameEngine {
    constructor() {
        this.scene = null;
        this.camera = null;
        this.renderer = null;
        this.clock = new THREE.Clock();
        this.isRunning = false;
        
        // Core systems
        this.physicsEngine = new PhysicsEngine();
        this.windSystem = new WindSystem();
        this.ballController = null;
        this.swingSystem = new SwingSystem();
        this.courseSystem = null;
        this.progressionSystem = new ProgressionSystem();
        
        // Game state
        this.currentMode = null;
        this.strokes = 0;
        this.ballPosition = new THREE.Vector3(0, 0, 0);
        
        // Lighting
        this.ambientLight = null;
        this.directionalLight = null;
        
        // Animation
        this.animationFrameId = null;
    }

    init(canvas) {
        // Scene
        this.scene = new THREE.Scene();
        this.scene.background = new THREE.Color(0x87CEEB); // Sky blue
        this.scene.fog = new THREE.Fog(0x87CEEB, 100, 1000);

        // Camera
        this.camera = new THREE.PerspectiveCamera(
            75,
            canvas.clientWidth / canvas.clientHeight,
            0.1,
            2000
        );
        this.camera.position.set(0, 50, 100);
        this.camera.lookAt(0, 0, 0);

        // Renderer
        this.renderer = new THREE.WebGLRenderer({
            canvas: canvas,
            antialias: true
        });
        this.renderer.setSize(canvas.clientWidth, canvas.clientHeight);
        this.renderer.shadowMap.enabled = true;
        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;

        // Lighting
        this.setupLighting();

        // Initialize systems
        this.ballController = new BallController(this.scene, this.physicsEngine);
        this.courseSystem = new CourseSystem(this.scene);
        
        // Create ball
        this.ballController.createBall(new THREE.Vector3(0, 0, 0));

        // Load progress
        this.progressionSystem.loadProgress();

        // Setup swing callback
        this.swingSystem.swingCompleteCallback = (swingData) => {
            this.handleSwingComplete(swingData);
        };

        // Handle window resize
        window.addEventListener('resize', () => this.handleResize());

        // Mouse events
        canvas.addEventListener('click', (e) => this.handleMouseClick(e));
        
        // Keyboard events
        window.addEventListener('keydown', (e) => this.handleKeyDown(e));
        
        // Gamepad events
        window.addEventListener('gamepadconnected', (e) => {
            console.log('Gamepad connected');
        });
    }

    setupLighting() {
        // Ambient light
        this.ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
        this.scene.add(this.ambientLight);

        // Directional light (sun)
        this.directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
        this.directionalLight.position.set(50, 100, 50);
        this.directionalLight.castShadow = true;
        this.directionalLight.shadow.mapSize.width = 2048;
        this.directionalLight.shadow.mapSize.height = 2048;
        this.directionalLight.shadow.camera.near = 0.5;
        this.directionalLight.shadow.camera.far = 500;
        this.directionalLight.shadow.camera.left = -200;
        this.directionalLight.shadow.camera.right = 200;
        this.directionalLight.shadow.camera.top = 200;
        this.directionalLight.shadow.camera.bottom = -200;
        this.scene.add(this.directionalLight);
    }

    start() {
        if (this.isRunning) return;
        this.isRunning = true;
        this.clock.start();
        this.animate();
    }

    stop() {
        this.isRunning = false;
        if (this.animationFrameId) {
            cancelAnimationFrame(this.animationFrameId);
        }
    }

    animate() {
        if (!this.isRunning) return;

        this.animationFrameId = requestAnimationFrame(() => this.animate());

        const deltaTime = this.clock.getDelta();

        // Update systems
        this.swingSystem.update(deltaTime);
        this.ballController.update(deltaTime);
        
        if (this.currentMode && this.currentMode.update) {
            this.currentMode.update(deltaTime);
        }

        // Update camera to follow ball
        this.updateCamera();

        // Render
        this.renderer.render(this.scene, this.camera);
    }

    updateCamera() {
        if (!this.ballController || !this.ballController.ball) return;
        
        const ballPos = this.ballController.getPosition();
        const targetPos = new THREE.Vector3(
            ballPos.x + 20,
            30,
            ballPos.z + 40
        );
        
        this.camera.position.lerp(targetPos, 0.05);
        this.camera.lookAt(ballPos);
    }

    handleMouseClick(e) {
        if (this.swingSystem.swingPhase === 'idle') {
            this.swingSystem.startSwing();
        } else {
            this.swingSystem.handleSwingInput();
        }
    }

    handleKeyDown(e) {
        switch (e.key) {
            case ' ':
                this.handleMouseClick(e);
                break;
            case 'Escape':
                // Pause handled by UI
                break;
        }
    }

    handleResize() {
        const canvas = this.renderer.domElement;
        const width = canvas.clientWidth;
        const height = canvas.clientHeight;

        this.camera.aspect = width / height;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(width, height);
    }

    loadMode(mode) {
        this.currentMode = mode;
        this.currentMode.init(this);
    }

    startHole() {
        const hole = this.courseSystem.getCurrentHole();
        if (!hole) return;

        // Reset ball position to tee
        this.ballController.setPosition(hole.tee.clone());
        this.ballPosition = hole.tee.clone();
        this.strokes = 0;

        // Generate wind
        this.windSystem.generateWind(0, 15);

        // Update camera
        this.camera.position.set(
            hole.tee.x + 20,
            30,
            hole.tee.z + 40
        );
        this.camera.lookAt(hole.tee);
        
        // Reset swing system
        this.swingSystem.cancelSwing();
    }

    handleSwingComplete(swingData) {
        const hole = this.courseSystem.getCurrentHole();
        const club = this.progressionSystem.getEquippedClub();
        const ball = this.progressionSystem.getEquippedBall();
        
        // Calculate power with club and ball modifiers
        const basePower = (swingData.power / 100) * club.power;
        const powerWithBall = basePower * (ball.power / 100);
        
        // Apply tempo penalty
        const tempoPenalty = 1 - (Math.abs(swingData.tempo) * 0.2);
        const finalPower = powerWithBall * tempoPenalty;
        
        // Calculate angle
        const angle = Math.PI / 3; // ~60 degrees launch angle
        
        // Get wind vector
        const windVector = this.windSystem.getWindVector();
        
        // Get lie
        const lieType = this.courseSystem.getTerrainType(this.ballPosition);
        const lie = this.physicsEngine.getLieType(lieType);
        
        // Apply spin
        const backspin = swingData.backspin * (club.spin / 100) * (ball.spin / 100);
        const sidespin = swingData.sidespin * (club.spin / 100) * (ball.spin / 100);
        
        // Hit the ball
        this.strokes++;
        this.ballController.hitBall(
            finalPower * 0.1, // Convert to m/s
            angle,
            backspin,
            sidespin,
            windVector,
            lie.multiplier
        );
        
        // Set callback for when ball stops
        this.ballController.onTrajectoryComplete = (position) => {
            this.ballPosition = position;
            this.checkHoleComplete();
        };
    }

    checkHoleComplete() {
        const hole = this.courseSystem.getCurrentHole();
        const distanceToPin = this.ballPosition.distanceTo(hole.pin);
        
        if (distanceToPin < 0.5) { // Ball is in the hole (0.5m tolerance)
            // Hole complete
            if (this.currentMode && this.currentMode.onHoleComplete) {
                this.currentMode.onHoleComplete(this.strokes, hole.par);
            }
        }
    }

    getGameState() {
        return {
            strokes: this.strokes,
            ballPosition: this.ballPosition,
            currentHole: this.courseSystem.currentHole,
            hole: this.courseSystem.getCurrentHole(),
            wind: this.windSystem.getWindDisplay(),
            swingPower: this.swingSystem.getPowerPercentage(),
            swingTempo: this.swingSystem.getTempoAngle()
        };
    }
}

