// Main App Logic for Multi-Game Arcade
const trackEvent = (eventName, eventData = {}) => {
    if (typeof window !== 'undefined' && typeof window.va === 'function') {
        window.va(eventName, eventData);
    }
};
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

                trackEvent('select_game', { game: gameType });
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
    trackEvent('page_loaded');
});
