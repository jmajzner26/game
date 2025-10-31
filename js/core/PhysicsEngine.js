class PhysicsEngine {
    constructor() {
        this.gravity = 9.8; // m/s²
        this.airDensity = 1.225; // kg/m³
        this.ballRadius = 0.02135; // meters (standard golf ball)
        this.ballMass = 0.04593; // kg
        this.dragCoefficient = 0.47; // sphere in air
        this.spinDecay = 0.95; // per second
    }

    // Calculate ball trajectory with wind, spin, and lie effects
    calculateTrajectory(initialVelocity, angle, backspin, sidespin, windVector, lieMultiplier, dt = 0.016) {
        const trajectory = [];
        let position = new THREE.Vector3(0, 0, 0);
        let velocity = new THREE.Vector3(
            initialVelocity * Math.cos(angle),
            initialVelocity * Math.sin(angle),
            0
        );
        
        let spin = {
            backspin: backspin,
            sidespin: sidespin
        };

        // Apply lie effect to initial velocity
        velocity.multiplyScalar(lieMultiplier);

        let time = 0;
        const maxTime = 15; // Maximum flight time (seconds)
        let onGround = false;

        while (time < maxTime && !onGround) {
            // Gravity
            const gravityForce = new THREE.Vector3(0, -this.gravity, 0);
            
            // Drag force (proportional to velocity squared)
            const speed = velocity.length();
            const dragForce = 0.5 * this.airDensity * Math.PI * this.ballRadius * this.ballRadius * 
                            this.dragCoefficient * speed * speed;
            const dragDirection = velocity.clone().normalize().multiplyScalar(-1);
            const dragVector = dragDirection.multiplyScalar(dragForce / this.ballMass);
            
            // Wind effect (windVector is Vector2, convert to Vector3)
            const windForce3D = new THREE.Vector3(windVector.x, 0, windVector.y).multiplyScalar(this.ballRadius * this.ballRadius * Math.PI / this.ballMass);
            
            // Magnus effect (spin)
            const magnusForce = this.calculateMagnusForce(velocity, spin);
            
            // Total acceleration
            const acceleration = new THREE.Vector3()
                .add(gravityForce)
                .add(dragVector)
                .add(windForce3D)
                .add(magnusForce);
            
            // Update velocity
            velocity.add(acceleration.clone().multiplyScalar(dt));
            
            // Update position
            position.add(velocity.clone().multiplyScalar(dt));
            
            // Check if ball hits ground
            if (position.y <= 0.01 && velocity.y < 0) {
                onGround = true;
                position.y = 0;
                // Bounce effect (simplified)
                const bounceDamping = 0.6;
                velocity.y *= -bounceDamping;
                velocity.x *= 0.8;
                velocity.z *= 0.8;
                spin.backspin *= 0.5;
                spin.sidespin *= 0.5;
            }
            
            // Decay spin over time
            spin.backspin *= Math.pow(this.spinDecay, dt);
            spin.sidespin *= Math.pow(this.spinDecay, dt);
            
            trajectory.push({
                position: position.clone(),
                velocity: velocity.clone(),
                spin: { ...spin },
                time: time
            });
            
            time += dt;
        }

        return trajectory;
    }

    // Magnus effect - spin affects ball flight
    calculateMagnusForce(velocity, spin) {
        const spinVector = new THREE.Vector3(
            spin.sidespin * 0.01, // Convert spin to force multiplier
            spin.backspin * 0.01,
            0
        );
        
        // Magnus force is perpendicular to velocity and spin axis
        const crossProduct = new THREE.Vector3().crossVectors(velocity, spinVector);
        const magnusCoefficient = 0.0001; // Fine-tuned for realistic effect
        return crossProduct.multiplyScalar(magnusCoefficient);
    }

    // Get lie type and multiplier based on terrain
    getLieType(terrainType) {
        const lieTypes = {
            'fairway': { name: 'Fairway', multiplier: 1.0, difficulty: 1.0 },
            'rough': { name: 'Rough', multiplier: 0.85, difficulty: 1.3 },
            'sand': { name: 'Sand', multiplier: 0.70, difficulty: 1.5 },
            'green': { name: 'Green', multiplier: 1.0, difficulty: 0.5 },
            'water': { name: 'Water', multiplier: 0.0, difficulty: 10.0 }
        };
        
        return lieTypes[terrainType] || lieTypes['fairway'];
    }

    // Calculate green reading (slope analysis)
    calculateGreenSlope(greenData, ballPosition) {
        // Simple slope calculation - could be enhanced with actual height maps
        const slope = {
            x: 0,
            y: 0,
            magnitude: 0,
            direction: 0
        };
        
        // This would use actual height data in a real implementation
        // For now, return a simplified version
        return slope;
    }

    // Apply spin to ball trajectory
    applySpinEffect(baseTrajectory, spin) {
        // Spin affects bounce and roll
        return baseTrajectory.map(point => {
            const spinInfluence = {
                backspin: spin.backspin * 0.1,
                sidespin: spin.sidespin * 0.1
            };
            return {
                ...point,
                spinInfluence
            };
        });
    }
}

