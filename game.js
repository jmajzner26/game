// Epic Tower Defense Game
class TowerDefenseGame {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        this.width = this.canvas.width;
        this.height = this.canvas.height;
        
        // Game state
        this.gameState = 'home'; // home, menu, playing, paused, gameOver
        this.difficulty = 'medium';
        this.health = 100;
        this.money = 500;
        this.wave = 1;
        this.score = 0;
        this.waveInProgress = false;
        this.selectedTowerType = null;
        this.draggedTowerType = null;
        this.dragPreview = null;
        this.selectedTower = null;
        this.powerUps = [];
        this.abilities = {
            freeze: { cooldown: 0, maxCooldown: 30000 }, // 30 seconds
            heal: { cooldown: 0, maxCooldown: 45000 }, // 45 seconds
            money: { cooldown: 0, maxCooldown: 60000 } // 60 seconds
        };
        
        // Difficulty settings
        this.difficultySettings = {
            easy: {
                enemySpeedMultiplier: 0.7,
                startingMoney: 800,
                enemyHealthMultiplier: 0.8,
                bossWaveInterval: 15,
                enemyRewardMultiplier: 1.2
            },
            medium: {
                enemySpeedMultiplier: 1.0,
                startingMoney: 500,
                enemyHealthMultiplier: 1.0,
                bossWaveInterval: 10,
                enemyRewardMultiplier: 1.0
            },
            hard: {
                enemySpeedMultiplier: 1.3,
                startingMoney: 300,
                enemyHealthMultiplier: 1.2,
                bossWaveInterval: 7,
                enemyRewardMultiplier: 0.8
            }
        };
        
        // Game objects
        this.towers = [];
        this.enemies = [];
        this.projectiles = [];
        this.particles = [];
        this.buildings = [];
        this.path = [];
        
        // Tower types with shots per minute (SPM) instead of fire rate
        this.towerTypes = {
            basic: { 
                cost: 50, damage: 25, range: 80, spm: 60, color: '#4a90e2', type: 'combat',
                description: 'Standard defensive tower with balanced stats.',
                icon: '🔫'
            },
            rapid: { 
                cost: 100, damage: 15, range: 70, spm: 120, color: '#ff6b6b', type: 'combat',
                description: 'Fast-firing tower perfect for swarms of weak enemies.',
                icon: '⚡'
            },
            heavy: { 
                cost: 200, damage: 60, range: 100, spm: 30, color: '#ffd700', type: 'combat',
                description: 'Powerful cannon that deals massive damage to tough enemies.',
                icon: '💥'
            },
            money: { 
                cost: 150, damage: 0, range: 0, spm: 0, color: '#32cd32', type: 'money', income: 25,
                description: 'Generates passive income between waves. Essential for economy!',
                icon: '💰'
            },
            ice: { 
                cost: 120, damage: 20, range: 90, spm: 50, color: '#87ceeb', type: 'combat', slowEffect: 0.5,
                description: 'Slows enemies by 50% for 3 seconds. Great for crowd control.',
                icon: '❄️'
            },
            poison: { 
                cost: 180, damage: 10, range: 85, spm: 75, color: '#9acd32', type: 'combat', poisonDamage: 5, poisonDuration: 5000,
                description: 'Poisons enemies dealing damage over time. Stacks with other towers.',
                icon: '☠️'
            },
            laser: { 
                cost: 300, damage: 80, range: 120, spm: 20, color: '#ff1493', type: 'combat', pierce: true,
                description: 'High-tech laser that pierces through multiple enemies.',
                icon: '🔴'
            },
            shield: { 
                cost: 250, damage: 0, range: 0, spm: 0, color: '#c0c0c0', type: 'support', shieldAmount: 20,
                description: 'Provides shields to nearby towers, reducing incoming damage.',
                icon: '🛡️'
            },
            sniper: { 
                cost: 400, damage: 150, range: 200, spm: 15, color: '#8B0000', type: 'combat', criticalChance: 0.3,
                description: 'Long-range sniper with high damage and critical hit chance.',
                icon: '🎯'
            },
            multi: { 
                cost: 350, damage: 30, range: 100, spm: 80, color: '#9370DB', type: 'combat', multiShot: 3,
                description: 'Fires multiple projectiles at once, great for crowd control.',
                icon: '💫'
            },
            emp: { 
                cost: 500, damage: 0, range: 120, spm: 10, color: '#FFD700', type: 'special', empDuration: 5000,
                description: 'Disables enemy abilities and slows them significantly.',
                icon: '⚡'
            }
        };
        
        // Enhanced enemy types
        this.enemyTypes = {
            basic: { health: 50, speed: 1, reward: 10, color: '#ff4444', size: 12 },
            fast: { health: 30, speed: 2, reward: 15, color: '#44ff44', size: 10 },
            tank: { health: 150, speed: 0.5, reward: 25, color: '#4444ff', size: 16 },
            flying: { health: 40, speed: 1.5, reward: 20, color: '#ff69b4', size: 14, flying: true },
            armored: { health: 200, speed: 0.8, reward: 35, color: '#8b4513', size: 18, armor: 0.3 },
            boss: { health: 500, speed: 0.3, reward: 100, color: '#8b0000', size: 25, boss: true }
        };
        
        // Tooltip system
        this.tooltip = document.getElementById('tooltip');
        this.tooltipTimeout = null;
        
        this.setupPath();
        this.setupBuildings();
        this.setupEventListeners();
        this.loadGameStats();
        this.showHomeScreen();
        this.gameLoop();
    }
    
    showHomeScreen() {
        document.getElementById('homeScreen').style.display = 'flex';
        document.getElementById('gameContainer').classList.remove('show');
        this.gameState = 'home';
    }
    
    hideHomeScreen() {
        document.getElementById('homeScreen').style.display = 'none';
        document.getElementById('gameContainer').classList.add('show');
    }
    
    setDifficulty(difficulty) {
        this.difficulty = difficulty;
        const settings = this.difficultySettings[difficulty];
        this.money = settings.startingMoney;
        this.updateUI();
    }
    
    loadGameStats() {
        const stats = JSON.parse(localStorage.getItem('towerDefenseStats') || '{}');
        document.getElementById('highScore').textContent = stats.highScore || 0;
        document.getElementById('gamesPlayed').textContent = stats.gamesPlayed || 0;
        document.getElementById('bestWave').textContent = stats.bestWave || 0;
    }
    
    saveGameStats() {
        const stats = JSON.parse(localStorage.getItem('towerDefenseStats') || '{}');
        stats.highScore = Math.max(stats.highScore || 0, this.score);
        stats.gamesPlayed = (stats.gamesPlayed || 0) + 1;
        stats.bestWave = Math.max(stats.bestWave || 0, this.wave - 1);
        localStorage.setItem('towerDefenseStats', JSON.stringify(stats));
        this.loadGameStats();
    }
    
    setupBuildings() {
        // Create decorative buildings and landscape elements
        this.buildings = [
            { x: 50, y: 100, type: 'house', size: 30 },
            { x: 150, y: 80, type: 'house', size: 25 },
            { x: 250, y: 120, type: 'house', size: 35 },
            { x: 400, y: 90, type: 'house', size: 28 },
            { x: 550, y: 110, type: 'house', size: 32 },
            { x: 700, y: 85, type: 'house', size: 30 },
            { x: 100, y: 400, type: 'tree', size: 20 },
            { x: 200, y: 380, type: 'tree', size: 25 },
            { x: 350, y: 420, type: 'tree', size: 22 },
            { x: 500, y: 400, type: 'tree', size: 28 },
            { x: 650, y: 390, type: 'tree', size: 24 },
            { x: 750, y: 410, type: 'tree', size: 26 }
        ];
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
        // Home screen difficulty selection
        document.querySelectorAll('.difficulty-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const difficulty = btn.dataset.difficulty;
                this.setDifficulty(difficulty);
                this.hideHomeScreen();
                this.startGame();
            });
        });
        
        // Canvas events for tower placement and selection
        this.canvas.addEventListener('click', (e) => {
            if (this.gameState === 'playing') {
                const rect = this.canvas.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;
                
                if (this.selectedTowerType) {
                    this.placeTower(x, y);
                } else {
                    this.selectTower(x, y);
                }
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

            // Tooltip events
            option.addEventListener('mouseenter', (e) => {
                const towerType = option.dataset.tower;
                this.showTooltip(e, towerType);
            });

            option.addEventListener('mouseleave', () => {
                this.hideTooltip();
            });

            option.addEventListener('mousemove', (e) => {
                if (this.tooltip.classList.contains('show')) {
                    this.updateTooltipPosition(e);
                }
            });
        });
        
        // Control buttons
        document.getElementById('start-wave').addEventListener('click', () => {
            this.startWave();
        });
        
        document.getElementById('pause-game').addEventListener('click', () => {
            this.togglePause();
        });
        
        document.getElementById('upgrade-tower').addEventListener('click', () => {
            this.upgradeSelectedTower();
        });
        
        document.getElementById('sell-tower').addEventListener('click', () => {
            this.sellSelectedTower();
        });
        
        document.getElementById('back-to-menu').addEventListener('click', () => {
            this.showHomeScreen();
        });
        
        document.getElementById('overlayButton').addEventListener('click', () => {
            this.startGame();
        });
        
        // Ability buttons
        document.getElementById('freeze-ability').addEventListener('click', () => {
            this.useAbility('freeze');
        });
        
        document.getElementById('heal-ability').addEventListener('click', () => {
            this.useAbility('heal');
        });
        
        document.getElementById('money-ability').addEventListener('click', () => {
            this.useAbility('money');
        });
    }
    
    startGame() {
        this.gameState = 'playing';
        document.getElementById('gameOverlay').classList.add('hidden');
    }
    
    selectTower(x, y) {
        // Find tower at click position
        for (const tower of this.towers) {
            const distance = Math.sqrt((x - tower.x) ** 2 + (y - tower.y) ** 2);
            if (distance <= tower.radius) {
                this.selectedTower = tower;
                this.selectedTowerType = null;
                document.querySelectorAll('.tower-option').forEach(opt => opt.classList.remove('selected'));
                return;
            }
        }
        // If no tower clicked, deselect
        this.selectedTower = null;
    }
    
    upgradeSelectedTower() {
        if (this.selectedTower && this.selectedTower.level < 3) {
            const upgradeCost = this.selectedTower.cost * 0.5;
            if (this.money >= upgradeCost) {
                this.money -= upgradeCost;
                this.selectedTower.level++;
                this.selectedTower.damage *= 1.5;
                this.selectedTower.range *= 1.2;
                this.updateUI();
            }
        }
    }
    
    sellSelectedTower() {
        if (this.selectedTower) {
            const sellValue = this.selectedTower.cost * 0.7;
            this.money += sellValue;
            this.towers = this.towers.filter(t => t !== this.selectedTower);
            this.selectedTower = null;
            this.updateUI();
        }
    }
    
    useAbility(abilityName) {
        const ability = this.abilities[abilityName];
        const now = Date.now();
        
        if (ability.cooldown > now) {
            return; // Still on cooldown
        }
        
        switch (abilityName) {
            case 'freeze':
                // Freeze all enemies for 5 seconds
                this.enemies.forEach(enemy => {
                    enemy.applySlow(0.9, 5000); // 90% slow for 5 seconds
                });
                ability.cooldown = now + ability.maxCooldown;
                break;
                
            case 'heal':
                // Heal base by 25 HP
                this.health = Math.min(100, this.health + 25);
                ability.cooldown = now + ability.maxCooldown;
                break;
                
            case 'money':
                // Give extra money
                this.money += 200;
                ability.cooldown = now + ability.maxCooldown;
                break;
        }
        
        this.updateAbilityUI();
        this.updateUI();
    }
    
    updateAbilityUI() {
        const now = Date.now();
        
        Object.keys(this.abilities).forEach(abilityName => {
            const ability = this.abilities[abilityName];
            const button = document.getElementById(`${abilityName}-ability`);
            const cooldownElement = button.querySelector('.ability-cooldown');
            
            if (ability.cooldown > now) {
                const remaining = Math.ceil((ability.cooldown - now) / 1000);
                button.disabled = true;
                cooldownElement.textContent = `${remaining}s`;
            } else {
                button.disabled = false;
                cooldownElement.textContent = 'Ready';
            }
        });
    }
    
    startWave() {
        if (!this.waveInProgress) {
            this.waveInProgress = true;
            this.spawnEnemies();
        }
    }
    
    spawnEnemies() {
        const settings = this.difficultySettings[this.difficulty];
        const enemyCount = 5 + this.wave * 2;
        const spawnDelay = 800;
        
        for (let i = 0; i < enemyCount; i++) {
            setTimeout(() => {
                let enemyType = 'basic';
                
                // Enhanced enemy spawning based on wave and difficulty
                if (this.wave > 2) {
                    const rand = Math.random();
                    if (rand < 0.3) enemyType = 'fast';
                    else if (rand < 0.5) enemyType = 'basic';
                }
                
                if (this.wave > 4) {
                    const rand = Math.random();
                    if (rand < 0.2) enemyType = 'flying';
                    else if (rand < 0.3) enemyType = 'tank';
                }
                
                if (this.wave > 6) {
                    const rand = Math.random();
                    if (rand < 0.15) enemyType = 'armored';
                    else if (rand < 0.2) enemyType = 'tank';
                }
                
                // Boss every N waves based on difficulty
                if (this.wave % settings.bossWaveInterval === 0 && i === enemyCount - 1) {
                    enemyType = 'boss';
                }
                
                const enemyStats = { ...this.enemyTypes[enemyType] };
                enemyStats.speed *= settings.enemySpeedMultiplier;
                enemyStats.health = Math.floor(enemyStats.health * settings.enemyHealthMultiplier);
                enemyStats.reward = Math.floor(enemyStats.reward * settings.enemyRewardMultiplier);
                
                this.enemies.push(new Enemy(this.path[0].x, this.path[0].y, enemyType, enemyStats));
                
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
    
    showTooltip(event, towerType) {
        const towerStats = this.towerTypes[towerType];
        const tooltipTitle = this.tooltip.querySelector('.tooltip-title');
        const tooltipStats = this.tooltip.querySelector('.tooltip-stats');
        const tooltipDescription = this.tooltip.querySelector('.tooltip-description');
        
        tooltipTitle.textContent = `${towerStats.icon} ${towerType.charAt(0).toUpperCase() + towerType.slice(1)} Tower`;
        
        let statsText = '';
        if (towerStats.type === 'combat') {
            statsText = `Damage: ${towerStats.damage}\nRange: ${towerStats.range}\nFire Rate: ${towerStats.spm} shots/min`;
            if (towerStats.slowEffect) statsText += `\nSlow: ${(towerStats.slowEffect * 100)}%`;
            if (towerStats.poisonDamage) statsText += `\nPoison: ${towerStats.poisonDamage}/s`;
            if (towerStats.pierce) statsText += '\nPierces enemies';
        } else if (towerStats.type === 'money') {
            statsText = `Income: $${towerStats.income}/3s`;
        } else if (towerStats.type === 'support') {
            statsText = `Shield: ${towerStats.shieldAmount}`;
        }
        
        tooltipStats.textContent = statsText;
        tooltipDescription.textContent = towerStats.description;
        
        this.updateTooltipPosition(event);
        this.tooltip.classList.add('show');
        
        // Clear any existing timeout
        if (this.tooltipTimeout) {
            clearTimeout(this.tooltipTimeout);
        }
    }
    
    hideTooltip() {
        this.tooltipTimeout = setTimeout(() => {
            this.tooltip.classList.remove('show');
        }, 100);
    }
    
    updateTooltipPosition(event) {
        const x = event.clientX + 10;
        const y = event.clientY - 10;
        this.tooltip.style.left = x + 'px';
        this.tooltip.style.top = y + 'px';
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
            const shouldContinue = projectile.update();
            if (!shouldContinue) return false;
            
            // Check collision with enemies
            for (let i = this.enemies.length - 1; i >= 0; i--) {
                const enemy = this.enemies[i];
                if (this.checkCollision(projectile, enemy)) {
                    // Apply damage
                    enemy.takeDamage(projectile.damage);
                    
                    // Apply special effects based on tower type
                    if (projectile.towerType === 'ice') {
                        enemy.applySlow(0.5, 3000); // 50% slow for 3 seconds
                    } else if (projectile.towerType === 'poison') {
                        enemy.applyPoison(5, 5000); // 5 damage/sec for 5 seconds
                    }
                    
                    // Handle laser piercing
                    if (projectile instanceof LaserProjectile) {
                        if (!projectile.hitEnemies.includes(enemy)) {
                            projectile.hitEnemies.push(enemy);
                            // Continue piercing if not too many hits
                            if (projectile.hitEnemies.length < 3) {
                                continue;
                            }
                        }
                    }
                    
                    // Remove enemy if dead
                    if (enemy.health <= 0) {
                        this.money += enemy.reward;
                        this.score += enemy.reward * 10; // Score is 10x the reward
                        this.enemies.splice(i, 1);
                    }
                    
                    // Remove projectile unless it's piercing
                    if (!(projectile instanceof LaserProjectile)) {
                        return false;
                    }
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
        // Clear canvas with enhanced background
        const gradient = this.ctx.createLinearGradient(0, 0, this.width, this.height);
        gradient.addColorStop(0, '#1a4d3a');
        gradient.addColorStop(0.5, '#2d5016');
        gradient.addColorStop(1, '#1a4d3a');
        this.ctx.fillStyle = gradient;
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        // Draw grass texture
        this.drawGrass();
        
        // Draw buildings
        this.drawBuildings();
        
        // Draw atmospheric effects
        this.drawAtmosphere();
        
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
        
        // Draw particle effects
        this.drawParticles();
        
        // Draw selected tower highlight
        if (this.selectedTower) {
            this.drawSelectedTowerHighlight();
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
    
    drawGrass() {
        // Draw grass texture
        this.ctx.save();
        this.ctx.globalAlpha = 0.3;
        for (let i = 0; i < 200; i++) {
            const x = Math.random() * this.width;
            const y = Math.random() * this.height;
            this.ctx.fillStyle = '#228B22';
            this.ctx.fillRect(x, y, 2, 8);
        }
        this.ctx.restore();
    }
    
    drawBuildings() {
        this.buildings.forEach(building => {
            this.ctx.save();
            
            if (building.type === 'house') {
                // Draw house
                this.ctx.fillStyle = '#8B4513';
                this.ctx.fillRect(building.x - building.size/2, building.y - building.size/2, building.size, building.size);
                
                // Draw roof
                this.ctx.fillStyle = '#DC143C';
                this.ctx.beginPath();
                this.ctx.moveTo(building.x - building.size/2, building.y - building.size/2);
                this.ctx.lineTo(building.x, building.y - building.size);
                this.ctx.lineTo(building.x + building.size/2, building.y - building.size/2);
                this.ctx.closePath();
                this.ctx.fill();
                
                // Draw door
                this.ctx.fillStyle = '#654321';
                this.ctx.fillRect(building.x - 3, building.y - building.size/2, 6, building.size/2);
                
                // Draw windows
                this.ctx.fillStyle = '#87CEEB';
                this.ctx.fillRect(building.x - building.size/3, building.y - building.size/3, 4, 4);
                this.ctx.fillRect(building.x + building.size/6, building.y - building.size/3, 4, 4);
                
            } else if (building.type === 'tree') {
                // Draw tree trunk
                this.ctx.fillStyle = '#8B4513';
                this.ctx.fillRect(building.x - 3, building.y - building.size/2, 6, building.size);
                
                // Draw tree leaves
                this.ctx.fillStyle = '#228B22';
                this.ctx.beginPath();
                this.ctx.arc(building.x, building.y - building.size/2, building.size/2, 0, Math.PI * 2);
                this.ctx.fill();
            }
            
            this.ctx.restore();
        });
    }
    
    drawSelectedTowerHighlight() {
        this.ctx.save();
        this.ctx.strokeStyle = '#ffff00';
        this.ctx.lineWidth = 3;
        this.ctx.beginPath();
        this.ctx.arc(this.selectedTower.x, this.selectedTower.y, this.selectedTower.radius + 5, 0, Math.PI * 2);
        this.ctx.stroke();
        
        // Draw level indicator
        this.ctx.fillStyle = '#ffff00';
        this.ctx.font = 'bold 12px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText(`L${this.selectedTower.level}`, this.selectedTower.x, this.selectedTower.y - this.selectedTower.radius - 10);
        
        this.ctx.restore();
    }
    
    drawAtmosphere() {
        // Draw floating particles
        const time = Date.now() * 0.001;
        for (let i = 0; i < 20; i++) {
            const x = (Math.sin(time + i) * 100 + i * 40) % this.width;
            const y = (Math.cos(time * 0.5 + i) * 50 + i * 30) % this.height;
            const alpha = Math.sin(time + i) * 0.3 + 0.3;
            
            this.ctx.globalAlpha = alpha;
            this.ctx.fillStyle = '#87ceeb';
            this.ctx.beginPath();
            this.ctx.arc(x, y, 1, 0, Math.PI * 2);
            this.ctx.fill();
        }
        this.ctx.globalAlpha = 1;
        
        // Draw ambient lighting
        const radialGradient = this.ctx.createRadialGradient(
            this.width * 0.3, this.height * 0.7, 0,
            this.width * 0.3, this.height * 0.7, 200
        );
        radialGradient.addColorStop(0, 'rgba(0, 255, 150, 0.1)');
        radialGradient.addColorStop(1, 'rgba(0, 255, 150, 0)');
        this.ctx.fillStyle = radialGradient;
        this.ctx.fillRect(0, 0, this.width, this.height);
    }
    
    drawParticles() {
        // Draw explosion particles for destroyed enemies
        const time = Date.now();
        for (let i = this.projectiles.length - 1; i >= 0; i--) {
            const projectile = this.projectiles[i];
            if (projectile.trail && projectile.trail.length > 0) {
                this.ctx.save();
                for (let j = 0; j < projectile.trail.length; j++) {
                    const trail = projectile.trail[j];
                    const alpha = (j + 1) / projectile.trail.length;
                    this.ctx.globalAlpha = alpha * 0.6;
                    this.ctx.fillStyle = projectile.color;
                    this.ctx.beginPath();
                    this.ctx.arc(trail.x, trail.y, 2, 0, Math.PI * 2);
                    this.ctx.fill();
                }
                this.ctx.restore();
            }
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
        document.getElementById('score').textContent = this.score;
        
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
        
        // Update upgrade/sell buttons
        const upgradeBtn = document.getElementById('upgrade-tower');
        const sellBtn = document.getElementById('sell-tower');
        
        if (this.selectedTower) {
            const upgradeCost = this.selectedTower.cost * 0.5;
            upgradeBtn.disabled = this.selectedTower.level >= 3 || this.money < upgradeCost;
            upgradeBtn.textContent = `Upgrade ($${upgradeCost})`;
            sellBtn.disabled = false;
            sellBtn.textContent = `Sell ($${Math.floor(this.selectedTower.cost * 0.7)})`;
        } else {
            upgradeBtn.disabled = true;
            upgradeBtn.textContent = 'Upgrade Tower';
            sellBtn.disabled = true;
            sellBtn.textContent = 'Sell Tower';
        }
    }
    
    gameOver() {
        this.gameState = 'gameOver';
        this.saveGameStats();
        document.getElementById('overlayTitle').textContent = 'Game Over!';
        document.getElementById('overlayMessage').textContent = `You survived ${this.wave - 1} waves!\nFinal Score: ${this.score}`;
        document.getElementById('overlayButton').textContent = 'Play Again';
        document.getElementById('gameOverlay').classList.remove('hidden');
        
        document.getElementById('overlayButton').onclick = () => {
            this.resetGame();
        };
    }
    
    resetGame() {
        const settings = this.difficultySettings[this.difficulty];
        this.health = 100;
        this.money = settings.startingMoney;
        this.wave = 1;
        this.score = 0;
        this.waveInProgress = false;
        this.towers = [];
        this.enemies = [];
        this.projectiles = [];
        this.selectedTowerType = null;
        this.selectedTower = null;
        document.querySelectorAll('.tower-option').forEach(opt => opt.classList.remove('selected'));
        this.startGame();
    }
    
    gameLoop() {
        this.update();
        this.render();
        this.updateAbilityUI();
        requestAnimationFrame(() => this.gameLoop());
    }
}

// Enhanced Enemy class
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
        this.radius = stats.size || 12;
        this.pathIndex = 0;
        this.lastUpdate = Date.now();
        
        // Enhanced properties
        this.armor = stats.armor || 0;
        this.flying = stats.flying || false;
        this.boss = stats.boss || false;
        this.slowEffect = 0;
        this.slowDuration = 0;
        this.poisonDamage = 0;
        this.poisonDuration = 0;
        this.shield = 0;
        this.empDisabled = false;
        this.empEndTime = 0;
    }
    
    update() {
        const now = Date.now();
        const deltaTime = now - this.lastUpdate;
        this.lastUpdate = now;
        
        // Apply poison damage
        if (this.poisonDuration > 0) {
            this.poisonDuration -= deltaTime;
            if (this.poisonDuration > 0) {
                this.takeDamage(this.poisonDamage * deltaTime / 1000, false);
            }
        }
        
        // Apply slow effect
        if (this.slowDuration > 0) {
            this.slowDuration -= deltaTime;
            if (this.slowDuration <= 0) {
                this.slowEffect = 0;
            }
        }
        
        // Check EMP effect
        if (this.empDisabled && now >= this.empEndTime) {
            this.empDisabled = false;
        }
        
        if (this.pathIndex < game.path.length - 1) {
            const target = game.path[this.pathIndex + 1];
            const dx = target.x - this.x;
            const dy = target.y - this.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            if (distance < 5) {
                this.pathIndex++;
            } else {
                const currentSpeed = this.speed * (1 - this.slowEffect);
                const moveDistance = currentSpeed * deltaTime * 0.1;
                this.x += (dx / distance) * moveDistance;
                this.y += (dy / distance) * moveDistance;
            }
        }
    }
    
    takeDamage(damage, applyArmor = true) {
        let actualDamage = damage;
        
        // Apply armor reduction
        if (applyArmor && this.armor > 0) {
            actualDamage *= (1 - this.armor);
        }
        
        // Apply shield
        if (this.shield > 0) {
            const shieldAbsorbed = Math.min(actualDamage, this.shield);
            this.shield -= shieldAbsorbed;
            actualDamage -= shieldAbsorbed;
        }
        
        this.health -= actualDamage;
    }
    
    applySlow(slowAmount, duration) {
        this.slowEffect = Math.max(this.slowEffect, slowAmount);
        this.slowDuration = Math.max(this.slowDuration, duration);
    }
    
    applyPoison(damage, duration) {
        this.poisonDamage = Math.max(this.poisonDamage, damage);
        this.poisonDuration = Math.max(this.poisonDuration, duration);
    }
    
    applyShield(amount) {
        this.shield += amount;
    }
    
    render(ctx) {
        // Draw enemy body with enhanced visuals
        ctx.save();
        
        // Boss enemies get special effects
        if (this.boss) {
            const pulse = Math.sin(Date.now() * 0.01) * 0.3 + 0.7;
            ctx.globalAlpha = pulse;
            
            // Boss aura
            ctx.strokeStyle = '#ff0000';
            ctx.lineWidth = 3;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 5, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        // Flying enemies get wings
        if (this.flying) {
            ctx.fillStyle = '#ffffff';
            ctx.beginPath();
            ctx.arc(this.x - 8, this.y - 5, 3, 0, Math.PI * 2);
            ctx.arc(this.x + 8, this.y - 5, 3, 0, Math.PI * 2);
            ctx.fill();
        }
        
        // Main body
        ctx.fillStyle = this.color;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        
        // Armor indicator
        if (this.armor > 0) {
            ctx.strokeStyle = '#c0c0c0';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 2, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        // Shield indicator
        if (this.shield > 0) {
            ctx.strokeStyle = '#00ffff';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 4, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        // Status effects
        if (this.slowEffect > 0) {
            ctx.fillStyle = '#87ceeb';
            ctx.beginPath();
            ctx.arc(this.x, this.y - this.radius - 8, 3, 0, Math.PI * 2);
            ctx.fill();
        }
        
        if (this.poisonDuration > 0) {
            ctx.fillStyle = '#9acd32';
            ctx.beginPath();
            ctx.arc(this.x + this.radius + 8, this.y - this.radius - 8, 3, 0, Math.PI * 2);
            ctx.fill();
        }
        
        // EMP effect indicator
        if (this.empDisabled) {
            ctx.fillStyle = '#FFD700';
            ctx.beginPath();
            ctx.arc(this.x - this.radius - 8, this.y - this.radius - 8, 3, 0, Math.PI * 2);
            ctx.fill();
        }
        
        ctx.restore();
        
        // Enhanced health bar
        const barWidth = this.boss ? 30 : 20;
        const barHeight = this.boss ? 6 : 4;
        const barX = this.x - barWidth / 2;
        const barY = this.y - this.radius - 12;
        
        // Background
        ctx.fillStyle = '#333';
        ctx.fillRect(barX - 1, barY - 1, barWidth + 2, barHeight + 2);
        
        // Health bar
        ctx.fillStyle = '#ff0000';
        ctx.fillRect(barX, barY, barWidth, barHeight);
        
        const healthPercent = this.health / this.maxHealth;
        ctx.fillStyle = this.boss ? '#ff6600' : '#00ff00';
        ctx.fillRect(barX, barY, barWidth * healthPercent, barHeight);
        
        // Boss crown
        if (this.boss) {
            ctx.fillStyle = '#ffd700';
            ctx.font = 'bold 12px Arial';
            ctx.textAlign = 'center';
            ctx.fillText('👑', this.x, this.y - this.radius - 20);
        }
    }
}

// Enhanced Tower class
class Tower {
    constructor(x, y, type, stats) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.towerType = stats.type || 'combat';
        this.damage = stats.damage;
        this.range = stats.range;
        this.spm = stats.spm; // Shots per minute
        this.fireRate = stats.spm > 0 ? 60000 / stats.spm : 0; // Convert SPM to milliseconds
        this.color = stats.color;
        this.radius = 15;
        this.lastShot = 0;
        this.target = null;
        this.income = stats.income || 0;
        this.lastMoneyGeneration = 0;
        this.cost = stats.cost;
        
        // Enhanced properties
        this.slowEffect = stats.slowEffect || 0;
        this.poisonDamage = stats.poisonDamage || 0;
        this.poisonDuration = stats.poisonDuration || 0;
        this.pierce = stats.pierce || false;
        this.shieldAmount = stats.shieldAmount || 0;
        this.criticalChance = stats.criticalChance || 0;
        this.multiShot = stats.multiShot || 0;
        this.empDuration = stats.empDuration || 0;
        this.level = 1;
        this.experience = 0;
    }
    
    update(enemies, projectiles) {
        const now = Date.now();
        
        if (this.towerType === 'combat') {
            // Find target
            this.target = this.findTarget(enemies);
            
            // Shoot if target is in range and enough time has passed
            if (this.target && now - this.lastShot >= this.fireRate) {
                this.shoot(projectiles);
                this.lastShot = now;
            }
        } else if (this.towerType === 'support') {
            // Apply shields to nearby towers
            this.applyShields();
        } else if (this.towerType === 'special') {
            // EMP tower - apply EMP effect to enemies
            this.applyEMPEffect(enemies);
        }
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
            if (this.pierce) {
                // Laser pierces through multiple enemies
                projectiles.push(new LaserProjectile(
                    this.x, this.y,
                    this.target,
                    this.damage, this.color
                ));
            } else if (this.multiShot) {
                // Multi-shot tower fires multiple projectiles
                for (let i = 0; i < this.multiShot; i++) {
                    projectiles.push(new Projectile(
                        this.x, this.y,
                        this.target,
                        this.damage, this.color, this.type
                    ));
                }
            } else if (this.criticalChance) {
                // Sniper tower with critical hit chance
                const isCritical = Math.random() < this.criticalChance;
                const damage = isCritical ? this.damage * 2 : this.damage;
                projectiles.push(new Projectile(
                    this.x, this.y,
                    this.target,
                    damage, this.color, this.type
                ));
            } else {
                // Standard projectile
                projectiles.push(new Projectile(
                    this.x, this.y,
                    this.target,
                    this.damage, this.color, this.type
                ));
            }
        }
    }
    
    applyShields() {
        // Find nearby towers and apply shields
        for (const tower of game.towers) {
            if (tower !== this && tower.towerType === 'combat') {
                const distance = Math.sqrt((tower.x - this.x) ** 2 + (tower.y - this.y) ** 2);
                if (distance <= this.range) {
                    // Apply shield effect (visual only for now)
                    tower.hasShield = true;
                }
            }
        }
    }
    
    applyEMPEffect(enemies) {
        const now = Date.now();
        
        // Apply EMP effect every few seconds
        if (now - this.lastShot >= this.fireRate) {
            for (const enemy of enemies) {
                const distance = Math.sqrt((enemy.x - this.x) ** 2 + (enemy.y - this.y) ** 2);
                if (distance <= this.range) {
                    // Apply EMP effect - slow and disable abilities
                    enemy.applySlow(0.7, this.empDuration); // 70% slow
                    enemy.empDisabled = true;
                    enemy.empEndTime = now + this.empDuration;
                }
            }
            this.lastShot = now;
        }
    }
    
    shouldGenerateMoney() {
        const now = Date.now();
        return now - this.lastMoneyGeneration >= 3000;
    }
    
    render(ctx) {
        ctx.save();
        
        // Draw tower base with enhanced visuals
        const gradient = ctx.createRadialGradient(this.x, this.y, 0, this.x, this.y, this.radius);
        gradient.addColorStop(0, this.color);
        gradient.addColorStop(1, this.darkenColor(this.color, 0.3));
        
        ctx.fillStyle = gradient;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        
        // Draw tower border
        ctx.strokeStyle = this.lightenColor(this.color, 0.3);
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.stroke();
        
        // Draw tower icon
        ctx.fillStyle = '#ffffff';
        ctx.font = 'bold 14px Arial';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        
        let icon = this.getTowerIcon();
        ctx.fillText(icon, this.x, this.y);
        
        // Special effects for different tower types
        if (this.towerType === 'money') {
            // Money tower pulsing effect
            const pulse = Math.sin(Date.now() * 0.005) * 0.2 + 0.8;
            ctx.strokeStyle = `rgba(255, 215, 0, ${pulse})`;
            ctx.lineWidth = 3;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 5, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        if (this.towerType === 'ice') {
            // Ice tower frost effect
            ctx.strokeStyle = 'rgba(135, 206, 235, 0.6)';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 3, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        if (this.towerType === 'poison') {
            // Poison tower toxic aura
            ctx.strokeStyle = 'rgba(154, 205, 50, 0.6)';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 3, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        if (this.towerType === 'laser') {
            // Laser tower energy field
            const energy = Math.sin(Date.now() * 0.01) * 0.3 + 0.7;
            ctx.strokeStyle = `rgba(255, 20, 147, ${energy})`;
            ctx.lineWidth = 3;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 4, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        if (this.towerType === 'support') {
            // Shield tower protective aura
            ctx.strokeStyle = 'rgba(192, 192, 192, 0.6)';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 3, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        if (this.towerType === 'special') {
            // EMP tower energy field
            const energy = Math.sin(Date.now() * 0.008) * 0.4 + 0.6;
            ctx.strokeStyle = `rgba(255, 215, 0, ${energy})`;
            ctx.lineWidth = 3;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 5, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        if (this.criticalChance > 0) {
            // Sniper tower scope effect
            ctx.strokeStyle = 'rgba(139, 0, 0, 0.6)';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 4, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        if (this.multiShot > 0) {
            // Multi tower burst effect
            ctx.strokeStyle = 'rgba(147, 112, 219, 0.6)';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 3, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        // Draw range circle when targeting (only for combat towers)
        if (this.target && this.towerType === 'combat') {
            ctx.strokeStyle = 'rgba(255, 255, 255, 0.3)';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.range, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        // Draw shield indicator
        if (this.hasShield) {
            ctx.strokeStyle = '#00ffff';
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.radius + 6, 0, Math.PI * 2);
            ctx.stroke();
        }
        
        ctx.restore();
    }
    
    getTowerIcon() {
        const icons = {
            basic: '🔫',
            rapid: '⚡',
            heavy: '💥',
            money: '💰',
            ice: '❄️',
            poison: '☠️',
            laser: '🔴',
            shield: '🛡️',
            sniper: '🎯',
            multi: '💫',
            emp: '⚡'
        };
        return icons[this.type] || '🔫';
    }
    
    darkenColor(color, amount) {
        const hex = color.replace('#', '');
        const r = Math.max(0, parseInt(hex.substr(0, 2), 16) * (1 - amount));
        const g = Math.max(0, parseInt(hex.substr(2, 2), 16) * (1 - amount));
        const b = Math.max(0, parseInt(hex.substr(4, 2), 16) * (1 - amount));
        return `rgb(${Math.floor(r)}, ${Math.floor(g)}, ${Math.floor(b)})`;
    }
    
    lightenColor(color, amount) {
        const hex = color.replace('#', '');
        const r = Math.min(255, parseInt(hex.substr(0, 2), 16) + (255 - parseInt(hex.substr(0, 2), 16)) * amount);
        const g = Math.min(255, parseInt(hex.substr(2, 2), 16) + (255 - parseInt(hex.substr(2, 2), 16)) * amount);
        const b = Math.min(255, parseInt(hex.substr(4, 2), 16) + (255 - parseInt(hex.substr(4, 2), 16)) * amount);
        return `rgb(${Math.floor(r)}, ${Math.floor(g)}, ${Math.floor(b)})`;
    }
}

// Enhanced Projectile class with homing
class Projectile {
    constructor(x, y, target, damage, color, towerType) {
        this.x = x;
        this.y = y;
        this.target = target;
        this.damage = damage;
        this.color = color;
        this.radius = 3;
        this.towerType = towerType;
        this.trail = [];
        this.speed = 4;
        this.homingStrength = 0.1; // How much to adjust towards target
        this.maxAge = 3000; // 3 seconds max lifetime
        this.age = 0;
        
        // Initial velocity towards target
        if (target) {
            const dx = target.x - x;
            const dy = target.y - y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            this.vx = (dx / distance) * this.speed;
            this.vy = (dy / distance) * this.speed;
        } else {
            this.vx = 0;
            this.vy = 0;
        }
    }
    
    update() {
        this.age += 16; // Assuming 60fps
        
        // Add current position to trail
        this.trail.push({ x: this.x, y: this.y });
        if (this.trail.length > 8) {
            this.trail.shift();
        }
        
        // Homing behavior - adjust velocity towards target
        if (this.target && this.target.health > 0) {
            const dx = this.target.x - this.x;
            const dy = this.target.y - this.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            if (distance > 0) {
                // Calculate desired velocity towards target
                const desiredVx = (dx / distance) * this.speed;
                const desiredVy = (dy / distance) * this.speed;
                
                // Gradually adjust current velocity towards desired velocity
                this.vx += (desiredVx - this.vx) * this.homingStrength;
                this.vy += (desiredVy - this.vy) * this.homingStrength;
            }
        }
        
        // Move projectile
        this.x += this.vx;
        this.y += this.vy;
        
        // Check if projectile is too old
        return this.age < this.maxAge;
    }
    
    render(ctx) {
        // Draw trail
        ctx.save();
        for (let i = 0; i < this.trail.length; i++) {
            const alpha = (i + 1) / this.trail.length;
            ctx.globalAlpha = alpha * 0.5;
            ctx.fillStyle = this.color;
            ctx.beginPath();
            ctx.arc(this.trail[i].x, this.trail[i].y, this.radius * alpha, 0, Math.PI * 2);
            ctx.fill();
        }
        ctx.restore();
        
        // Draw main projectile
        ctx.fillStyle = this.color;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        
        // Add glow effect
        ctx.shadowColor = this.color;
        ctx.shadowBlur = 5;
        ctx.fillStyle = this.color;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        ctx.shadowBlur = 0;
    }
}

// Laser Projectile class for piercing attacks with homing
class LaserProjectile {
    constructor(x, y, target, damage, color) {
        this.x = x;
        this.y = y;
        this.target = target;
        this.damage = damage;
        this.color = color;
        this.hitEnemies = [];
        this.lifetime = 1000; // 1 second
        this.startTime = Date.now();
        this.speed = 6;
        this.homingStrength = 0.15; // Stronger homing for laser
        
        // Initial velocity towards target
        if (target) {
            const dx = target.x - x;
            const dy = target.y - y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            this.vx = (dx / distance) * this.speed;
            this.vy = (dy / distance) * this.speed;
        } else {
            this.vx = 0;
            this.vy = 0;
        }
    }
    
    update() {
        // Homing behavior for laser
        if (this.target && this.target.health > 0) {
            const dx = this.target.x - this.x;
            const dy = this.target.y - this.y;
            const distance = Math.sqrt(dx * dx + dy * dy);
            
            if (distance > 0) {
                const desiredVx = (dx / distance) * this.speed;
                const desiredVy = (dy / distance) * this.speed;
                
                this.vx += (desiredVx - this.vx) * this.homingStrength;
                this.vy += (desiredVy - this.vy) * this.homingStrength;
            }
        }
        
        this.x += this.vx;
        this.y += this.vy;
        
        // Check if laser should expire
        if (Date.now() - this.startTime > this.lifetime) {
            return false;
        }
        
        return true;
    }
    
    render(ctx) {
        // Draw laser beam
        ctx.save();
        
        // Create gradient for laser effect
        const gradient = ctx.createLinearGradient(this.x - this.vx * 10, this.y - this.vy * 10, this.x, this.y);
        gradient.addColorStop(0, 'rgba(255, 255, 255, 0)');
        gradient.addColorStop(0.5, this.color);
        gradient.addColorStop(1, this.color);
        
        ctx.strokeStyle = gradient;
        ctx.lineWidth = 4;
        ctx.lineCap = 'round';
        ctx.beginPath();
        ctx.moveTo(this.x - this.vx * 20, this.y - this.vy * 20);
        ctx.lineTo(this.x, this.y);
        ctx.stroke();
        
        // Add glow effect
        ctx.shadowColor = this.color;
        ctx.shadowBlur = 10;
        ctx.strokeStyle = gradient;
        ctx.lineWidth = 6;
        ctx.beginPath();
        ctx.moveTo(this.x - this.vx * 20, this.y - this.vy * 20);
        ctx.lineTo(this.x, this.y);
        ctx.stroke();
        ctx.shadowBlur = 0;
        
        ctx.restore();
    }
}

// Initialize game when page loads
let game;
window.addEventListener('load', () => {
    game = new TowerDefenseGame();
});
