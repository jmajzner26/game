class GameModeManager {
    static createMode(modeType, gameEngine) {
        switch (modeType) {
            case '9-hole':
                return new CourseMode(gameEngine, 9);
            case '18-hole':
                return new CourseMode(gameEngine, 18);
            case 'driving-range':
                return new DrivingRangeMode(gameEngine);
            case 'putting-challenge':
                return new PuttingChallengeMode(gameEngine);
            case 'daily-event':
                return new DailyEventMode(gameEngine);
            default:
                return null;
        }
    }
}

// Make available globally
window.GameModeManager = GameModeManager;

