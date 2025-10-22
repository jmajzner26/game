// 67 Tetris Game Logic
class TetrisGame {
    constructor() {
        this.canvas = document.getElementById('tetris-canvas');
        this.ctx = this.canvas.getContext('2d');
        this.nextCanvas = document.getElementById('next-canvas');
        this.nextCtx = this.nextCanvas.getContext('2d');
        
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
        
        // Board dimensions
        this.boardWidth = 10;
        this.boardHeight = 20;
        this.blockSize = 30;
        
        // Game timing
        this.dropTime = 0;
        this.dropInterval = 1000; // 1 second initially
        this.lastTime = 0;
        
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
    }
    
    setupEventListeners() {
        document.addEventListener('keydown', (e) => {
            if (!this.gameRunning || this.gamePaused) return;
            
            switch(e.key) {
                case 'ArrowLeft':
                    this.movePiece(-1, 0);
                    break;
                case 'ArrowRight':
                    this.movePiece(1, 0);
                    break;
                case 'ArrowDown':
                    this.movePiece(0, 1);
                    break;
                case 'ArrowUp':
                    this.rotatePiece();
                    break;
                case ' ':
                    e.preventDefault();
                    this.hardDrop();
                    break;
                case 'p':
                case 'P':
                    this.togglePause();
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
        this.dropInterval = 1000;
        
        // Clear board
        for (let y = 0; y < this.boardHeight; y++) {
            for (let x = 0; x < this.boardWidth; x++) {
                this.board[y][x] = 0;
            }
        }
        
        this.spawnPiece();
        this.spawnNextPiece();
        this.hideOverlay();
        this.updateUI();
        this.gameLoop();
    }
    
    restartGame() {
        this.showOverlay('67 Tetris', 'Press SPACE to start!');
        this.gameRunning = false;
        this.gamePaused = false;
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
    
    spawnPiece() {
        if (!this.nextPiece) {
            this.nextPiece = this.createRandomPiece();
        }
        
        this.currentPiece = this.nextPiece;
        this.nextPiece = this.createRandomPiece();
        
        // Check if game over
        if (this.checkCollision(this.currentPiece, 0, 0)) {
            this.gameOver();
        }
        
        this.drawNextPiece();
    }
    
    createRandomPiece() {
        const type = this.pieceTypes[Math.floor(Math.random() * this.pieceTypes.length)];
        const piece = this.pieces[type];
        
        return {
            shape: piece.shape,
            color: piece.color,
            x: Math.floor(this.boardWidth / 2) - Math.floor(piece.shape[0].length / 2),
            y: 0
        };
    }
    
    movePiece(dx, dy) {
        if (!this.currentPiece) return;
        
        if (!this.checkCollision(this.currentPiece, this.currentPiece.x + dx, this.currentPiece.y + dy)) {
            this.currentPiece.x += dx;
            this.currentPiece.y += dy;
        }
    }
    
    rotatePiece() {
        if (!this.currentPiece) return;
        
        const rotated = this.rotateMatrix(this.currentPiece.shape);
        
        if (!this.checkCollision({...this.currentPiece, shape: rotated}, this.currentPiece.x, this.currentPiece.y)) {
            this.currentPiece.shape = rotated;
        }
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
    
    hardDrop() {
        if (!this.currentPiece) return;
        
        while (!this.checkCollision(this.currentPiece, this.currentPiece.x, this.currentPiece.y + 1)) {
            this.currentPiece.y++;
        }
        
        this.placePiece();
    }
    
    checkCollision(piece, x, y) {
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const newX = x + px;
                    const newY = y + py;
                    
                    // Check boundaries
                    if (newX < 0 || newX >= this.boardWidth || newY >= this.boardHeight) {
                        return true;
                    }
                    
                    // Check collision with placed pieces
                    if (newY >= 0 && this.board[newY][newX]) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    placePiece() {
        if (!this.currentPiece) return;
        
        // Place piece on board
        for (let py = 0; py < this.currentPiece.shape.length; py++) {
            for (let px = 0; px < this.currentPiece.shape[py].length; px++) {
                if (this.currentPiece.shape[py][px]) {
                    const x = this.currentPiece.x + px;
                    const y = this.currentPiece.y + py;
                    
                    if (y >= 0) {
                        this.board[y][x] = this.currentPiece.color;
                    }
                }
            }
        }
        
        // Check for completed lines
        this.clearLines();
        
        // Spawn next piece
        this.spawnPiece();
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
            this.score += linesCleared * 10; // 10 points per line
            
            // Check for victory
            if (this.score >= this.targetScore) {
                this.victory();
                return;
            }
            
            // Increase level every 10 lines
            this.level = Math.floor(this.lines / 10) + 1;
            this.dropInterval = Math.max(100, 1000 - (this.level - 1) * 100);
            
            this.updateUI();
            this.animateScore();
        }
    }
    
    gameLoop(currentTime = 0) {
        if (!this.gameRunning || this.gamePaused) return;
        
        const deltaTime = currentTime - this.lastTime;
        this.lastTime = currentTime;
        
        this.dropTime += deltaTime;
        
        if (this.dropTime >= this.dropInterval) {
            this.movePiece(0, 1);
            if (this.checkCollision(this.currentPiece, this.currentPiece.x, this.currentPiece.y + 1)) {
                this.placePiece();
            }
            this.dropTime = 0;
        }
        
        this.draw();
        requestAnimationFrame((time) => this.gameLoop(time));
    }
    
    draw() {
        // Clear canvas
        this.ctx.fillStyle = '#000';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw board
        this.drawBoard();
        
        // Draw current piece
        if (this.currentPiece) {
            this.drawPiece(this.currentPiece);
        }
        
        // Draw grid
        this.drawGrid();
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
    
    drawNextPiece() {
        if (!this.nextPiece) return;
        
        this.nextCtx.fillStyle = '#000';
        this.nextCtx.fillRect(0, 0, this.nextCanvas.width, this.nextCanvas.height);
        
        const blockSize = 20;
        const offsetX = (this.nextCanvas.width - this.nextPiece.shape[0].length * blockSize) / 2;
        const offsetY = (this.nextCanvas.height - this.nextPiece.shape.length * blockSize) / 2;
        
        this.nextCtx.fillStyle = this.nextPiece.color;
        
        for (let py = 0; py < this.nextPiece.shape.length; py++) {
            for (let px = 0; px < this.nextPiece.shape[py].length; px++) {
                if (this.nextPiece.shape[py][px]) {
                    const x = offsetX + px * blockSize;
                    const y = offsetY + py * blockSize;
                    
                    this.nextCtx.fillRect(x, y, blockSize, blockSize);
                    
                    this.nextCtx.strokeStyle = '#333';
                    this.nextCtx.lineWidth = 1;
                    this.nextCtx.strokeRect(x, y, blockSize, blockSize);
                }
            }
        }
    }
    
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
    }
    
    victory() {
        this.gameRunning = false;
        this.showOverlay('🎉 VICTORY! 🎉', `You reached exactly ${this.targetScore} points!`);
        
        // Add victory animation
        const scoreElement = document.getElementById('current-score');
        scoreElement.classList.add('victory');
    }
}

// Initialize the game when the page loads
let tetrisGame;
window.addEventListener('load', () => {
    tetrisGame = new TetrisGame();
});
