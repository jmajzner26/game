class CourseSystem {
    constructor(scene) {
        this.scene = scene;
        this.courses = {};
        this.currentCourse = null;
        this.currentHole = 0;
        this.terrain = null;
        this.holes = [];
    }

    // Load a course (9 or 18 holes)
    loadCourse(courseName, holeCount = 18) {
        this.holes = this.generateHoles(holeCount);
        this.currentHole = 0;
        this.buildCourse();
        return this.holes;
    }

    // Generate holes with varying par and difficulty
    generateHoles(count) {
        const holes = [];
        const parDistribution = [3, 3, 4, 4, 4, 4, 4, 5, 5]; // 18-hole distribution
        
        for (let i = 0; i < count; i++) {
            const parIndex = i % parDistribution.length;
            let par = parDistribution[parIndex];
            
            // Add some variation
            if (i === 0 || i === count - 1) par = 4; // First and last hole are par 4
            
            holes.push({
                number: i + 1,
                par: par,
                distance: this.getDistanceForPar(par),
                tee: this.generateTeePosition(i),
                pin: this.generatePinPosition(par, i),
                green: this.generateGreen(i),
                hazards: this.generateHazards(par, i),
                fairway: this.generateFairway(par, i)
            });
        }
        
        return holes;
    }

    getDistanceForPar(par) {
        // Distance in yards
        if (par === 3) return 120 + Math.random() * 80; // 120-200
        if (par === 4) return 300 + Math.random() * 200; // 300-500
        if (par === 5) return 480 + Math.random() * 180; // 480-660
        return 350;
    }

    generateTeePosition(holeIndex) {
        // Vary tee positions
        const angle = (holeIndex / 18) * Math.PI * 2;
        const distance = 10 + holeIndex * 5;
        return new THREE.Vector3(
            Math.cos(angle) * distance,
            0.5,
            Math.sin(angle) * distance
        );
    }

    generatePinPosition(par, holeIndex) {
        const tee = this.generateTeePosition(holeIndex);
        const distance = this.getDistanceForPar(par);
        const angle = Math.PI + (holeIndex / 18) * Math.PI * 2;
        
        return new THREE.Vector3(
            tee.x + Math.cos(angle) * (distance * 0.91), // Convert yards to meters
            0.01,
            tee.z + Math.sin(angle) * (distance * 0.91)
        );
    }

    generateGreen(holeIndex) {
        // Generate green area with slope
        return {
            center: new THREE.Vector3(0, 0, 0),
            radius: 10, // meters
            slope: {
                x: (Math.random() - 0.5) * 0.05, // -0.025 to 0.025
                z: (Math.random() - 0.5) * 0.05
            }
        };
    }

    generateHazards(par, holeIndex) {
        const hazards = [];
        
        // Water hazards
        if (Math.random() > 0.5) {
            hazards.push({
                type: 'water',
                position: new THREE.Vector3(0, -0.5, 0),
                radius: 15 + Math.random() * 10
            });
        }
        
        // Sand bunkers
        if (Math.random() > 0.6) {
            hazards.push({
                type: 'sand',
                position: new THREE.Vector3(0, 0, 0),
                radius: 5 + Math.random() * 5
            });
        }
        
        return hazards;
    }

    generateFairway(par, holeIndex) {
        return {
            width: 30 + Math.random() * 20, // meters
            length: this.getDistanceForPar(par) * 0.91,
            roughness: 0.1
        };
    }

    buildCourse() {
        // Create terrain mesh
        const geometry = new THREE.PlaneGeometry(2000, 2000, 50, 50);
        const material = new THREE.MeshStandardMaterial({
            color: 0x228B22,
            roughness: 0.8,
            metalness: 0.1
        });
        
        this.terrain = new THREE.Mesh(geometry, material);
        this.terrain.rotation.x = -Math.PI / 2;
        this.terrain.receiveShadow = true;
        this.scene.add(this.terrain);
        
        // Create green areas
        this.holes.forEach(hole => {
            this.createGreen(hole.green, hole.pin);
            this.createHazards(hole.hazards);
        });
    }

    createGreen(green, pinPosition) {
        const greenGeometry = new THREE.CircleGeometry(green.radius, 32);
        const greenMaterial = new THREE.MeshStandardMaterial({
            color: 0x90EE90,
            roughness: 0.3,
            metalness: 0.1
        });
        
        const greenMesh = new THREE.Mesh(greenGeometry, greenMaterial);
        greenMesh.position.copy(pinPosition);
        greenMesh.rotation.x = -Math.PI / 2;
        greenMesh.position.y = 0.01;
        this.scene.add(greenMesh);
        
        // Create flag
        const flagGeometry = new THREE.CylinderGeometry(0.05, 0.05, 3, 8);
        const flagMaterial = new THREE.MeshStandardMaterial({ color: 0xffffff });
        const pole = new THREE.Mesh(flagGeometry, flagMaterial);
        pole.position.copy(pinPosition);
        pole.position.y = 1.5;
        this.scene.add(pole);
        
        // Flag
        const flagGeometry2 = new THREE.PlaneGeometry(1, 0.8);
        const flagMaterial2 = new THREE.MeshStandardMaterial({ color: 0xff0000 });
        const flag = new THREE.Mesh(flagGeometry2, flagMaterial2);
        flag.position.copy(pinPosition);
        flag.position.y = 2;
        flag.position.x += 0.5;
        this.scene.add(flag);
    }

    createHazards(hazards) {
        hazards.forEach(hazard => {
            if (hazard.type === 'water') {
                const waterGeometry = new THREE.CircleGeometry(hazard.radius, 32);
                const waterMaterial = new THREE.MeshStandardMaterial({
                    color: 0x0066cc,
                    transparent: true,
                    opacity: 0.7,
                    roughness: 0.1,
                    metalness: 0.9
                });
                
                const waterMesh = new THREE.Mesh(waterGeometry, waterMaterial);
                waterMesh.position.copy(hazard.position);
                waterMesh.rotation.x = -Math.PI / 2;
                waterMesh.position.y = -0.5;
                this.scene.add(waterMesh);
            } else if (hazard.type === 'sand') {
                const sandGeometry = new THREE.ConeGeometry(hazard.radius, 0.5, 32);
                const sandMaterial = new THREE.MeshStandardMaterial({
                    color: 0xC2B280,
                    roughness: 0.9
                });
                
                const sandMesh = new THREE.Mesh(sandGeometry, sandMaterial);
                sandMesh.position.copy(hazard.position);
                sandMesh.position.y = 0.25;
                this.scene.add(sandMesh);
            }
        });
    }

    getCurrentHole() {
        return this.holes[this.currentHole];
    }

    getHoleCount() {
        return this.holes.length;
    }

    nextHole() {
        this.currentHole++;
        return this.currentHole < this.holes.length;
    }

    reset() {
        this.currentHole = 0;
    }

    getTerrainType(position) {
        // Determine terrain type at position
        // Simplified - check distance to hazards and green
        const currentHole = this.getCurrentHole();
        
        // Check if on green
        const greenDistance = position.distanceTo(currentHole.pin);
        if (greenDistance < currentHole.green.radius) {
            return 'green';
        }
        
        // Check hazards
        for (const hazard of currentHole.hazards) {
            const hazardDistance = position.distanceTo(hazard.position);
            if (hazardDistance < hazard.radius) {
                return hazard.type;
            }
        }
        
        // Default to fairway
        return 'fairway';
    }
}

