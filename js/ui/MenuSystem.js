class MenuSystem {
    constructor(gameEngine) {
        this.gameEngine = gameEngine;
        this.mainMenu = document.getElementById('main-menu');
        this.gameScreen = document.getElementById('game-screen');
        this.garageScreen = document.getElementById('garage-screen');
        this.loadingScreen = document.getElementById('loading-screen');
        
        this.setupEventListeners();
    }

    setupEventListeners() {
        // Main menu buttons
        const btn9Hole = document.getElementById('btn-9-hole');
        const btn18Hole = document.getElementById('btn-18-hole');
        const btnDrivingRange = document.getElementById('btn-driving-range');
        const btnPuttingChallenge = document.getElementById('btn-putting-challenge');
        const btnDailyEvent = document.getElementById('btn-daily-event');
        const btnGarage = document.getElementById('btn-garage');

        if (btn9Hole) {
            btn9Hole.addEventListener('click', () => this.startGame('9-hole'));
        }
        if (btn18Hole) {
            btn18Hole.addEventListener('click', () => this.startGame('18-hole'));
        }
        if (btnDrivingRange) {
            btnDrivingRange.addEventListener('click', () => this.startGame('driving-range'));
        }
        if (btnPuttingChallenge) {
            btnPuttingChallenge.addEventListener('click', () => this.startGame('putting-challenge'));
        }
        if (btnDailyEvent) {
            btnDailyEvent.addEventListener('click', () => this.startGame('daily-event'));
        }
        if (btnGarage) {
            btnGarage.addEventListener('click', () => this.showGarage());
        }

        // Garage back button
        const btnGarageBack = document.getElementById('btn-garage-back');
        if (btnGarageBack) {
            btnGarageBack.addEventListener('click', () => this.showMainMenu());
        }

        // Game controls
        const btnPause = document.getElementById('btn-pause');
        const btnMenu = document.getElementById('btn-menu');
        const btnResume = document.getElementById('btn-resume');
        const btnRestart = document.getElementById('btn-restart');
        const btnQuit = document.getElementById('btn-quit');

        if (btnPause) {
            btnPause.addEventListener('click', () => this.pauseGame());
        }
        if (btnMenu) {
            btnMenu.addEventListener('click', () => this.quitToMenu());
        }
        if (btnResume) {
            btnResume.addEventListener('click', () => this.resumeGame());
        }
        if (btnRestart) {
            btnRestart.addEventListener('click', () => this.restartGame());
        }
        if (btnQuit) {
            btnQuit.addEventListener('click', () => this.quitToMenu());
        }

        // Garage tabs
        const tabButtons = document.querySelectorAll('.tab-btn');
        tabButtons.forEach(btn => {
            btn.addEventListener('click', (e) => {
                const tab = e.target.dataset.tab;
                this.switchGarageTab(tab);
            });
        });
    }

    showMainMenu() {
        if (this.mainMenu) this.mainMenu.classList.remove('hidden');
        if (this.gameScreen) this.gameScreen.classList.add('hidden');
        if (this.garageScreen) this.garageScreen.classList.add('hidden');
    }

    showGameScreen() {
        if (this.mainMenu) this.mainMenu.classList.add('hidden');
        if (this.gameScreen) this.gameScreen.classList.remove('hidden');
        if (this.garageScreen) this.garageScreen.classList.add('hidden');
    }

    showGarage() {
        if (this.mainMenu) this.mainMenu.classList.add('hidden');
        if (this.gameScreen) this.gameScreen.classList.add('hidden');
        if (this.garageScreen) this.garageScreen.classList.remove('hidden');
        
        this.updateGarage();
    }

    startGame(modeType) {
        this.showLoading();
        
        setTimeout(() => {
            this.showGameScreen();
            this.hideLoading();
            
            if (this.gameEngine && this.gameEngine.loadMode) {
                // Load appropriate mode
                const GameModeManager = window.GameModeManager;
                if (GameModeManager) {
                    const mode = GameModeManager.createMode(modeType, this.gameEngine);
                    if (mode) {
                        this.gameEngine.loadMode(mode);
                        this.gameEngine.start();
                    }
                }
            }
        }, 500);
    }

    pauseGame() {
        const pauseMenu = document.getElementById('pause-menu');
        if (pauseMenu) pauseMenu.classList.remove('hidden');
        if (this.gameEngine) this.gameEngine.stop();
    }

    resumeGame() {
        const pauseMenu = document.getElementById('pause-menu');
        if (pauseMenu) pauseMenu.classList.add('hidden');
        if (this.gameEngine) this.gameEngine.start();
    }

    restartGame() {
        this.resumeGame();
        // Restart logic handled by game mode
        if (this.gameEngine && this.gameEngine.currentMode && this.gameEngine.currentMode.restart) {
            this.gameEngine.currentMode.restart();
        }
    }

    quitToMenu() {
        if (this.gameEngine) this.gameEngine.stop();
        this.showMainMenu();
    }

    showLoading() {
        if (this.loadingScreen) this.loadingScreen.classList.remove('hidden');
    }

    hideLoading() {
        if (this.loadingScreen) this.loadingScreen.classList.add('hidden');
    }

    switchGarageTab(tab) {
        const tabButtons = document.querySelectorAll('.tab-btn');
        tabButtons.forEach(btn => {
            if (btn.dataset.tab === tab) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });

        this.updateGarageContent(tab);
    }

    updateGarage() {
        const activeTab = document.querySelector('.tab-btn.active');
        const tab = activeTab ? activeTab.dataset.tab : 'clubs';
        this.updateGarageContent(tab);
    }

    updateGarageContent(tab) {
        const garageContent = document.getElementById('garage-content');
        if (!garageContent || !this.gameEngine) return;

        if (tab === 'clubs') {
            this.renderClubs(garageContent);
        } else if (tab === 'balls') {
            this.renderBalls(garageContent);
        }
    }

    renderClubs(container) {
        if (!this.gameEngine || !this.gameEngine.progressionSystem) return;

        const clubs = this.gameEngine.progressionSystem.getAllClubs();
        const equippedClub = this.gameEngine.progressionSystem.equippedClub;
        const unlockedClubs = this.gameEngine.progressionSystem.unlockedClubs;

        container.innerHTML = '<div class="item-grid"></div>';
        const grid = container.querySelector('.item-grid');

        clubs.forEach(club => {
            const card = document.createElement('div');
            card.className = 'item-card';
            
            if (!unlockedClubs.has(club.id)) {
                card.classList.add('locked');
            }
            
            if (club.id === equippedClub) {
                card.classList.add('equipped');
            }

            card.innerHTML = `
                <h3>${club.name}</h3>
                <div class="item-stats">
                    <div>Power: ${club.power}</div>
                    <div>Accuracy: ${club.accuracy}</div>
                    <div>Spin: ${club.spin}</div>
                    <div>Distance: ${club.distance} yds</div>
                </div>
                ${!unlockedClubs.has(club.id) ? '<p style="color: #ff6b6b; margin-top: 10px;">Locked</p>' : ''}
                ${club.id === equippedClub ? '<p style="color: #22c55e; margin-top: 10px;">Equipped</p>' : ''}
            `;

            if (unlockedClubs.has(club.id) && club.id !== equippedClub) {
                card.addEventListener('click', () => {
                    this.gameEngine.progressionSystem.equipClub(club.id);
                    this.updateGarage();
                });
            }

            grid.appendChild(card);
        });
    }

    renderBalls(container) {
        if (!this.gameEngine || !this.gameEngine.progressionSystem) return;

        const balls = this.gameEngine.progressionSystem.getAllBalls();
        const equippedBall = this.gameEngine.progressionSystem.equippedBall;
        const unlockedBalls = this.gameEngine.progressionSystem.unlockedBalls;

        container.innerHTML = '<div class="item-grid"></div>';
        const grid = container.querySelector('.item-grid');

        balls.forEach(ball => {
            const card = document.createElement('div');
            card.className = 'item-card';
            
            if (!unlockedBalls.has(ball.id)) {
                card.classList.add('locked');
            }
            
            if (ball.id === equippedBall) {
                card.classList.add('equipped');
            }

            card.innerHTML = `
                <h3>${ball.name}</h3>
                <div class="item-stats">
                    <div>Power: ${ball.power}</div>
                    <div>Spin: ${ball.spin}</div>
                    <div>Control: ${ball.control}</div>
                    <div>Durability: ${ball.durability}</div>
                </div>
                ${!unlockedBalls.has(ball.id) ? '<p style="color: #ff6b6b; margin-top: 10px;">Locked</p>' : ''}
                ${ball.id === equippedBall ? '<p style="color: #22c55e; margin-top: 10px;">Equipped</p>' : ''}
            `;

            if (unlockedBalls.has(ball.id) && ball.id !== equippedBall) {
                card.addEventListener('click', () => {
                    this.gameEngine.progressionSystem.equipBall(ball.id);
                    this.updateGarage();
                });
            }

            grid.appendChild(card);
        });
    }
}

