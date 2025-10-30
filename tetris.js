// 67 Tetris Game Logic - Strategic Block Placement
class TetrisGame {
    constructor() {
        this.canvas = document.getElementById('tetris-canvas');
        this.ctx = this.canvas.getContext('2d');
        // Drag/drop placement gameplay only; no separate next-canvas
        
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
        this.selectedPiece = null;
        this.pieceInventory = [];
        this.maxInventorySize = 5;
        
        // Board dimensions
        this.boardWidth = 10;
        this.boardHeight = 20;
        this.blockSize = 30;
        
        // Mouse/touch interaction
        this.mousePos = { x: 0, y: 0 };
        this.isDragging = false;
        this.dragOffset = { x: 0, y: 0 };
        
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
        
        // Initialize piece inventory
        this.pieceInventory = [];
        for (let i = 0; i < this.maxInventorySize; i++) {
            this.pieceInventory.push(this.createRandomPiece());
        }
        
        this.setupEventListeners();
        this.updateUI();
        this.draw();
    }
    
    setupEventListeners() {
        // Mouse events for piece placement
        this.canvas.addEventListener('mousedown', (e) => {
            if (!this.gameRunning) return;
            this.handleMouseDown(e);
        });
        
        this.canvas.addEventListener('mousemove', (e) => {
            if (!this.gameRunning) return;
            this.handleMouseMove(e);
        });
        
        this.canvas.addEventListener('mouseup', (e) => {
            if (!this.gameRunning) return;
            this.handleMouseUp(e);
        });
        
        // Touch events for mobile
        this.canvas.addEventListener('touchstart', (e) => {
            if (!this.gameRunning) return;
            e.preventDefault();
            this.handleMouseDown(e.touches[0]);
        });
        
        this.canvas.addEventListener('touchmove', (e) => {
            if (!this.gameRunning) return;
            e.preventDefault();
            this.handleMouseMove(e.touches[0]);
        });
        
        this.canvas.addEventListener('touchend', (e) => {
            if (!this.gameRunning) return;
            e.preventDefault();
            this.handleMouseUp(e);
        });
        
        // Keyboard events
        document.addEventListener('keydown', (e) => {
            if (e.code === 'Space') {
                e.preventDefault();
                if (!this.gameRunning) {
                    this.startGame();
                    return;
                }
            }
            if (!this.gameRunning || this.gamePaused) return;
            switch(e.key) {
                case 'ArrowUp':
                    if (this.selectedPiece) this.rotateSelectedPiece();
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
        this.selectedPiece = null;
        
        // Clear board
        for (let y = 0; y < this.boardHeight; y++) {
            for (let x = 0; x < this.boardWidth; x++) {
                this.board[y][x] = 0;
            }
        }
        
        // Refill inventory
        this.pieceInventory = [];
        for (let i = 0; i < this.maxInventorySize; i++) {
            this.pieceInventory.push(this.createRandomPiece());
        }
        
        this.hideOverlay();
        this.updateUI();
        this.draw();
        if (typeof window !== 'undefined' && typeof window.va === 'function') {
            window.va('tetris_start');
        }
    }
    
    restartGame() {
        this.showOverlay('67 Tetris', 'Click pieces to place them strategically!');
        this.gameRunning = false;
        this.gamePaused = false;
    }
    
    // Mouse handling methods
    handleMouseDown(e) {
        const rect = this.canvas.getBoundingClientRect();
        this.mousePos.x = e.clientX - rect.left;
        this.mousePos.y = e.clientY - rect.top;
        
        // Check if clicking on a piece in inventory
        const inventoryY = this.canvas.height + 20;
        const pieceWidth = 60;
        const pieceSpacing = 80;
        
        for (let i = 0; i < this.pieceInventory.length; i++) {
            const pieceX = i * pieceSpacing + 20;
            const pieceY = inventoryY;
            
            if (this.mousePos.x >= pieceX && this.mousePos.x <= pieceX + pieceWidth &&
                this.mousePos.y >= pieceY && this.mousePos.y <= pieceY + pieceWidth) {
                
                this.selectedPiece = {
                    piece: this.pieceInventory[i],
                    index: i,
                    x: this.mousePos.x,
                    y: this.mousePos.y
                };
                this.isDragging = true;
                this.dragOffset.x = 0;
                this.dragOffset.y = 0;
                break;
            }
        }
    }
    
    handleMouseMove(e) {
        if (!this.isDragging || !this.selectedPiece) return;
        
        const rect = this.canvas.getBoundingClientRect();
        this.mousePos.x = e.clientX - rect.left;
        this.mousePos.y = e.clientY - rect.top;
        
        this.selectedPiece.x = this.mousePos.x;
        this.selectedPiece.y = this.mousePos.y;
        
        this.draw();
    }
    
    handleMouseUp(e) {
        if (!this.isDragging || !this.selectedPiece) return;
        
        const rect = this.canvas.getBoundingClientRect();
        this.mousePos.x = e.clientX - rect.left;
        this.mousePos.y = e.clientY - rect.top;
        
        // Try to place the piece
        const boardX = Math.floor(this.mousePos.x / this.blockSize);
        const boardY = Math.floor(this.mousePos.y / this.blockSize);
        
        if (this.canPlacePiece(this.selectedPiece.piece, boardX, boardY)) {
            this.placePiece(this.selectedPiece.piece, boardX, boardY);
            this.pieceInventory.splice(this.selectedPiece.index, 1);
            
            // Add new piece to inventory
            this.pieceInventory.push(this.createRandomPiece());
            
            // Check for completed lines
            this.clearLines();
            
            // Check for victory
            if (this.score >= this.targetScore) {
                this.victory();
                return;
            }
            
            this.updateUI();
            if (typeof window !== 'undefined' && typeof window.va === 'function') {
                window.va('tetris_place_piece', { score: this.score, lines: this.lines });
            }
        }
        
        this.selectedPiece = null;
        this.isDragging = false;
        this.draw();
    }
    
    canPlacePiece(piece, x, y) {
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const newX = x + px;
                    const newY = y + py;
                    
                    // Check boundaries
                    if (newX < 0 || newX >= this.boardWidth || newY < 0 || newY >= this.boardHeight) {
                        return false;
                    }
                    
                    // Check collision with existing pieces
                    if (this.board[newY][newX] !== 0) {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    
    placePiece(piece, x, y) {
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const newX = x + px;
                    const newY = y + py;
                    this.board[newY][newX] = piece.color;
                }
            }
        }
    }
    
    rotateSelectedPiece() {
        if (!this.selectedPiece) return;
        
        const rotated = this.rotateMatrix(this.selectedPiece.piece.shape);
        this.selectedPiece.piece.shape = rotated;
        this.draw();
    }
    
    createRandomPiece() {
        const type = this.pieceTypes[Math.floor(Math.random() * this.pieceTypes.length)];
        const piece = this.pieces[type];
        
        return {
            shape: piece.shape,
            color: piece.color
        };
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
    // Classic falling-piece functions removed for this drag-and-drop variant
    
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
        
        // Draw piece inventory
        this.drawPieceInventory();
        
        // Draw selected piece being dragged with placement preview
        if (this.selectedPiece && this.isDragging) {
            this.drawPlacementPreview();
            this.drawDraggedPiece();
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
    
    drawPieceInventory() {
        const inventoryY = this.canvas.height + 20;
        const pieceWidth = 60;
        const pieceSpacing = 80;
        
        // Draw inventory background
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
        this.ctx.fillRect(0, inventoryY - 10, this.canvas.width, pieceWidth + 20);
        
        // Draw each piece in inventory
        for (let i = 0; i < this.pieceInventory.length; i++) {
            const piece = this.pieceInventory[i];
            const pieceX = i * pieceSpacing + 20;
            const pieceY = inventoryY;
            
            // Draw piece background
            this.ctx.fillStyle = 'rgba(255, 255, 255, 0.1)';
            this.ctx.fillRect(pieceX, pieceY, pieceWidth, pieceWidth);
            this.ctx.strokeStyle = 'rgba(255, 255, 255, 0.3)';
            this.ctx.lineWidth = 2;
            this.ctx.strokeRect(pieceX, pieceY, pieceWidth, pieceWidth);
            
            // Draw piece blocks
            const blockSize = pieceWidth / 4;
            const offsetX = pieceX + (pieceWidth - piece.shape[0].length * blockSize) / 2;
            const offsetY = pieceY + (pieceWidth - piece.shape.length * blockSize) / 2;
            
            this.ctx.fillStyle = piece.color;
            
            for (let py = 0; py < piece.shape.length; py++) {
                for (let px = 0; px < piece.shape[py].length; px++) {
                    if (piece.shape[py][px]) {
                        const x = offsetX + px * blockSize;
                        const y = offsetY + py * blockSize;
                        
                        this.ctx.fillRect(x, y, blockSize, blockSize);
                        
                        this.ctx.strokeStyle = '#333';
                        this.ctx.lineWidth = 1;
                        this.ctx.strokeRect(x, y, blockSize, blockSize);
                    }
                }
            }
        }
    }
    
    drawDraggedPiece() {
        if (!this.selectedPiece) return;
        
        const piece = this.selectedPiece.piece;
        const blockSize = 20;
        const offsetX = this.selectedPiece.x - (piece.shape[0].length * blockSize) / 2;
        const offsetY = this.selectedPiece.y - (piece.shape.length * blockSize) / 2;
        
        // Draw shadow
        this.ctx.fillStyle = 'rgba(255, 255, 255, 0.3)';
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const x = offsetX + px * blockSize + 2;
                    const y = offsetY + py * blockSize + 2;
                    this.ctx.fillRect(x, y, blockSize, blockSize);
                }
            }
        }
        
        // Draw piece
        this.ctx.fillStyle = piece.color;
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const x = offsetX + px * blockSize;
                    const y = offsetY + py * blockSize;
                    
                    this.ctx.fillRect(x, y, blockSize, blockSize);
                    
                    this.ctx.strokeStyle = '#333';
                    this.ctx.lineWidth = 1;
                    this.ctx.strokeRect(x, y, blockSize, blockSize);
                }
            }
        }
    }

    drawPlacementPreview() {
        if (!this.selectedPiece) return;
        const piece = this.selectedPiece.piece;
        const boardX = Math.floor(this.mousePos.x / this.blockSize);
        const boardY = Math.floor(this.mousePos.y / this.blockSize);
        const valid = this.canPlacePiece(piece, boardX, boardY);
        const color = valid ? 'rgba(78,205,196,0.35)' : 'rgba(255,107,107,0.35)';
        this.ctx.fillStyle = color;
        for (let py = 0; py < piece.shape.length; py++) {
            for (let px = 0; px < piece.shape[py].length; px++) {
                if (piece.shape[py][px]) {
                    const x = (boardX + px) * this.blockSize;
                    const y = (boardY + py) * this.blockSize;
                    this.ctx.fillRect(x, y, this.blockSize, this.blockSize);
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
