class WindSystem {
    constructor() {
        this.windSpeed = 0; // mph
        this.windDirection = 0; // radians (0 = right, PI/2 = up)
        this.windVariation = 0.1; // Variation in wind strength
        this.baseWindSpeed = 5;
    }

    // Generate random wind for a hole
    generateWind(minSpeed = 0, maxSpeed = 15) {
        this.windSpeed = minSpeed + Math.random() * (maxSpeed - minSpeed);
        this.windDirection = Math.random() * Math.PI * 2;
        return this.getWindVector();
    }

    // Get wind as vector (x, y components)
    getWindVector() {
        return new THREE.Vector2(
            Math.cos(this.windDirection) * this.windSpeed * 0.447, // Convert mph to m/s
            Math.sin(this.windDirection) * this.windSpeed * 0.447
        );
    }

    // Get wind in mph with direction arrow
    getWindDisplay() {
        const directions = ['→', '↗', '↑', '↖', '←', '↙', '↓', '↘'];
        const dirIndex = Math.floor((this.windDirection / (Math.PI * 2)) * 8) % 8;
        const arrow = directions[dirIndex];
        return `${Math.round(this.windSpeed)} mph ${arrow}`;
    }

    // Calculate effective wind on ball trajectory
    calculateWindEffect(windVector, ballVelocity) {
        // Wind has more effect at higher altitudes and slower ball speeds
        const relativeWind = windVector.clone().sub(new THREE.Vector2(ballVelocity.x, ballVelocity.z));
        return relativeWind.multiplyScalar(0.3); // Wind influence multiplier
    }

    // Add wind variation (gusts)
    addWindVariation(time) {
        const variation = Math.sin(time * 0.5) * this.windVariation * this.windSpeed;
        return this.windSpeed + variation;
    }
}

