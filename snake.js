class SnakeGame {
    constructor() {
        this.canvas = document.getElementById('snake-canvas');
        this.ctx = this.canvas.getContext('2d');
        this.size = 20; // grid cell size
        this.grid = this.canvas.width / this.size;
        this.reset();
        this.bindEvents();
        this.draw();
    }

    reset() {
        this.snake = [{ x: 5, y: 5 }];
        this.direction = { x: 1, y: 0 };
        this.pendingDir = this.direction;
        this.food = this.randomFood();
        this.score = 0;
        this.running = false;
        this.gameOver = false;
        this.speedMs = 140;
        if (typeof document !== 'undefined') {
            document.getElementById('snake-score').textContent = this.score;
            this.showOverlay('Snake', 'Press SPACE to start!');
        }
    }

    bindEvents() {
        document.addEventListener('keydown', (e) => {
            const active = !document.getElementById('snake-game').classList.contains('hidden');
            if (!active) return;
            if (this.gameOver && e.key.toLowerCase() === 'r') {
                this.reset();
                return;
            }
            if (!this.running && e.code === 'Space') {
                this.start();
                return;
            }
            if (!this.running) return;
            switch (e.key) {
                case 'ArrowUp':
                    if (this.direction.y === 0) this.pendingDir = { x: 0, y: -1 }; break;
                case 'ArrowDown':
                    if (this.direction.y === 0) this.pendingDir = { x: 0, y: 1 }; break;
                case 'ArrowLeft':
                    if (this.direction.x === 0) this.pendingDir = { x: -1, y: 0 }; break;
                case 'ArrowRight':
                    if (this.direction.x === 0) this.pendingDir = { x: 1, y: 0 }; break;
                case 'r': case 'R':
                    this.reset();
                    break;
            }
        });

        const btn = document.getElementById('snake-start-btn');
        if (btn) btn.addEventListener('click', () => this.start());
    }

    start() {
        if (this.running) return;
        this.running = true;
        this.gameOver = false;
        this.hideOverlay();
        if (typeof window !== 'undefined' && typeof window.va === 'function') {
            window.va('snake_start');
        }
        this.tick();
    }

    tick() {
        if (!this.running) return;
        this.update();
        this.draw();
        setTimeout(() => this.tick(), this.speedMs);
    }

    update() {
        // apply pending direction (prevents instant reversal)
        this.direction = this.pendingDir;
        const head = { x: this.snake[0].x + this.direction.x, y: this.snake[0].y + this.direction.y };

        // wall collision
        if (head.x < 0 || head.y < 0 || head.x >= this.grid || head.y >= this.grid) {
            return this.end();
        }
        // self collision
        for (let i = 0; i < this.snake.length; i++) {
            if (this.snake[i].x === head.x && this.snake[i].y === head.y) {
                return this.end();
            }
        }

        this.snake.unshift(head);
        // food
        if (head.x === this.food.x && head.y === this.food.y) {
            this.score += 1;
            document.getElementById('snake-score').textContent = this.score;
            this.food = this.randomFood();
            // speed up slightly
            this.speedMs = Math.max(70, this.speedMs - 3);
        } else {
            this.snake.pop();
        }
    }

    end() {
        this.running = false;
        this.gameOver = true;
        this.showOverlay('Game Over!', `Score: ${this.score}`);
        if (typeof window !== 'undefined' && typeof window.va === 'function') {
            window.va('snake_game_over', { score: this.score });
        }
    }

    randomFood() {
        let pos;
        do {
            pos = { x: Math.floor(Math.random() * this.grid), y: Math.floor(Math.random() * this.grid) };
        } while (this.snake && this.snake.some(p => p.x === pos.x && p.y === pos.y));
        return pos;
    }

    draw() {
        // bg
        this.ctx.fillStyle = '#000';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        // grid (subtle)
        this.ctx.strokeStyle = '#222';
        for (let i = 0; i <= this.grid; i++) {
            this.ctx.beginPath();
            this.ctx.moveTo(i * this.size, 0);
            this.ctx.lineTo(i * this.size, this.canvas.height);
            this.ctx.stroke();
            this.ctx.beginPath();
            this.ctx.moveTo(0, i * this.size);
            this.ctx.lineTo(this.canvas.width, i * this.size);
            this.ctx.stroke();
        }
        // food
        this.ctx.fillStyle = '#ff6b6b';
        this.ctx.fillRect(this.food.x * this.size, this.food.y * this.size, this.size, this.size);
        // snake
        this.ctx.fillStyle = '#4ecdc4';
        for (let i = 0; i < this.snake.length; i++) {
            const p = this.snake[i];
            this.ctx.fillRect(p.x * this.size, p.y * this.size, this.size, this.size);
            this.ctx.strokeStyle = '#333';
            this.ctx.strokeRect(p.x * this.size, p.y * this.size, this.size, this.size);
        }
    }

    showOverlay(title, message) {
        const o = document.getElementById('snake-overlay');
        document.getElementById('snake-overlay-title').textContent = title;
        document.getElementById('snake-overlay-message').textContent = message;
        o.classList.remove('hidden');
    }

    hideOverlay() {
        const o = document.getElementById('snake-overlay');
        o.classList.add('hidden');
    }
}

let snakeGame;
window.addEventListener('load', () => {
    const canvas = document.getElementById('snake-canvas');
    if (canvas) {
        snakeGame = new SnakeGame();
    }
});


