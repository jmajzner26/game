// Main App Logic for Multi-Game Arcade
class GameArcade {
    constructor() {
        this.currentGame = 'tetris';
        this.init();
    }
    
    init() {
        this.setupNavigation();
        this.showGame('tetris');
    }
    
    setupNavigation() {
        const navButtons = document.querySelectorAll('.nav-btn');
        
        navButtons.forEach(button => {
            button.addEventListener('click', () => {
                const gameType = button.dataset.game;
                
                // Update active button
                navButtons.forEach(btn => btn.classList.remove('active'));
                button.classList.add('active');
                
                // Show selected game
                this.showGame(gameType);
            });
        });
    }
    
    showGame(gameType) {
        // Hide all games
        const gameContainers = document.querySelectorAll('.game-container');
        gameContainers.forEach(container => {
            container.classList.add('hidden');
        });
        
        // Show selected game
        const targetGame = document.getElementById(`${gameType}-game`);
        if (targetGame) {
            targetGame.classList.remove('hidden');
        }
        
        this.currentGame = gameType;
    }
}

// Initialize the arcade when the page loads
let gameArcade;
window.addEventListener('load', () => {
    gameArcade = new GameArcade();
});
