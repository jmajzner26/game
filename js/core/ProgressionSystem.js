class ProgressionSystem {
    constructor() {
        this.clubs = [];
        this.balls = [];
        this.unlockedClubs = new Set(['driver', 'iron_7', 'wedge', 'putter']);
        this.unlockedBalls = new Set(['standard']);
        this.equippedClub = 'driver';
        this.equippedBall = 'standard';
        
        this.initializeClubs();
        this.initializeBalls();
    }

    initializeClubs() {
        this.clubs = [
            {
                id: 'driver',
                name: 'Driver',
                power: 100,
                accuracy: 60,
                spin: 20,
                distance: 250,
                unlocked: true
            },
            {
                id: 'iron_3',
                name: '3 Iron',
                power: 85,
                accuracy: 70,
                spin: 40,
                distance: 210,
                unlocked: false,
                unlockCondition: 'score_under_par'
            },
            {
                id: 'iron_5',
                name: '5 Iron',
                power: 75,
                accuracy: 75,
                spin: 50,
                distance: 180,
                unlocked: false,
                unlockCondition: 'birdie_5'
            },
            {
                id: 'iron_7',
                name: '7 Iron',
                power: 65,
                accuracy: 80,
                spin: 60,
                distance: 150,
                unlocked: true
            },
            {
                id: 'iron_9',
                name: '9 Iron',
                power: 55,
                accuracy: 85,
                spin: 70,
                distance: 120,
                unlocked: false,
                unlockCondition: 'par_10'
            },
            {
                id: 'wedge',
                name: 'Wedge',
                power: 45,
                accuracy: 90,
                spin: 80,
                distance: 80,
                unlocked: true
            },
            {
                id: 'putter',
                name: 'Putter',
                power: 10,
                accuracy: 95,
                spin: 5,
                distance: 20,
                unlocked: true
            }
        ];
    }

    initializeBalls() {
        this.balls = [
            {
                id: 'standard',
                name: 'Standard Ball',
                power: 100,
                spin: 100,
                control: 100,
                durability: 100,
                unlocked: true
            },
            {
                id: 'distance',
                name: 'Distance Ball',
                power: 110,
                spin: 80,
                control: 90,
                durability: 95,
                unlocked: false,
                unlockCondition: 'drive_300'
            },
            {
                id: 'spin',
                name: 'Spin Ball',
                power: 90,
                spin: 120,
                control: 110,
                durability: 85,
                unlocked: false,
                unlockCondition: 'backspin_50'
            },
            {
                id: 'control',
                name: 'Control Ball',
                power: 95,
                spin: 110,
                control: 120,
                durability: 90,
                unlocked: false,
                unlockCondition: 'accuracy_80'
            },
            {
                id: 'premium',
                name: 'Premium Ball',
                power: 105,
                spin: 110,
                control: 110,
                durability: 105,
                unlocked: false,
                unlockCondition: 'complete_course'
            }
        ];
    }

    getClub(id) {
        return this.clubs.find(c => c.id === id);
    }

    getBall(id) {
        return this.balls.find(b => b.id === id);
    }

    getEquippedClub() {
        return this.getClub(this.equippedClub);
    }

    getEquippedBall() {
        return this.getBall(this.equippedBall);
    }

    equipClub(id) {
        if (this.unlockedClubs.has(id)) {
            this.equippedClub = id;
            this.saveProgress();
            return true;
        }
        return false;
    }

    equipBall(id) {
        if (this.unlockedBalls.has(id)) {
            this.equippedBall = id;
            this.saveProgress();
            return true;
        }
        return false;
    }

    unlockClub(id) {
        if (!this.unlockedClubs.has(id)) {
            this.unlockedClubs.add(id);
            const club = this.getClub(id);
            if (club) club.unlocked = true;
            this.saveProgress();
            return true;
        }
        return false;
    }

    unlockBall(id) {
        if (!this.unlockedBalls.has(id)) {
            this.unlockedBalls.add(id);
            const ball = this.getBall(id);
            if (ball) ball.unlocked = true;
            this.saveProgress();
            return true;
        }
        return false;
    }

    checkUnlockConditions(achievement) {
        // Check various achievements and unlock items
        const achievements = {
            'score_under_par': () => this.unlockClub('iron_3'),
            'birdie_5': () => this.unlockClub('iron_5'),
            'par_10': () => this.unlockClub('iron_9'),
            'drive_300': () => this.unlockBall('distance'),
            'backspin_50': () => this.unlockBall('spin'),
            'accuracy_80': () => this.unlockBall('control'),
            'complete_course': () => this.unlockBall('premium')
        };

        if (achievements[achievement]) {
            achievements[achievement]();
        }
    }

    saveProgress() {
        const progress = {
            unlockedClubs: Array.from(this.unlockedClubs),
            unlockedBalls: Array.from(this.unlockedBalls),
            equippedClub: this.equippedClub,
            equippedBall: this.equippedBall
        };
        localStorage.setItem('golfProgress', JSON.stringify(progress));
    }

    loadProgress() {
        const saved = localStorage.getItem('golfProgress');
        if (saved) {
            const progress = JSON.parse(saved);
            this.unlockedClubs = new Set(progress.unlockedClubs || ['driver', 'iron_7', 'wedge', 'putter']);
            this.unlockedBalls = new Set(progress.unlockedBalls || ['standard']);
            this.equippedClub = progress.equippedClub || 'driver';
            this.equippedBall = progress.equippedBall || 'standard';
            
            // Update club/ball unlocked status
            this.clubs.forEach(club => {
                club.unlocked = this.unlockedClubs.has(club.id);
            });
            this.balls.forEach(ball => {
                ball.unlocked = this.unlockedBalls.has(ball.id);
            });
        }
    }

    getAllClubs() {
        return this.clubs;
    }

    getAllBalls() {
        return this.balls;
    }
}

