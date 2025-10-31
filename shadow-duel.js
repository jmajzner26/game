// Shadow Duel - Browser Version
class ShadowDuelGame {
    constructor() {
        this.canvas = document.getElementById('shadow-duel-canvas');
        this.ctx = this.canvas.getContext('2d');
        this.width = this.canvas.width = 800;
        this.height = this.canvas.height = 600;
        
        // Game state
        this.gameState = 'menu'; // menu, playing, paused, gameOver
        this.player = {
            x: 200,
            y: 400,
            width: 40,
            height: 60,
            health: 100,
            maxHealth: 100,
            stamina: 100,
            maxStamina: 100,
            facingRight: true,
            attacking: false,
            attackCooldown: 0,
            attackHitFrames: 0,
            parrying: false,
            dodging: false,
            dodgeCooldown: 0,
            velocity: { x: 0, y: 0 },
            onGround: true,
            color: '#4ecdc4'
        };
        
        this.enemy = {
            x: 600,
            y: 400,
            width: 40,
            height: 60,
            health: 100,
            maxHealth: 100,
            stamina: 100,
            maxStamina: 100,
            facingRight: false,
            attacking: false,
            attackCooldown: 0,
            attackHitFrames: 0,
            parrying: false,
            velocity: { x: 0, y: 0 },
            onGround: true,
            aiCooldown: 0,
            color: '#f5576c'
        };
        
        this.keys = {};
        this.mouse = { x: 0, y: 0 };
        this.camera = { x: 0, y: 0 };
        this.comboCount = 0;
        this.comboTimer = 0;
        this.score = 0;
        this.slowMotion = false;
        this.slowMotionTimer = 0;
        this.particles = [];
        this.hitEffects = [];
        this.hasHitThisAttack = false;
        this.enemyHasHitThisAttack = false;
        
        this.setupControls();
        this.gameLoop();
    }
    
    setupControls() {
        document.addEventListener('keydown', (e) => {
            this.keys[e.key.toLowerCase()] = true;
            
            if (e.key === 'p' || e.key === 'P') {
                if (this.gameState === 'playing') {
                    this.gameState = 'paused';
                } else if (this.gameState === 'paused') {
                    this.gameState = 'playing';
                }
            }
            
            if (this.gameState === 'menu' && e.key === ' ') {
                this.startGame();
            }
        });
        
        document.addEventListener('keyup', (e) => {
            this.keys[e.key.toLowerCase()] = false;
        });
        
        this.canvas.addEventListener('click', (e) => {
            if (this.gameState === 'menu') {
                this.startGame();
            }
        });
    }
    
    startGame() {
        this.gameState = 'playing';
        this.player.health = 100;
        this.enemy.health = 100;
        this.player.x = 200;
        this.enemy.x = 600;
        this.score = 0;
        this.comboCount = 0;
        this.slowMotion = false;
        this.particles = [];
        this.hitEffects = [];
    }
    
    update(deltaTime) {
        if (this.gameState !== 'playing') return;
        
        const dt = this.slowMotion ? deltaTime * 0.2 : deltaTime;
        
        // Update timers
        this.comboTimer = Math.max(0, this.comboTimer - dt);
        if (this.comboTimer <= 0) {
            this.comboCount = 0;
        }
        
        if (this.slowMotion) {
            this.slowMotionTimer -= dt;
            if (this.slowMotionTimer <= 0) {
                this.slowMotion = false;
            }
        }
        
        // Player controls
        this.updatePlayer(dt);
        
        // Enemy AI
        this.updateEnemy(dt);
        
        // Physics
        this.updatePhysics(dt);
        
        // Combat collision
        this.checkCombat(dt);
        
        // Update effects
        this.updateEffects(dt);
        
        // Camera
        this.updateCamera();
        
        // Check game over
        if (this.player.health <= 0 || this.enemy.health <= 0) {
            this.gameState = 'gameOver';
        }
    }
    
    updatePlayer(dt) {
        const player = this.player;
        
        // Gravity
        if (!player.onGround) {
            player.velocity.y += 800 * dt; // gravity
        } else if (player.velocity.y > 0) {
            player.velocity.y = 0;
        }
        
        // Movement
        if (!player.attacking && !player.dodging && !player.parrying) {
            let moving = false;
            
            if (this.keys['a']) {
                player.velocity.x = -200;
                player.facingRight = false;
                moving = true;
            } else if (this.keys['d']) {
                player.velocity.x = 200;
                player.facingRight = true;
                moving = true;
            } else {
                player.velocity.x *= 0.8; // friction
            }
            
            // Stamina regen when not moving
            if (!moving && player.stamina < player.maxStamina) {
                player.stamina = Math.min(player.maxStamina, player.stamina + 30 * dt);
            }
        }
        
        // Attacking
        if (this.keys[' '] && !player.attacking && !player.parrying && !player.dodging && player.stamina >= 15) {
            player.attacking = true;
            player.attackCooldown = 0.4;
            player.attackHitFrames = 0.2; // Hitbox active for 0.2 seconds
            player.stamina -= 15;
            this.hasHitThisAttack = false; // Reset hit flag for new attack
        }
        
        if (player.attacking) {
            player.attackCooldown -= dt;
            player.attackHitFrames -= dt;
            if (player.attackCooldown <= 0) {
                player.attacking = false;
                this.hasHitThisAttack = false;
            }
        }
        
        // Parrying
        if (this.keys['s'] && !player.parrying && !player.attacking && !player.dodging && player.stamina >= 20) {
            player.parrying = true;
            player.stamina -= 20;
        }
        
        if (player.parrying) {
            player.parrying = false; // Single frame
        }
        
        // Dodging
        if (this.keys['shift'] && !player.dodging && !player.attacking && !player.parrying && player.stamina >= 15) {
            player.dodging = true;
            player.dodgeCooldown = 0.3;
            player.stamina -= 15;
            this.createDodgeEffect(player);
        }
        
        if (player.dodging) {
            player.velocity.x = player.facingRight ? 400 : -400;
            player.dodgeCooldown -= dt;
            if (player.dodgeCooldown <= 0) {
                player.dodging = false;
            }
        }
    }
    
    updateEnemy(dt) {
        const enemy = this.enemy;
        const player = this.player;
        
        // Gravity
        if (!enemy.onGround) {
            enemy.velocity.y += 800 * dt;
        } else if (enemy.velocity.y > 0) {
            enemy.velocity.y = 0;
        }
        
        // Simple AI
        if (enemy.aiCooldown <= 0) {
            const distance = player.x - enemy.x;
            
            // Attack if close enough
            if (Math.abs(distance) < 100 && !enemy.attacking && enemy.stamina >= 15) {
                enemy.attacking = true;
                enemy.attackCooldown = 0.4;
                enemy.attackHitFrames = 0.2; // Hitbox active for 0.2 seconds
                enemy.stamina -= 15;
                enemy.aiCooldown = 1.0;
                this.enemyHasHitThisAttack = false; // Reset hit flag for new attack
            }
            // Try to parry if player is attacking
            else if (player.attacking && Math.abs(distance) < 100 && enemy.stamina >= 20) {
                enemy.parrying = true;
                enemy.stamina -= 20;
                enemy.aiCooldown = 0.5;
                this.createParryEffect(enemy);
            }
            // Move towards player
            else if (Math.abs(distance) > 80 && !enemy.attacking && !enemy.parrying) {
                enemy.velocity.x = distance > 0 ? 150 : -150;
                enemy.facingRight = distance > 0;
                enemy.aiCooldown = 0.2;
            }
        } else {
            enemy.aiCooldown -= dt;
        }
        
        if (enemy.attacking) {
            enemy.attackCooldown -= dt;
            enemy.attackHitFrames -= dt;
            if (enemy.attackCooldown <= 0) {
                enemy.attacking = false;
                this.enemyHasHitThisAttack = false;
            }
        }
        
        if (enemy.parrying) {
            enemy.parrying = false; // Single frame
        }
        
        // Stamina regen
        if (enemy.stamina < enemy.maxStamina) {
            enemy.stamina = Math.min(enemy.maxStamina, enemy.stamina + 25 * dt);
        }
        
        enemy.velocity.x *= 0.8; // Friction
    }
    
    updatePhysics(dt) {
        // Update positions
        this.player.x += this.player.velocity.x * dt;
        this.player.y += this.player.velocity.y * dt;
        this.enemy.x += this.enemy.velocity.x * dt;
        this.enemy.y += this.enemy.velocity.y * dt;
        
        // Ground collision
        const groundY = 450;
        if (this.player.y + this.player.height > groundY) {
            this.player.y = groundY - this.player.height;
            this.player.onGround = true;
        } else {
            this.player.onGround = false;
        }
        
        if (this.enemy.y + this.enemy.height > groundY) {
            this.enemy.y = groundY - this.enemy.height;
            this.enemy.onGround = true;
        } else {
            this.enemy.onGround = false;
        }
        
        // Boundary collision
        const boundary = 50;
        this.player.x = Math.max(boundary, Math.min(this.width - boundary, this.player.x));
        this.enemy.x = Math.max(boundary, Math.min(this.width - boundary, this.enemy.x));
    }
    
    checkCombat(dt) {
        const player = this.player;
        const enemy = this.enemy;
        const distance = Math.abs(player.x - enemy.x);
        
        // Player attacking enemy - only hit during hitbox window and once per attack
        if (player.attacking && player.attackHitFrames > 0 && !enemy.parrying && distance < 100 && !this.hasHitThisAttack) {
            enemy.health -= 10;
            this.comboCount++;
            this.comboTimer = 2.0;
            this.createHitEffect(enemy.x, enemy.y);
            this.hasHitThisAttack = true; // Prevent multiple hits from same attack
            
            if (enemy.health <= 0) {
                this.slowMotion = true;
                this.slowMotionTimer = 1.5;
                this.createFinisherEffect();
                this.score += 1000;
            }
        }
        
        // Enemy attacking player - only hit during hitbox window and once per attack
        if (enemy.attacking && enemy.attackHitFrames > 0 && !player.parrying && !player.dodging && distance < 100 && !this.enemyHasHitThisAttack) {
            player.health -= 10;
            this.createHitEffect(player.x, player.y);
            this.enemyHasHitThisAttack = true; // Prevent multiple hits from same attack
        }
        
        // Perfect parry
        if (player.parrying && enemy.attacking && distance < 100) {
            this.comboCount++;
            this.comboTimer = 2.0;
            this.createParryEffect(player);
            this.score += 50;
            player.stamina = Math.min(player.maxStamina, player.stamina + 15);
        }
    }
    
    updateEffects(dt) {
        // Update particles
        this.particles = this.particles.filter(p => {
            p.life -= dt;
            return p.life > 0;
        });
        
        // Update hit effects
        this.hitEffects = this.hitEffects.filter(e => {
            e.life -= dt;
            return e.life > 0;
        });
    }
    
    updateCamera() {
        // Center camera on combat area
        const center = (this.player.x + this.enemy.x) / 2;
        this.camera.x = center - this.width / 2;
    }
    
    createHitEffect(x, y) {
        for (let i = 0; i < 10; i++) {
            this.particles.push({
                x: x,
                y: y,
                vx: (Math.random() - 0.5) * 200,
                vy: (Math.random() - 0.5) * 200,
                life: 0.3,
                color: '#ffffff',
                size: 3
            });
        }
    }
    
    createParryEffect(fighter) {
        this.hitEffects.push({
            x: fighter.x,
            y: fighter.y,
            life: 0.5,
            type: 'parry'
        });
        
        for (let i = 0; i < 20; i++) {
            this.particles.push({
                x: fighter.x,
                y: fighter.y,
                vx: (Math.random() - 0.5) * 300,
                vy: (Math.random() - 0.5) * 300,
                life: 0.5,
                color: '#ffff00',
                size: 4
            });
        }
    }
    
    createDodgeEffect(fighter) {
        this.particles.push({
            x: fighter.x,
            y: fighter.y,
            vx: 0,
            vy: 0,
            life: 0.4,
            color: '#4ecdc4',
            size: 30,
            expanding: true
        });
    }
    
    createFinisherEffect() {
        for (let i = 0; i < 50; i++) {
            this.particles.push({
                x: this.enemy.x,
                y: this.enemy.y,
                vx: (Math.random() - 0.5) * 400,
                vy: (Math.random() - 0.5) * 400,
                life: 2.0,
                color: '#ff0000',
                size: 5
            });
        }
    }
    
    render() {
        this.ctx.clearRect(0, 0, this.width, this.height);
        
        if (this.gameState === 'menu') {
            this.renderMenu();
        } else if (this.gameState === 'paused') {
            this.renderGame();
            this.renderPauseScreen();
        } else if (this.gameState === 'gameOver') {
            this.renderGame();
            this.renderGameOver();
        } else {
            this.renderGame();
        }
    }
    
    renderMenu() {
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.8)';
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        this.ctx.fillStyle = '#4ecdc4';
        this.ctx.font = 'bold 48px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText('SHADOW DUEL', this.width / 2, 200);
        
        this.ctx.fillStyle = 'white';
        this.ctx.font = '24px Arial';
        this.ctx.fillText('Press SPACE or Click to Play', this.width / 2, 300);
        
        this.ctx.font = '16px Arial';
        this.ctx.fillText('WASD: Move | SPACE: Attack | S: Parry | SHIFT: Dodge', this.width / 2, 350);
    }
    
    renderGame() {
        // Background
        this.ctx.fillStyle = 'rgba(40, 40, 60, 0.9)';
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        // Ground
        const groundY = 450;
        this.ctx.fillStyle = 'rgba(60, 60, 80, 0.8)';
        this.ctx.fillRect(0, groundY, this.width, this.height - groundY);
        
        // Grid pattern
        this.ctx.strokeStyle = 'rgba(100, 100, 120, 0.3)';
        this.ctx.lineWidth = 1;
        for (let i = 0; i < this.width; i += 50) {
            this.ctx.beginPath();
            this.ctx.moveTo(i, 0);
            this.ctx.lineTo(i, this.height);
            this.ctx.stroke();
        }
        for (let i = 0; i < this.height; i += 50) {
            this.ctx.beginPath();
            this.ctx.moveTo(0, i);
            this.ctx.lineTo(this.width, i);
            this.ctx.stroke();
        }
        
        // Render fighters
        this.renderFighter(this.player);
        this.renderFighter(this.enemy);
        
        // Render effects
        this.renderEffects();
        
        // UI
        this.renderUI();
    }
    
    renderFighter(fighter) {
        const ctx = this.ctx;
        
        // Shield effect when parrying
        if (fighter.parrying) {
            ctx.save();
            ctx.globalAlpha = 0.5;
            ctx.fillStyle = '#ffff00';
            ctx.beginPath();
            ctx.arc(fighter.x, fighter.y, 60, 0, Math.PI * 2);
            ctx.fill();
            ctx.restore();
        }
        
        // Shadow
        ctx.fillStyle = 'rgba(0, 0, 0, 0.3)';
        ctx.beginPath();
        ctx.ellipse(fighter.x, fighter.y + fighter.height / 2 + 15, 20, 10, 0, 0, Math.PI * 2);
        ctx.fill();
        
        // Fighter body
        ctx.fillStyle = fighter.color;
        ctx.fillRect(fighter.x - fighter.width / 2, fighter.y - fighter.height, fighter.width, fighter.height);
        
        // Head
        ctx.fillStyle = fighter.color;
        ctx.beginPath();
        ctx.arc(fighter.x, fighter.y - fighter.height, 15, 0, Math.PI * 2);
        ctx.fill();
        
        // Arms (for attack animation)
        if (fighter.attacking) {
            ctx.fillStyle = fighter.color;
            ctx.fillRect(
                fighter.x + (fighter.facingRight ? fighter.width / 2 : -fighter.width / 2 - 40),
                fighter.y - fighter.height / 2,
                40,
                12
            );
        }
        
        // Health bar
        const barWidth = 60;
        const barHeight = 8;
        ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
        ctx.fillRect(fighter.x - barWidth / 2, fighter.y - fighter.height - 30, barWidth, barHeight);
        ctx.fillStyle = fighter.health > 30 ? '#00ff00' : '#ff0000';
        ctx.fillRect(fighter.x - barWidth / 2, fighter.y - fighter.height - 30, barWidth * (fighter.health / fighter.maxHealth), barHeight);
        
        // Stamina bar
        ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
        ctx.fillRect(fighter.x - barWidth / 2, fighter.y - fighter.height - 22, barWidth, 4);
        ctx.fillStyle = '#00ffff';
        ctx.fillRect(fighter.x - barWidth / 2, fighter.y - fighter.height - 22, barWidth * (fighter.stamina / fighter.maxStamina), 4);
    }
    
    renderEffects() {
        // Particles
        this.particles.forEach(p => {
            this.ctx.fillStyle = p.color;
            this.ctx.globalAlpha = p.life;
            if (p.expanding) {
                this.ctx.fillRect(p.x - p.size, p.y - p.size, p.size * 2, p.size * 2);
            } else {
                this.ctx.fillRect(p.x - p.size / 2, p.y - p.size / 2, p.size, p.size);
            }
            this.ctx.globalAlpha = 1;
        });
    }
    
    renderUI() {
        // Score
        this.ctx.fillStyle = 'white';
        this.ctx.font = 'bold 24px Arial';
        this.ctx.textAlign = 'left';
        this.ctx.fillText('Score: ' + this.score, 20, 40);
        
        // Combo
        if (this.comboCount > 0) {
            this.ctx.fillStyle = '#ffff00';
            this.ctx.font = 'bold 32px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.fillText('COMBO x' + this.comboCount + '!', this.width / 2, 80);
        }
        
        // Slow motion indicator
        if (this.slowMotion) {
            this.ctx.fillStyle = '#ff00ff';
            this.ctx.font = 'bold 40px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.fillText('SLOW MOTION!', this.width / 2, this.height / 2);
        }
    }
    
    renderPauseScreen() {
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.7)';
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        this.ctx.fillStyle = 'white';
        this.ctx.font = 'bold 48px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText('PAUSED', this.width / 2, this.height / 2);
        
        this.ctx.font = '20px Arial';
        this.ctx.fillText('Press P to Resume', this.width / 2, this.height / 2 + 40);
    }
    
    renderGameOver() {
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.8)';
        this.ctx.fillRect(0, 0, this.width, this.height);
        
        this.ctx.fillStyle = this.player.health > 0 ? '#00ff00' : '#ff0000';
        this.ctx.font = 'bold 48px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText(this.player.health > 0 ? 'VICTORY!' : 'DEFEAT!', this.width / 2, this.height / 2);
        
        this.ctx.fillStyle = 'white';
        this.ctx.font = '32px Arial';
        this.ctx.fillText('Final Score: ' + this.score, this.width / 2, this.height / 2 + 60);
        
        this.ctx.font = '20px Arial';
        this.ctx.fillText('Press R to Restart or Reload Page', this.width / 2, this.height / 2 + 120);
    }
    
    gameLoop() {
        let lastTime = 0;
        
        const loop = (time) => {
            const deltaTime = (time - lastTime) / 1000;
            lastTime = time;
            
            this.update(deltaTime);
            this.render();
            
            requestAnimationFrame(loop);
        };
        
        requestAnimationFrame(loop);
    }
}

// Initialize game when canvas is available
window.addEventListener('load', () => {
    const canvas = document.getElementById('shadow-duel-canvas');
    if (canvas) {
        const game = new ShadowDuelGame();
        
        // Restart on R key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'r' || e.key === 'R') {
                game.startGame();
            }
        });
    }
});

