class PongGame {
    constructor() {
        this.canvas = document.getElementById('pong-canvas');
        this.ctx = this.canvas.getContext('2d');
        this.reset();
        this.bind();
        this.draw();
    }
    reset() {
        this.ball = { x: this.canvas.width/2, y: this.canvas.height/2, vx: 3, vy: 2, r: 8 };
        this.paddleH = 80; this.paddleW = 10;
        this.left = { x: 10, y: this.canvas.height/2 - this.paddleH/2 };
        this.right = { x: this.canvas.width - 20, y: this.canvas.height/2 - this.paddleH/2 };
        this.up = false; this.down = false;
        this.running = false;
        this.score = { left: 0, right: 0 };
        this.showOverlay('Pong', 'Press SPACE to start');
    }
    bind() {
        document.addEventListener('keydown', (e) => {
            const active = !document.getElementById('pong-game').classList.contains('hidden');
            if (!active) return;
            if (e.code === 'Space' && !this.running) { this.start(); }
            if (!this.running) return;
            if (e.key === 'ArrowUp') this.up = true;
            if (e.key === 'ArrowDown') this.down = true;
        });
        document.addEventListener('keyup', (e) => {
            if (e.key === 'ArrowUp') this.up = false;
            if (e.key === 'ArrowDown') this.down = false;
        });
        const btn = document.getElementById('pong-start-btn');
        if (btn) btn.addEventListener('click', () => this.start());
    }
    start() {
        this.running = true;
        this.hideOverlay();
        if (typeof window !== 'undefined' && typeof window.va === 'function') window.va('pong_start');
        requestAnimationFrame(() => this.loop());
    }
    loop() {
        if (!this.running) return;
        this.update();
        this.draw();
        requestAnimationFrame(() => this.loop());
    }
    update() {
        // player paddle move
        if (this.up) this.left.y -= 5;
        if (this.down) this.left.y += 5;
        this.left.y = Math.max(0, Math.min(this.canvas.height - this.paddleH, this.left.y));
        // simple AI follows ball
        const aiCenter = this.right.y + this.paddleH/2;
        if (aiCenter < this.ball.y - 10) this.right.y += 4; else if (aiCenter > this.ball.y + 10) this.right.y -= 4;
        this.right.y = Math.max(0, Math.min(this.canvas.height - this.paddleH, this.right.y));
        // ball move
        this.ball.x += this.ball.vx;
        this.ball.y += this.ball.vy;
        if (this.ball.y < this.ball.r || this.ball.y > this.canvas.height - this.ball.r) this.ball.vy *= -1;
        // paddle collisions
        if (this.ball.x - this.ball.r < this.left.x + this.paddleW && this.ball.y > this.left.y && this.ball.y < this.left.y + this.paddleH) {
            this.ball.vx = Math.abs(this.ball.vx) + 0.2;
        }
        if (this.ball.x + this.ball.r > this.right.x && this.ball.y > this.right.y && this.ball.y < this.right.y + this.paddleH) {
            this.ball.vx = -Math.abs(this.ball.vx) - 0.2;
        }
        // scoring
        if (this.ball.x < 0) { this.score.right++; this.resetBall(1); }
        if (this.ball.x > this.canvas.width) { this.score.left++; this.resetBall(-1); }
    }
    resetBall(dir) {
        this.ball.x = this.canvas.width/2; this.ball.y = this.canvas.height/2;
        this.ball.vx = 3 * dir; this.ball.vy = (Math.random()*4-2) || 2;
    }
    draw() {
        this.ctx.fillStyle = '#000'; this.ctx.fillRect(0,0,this.canvas.width,this.canvas.height);
        this.ctx.fillStyle = '#fff';
        // center line
        this.ctx.fillRect(this.canvas.width/2 - 1, 0, 2, this.canvas.height);
        // paddles
        this.ctx.fillRect(this.left.x, this.left.y, this.paddleW, this.paddleH);
        this.ctx.fillRect(this.right.x, this.right.y, this.paddleW, this.paddleH);
        // ball
        this.ctx.beginPath(); this.ctx.arc(this.ball.x, this.ball.y, this.ball.r, 0, Math.PI*2); this.ctx.fill();
        // score
        this.ctx.font = '20px Arial'; this.ctx.fillText(`${this.score.left} : ${this.score.right}`, this.canvas.width/2 - 20, 20);
    }
    showOverlay(title,msg){ const o=document.getElementById('pong-overlay'); o.classList.remove('hidden'); }
    hideOverlay(){ const o=document.getElementById('pong-overlay'); o.classList.add('hidden'); }
}
let pongGame; window.addEventListener('load', ()=>{ if (document.getElementById('pong-canvas')) pongGame = new PongGame(); });


