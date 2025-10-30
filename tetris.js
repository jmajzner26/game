// 67 Tetris Game Logic - Classic Falling Piece
class TetrisGame {
    constructor() {
        this.canvas = document.getElementById('tetris-canvas');
        this.ctx = this.canvas.getContext('2d');
        this.nextCanvas = document.getElementById('next-canvas');
        this.nextCtx = this.nextCanvas ? this.nextCanvas.getContext('2d') : null;
        
        // Game state
        this.board = [];
        this.currentPiece = null;
        this.nextPiece = null;
        this.score = 0;
        this.lines = 0;
        this.level = 1;
        this.gameRunning = false;
        this.gamePaused = false;
        this.targetScore = 67;
        this.dropCounter = 0;
        this.dropInterval = 1000; // ms, gets faster with level
        this.lastTime = 0;
        
        // Board dimensions
        this.boardWidth = 10;
        this.boardHeight = 20;
        this.blockSize = 30;
        
        // Input state
        
        // Tetris pieces (Tetrominoes)
        this.pieces = {
            I: {
                shape: [
                    [0, 0, 0, 0],
                    [1, 1, 1, 1],
                    [0, 0, 0, 0],
                    [0, 0, 0, 0]
                ],
                color: '#00f0f0'
            },
            O: {
                shape: [
                    [1, 1],
                    [1, 1]
                ],
                color: '#f0f000'
            },
            T: {
                shape: [
                    [0, 1, 0],
                    [1, 1, 1],
                    [0, 0, 0]
                ],
                color: '#a000f0'
            },
            S: {
                shape: [
                    [0, 1, 1],
                    [1, 1, 0],
                    [0, 0, 0]
                ],
                color: '#00f000'
            },
            Z: {
                shape: [
                    [1, 1, 0],
                    [0, 1, 1],
                    [0, 0, 0]
                ],
                color: '#f00000'
            },
            J: {
                shape: [
                    [1, 0, 0],
                    [1, 1, 1],
                    [0, 0, 0]
                ],
                color: '#0000f0'
            },
            L: {
                shape: [
                    [0, 0, 1],
                    [1, 1, 1],
                    [0, 0, 0]
                ],
                color: '#f0a000'
            }
        };
        
        this.pieceTypes = Object.keys(this.pieces);
        
        this.init();
    }
    
    init() {
        // Initialize empty board
        for (let y = 0; y < this.boardHeight; y++) {
            this.board[y] = [];
            for (let x = 0; x < this.boardWidth; x++) {
                this.board[y][x] = 0;
            }
        }
        
        this.setupEventListeners();
        this.updateUI();
        this.draw();
    }
    
    setupEventListeners() {
        // Keyboard events
        document.addEventListener('keydown', (e) => {
            if (e.code === 'Space' && !this.gameRunning) {
                e.preventDefault();
                this.startGame();
                return;
            }
            if (!this.gameRunning) return;
            if (e.code === 'Space') {
                e.preventDefault();
                this.hardDrop();
                return;
            }
            if (this.gamePaused) return;
            switch(e.key) {
                case 'ArrowUp':
                    this.rotatePiece();
                    break;
                case 'ArrowLeft':
                    this.movePiece(-1, 0);
                    break;
                case 'ArrowRight':
                    this.movePiece(1, 0);
                    break;
                case 'ArrowDown':
                    this.softDrop();
                    break;
                case 'p':
                case 'P':
                    this.togglePause();
                    break;
                case 'r':
                case 'R':
                    this.restartGame();
                    break;
            }
        });
        
        document.getElementById('start-btn').addEventListener('click', () => {
            this.startGame();
        });
        
        document.getElementById('pause-btn').addEventListener('click', () => {
            this.togglePause();
        });
        
        document.getElementById('restart-btn').addEventListener('click', () => {
            this.restartGame();
        });
    }
    
    startGame() {
        this.gameRunning = true;
        this.gamePaused = false;
        this.score = 0;
        this.lines = 0;
        this.level = 1;
        this.dropCounter = 0;
        this.lastTime = 0;
        
        // Clear board
        for (let y = 0; y < this.boardHeight; y++) {
            for (let x = 0; x < this.boardWidth; x++) {
                this.board[y][x] = 0;
            }
        }
        
        this.nextPiece = this.createRandomPiece();
        this.spawnPiece();
        
        this.hideOverlay();
        this.updateUI();
        this.draw();
        requestAnimationFrame((t) => this.loop(t));
        if (typeof window !== 'undefined' && typeof window.va === 'function') {
            window.va('tetris_start');
        }
    }
    
    restartGame() {
        this.showOverlay('67 Tetris', 'Click pieces to place them strategically!');
        this.gameRunning = false;
        this.gamePaused = false;
    }
    
    // Game loop
    loop(time) {
        if (!this.gameRunning) return;
        const delta = this.lastTime ? time - this.lastTime : 0;
        this.lastTime = time;
        if (!this.gamePaused) {
            this.dropCounter += delta;
            if (this.dropCounter > this.dropInterval) {
                this.softDrop();
            }
            this.draw();
        }
        requestAnimationFrame((t) => this.loop(t));
    }
    
    // Piece helpers
    spawnPiece() {
        this.currentPiece = this.nextPiece || this.createRandomPiece();
        this.currentPiece.x = Math.floor(this.boardWidth / 2) - Math.floor(this.currentPiece.shape[0].length / 2);
        this.currentPiece.y = 0;
        this.nextPiece = this.createRandomPiece();
        if (this.checkCollision(this.currentPiece, this.currentPiece.x, this.currentPiece.y)) {
            this.gameOver();
        }
        this.drawNextPiece();
    }

    checkCollision(piece, x, y) {
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const newX = x + px;
                    const newY = y + py;
                    if (newX < 0 || newX >= this.boardWidth || newY >= this.boardHeight) return true;
                    if (newY >= 0 && this.board[newY][newX]) return true;
                }
            }
        }
        return false;
    }

    movePiece(dx, dy) {
        if (!this.currentPiece) return;
        const nx = this.currentPiece.x + dx;
        const ny = this.currentPiece.y + dy;
        if (!this.checkCollision(this.currentPiece, nx, ny)) {
            this.currentPiece.x = nx;
            this.currentPiece.y = ny;
            this.draw();
        } else if (dy === 1) {
            // Lock
            this.lockPiece();
        }
    }

    softDrop() {
        this.movePiece(0, 1);
        this.dropCounter = 0;
    }

    hardDrop() {
        if (!this.currentPiece) return;
        while (!this.checkCollision(this.currentPiece, this.currentPiece.x, this.currentPiece.y + 1)) {
            this.currentPiece.y++;
        }
        this.lockPiece();
    }

    rotatePiece() {
        if (!this.currentPiece) return;
        const rotated = this.rotateMatrix(this.currentPiece.shape);
        // simple wall kick: try offsets -1,1,-2,2
        const offsets = [0, -1, 1, -2, 2];
        for (const off of offsets) {
            if (!this.checkCollision({ ...this.currentPiece, shape: rotated }, this.currentPiece.x + off, this.currentPiece.y)) {
                this.currentPiece.shape = rotated;
                this.currentPiece.x += off;
                this.draw();
                return;
            }
        }
    }

    lockPiece() {
        for (let py = 0; py < this.currentPiece.shape.length; py++) {
            for (let px = 0; px < this.currentPiece.shape[py].length; px++) {
                if (this.currentPiece.shape[py][px]) {
                    const x = this.currentPiece.x + px;
                    const y = this.currentPiece.y + py;
                    if (y >= 0) this.board[y][x] = this.currentPiece.color;
                }
            }
        }
        const beforeLines = this.lines;
        this.clearLines();
        const cleared = this.lines - beforeLines;
        // Scoring: 1 point per line
        if (cleared > 0) {
            this.score += cleared;
            this.level = Math.floor(this.lines / 10) + 1;
            // speed up
            this.dropInterval = Math.max(200, 1000 - (this.level - 1) * 75);
            this.updateUI();
            this.animateScore();
        }
        // Victory condition (optional target)
        if (this.score >= this.targetScore) {
            this.victory();
            return;
        }
        this.spawnPiece();
        if (typeof window !== 'undefined' && typeof window.va === 'function') {
            window.va('tetris_place_piece', { score: this.score, lines: this.lines });
        }
    }
    
    createRandomPiece() {
        const type = this.pieceTypes[Math.floor(Math.random() * this.pieceTypes.length)];
        const piece = this.pieces[type];
        return { shape: piece.shape.map(r => r.slice()), color: piece.color, x: 0, y: 0 };
    }
    
    rotateMatrix(matrix) {
        const N = matrix.length;
        const rotated = [];
        
        for (let i = 0; i < N; i++) {
            rotated[i] = [];
            for (let j = 0; j < N; j++) {
                rotated[i][j] = matrix[N - 1 - j][i];
            }
        }
        
        return rotated;
    }
    
    togglePause() {
        if (!this.gameRunning) return;
        
        this.gamePaused = !this.gamePaused;
        if (this.gamePaused) {
            this.showOverlay('Paused', 'Press P to resume');
        } else {
            this.hideOverlay();
        }
    }
    drawNextPiece() {
        if (!this.nextCtx || !this.nextPiece) return;
        const ctx = this.nextCtx;
        ctx.clearRect(0, 0, this.nextCanvas.width, this.nextCanvas.height);
        const size = 20;
        const shape = this.nextPiece.shape;
        const w = shape[0].length * size;
        const h = shape.length * size;
        const offsetX = Math.floor((this.nextCanvas.width - w) / 2);
        const offsetY = Math.floor((this.nextCanvas.height - h) / 2);
        ctx.fillStyle = this.nextPiece.color;
        for (let py = 0; py < shape.length; py++) {
            for (let px = 0; px < shape[py].length; px++) {
                if (shape[py][px]) {
                    const x = offsetX + px * size;
                    const y = offsetY + py * size;
                    ctx.fillRect(x, y, size, size);
                    ctx.strokeStyle = '#333';
                    ctx.lineWidth = 1;
                    ctx.strokeRect(x, y, size, size);
                }
            }
        }
    }
    
    clearLines() {
        let linesCleared = 0;
        
        for (let y = this.boardHeight - 1; y >= 0; y--) {
            if (this.board[y].every(cell => cell !== 0)) {
                // Remove the line
                this.board.splice(y, 1);
                // Add new empty line at top
                this.board.unshift(new Array(this.boardWidth).fill(0));
                linesCleared++;
                y++; // Check the same line again
            }
        }
        
        if (linesCleared > 0) {
            this.lines += linesCleared;
            // Score now handled in lockPiece using lines cleared delta
            // Increase level every 10 lines
            this.level = Math.floor(this.lines / 10) + 1;
            this.updateUI();
            this.animateScore();
        }
    }
    
    draw() {
        // Clear canvas
        this.ctx.fillStyle = '#000';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw board
        this.drawBoard();
        
        // Draw grid
        this.drawGrid();
        
        // Draw current falling piece
        if (this.currentPiece) {
            this.drawPiece(this.currentPiece);
        }
    }
    
    drawBoard() {
        for (let y = 0; y < this.boardHeight; y++) {
            for (let x = 0; x < this.boardWidth; x++) {
                if (this.board[y][x]) {
                    this.ctx.fillStyle = this.board[y][x];
                    this.ctx.fillRect(x * this.blockSize, y * this.blockSize, this.blockSize, this.blockSize);
                    
                    // Add border
                    this.ctx.strokeStyle = '#333';
                    this.ctx.lineWidth = 1;
                    this.ctx.strokeRect(x * this.blockSize, y * this.blockSize, this.blockSize, this.blockSize);
                }
            }
        }
    }
    
    drawPiece(piece) {
        this.ctx.fillStyle = piece.color;
        
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const x = (piece.x + px) * this.blockSize;
                    const y = (piece.y + py) * this.blockSize;
                    
                    this.ctx.fillRect(x, y, this.blockSize, this.blockSize);
                    
                    // Add border
                    this.ctx.strokeStyle = '#333';
                    this.ctx.lineWidth = 1;
                    this.ctx.strokeRect(x, y, this.blockSize, this.blockSize);
                }
            }
        }
    }
    
    drawGrid() {
        this.ctx.strokeStyle = '#333';
        this.ctx.lineWidth = 1;
        
        // Vertical lines
        for (let x = 0; x <= this.boardWidth; x++) {
            this.ctx.beginPath();
            this.ctx.moveTo(x * this.blockSize, 0);
            this.ctx.lineTo(x * this.blockSize, this.canvas.height);
            this.ctx.stroke();
        }
        
        // Horizontal lines
        for (let y = 0; y <= this.boardHeight; y++) {
            this.ctx.beginPath();
            this.ctx.moveTo(0, y * this.blockSize);
            this.ctx.lineTo(this.canvas.width, y * this.blockSize);
            this.ctx.stroke();
        }
    }
    
    // Removed drag/drop helpers from classic mode
    
    updateUI() {
        document.getElementById('current-score').textContent = this.score;
        document.getElementById('lines-cleared').textContent = this.lines;
        document.getElementById('level').textContent = this.level;
    }
    
    animateScore() {
        const scoreElement = document.getElementById('current-score');
        scoreElement.classList.add('score-increase');
        setTimeout(() => {
            scoreElement.classList.remove('score-increase');
        }, 500);
    }
    
    showOverlay(title, message) {
        document.getElementById('overlay-title').textContent = title;
        document.getElementById('overlay-message').textContent = message;
        document.getElementById('game-overlay').classList.remove('hidden');
    }
    
    hideOverlay() {
        document.getElementById('game-overlay').classList.add('hidden');
    }
    
    gameOver() {
        this.gameRunning = false;
        this.showOverlay('Game Over!', `You scored ${this.score} points!`);
        if (typeof window !== 'undefined' && typeof window.va === 'function') {
            window.va('tetris_game_over');
        }
    }
    
    victory() {
        this.gameRunning = false;
        this.showOverlay('🎉 VICTORY! 🎉', `You reached exactly ${this.targetScore} points!`);
        
        // Add victory animation
        const scoreElement = document.getElementById('current-score');
        scoreElement.classList.add('victory');
        if (typeof window !== 'undefined' && typeof window.va === 'function') {
            window.va('tetris_victory');
        }
    }
}

// Initialize the game when the page loads
let tetrisGame;
window.addEventListener('load', () => {
    tetrisGame = new TetrisGame();
});
