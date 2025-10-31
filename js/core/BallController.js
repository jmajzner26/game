class BallController {
    constructor(scene, physicsEngine) {
        this.scene = scene;
        this.physicsEngine = physicsEngine;
        this.ball = null;
        this.trajectory = null;
        this.currentPosition = new THREE.Vector3(0, 0, 0);
        this.isMoving = false;
        this.trajectoryIndex = 0;
        this.onTrajectoryComplete = null;
    }

    createBall(position = new THREE.Vector3(0, 0, 0)) {
        const geometry = new THREE.SphereGeometry(0.02135, 16, 16);
        const material = new THREE.MeshStandardMaterial({
            color: 0xffffff,
            metalness: 0.3,
            roughness: 0.2
        });
        
        this.ball = new THREE.Mesh(geometry, material);
        this.ball.position.copy(position);
        this.ball.castShadow = true;
        this.ball.receiveShadow = true;
        this.scene.add(this.ball);
        
        this.currentPosition = position.clone();
    }

    hitBall(velocity, angle, backspin, sidespin, windVector, lieMultiplier) {
        // Start from current position
        const startPos = this.currentPosition.clone();
        
        this.trajectory = this.physicsEngine.calculateTrajectory(
            velocity,
            angle,
            backspin,
            sidespin,
            windVector,
            lieMultiplier
        );
        
        // Translate trajectory to start position
        this.trajectory = this.trajectory.map(point => ({
            ...point,
            position: point.position.clone().add(startPos)
        }));
        
        this.trajectoryIndex = 0;
        this.isMoving = true;
    }

    update(deltaTime) {
        if (!this.isMoving || !this.trajectory || this.trajectoryIndex >= this.trajectory.length) {
            if (this.isMoving && this.trajectoryIndex >= this.trajectory.length) {
                this.isMoving = false;
                if (this.onTrajectoryComplete) {
                    this.onTrajectoryComplete(this.currentPosition);
                }
            }
            return;
        }

        // Advance trajectory based on time
        const trajectorySpeed = 10; // frames per second
        this.trajectoryIndex += deltaTime * trajectorySpeed;
        
        if (this.trajectoryIndex >= this.trajectory.length) {
            this.trajectoryIndex = this.trajectory.length - 1;
            this.isMoving = false;
            if (this.onTrajectoryComplete) {
                this.onTrajectoryComplete(this.currentPosition);
            }
        } else {
            const point = this.trajectory[Math.floor(this.trajectoryIndex)];
            if (point) {
                this.currentPosition.copy(point.position);
                
                if (this.ball) {
                    this.ball.position.copy(this.currentPosition);
                }
            }
        }
    }

    getPosition() {
        return this.currentPosition.clone();
    }

    setPosition(position) {
        this.currentPosition.copy(position);
        if (this.ball) {
            this.ball.position.copy(this.currentPosition);
        }
    }

    reset() {
        this.isMoving = false;
        this.trajectory = null;
        this.trajectoryIndex = 0;
    }

    showTrajectoryPreview(points) {
        // Visual trajectory preview could be implemented here
    }

    hideTrajectoryPreview() {
        // Remove preview
    }
}

