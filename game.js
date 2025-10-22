// Tower Defense Game
class TowerDefenseGame {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.width = this.canvas.width;
        this.height = this.canvas.height;
        
        // Game state
        this.gameState = 'menu'; // menu, playing, paused, gameOver
        this.health = 100;
        this.money = 500;
        this.wave = 1;
        this.waveInProgress = false;
        this.selectedTowerType = null;
        this.draggedTowerType = null;
        this.dragPreview = null;
        
        // Game objects
        this.towers = [];
        this.enemies = [];
        this.projectiles = [];
        this.path = [];
        
        // Tower types
        this.towerTypes = {
            basic: { cost: 50, damage: 25, range: 80, fireRate: 1000, color: '#4a90e2', type: 'combat' },
            rapid: { cost: 100, damage: 15, range: 70, fireRate: 500, color: '#ff6b6b', type: 'combat' },
            heavy: { cost: 200, damage: 60, range: 100, fireRate: 2000, color: '#ffd700', type: 'combat' },
            money: { cost: 150, damage: 0, range: 0, fireRate: 0, color: '#32cd32', type: 'money', income: 25 }
        };
        
        // Enemy types
        this.enemyTypes = {
            basic: { health: 50, speed: 1, reward: 10, color: '#ff4444' },
            fast: { health: 30, speed: 2, reward: 15, color: '#44ff44' },
            tank: { health: 150, speed: 0.5, reward: 25, color: '#4444ff' }
        };
        
        this.setupPath();
        this.setupEventListeners();
        this.gameLoop();
    }
    
    setupPath() {
        // Create a winding path from left to right
        this.path = [
            { x: 0, y: 300 },
            { x: 150, y: 300 },
            { x: 150, y: 150 },
            { x: 300, y: 150 },
            { x: 300, y: 450 },
            { x: 500, y: 450 },
            { x: 500, y: 200 },
            { x: 700, y: 200 },
            { x: 800, y: 200 }
        ];
    }
    
    setupEventListeners() {
        // Canvas events for tower placement
        this.canvas.addEventListener('click', (e) => {
            if (this.gameState === 'playing' && this.selectedTowerType) {
                const rect = this.canvas.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;
                this.placeTower(x, y);
            }
        });

        this.canvas.addEventListener('dragover', (e) => {
            e.preventDefault();
        });

        this.canvas.addEventListener('drop', (e) => {
            e.preventDefault();
            if (this.gameState === 'playing' && this.draggedTowerType) {
                const rect = this.canvas.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;
                this.selectedTowerType = this.draggedTowerType;
                this.placeTower(x, y);
                this.draggedTowerType = null;
                this.dragPreview = null;
            }
        });

        this.canvas.addEventListener('mousemove', (e) => {
            if (this.draggedTowerType) {
                const rect = this.canvas.getBoundingClientRect();
                this.dragPreview = {
                    x: e.clientX - rect.left,
                    y: e.clientY - rect.top,
                    type: this.draggedTowerType
                };
            }
        });
        
        // Tower selection and drag events
        document.querySelectorAll('.tower-option').forEach(option => {
            // Click to select (original functionality)
            option.addEventListener('click', () => {
                const towerType = option.dataset.tower;
                if (this.money >= this.towerTypes[towerType].cost) {
                    this.selectedTowerType = towerType;
                    document.querySelectorAll('.tower-option').forEach(opt => opt.classList.remove('selected'));
                    option.classList.add('selected');
                }
            });

            // Drag start
            option.addEventListener('dragstart', (e) => {
                const towerType = option.dataset.tower;
                if (this.money >= this.towerTypes[towerType].cost) {
                    this.draggedTowerType = towerType;
                    option.classList.add('dragging');
                    e.dataTransfer.effectAllowed = 'copy';
                    e.dataTransfer.setData('text/plain', towerType);
                } else {
                    e.preventDefault();
                }
            });

            // Drag end
            option.addEventListener('dragend', (e) => {
                option.classList.remove('dragging');
                this.draggedTowerType = null;
                this.dragPreview = null;
            });
        });
        
        // Control buttons
        document.getElementById('start-wave').addEventListener('click', () => {
            this.startWave();
        });
        
        document.getElementById('pause-game').addEventListener('click', () => {
            this.togglePause();
        });
        
        document.getElementById('overlayButton').addEventListener('click', () => {
            this.startGame();
        });
    }
    
    startGame() {
        this.gameState = 'playing';
        document.getElementById('gameOverlay').classList.add('hidden');
    }
    
    startWave() {
        if (!this.waveInProgress) {
            this.waveInProgress = true;
            this.spawnEnemies();
        }
    }
    
    spawnEnemies() {
        const enemyCount = 5 + this.wave * 2;
        const spawnDelay = 1000;
        
        for (let i = 0; i < enemyCount; i++) {
            setTimeout(() => {
                let enemyType = 'basic';
                if (this.wave > 3) enemyType = Math.random() < 0.3 ? 'fast' : 'basic';
                if (this.wave > 6) enemyType = Math.random() < 0.2 ? 'tank' : enemyType;
                
                this.enemies.push(new Enemy(this.path[0].x, this.path[0].y, enemyType, this.enemyTypes[enemyType]));
                
                if (i === enemyCount - 1) {
                    setTimeout(() => {
                        this.waveInProgress = false;
                        this.wave++;
                        this.updateUI();
                    }, spawnDelay * 2);
                }
            }, i * spawnDelay);
        }
    }
    
    placeTower(x, y) {
        const towerType = this.towerTypes[this.selectedTowerType];
        
        // Check if position is valid (not too close to other towers) and affordable
        if (this.isValidTowerPosition(x, y) && this.money >= towerType.cost) {
            this.towers.push(new Tower(x, y, this.selectedTowerType, towerType));
            this.money -= towerType.cost;
            this.selectedTowerType = null;
            document.querySelectorAll('.tower-option').forEach(opt => opt.classList.remove('selected'));
            this.updateUI();
        }
    }
    
    isValidTowerPosition(x, y) {
        // Only check distance from other towers (allow placement anywhere including on path)
        for (const tower of this.towers) {
            if (Math.sqrt((x - tower.x) ** 2 + (y - tower.y) ** 2) < 40) {
                return false;
            }
        }
        
        return true;
    }
    
    distanceToLineSegment(px, py, lineStart, lineEnd) {
        const A = px - lineStart.x;
        const B = py - lineStart.y;
        const C = lineEnd.x - lineStart.x;
        const D = lineEnd.y - lineStart.y;
        
        const dot = A * C + B * D;
        const lenSq = C * C + D * D;
        let param = -1;
        
        if (lenSq !== 0) {
            param = dot / lenSq;
        }
        
        let xx, yy;
        
        if (param < 0) {
            xx = lineStart.x;
            yy = lineStart.y;
        } else if (param > 1) {
            xx = lineEnd.x;
            yy = lineEnd.y;
        } else {
            xx = lineStart.x + param * C;
            yy = lineStart.y + param * D;
        }
        
        const dx = px - xx;
        const dy = py - yy;
        return Math.sqrt(dx * dx + dy * dy);
    }
    
    togglePause() {
        if (this.gameState === 'playing') {
            this.gameState = 'paused';
        } else if (this.gameState === 'paused') {
            this.gameState = 'playing';
        }
    }
    
    update() {
        if (this.gameState !== 'playing') return;
        
        // Update enemies
        this.enemies = this.enemies.filter(enemy => {
            enemy.update();
            
            // Check if enemy reached the end
            if (enemy.pathIndex >= this.path.length - 1) {
                this.health -= 10;
                this.updateUI();
                if (this.health <= 0) {
                    this.gameOver();
                }
                return false;
            }
            return true;
        });
        
        // Update towers
        this.towers.forEach(tower => {
            tower.update(this.enemies, this.projectiles);
            
            // Handle money towers
            if (tower.type === 'money' && !this.waveInProgress) {
                // Generate money at the end of each wave
                if (tower.shouldGenerateMoney()) {
                    this.money += tower.income;
                    tower.lastMoneyGeneration = Date.now();
                }
            }
        });
        
        // Update projectiles
        this.projectiles = this.projectiles.filter(projectile => {
            projectile.update();
            
            // Check collision with enemies
            for (let i = this.enemies.length - 1; i >= 0; i--) {
                const enemy = this.enemies[i];
                if (this.checkCollision(projectile, enemy)) {
                    enemy.takeDamage(projectile.damage);
                    if (enemy.health <= 0) {
                        this.money += enemy.reward;
                        this.enemies.splice(i, 1);
                    }
                    return false;
                }
            }
            
            return projectile.x >= 0 && projectile.x <= this.width && 
                   projectile.y >= 0 && projectile.y <= this.height;
        });
        
        this.updateUI();
    }
    
    checkCollision(projectile, enemy) {
        const dx = projectile.x - enemy.x;
        const dy = projectile.y - enemy.y;
        const distance = Math.sqrt(dx * dx + dy * dy);
        return distance < enemy.radius + projectile.radius;
    }
    
    render() {
        // Clear canvas
        this.ctx.fillStyle = '#2d5016';
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        // Draw path
        this.drawPath();
        
        // Draw towers
        this.towers.forEach(tower => tower.render(this.ctx));
        
        // Draw enemies
        this.enemies.forEach(enemy => enemy.render(this.ctx));
        
        // Draw projectiles
        this.projectiles.forEach(projectile => projectile.render(this.ctx));
        
        // Draw drag preview
        if (this.dragPreview) {
            this.drawDragPreview(this.dragPreview.x, this.dragPreview.y, this.dragPreview.type);
        }
        
        // Draw tower placement preview
        if (this.selectedTowerType) {
            this.canvas.style.cursor = 'crosshair';
        } else if (this.draggedTowerType) {
            this.canvas.style.cursor = 'grabbing';
        } else {
            this.canvas.style.cursor = 'default';
        }
    }
    
    drawPath() {
        this.ctx.strokeStyle = '#8B4513';
        this.ctx.lineWidth = 16;
        this.ctx.lineCap = 'round';
        this.ctx.lineJoin = 'round';
        
        this.ctx.beginPath();
        this.ctx.moveTo(this.path[0].x, this.path[0].y);
        
        for (let i = 1; i < this.path.length; i++) {
            this.ctx.lineTo(this.path[i].x, this.path[i].y);
        }
        
        this.ctx.stroke();
        
        // Draw path borders
        this.ctx.strokeStyle = '#654321';
        this.ctx.lineWidth = 20;
        this.ctx.beginPath();
        this.ctx.moveTo(this.path[0].x, this.path[0].y);
        
        for (let i = 1; i < this.path.length; i++) {
            this.ctx.lineTo(this.path[i].x, this.path[i].y);
        }
        
        this.ctx.stroke();
    }
    
    drawDragPreview(x, y, towerType) {
        const towerStats = this.towerTypes[towerType];
        
        // Draw semi-transparent tower preview
        this.ctx.globalAlpha = 0.6;
        this.ctx.fillStyle = towerStats.color;
        this.ctx.beginPath();
        this.ctx.arc(x, y, 15, 0, Math.PI * 2);
        this.ctx.fill();
        
        // Draw tower icon
        this.ctx.globalAlpha = 0.8;
        this.ctx.fillStyle = '#ffffff';
        this.ctx.font = 'bold 12px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        
        let icon = '🔫';
        if (towerType === 'rapid') icon = '⚡';
        else if (towerType === 'heavy') icon = '💥';
        else if (towerType === 'money') icon = '💰';
        
        this.ctx.fillText(icon, x, y);
        
        // Draw range circle for combat towers
        if (towerStats.type === 'combat') {
            this.ctx.globalAlpha = 0.3;
            this.ctx.strokeStyle = '#ffffff';
            this.ctx.lineWidth = 2;
            this.ctx.beginPath();
            this.ctx.arc(x, y, towerStats.range, 0, Math.PI * 2);
            this.ctx.stroke();
        }
        
        // Reset alpha
        this.ctx.globalAlpha = 1.0;
        
        // Draw validity indicator
        const isValid = this.isValidTowerPosition(x, y);
        this.ctx.strokeStyle = isValid ? '#00ff00' : '#ff0000';
        this.ctx.lineWidth = 3;
        this.ctx.beginPath();
        this.ctx.arc(x, y, 18, 0, Math.PI * 2);
        this.ctx.stroke();
    }
    
    updateUI() {
        document.getElementById('health').textContent = this.health;
        document.getElementById('money').textContent = this.money;
        document.getElementById('wave').textContent = this.wave;
        
        // Update tower costs
        document.querySelectorAll('.tower-option').forEach(option => {
            const towerType = option.dataset.tower;
            const cost = this.towerTypes[towerType].cost;
            option.querySelector('.tower-cost').textContent = `$${cost}`;
            
            if (this.money < cost) {
                option.style.opacity = '0.5';
            } else {
                option.style.opacity = '1';
            }
        });
    }
    
    gameOver() {
        this.gameState = 'gameOver';
        document.getElementById('overlayTitle').textContent = 'Game Over!';
        document.getElementById('overlayMessage').textContent = `You survived ${this.wave - 1} waves!`;
        document.getElementById('overlayButton').textContent = 'Play Again';
        document.getElementById('gameOverlay').classList.remove('hidden');
        
        document.getElementById('overlayButton').onclick = () => {
            this.resetGame();
        };
    }
    
    resetGame() {
        this.health = 100;
        this.money = 500;
        this.wave = 1;
        this.waveInProgress = false;
        this.towers = [];
        this.enemies = [];
        this.projectiles = [];
        this.selectedTowerType = null;
        document.querySelectorAll('.tower-option').forEach(opt => opt.classList.remove('selected'));
        this.startGame();
    }
    
    gameLoop() {
        this.update();
        this.render();
        requestAnimationFrame(() => this.gameLoop());
    }
}

// Enemy class
class Enemy {
    constructor(x, y, type, stats) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.maxHealth = stats.health;
        this.health = stats.health;
        this.speed = stats.speed;
        this.reward = stats.reward;
        this.color = stats.color;
        this.radius = 12;
        this.pathIndex = 0;
        this.lastUpdate = Date.now();
    }
    
    update() {
        const now = Date.now();
        const deltaTime = now - this.lastUpdate;
        this.lastUpdate = now;
        
        if (this.pathIndex < game.path.length - 1) {
            const target = game.path[this.pathIndex + 1];
            const dx = target.x - this.x;
            const dy = target.y - this.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            if (distance < 5) {
                this.pathIndex++;
            } else {
                const moveDistance = this.speed * deltaTime * 0.1;
                this.x += (dx / distance) * moveDistance;
                this.y += (dy / distance) * moveDistance;
            }
        }
    }
    
    takeDamage(damage) {
        this.health -= damage;
    }
    
    render(ctx) {
        // Draw enemy body
        ctx.fillStyle = this.color;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        
        // Draw health bar
        const barWidth = 20;
        const barHeight = 4;
        const barX = this.x - barWidth / 2;
        const barY = this.y - this.radius - 8;
        
        ctx.fillStyle = '#ff0000';
        ctx.fillRect(barX, barY, barWidth, barHeight);
        
        ctx.fillStyle = '#00ff00';
        ctx.fillRect(barX, barY, (this.health / this.maxHealth) * barWidth, barHeight);
    }
}

// Tower class
class Tower {
    constructor(x, y, type, stats) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.towerType = stats.type || 'combat';
        this.damage = stats.damage;
        this.range = stats.range;
        this.fireRate = stats.fireRate;
        this.color = stats.color;
        this.radius = 15;
        this.lastShot = 0;
        this.target = null;
        this.income = stats.income || 0;
        this.lastMoneyGeneration = 0;
    }
    
    update(enemies, projectiles) {
        const now = Date.now();
        
        // Only handle combat towers
        if (this.towerType === 'combat') {
            // Find target
            this.target = this.findTarget(enemies);
            
            // Shoot if target is in range and enough time has passed
            if (this.target && now - this.lastShot >= this.fireRate) {
                this.shoot(projectiles);
                this.lastShot = now;
            }
        }
    }
    
    shouldGenerateMoney() {
        const now = Date.now();
        // Generate money every 3 seconds when wave is not in progress
        return now - this.lastMoneyGeneration >= 3000;
    }
    
    findTarget(enemies) {
        let closestEnemy = null;
        let closestDistance = this.range;
        
        for (const enemy of enemies) {
            const distance = Math.sqrt((enemy.x - this.x) ** 2 + (enemy.y - this.y) ** 2);
            if (distance <= this.range && distance < closestDistance) {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }
        
        return closestEnemy;
    }
    
    shoot(projectiles) {
        if (this.target) {
            const dx = this.target.x - this.x;
            const dy = this.target.y - this.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            projectiles.push(new Projectile(
                this.x, this.y,
                dx / distance, dy / distance,
                this.damage, this.color
            ));
        }
    }
    
    render(ctx) {
        // Draw tower base
        ctx.fillStyle = this.color;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        
        // Draw money tower special effect
        if (this.towerType === 'money') {
            // Draw dollar sign
            ctx.fillStyle = '#ffd700';
            ctx.font = 'bold 12px Arial';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('$', this.x, this.y);
            
            // Draw pulsing effect
            const pulse = Math.sin(Date.now() * 0.005) * 0.2 + 0.8;
            ctx.strokeStyle = `rgba(255, 215, 0, ${pulse})`;
            ctx.lineWidth = 3;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 5, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        // Draw range circle when hovering (only for combat towers)
        if (this.target && this.towerType === 'combat') {
            ctx.strokeStyle = 'rgba(255, 255, 255, 0.3)';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.range, 0, Math.PI * 2);
            ctx.stroke();
        }
    }
}

// Projectile class
class Projectile {
    constructor(x, y, vx, vy, damage, color) {
        this.x = x;
        this.y = y;
        this.vx = vx * 3;
        this.vy = vy * 3;
        this.damage = damage;
        this.color = color;
        this.radius = 3;
    }
    
    update() {
        this.x += this.vx;
        this.y += this.vy;
    }
    
    render(ctx) {
        ctx.fillStyle = this.color;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
    }
}

// Initialize game when page loads
let game;
window.addEventListener('load', () => {
    game = new TowerDefenseGame();
});
