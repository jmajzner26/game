class ClubSelector {
    constructor(gameEngine) {
        this.gameEngine = gameEngine;
        this.root = document.getElementById('club-selector');
        this.grid = document.getElementById('club-grid');
        this.btnClose = document.getElementById('club-close');

        if (this.btnClose) this.btnClose.addEventListener('click', () => this.hide());
        
        // Re-render when equipment changes
        this.render();
    }

    toggle() {
        if (!this.root) return;
        if (this.root.classList.contains('hidden')) this.show(); else this.hide();
    }

    show() {
        if (!this.root) return;
        this.render();
        this.root.classList.remove('hidden');
    }

    hide() {
        if (!this.root) return;
        this.root.classList.add('hidden');
    }

    render() {
        if (!this.grid || !this.gameEngine) return;
        const ps = this.gameEngine.progressionSystem;
        if (!ps) return;

        const clubs = ps.getAllClubs();
        const equipped = ps.equippedClub;
        const unlocked = ps.unlockedClubs;

        this.grid.innerHTML = '';

        clubs.forEach(club => {
            const card = document.createElement('div');
            card.className = 'club-card';
            if (!unlocked.has(club.id)) card.classList.add('locked');
            if (club.id === equipped) card.classList.add('equipped');

            const icon = this.iconForClub(club.id);
            const carry = Math.round(club.distance);

            card.innerHTML = `
                <div class="club-row">
                    <div class="club-left">
                        <div class="club-icon">${icon}</div>
                        <div class="club-name">${club.name}</div>
                    </div>
                    <div class="club-dist">${carry} yds</div>
                </div>
                <div class="club-stats">
                    <span>Power ${club.power}</span>
                    <span>Acc ${club.accuracy}</span>
                    <span>Spin ${club.spin}</span>
                </div>
            `;

            if (unlocked.has(club.id)) {
                card.addEventListener('click', () => {
                    if (ps.equipClub(club.id)) {
                        this.render();
                        // Update HUD club label immediately
                        if (window.hud) {
                            window.hud.update({ clubName: club.name });
                        }
                        // Close after selecting
                        this.hide();
                    }
                });
            }

            this.grid.appendChild(card);
        });
    }

    iconForClub(id) {
        // Simple glyphs per club type
        if (id === 'driver') return 'D';
        if (id.startsWith('iron_')) return id.split('_')[1];
        if (id === 'wedge') return 'W';
        if (id === 'putter') return 'P';
        return 'C';
    }
}
